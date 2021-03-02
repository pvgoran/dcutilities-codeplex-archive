using System;
using System.Threading;


namespace DigitallyCreated.Utilities.Bcl.Concurrency
{
	/// <summary>
	/// A semaphore is a concurrency utility that contains a number of "tokens". Threads try to acquire
	/// (take) and release (put) these tokens into the semaphore. When a semaphore contains no tokens,
	/// threads that try to acquire a token will block until a token is released into the semaphore.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This semaphore implementation makes no guarantees as to the order that client threads acquire
	/// and release tokens (this means a thread that calls acquire/release before another thread may
	/// in fact be served after a second thread).
	/// </para>
	/// <para>
	/// This class supports the safe use of interrupts. An interrupt that occurs within a method of
	/// this class results in the action performed by the method not occurring.
	/// </para>
	/// <para>
	/// This class does not support the safe use of <see cref="Thread.Abort()"/>. Its use may leave
	/// this class in an undefined state.
	/// </para>
	/// </remarks>
	public class Semaphore : ISemaphore
	{
		/// <summary>
		/// The lock object for this class
		/// </summary>
		protected readonly object _Lock;

		/// <summary>
		/// The tokens count for this class
		/// </summary>
		protected int _Tokens;


		/// <summary>
		/// Constructor, creates a Semaphore
		/// </summary>
		/// <param name="tokens">The number of tokens the semaphore will start with</param>
		public Semaphore(int tokens)
		{
			_Lock = new object();
			_Tokens = tokens;
		}


		/// <summary>
		/// Try to acquire a token but time out if a token cannot be acquired after certain amount of time.
		/// </summary>
		/// <param name="millisecondsTimeout">
		/// The number of milliseconds to wait to acquire a token. This can be set to 
		/// <see cref="Timeout.Infinite"/> if you want to wait forever.
		/// </param>
		/// <returns>
		/// true if a token was acquired successfully; false if a token has not been acquired after the amount
		/// of time specified by <paramref name="millisecondsTimeout"/> has elapsed.
		/// </returns>
		/// <exception cref="ThreadInterruptedException">
		/// If the calling thread was interrupted while waiting to acquire a token
		/// </exception>
		public bool TryAcquire(int millisecondsTimeout)
		{
			DateTime timeoutTime = DateTime.Now.AddMilliseconds(millisecondsTimeout);

			lock (_Lock)
			{
				for (;;)
				{
					if (_Tokens > 0)
					{
						_Tokens--;
						return true;
					}

					if (millisecondsTimeout != Timeout.Infinite)
					{
						millisecondsTimeout = (int)(timeoutTime - DateTime.Now).TotalMilliseconds;
						if (millisecondsTimeout <= 0)
							return false;
					}

					try
					{
						Monitor.Wait(_Lock, millisecondsTimeout);
					}
					catch (ThreadInterruptedException)
					{
						//I might have been pulsed as well, so pulse the next waiting thread
						Monitor.Pulse(_Lock);
						throw;
					}

					
				}
			}
		}


		/// <summary>
		/// Acquires a token waiting for as long as necessary to do so.
		/// </summary>
		/// <exception cref="ThreadInterruptedException">
		/// If the calling thread was interrupted while waiting to acquire a token
		/// </exception>
		public void Acquire()
		{
			TryAcquire(Timeout.Infinite);
		}


		/// <summary>
		/// Acquires a token waiting for as long as necessary to do so. 
		/// <see cref="ThreadInterruptedException"/> are guaranteed not be thrown by this method.
		/// </summary>
		public void ForceAcquire()
		{
			ForceTryAcquire(Timeout.Infinite);
		}


		/// <summary>
		/// Try to acquire a token but time out if a token cannot be acquired after certain amount of time.
		/// <see cref="ThreadInterruptedException"/> are guaranteed not be thrown by this method.
		/// </summary>
		/// <param name="millisecondsTimeout">
		/// The number of milliseconds to wait to acquire a token. This can be set to 
		/// <see cref="Timeout.Infinite"/> if you want to wait forever.
		/// </param>
		/// <returns>
		/// true if a token was acquired successfully; false if a token has not been acquired after the amount
		/// of time specified by <paramref name="millisecondsTimeout"/> has elapsed.
		/// </returns>
		public bool ForceTryAcquire(int millisecondsTimeout)
		{
			bool retval;
			
			try {}
			finally
			{
				retval = TryAcquire(millisecondsTimeout);
			}

			return retval;
		}


		/// <summary>
		/// Releases a token.
		/// </summary>
		/// <exception cref="ThreadInterruptedException">
		/// If the calling thread was interrupted while waiting to release a token
		/// </exception>
		public void Release()
		{
			ReleaseMany(1);
		}


		/// <summary>
		/// Releases many tokens.
		/// </summary>
		/// <param name="tokens">The number of tokens to release</param>
		/// <exception cref="ThreadInterruptedException">
		/// If the calling thread was interrupted while waiting to release tokens
		/// </exception>
		public virtual void ReleaseMany(int tokens)
		{
			lock (_Lock)
			{
				_Tokens += tokens;

				for (int i = 0; i < tokens; i++)
					Monitor.Pulse(_Lock);
			}
		}


		/// <summary>
		/// Releases a token. <see cref="ThreadInterruptedException"/> are guaranteed not be thrown by this
		/// method.
		/// </summary>
		public void ForceRelease()
		{
			ForceReleaseMany(1);
		}


		/// <summary>
		/// Releases many tokens. <see cref="ThreadInterruptedException"/> are guaranteed not be thrown by this
		/// method.
		/// </summary>
		/// <param name="tokens">The number of tokens to release</param>
		public void ForceReleaseMany(int tokens)
		{
			try {}
			finally //Locks cannnot be interrupted in a finally block
			{
				ReleaseMany(tokens);
			}
		}
	}
}
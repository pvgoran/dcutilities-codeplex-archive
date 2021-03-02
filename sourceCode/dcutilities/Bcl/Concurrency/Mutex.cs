using System;
using System.Threading;


namespace DigitallyCreated.Utilities.Bcl.Concurrency
{
	/// <summary>
	/// A mutex is a concurrency utility that only allows one thread at a time to acquire it without blocking.
	/// The mutex must be released before it is able to be acquired again. In this manner, it works like a 
	/// semaphore but one that can contain only a single token.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This mutex implementation makes no guarantees as to the order that client threads acquire
	/// and release it (this means a thread that calls acquire/release before another thread may
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
	public class Mutex : Semaphore
	{
		/// <summary>
		/// Constructor, creates a Mutex
		/// </summary>
		/// <param name="preacquired">
		/// True if you want the first call to an acquire method to block, false if you don't.
		/// </param>
		public Mutex(bool preacquired)
			: base(preacquired ? 0 : 1)
		{
		}


		/// <summary>
		/// Releases many tokens. (A mutex does not support the release of more than 1 token)
		/// </summary>
		/// <param name="tokens">The number of tokens to release (must be 1)</param>
		/// <exception cref="ThreadInterruptedException">
		/// If the calling thread was interrupted while waiting to release tokens
		/// </exception>
		/// <exception cref="ArgumentException">If <paramref name="tokens"/> is not 1</exception>
		/// <exception cref="NotSupportedException">If the mutex has already been released</exception>
		public override void ReleaseMany(int tokens)
		{
			if (tokens > 1 || tokens < 1)
				throw new ArgumentException("You cannot release more or less than 1 token");

			lock (_Lock)
			{
				if (_Tokens == 1)
					throw new NotSupportedException("The mutex has been released already");

				base.ReleaseMany(1);
			}
		}
	}
}
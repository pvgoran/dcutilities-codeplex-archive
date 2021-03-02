using System;
using System.Threading;


namespace DigitallyCreated.Utilities.Bcl.Concurrency
{
	/// <summary>
	/// A Reader-Writer Lock is a concurrency utility that allows multiple threads to read concurrently, but
	/// as soon as a thread needs to write, all other threads except the writer are denied access (or more
	/// accurately, have to wait before they are allowed access) until the writer thread is done.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This Reader-Writer Lock is tuned so that while a writer has the write lock, the readers queue
	/// in front of any other waiting writer. This allows a whole lot of readers to read concurrently
	/// before the next writer is allowed in. This results in higher performance when there are a lot
	/// of readers, and also stops writers from being able to starve readers.
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
	public class ReaderWriterLock : IReaderWriterLock
	{
		private readonly object _Lock;
		private readonly Mutex _WriteLock;
		private readonly FifoMutex _ReadersTurnstile;
		private readonly FifoMutex _WritersTurnstile;
		private int _NumReaders;


		/// <summary>
		/// Constructor, creates a ReaderWriterLock
		/// </summary>
		public ReaderWriterLock()
		{
			_Lock = new object();
			_WriteLock = new Mutex(false);
			_ReadersTurnstile = new FifoMutex(false);
			_WritersTurnstile = new FifoMutex(false);
			_NumReaders = 0;
		}


		/// <summary>
		/// Acquires the read lock.
		/// </summary>
		/// <exception cref="ThreadInterruptedException">
		/// If the calling thread was interrupted while trying to acquire the read lock
		/// </exception>
		public void AcquireRead()
		{
			_ReadersTurnstile.Acquire();
			_ReadersTurnstile.ForceRelease(); //We must release this (no interrupts), 
			                                  //or else the readers turnstile may lock up forever
			lock (_Lock) 
			{
				if (_NumReaders == 0)
					_WriteLock.Acquire(); //Block writers. Safe to be interrupted here

				_NumReaders++;
			}
		}


		/// <summary>
		/// Acquires the read lock. This method is guaranteed not to throw a 
		/// <see cref="ThreadInterruptedException"/>.
		/// </summary>
		public void ForceAcquireRead()
		{
			try {}
			finally
			{
				AcquireRead();
			}
		}


		/// <summary>
		/// Acquires the write lock.
		/// </summary>
		/// <exception cref="ThreadInterruptedException">
		/// If the calling thread was interrupted while trying to acquire the write lock
		/// </exception>
		public void AcquireWrite()
		{
			//Block any new writers from acquiring
			//This prevents another writer from blocking readers from
			//queueing against the _ReadersTurnstile
			_WritersTurnstile.Acquire(); //Safe to be interrupted here

			try
			{
				//Block any new readers from acquiring
				_ReadersTurnstile.Acquire();
			}
			catch (ThreadInterruptedException)
			{
				//Undo all locks so far
				_WritersTurnstile.ForceRelease();
				throw;
			}
			 
			try
			{
				_WriteLock.Acquire();
			}
			catch (ThreadInterruptedException)
			{
				//Undo all locks so far
				_ReadersTurnstile.ForceRelease();
				_WritersTurnstile.ForceRelease();
				throw;
			}
			
		}


		/// <summary>
		/// Acquires the write lock. This method is guaranteed not to throw a 
		/// <see cref="ThreadInterruptedException"/>.
		/// </summary>
		public void ForceAcquireWrite()
		{
			try {}
			finally
			{
				AcquireWrite();
			}
		}


		/// <summary>
		/// Releases the read lock. This method is guaranteed not to throw a 
		/// <see cref="ThreadInterruptedException"/>.
		/// </summary>
		public void ReleaseRead()
		{
			try {}
			finally
			{
				lock (_Lock)
				{
					if (_NumReaders == 1)
						_WriteLock.Release();

					_NumReaders--;
				}
			}
		}


		/// <summary>
		/// Releases the write lock. This method is guaranteed not to throw a 
		/// <see cref="ThreadInterruptedException"/>.
		/// </summary>
		public void ReleaseWrite()
		{
			try {}
			finally
			{
				_WriteLock.Release();
				_ReadersTurnstile.Release();
				_WritersTurnstile.Release();
			}
		}


		/// <summary>
		/// Acquires the read lock and returns an <see cref="IDisposable"/> that, when disposed, will release
		/// the read lock.
		/// </summary>
		/// <remarks>
		/// The returned <see cref="IDisposable"/> is guaranteed to release the acquired read lock and will
		/// not be interrupted
		/// </remarks>
		/// <returns>An <see cref="IDisposable"/> that will release the read lock when disposed</returns>
		/// <exception cref="ThreadInterruptedException">
		/// If the calling thread was interrupted while trying to acquire the read lock.
		/// </exception>
		public IDisposable AcquireDisposableRead()
		{
			AcquireRead();
			return new LockDisposable(ReleaseRead);
		}


		/// <summary>
		/// Acquires the read lock and returns an <see cref="IDisposable"/> that, when disposed, will release
		/// the read lock. This method is guaranteed not to throw a <see cref="ThreadInterruptedException"/>.
		/// </summary>
		/// <remarks>
		/// The returned <see cref="IDisposable"/> is guaranteed to release the acquired read lock and will
		/// not be interrupted
		/// </remarks>
		/// <returns>An <see cref="IDisposable"/> that will release the read lock when disposed</returns>
		public IDisposable ForceAcquireDisposableRead()
		{
			IDisposable disposable;

			try {}
			finally
			{
				disposable = AcquireDisposableRead();
			}

			return disposable;
		}


		/// <summary>
		/// Acquires the write lock and returns an <see cref="IDisposable"/> that, when disposed, will release
		/// the write lock.
		/// </summary>
		/// <remarks>
		/// The returned <see cref="IDisposable"/> is guaranteed to release the acquired write lock and will
		/// not be interrupted
		/// </remarks>
		/// <returns>An <see cref="IDisposable"/> that will release the write lock when disposed</returns>
		/// <exception cref="ThreadInterruptedException">
		/// If the calling thread was interrupted while trying to acquire the write lock.
		/// </exception>
		public IDisposable AcquireDisposableWrite()
		{
			AcquireWrite();
			return new LockDisposable(ReleaseWrite);
		}


		/// <summary>
		/// Acquires the write lock and returns an <see cref="IDisposable"/> that, when disposed, will release
		/// the write lock. This method is guaranteed not to throw a <see cref="ThreadInterruptedException"/>.
		/// </summary>
		/// <remarks>
		/// The returned <see cref="IDisposable"/> is guaranteed to release the acquired write lock and will
		/// not be interrupted
		/// </remarks>
		/// <returns>An <see cref="IDisposable"/> that will release the write lock when disposed</returns>
		public IDisposable ForceAcquireDisposableWrite()
		{
			IDisposable disposable;

			try { }
			finally
			{
				disposable = AcquireDisposableWrite();
			}

			return disposable;
		}


		/// <summary>
		/// An <see cref="IDisposable"/> that releases a lock when disposed
		/// </summary>
		private class LockDisposable : IDisposable
		{
			private readonly Action _ReleaseLock;
			private bool _HasBeenDisposed;

			/// <summary>
			/// Constructor, creates a LockDisposable
			/// </summary>
			/// <param name="releaseLock">The method to call to release the lock</param>
			public LockDisposable(Action releaseLock)
			{
				_ReleaseLock = releaseLock;
				_HasBeenDisposed = false;
			}


			/// <summary>
			/// Releases the lock
			/// </summary>
			public void Dispose()
			{
				if (_HasBeenDisposed) 
					return;

				_ReleaseLock();
				_HasBeenDisposed = true;
			}
		}
	}
}
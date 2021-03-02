using System;
using System.Threading;


namespace DigitallyCreated.Utilities.Bcl.Concurrency
{
	/// <summary>
	/// A Reader-Writer Lock is a concurrency utility that allows multiple threads to read concurrently, but
	/// as soon as a thread needs to write, all other threads except the writer are denied access (or more
	/// accurately, have to wait before they are allowed access) until the writer thread is done
	/// </summary>
	public interface IReaderWriterLock
	{
		/// <summary>
		/// Acquires the read lock.
		/// </summary>
		/// <exception cref="ThreadInterruptedException">
		/// If the calling thread was interrupted while trying to acquire the read lock
		/// </exception>
		void AcquireRead();

		/// <summary>
		/// Acquires the read lock. This method is guaranteed not to throw a 
		/// <see cref="ThreadInterruptedException"/>.
		/// </summary>
		void ForceAcquireRead();

		/// <summary>
		/// Acquires the write lock.
		/// </summary>
		/// <exception cref="ThreadInterruptedException">
		/// If the calling thread was interrupted while trying to acquire the write lock
		/// </exception>
		void AcquireWrite();

		/// <summary>
		/// Acquires the write lock. This method is guaranteed not to throw a 
		/// <see cref="ThreadInterruptedException"/>.
		/// </summary>
		void ForceAcquireWrite();

		/// <summary>
		/// Releases the read lock. This method is guaranteed not to throw a 
		/// <see cref="ThreadInterruptedException"/>.
		/// </summary>
		/// <exception cref="ThreadInterruptedException">
		/// If the calling thread was interrupted while trying to acquire the write lock
		/// </exception>
		void ReleaseRead();

		/// <summary>
		/// Releases the write lock. This method is guaranteed not to throw a 
		/// <see cref="ThreadInterruptedException"/>.
		/// </summary>
		void ReleaseWrite();

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
		IDisposable AcquireDisposableRead();

		/// <summary>
		/// Acquires the read lock and returns an <see cref="IDisposable"/> that, when disposed, will release
		/// the read lock. This method is guaranteed not to throw a <see cref="ThreadInterruptedException"/>.
		/// </summary>
		/// <remarks>
		/// The returned <see cref="IDisposable"/> is guaranteed to release the acquired read lock and will
		/// not be interrupted
		/// </remarks>
		/// <returns>An <see cref="IDisposable"/> that will release the read lock when disposed</returns>
		IDisposable ForceAcquireDisposableRead();

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
		IDisposable AcquireDisposableWrite();

		/// <summary>
		/// Acquires the write lock and returns an <see cref="IDisposable"/> that, when disposed, will release
		/// the write lock. This method is guaranteed not to throw a <see cref="ThreadInterruptedException"/>.
		/// </summary>
		/// <remarks>
		/// The returned <see cref="IDisposable"/> is guaranteed to release the acquired write lock and will
		/// not be interrupted
		/// </remarks>
		/// <returns>An <see cref="IDisposable"/> that will release the write lock when disposed</returns>
		IDisposable ForceAcquireDisposableWrite();
	}
}
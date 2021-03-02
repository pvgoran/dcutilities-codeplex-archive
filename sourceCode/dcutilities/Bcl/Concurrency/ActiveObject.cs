using System;
using System.Threading;


namespace DigitallyCreated.Utilities.Bcl.Concurrency
{
	/// <summary>
	/// ActiveObject is an abstract skeleton class that provides some basic framework for inheritors that
	/// want to run their logic in a self-contained thread. You will likely want to derive from
	/// either <see cref="BoolStopActiveObject"/> or <see cref="InterruptStopActiveObject"/> and not
	/// this class.
	/// </summary>
	public abstract class ActiveObject
	{
		private readonly Thread _Thread;

		/// <summary>
		/// The thread that runs the logic in this <see cref="ActiveObject"/>
		/// </summary>
		protected Thread Thread
		{
			get { return _Thread; }
		}

		/// <summary>
		/// This event is raised when the thread contained by this <see cref="ActiveObject"/>stops executing.
		/// Note that this event is raised on the thread internal to this <see cref="ActiveObject"/>.
		/// </summary>
		/// <remarks>
		/// Note that this event does not occur when the threads ends due to an unhandled exception.
		/// This is not surprising as an unhandled exception in a thread will cause the entire
		/// application to terminate, thereby making the Stopped event irrelevant.
		/// </remarks>
		public event EventHandler Stopped;


		/// <summary>
		/// Constructor, creates an ActiveObject instance
		/// </summary>
		/// <param name="threadName">The name of the internal thread</param>
		/// <param name="backgroundThread">Whether or not the thread is a background thread</param>
		protected ActiveObject(string threadName, bool backgroundThread)
		{
			_Thread = new Thread(DoRun);
			_Thread.Name = threadName;
			_Thread.IsBackground = backgroundThread;
		}


		/// <summary>
		/// Starts this <see cref="ActiveObject"/>'s internal thread
		/// </summary>
		public void Start()
		{
			_Thread.Start();
		}


		/// <summary>
		/// Request that this <see cref="ActiveObject"/> stop executing as soon as possible.
		/// This method will return immediately and not wait for the thread to stop.
		/// </summary>
		public abstract void RequestStop();

		
		/// <summary>
		/// Request that this <see cref="ActiveObject"/> stop executing as soon as possible. This method will
		/// not return until the thread has stopped or the timeout has expired.
		/// </summary>
		/// <param name="millisecondsTimeout">
		/// The time to wait for the thread to stop. This can be set to <see cref="Timeout.Infinite"/> if you
		/// want to wait forever.
		/// </param>
		/// <returns>
		/// true if the thread has terminated; false if the thread has not terminated after the amount of time
		/// specified by <paramref name="millisecondsTimeout"/> has elapsed.
		/// </returns>
		/// <exception cref="ThreadStateException">
		/// The internal thread has not been started by a call to <see cref="Start"/>
		/// </exception>
		public virtual bool RequestStop(int millisecondsTimeout)
		{
			RequestStop();

			return _Thread.Join(millisecondsTimeout);
		}


		/// <summary>
		/// Blocks the calling thread until the thread internal to this <see cref="ActiveObject"/> terminates.
		/// </summary>
		/// <exception cref="ThreadInterruptedException">
		/// The calling thread is interrupted while waiting.
		/// </exception>
		/// <exception cref="ThreadStateException">
		/// The internal thread has not been started by a call to <see cref="Start"/>
		/// </exception>
		public void Join()
		{
			_Thread.Join();
		}


		/// <summary>
		/// Blocks the calling thread until the thread internal to this <see cref="ActiveObject"/> terminates
		/// or a timeout occurs.
		/// </summary>
		/// <param name="millisecondsTimeout">
		/// The time to wait for the thread to stop. This can be set to <see cref="Timeout.Infinite"/> if you
		/// want to wait forever.
		/// </param>
		/// <returns>
		/// true if the thread has terminated; false if the thread has not terminated after the amount of time
		/// specified by <paramref name="millisecondsTimeout"/> has elapsed.
		/// </returns>
		/// <exception cref="ThreadStateException">
		/// The internal thread has not been started by a call to <see cref="Start"/>
		/// </exception>
		public bool Join(int millisecondsTimeout)
		{
			return _Thread.Join(millisecondsTimeout);
		}


		/// <summary>
		/// Wrapper for <see cref="Run"/> that raises the <see cref="Stopped"/> event
		/// </summary>
		private void DoRun()
		{
			Run();
			if (Stopped != null) Stopped(this, EventArgs.Empty);
		}


		/// <summary>
		/// Runs the execution logic behind this <see cref="ActiveObject"/>
		/// </summary>
		/// <remarks>
		/// Inheritors must override this and put the execution logic that they want to 
		/// run on the internal thread in here.
		/// </remarks>
		protected abstract void Run();
	}
}
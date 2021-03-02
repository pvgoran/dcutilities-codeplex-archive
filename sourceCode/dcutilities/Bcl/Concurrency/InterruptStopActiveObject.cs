using System.Threading;


namespace DigitallyCreated.Utilities.Bcl.Concurrency
{
	/// <summary>
	/// InterruptStopActiveObject is an abstract skeleton class that provides some basic framework for
	/// inheritors that want to run their logic in a self-contained thread. It enables inheritors to be
	/// stopped by external threads by using thread interrupts.
	/// </summary>
	/// <remarks>
	/// InterruptStopActiveObject handles stopping threads midway through execution by triggering an
	/// interrupt on the internal thread used when <see cref="RequestStop"/> is called by another thread.
	/// Implementors of InterruptStopActiveObject need to catch <see cref="ThreadInterruptedException"/>s
	/// at the points they might occur in their logic and stop executing as soon as possible.
	/// </remarks>
	public abstract class InterruptStopActiveObject : ActiveObject
	{

		/// <summary>
		/// Constructor, creates an InterruptStopActiveObject instance
		/// </summary>
		/// <param name="threadName">The name of the internal thread</param>
		/// <param name="backgroundThread">Whether or not the thread is a background thread</param>
		protected InterruptStopActiveObject(string threadName, bool backgroundThread)
			: base(threadName, backgroundThread)
		{
		}


		/// <summary>
		/// Request that this <see cref="ActiveObject"/> stop executing as soon as possible.
		/// This method will return immediately and not wait for the thread to stop.
		/// </summary>
		public override void RequestStop()
		{
			Thread.Interrupt();
		}
	}
}
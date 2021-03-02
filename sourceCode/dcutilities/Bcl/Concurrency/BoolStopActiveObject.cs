namespace DigitallyCreated.Utilities.Bcl.Concurrency
{
	/// <summary>
	/// BoolStopActiveObject is an abstract skeleton class that provides some basic framework for
	/// inheritors that want to run their logic in a self-contained thread. It enables inheritors to be
	/// stopped by external threads by using a boolean flag behaviour.
	/// </summary>
	/// <remarks>
	/// BoolStopActiveObject handles stopping threads midway though execution via the 
	/// <see cref="StopRequested"/> protected boolean, which is set to true when 
	/// <see cref="RequestStop"/> is called by another thread. Implementors of BoolStopActiveObject need
	/// to inspect this variable regularly to see whether they need to stop.
	/// </remarks>
	public abstract class BoolStopActiveObject : ActiveObject
	{
		private bool _StopRequested;

		/// <summary>
		/// If true, inheritors should stop executing as soon as possible
		/// </summary>
		protected bool StopRequested
		{
			get { return _StopRequested; }
			set { _StopRequested = value; }
		}


		/// <summary>
		/// Constructor, creates an BoolStopActiveObject instance
		/// </summary>
		/// <param name="threadName">The name of the internal thread</param>
		/// <param name="backgroundThread">Whether or not the thread is a background thread</param>
		protected BoolStopActiveObject(string threadName, bool backgroundThread)
			: base(threadName, backgroundThread)
		{
		}


		/// <summary>
		/// Request that this <see cref="ActiveObject"/> stop executing as soon as possible.
		/// This method will return immediately and not wait for the thread to stop.
		/// </summary>
		public override void RequestStop()
		{
			_StopRequested = true;
		}
	}
}
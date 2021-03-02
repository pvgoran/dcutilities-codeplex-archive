using System;

namespace DigitallyCreated.Utilities.ErrorReporting
{
	/// <summary>
	/// This enumerates the different severities of error that can be reported
	/// </summary>
	public enum ErrorSeverity
	{
		/// <summary>
		/// A very severe crash or data corruption has been detected. This level
		/// is specifically for unrecoverable errors.
		/// </summary>
		Fatal,

		/// <summary>
		/// An error has occurred that can be recovered from
		/// </summary>
		Error,

		/// <summary>
		/// A dubious error has occurred that doesn't cause too much havoc
		/// and can easily be recovered from
		/// </summary>
		Warning,

		/// <summary>
		/// Something interesting has happened and you might want to know 
		/// about it
		/// </summary>
		Information
	}
}
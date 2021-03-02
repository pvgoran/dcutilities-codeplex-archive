using System;


namespace DigitallyCreated.Utilities.Bcl
{
	/// <summary>
	/// Thrown when something is in an illegal state
	/// </summary>
	public class IllegalStateException : Exception
	{
		/// <summary>
		/// Creates a new <see cref="IllegalStateException"/> without
		/// a message or an inner exception
		/// </summary>
		public IllegalStateException()
		{
		}


		/// <summary>
		/// Creates a new <see cref="IllegalStateException"/> with
		/// a message but not an inner exception
		/// </summary>
		/// <param name="message">A message detailing the error that occurred</param>
		public IllegalStateException(string message)
			: base(message)
		{
		}


		/// <summary>
		/// Creates a new <see cref="IllegalStateException"/> with both
		/// a message and an inner exception
		/// </summary>
		/// <param name="message">A message detailing the error that occurred</param>
		/// <param name="innerException">The exception that was the cause of this exception</param>
		public IllegalStateException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
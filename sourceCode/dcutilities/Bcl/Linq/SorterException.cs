using System;


namespace DigitallyCreated.Utilities.Bcl.Linq
{
	/// <summary>
	/// A SorterException is thrown when an error occurs using a sorter
	/// </summary>
	public class SorterException : Exception
	{
		/// <summary>
		/// Creates a new <see cref="SorterException"/> without
		/// a message or an inner exception
		/// </summary>
		public SorterException()
		{
		}


		/// <summary>
		/// Creates a new <see cref="SorterException"/> with
		/// a message but not an inner exception
		/// </summary>
		/// <param name="message">A message detailing the error that occurred</param>
		public SorterException(string message)
			: base(message)
		{
		}


		/// <summary>
		/// Creates a new <see cref="SorterException"/> with both
		/// a message and an inner exception
		/// </summary>
		/// <param name="message">A message detailing the error that occurred</param>
		/// <param name="innerException">The exception that was the cause of this exception</param>
		public SorterException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
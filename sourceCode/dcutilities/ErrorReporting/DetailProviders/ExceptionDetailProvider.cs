using System;
using System.Collections.Generic;


namespace DigitallyCreated.Utilities.ErrorReporting.DetailProviders
{
	/// <summary>
	/// This class is an <see cref="IDetailProvider"/> and provides information
	/// about the <see cref="Exception"/> exception type.
	/// </summary>
	public class ExceptionDetailProvider : AbstractDetailProvider<Exception>
	{
		/// <summary>
		/// Gets a dictionary of details about the exception where the key is the detail's
		/// name and the key is the object that is the detail.
		/// </summary>
		/// <param name="e">The exception to read the details from</param>
		/// <returns>
		/// A dictionary of details about the exception where the key is the detail's
		/// name and the value is the object that is the detail.
		/// </returns>
		protected override IDictionary<string, object> DoGetDetail(Exception e)
		{
			IDictionary<string, object> details = new Dictionary<string, object>();

			details.Add("Message", e.Message);

			if (e.Data.Count != 0)
				details.Add("Data", e.Data);

			return details;
		}
	}
}
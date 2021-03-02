using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace DigitallyCreated.Utilities.ErrorReporting.DetailProviders
{
	/// <summary>
	/// This <see cref="IDetailProvider"/> provides information about
	/// <see cref="HttpException"/>s
	/// </summary>
	public class HttpExceptionDetailProvider : AbstractDetailProvider<HttpException>
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
		protected override IDictionary<string, object> DoGetDetail(HttpException e)
		{
			IDictionary<string, object> detail = new Dictionary<string, object>();

			detail.Add("HTTP Code", e.GetHttpCode());

			return detail;
		}
	}
}
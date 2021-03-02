using System;
using System.Web;
using System.Web.Mvc;


namespace DigitallyCreated.Utilities.Mvc
{
	/// <summary>
	/// An <see cref="ActionResult"/> that works similar to <see cref="RedirectResult"/> except that
	/// it sets the HTTP return code to 301 (Permanently Moved) rather than 302 (Found). This
	/// HTTP return code is more suitable for redirects from old URLs to new ones. It will permanently
	/// redirect the user's browser to the specified URL.
	/// </summary>
	/// <remarks>
	/// You can use the <see cref="PermanentRedirectResultExtensions.PermanentlyRedirect"/> extension
	/// method off <see cref="Controller"/> to easily create one of these <see cref="ActionResult"/>s
	/// </remarks>
	public class PermanentRedirectResult : ActionResult
	{
		/// <summary>
		/// The URL being redirected to
		/// </summary>
		public string Url { get; private set; }


		/// <summary>
		/// Constructor, creates a PermanentRedirectResult to the specified URL
		/// </summary>
		/// <param name="url">The URL to redirect to</param>
		/// <exception cref="ArgumentException">If <paramref name="url"/> is null or empty</exception>
		public PermanentRedirectResult(string url)
		{
			if (String.IsNullOrEmpty(url))
				throw new ArgumentException("url cannot be null or empty", url);

			Url = url;
		}


		/// <summary>
		/// Enables processing of the result of an action method by a custom type that inherits from 
		/// <see cref="ActionResult"/>.
		/// </summary>
		/// <param name="context">The context within which the result is executed.</param>
		public override void ExecuteResult(ControllerContext context)
		{
			string url = Url;

			if (context == null)
				throw new ArgumentNullException("context");

			//Convert virtual path
			if (url[0] == '~')
			{
				//VirtualPathUtility.ToAbsolute doesn't support query parameters, so chop them off and
				//add them back after
				string queryParams = null;
				int index = url.IndexOf('?');
				if (index != -1)
				{
					queryParams = url.Substring(index);
					url = url.Substring(0, index);
				}
				url = VirtualPathUtility.ToAbsolute(url);
				if (queryParams != null)
					url += queryParams;
			}

			context.HttpContext.Response.StatusCode = 301;
			context.HttpContext.Response.RedirectLocation = url;
		}
	}
}
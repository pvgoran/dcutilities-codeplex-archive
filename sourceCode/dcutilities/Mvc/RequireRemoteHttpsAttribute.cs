using System;
using System.Web.Mvc;


namespace DigitallyCreated.Utilities.Mvc
{
	/// <summary>
	/// Represents an attribute that forces an unsecured HTTP request to be re-sent over HTTPS
	/// but only if the request is a remote request (non-localhost).
	/// </summary>
	public class RequireRemoteHttpsAttribute : RequireHttpsAttribute
	{
		/// <summary>
		/// Called when authorization is required.
		/// </summary>
		/// <param name="filterContext">The filter context.</param>
		public override void OnAuthorization(AuthorizationContext filterContext)
		{
			if (filterContext == null)
				throw new ArgumentNullException("filterContext");

			if (filterContext.HttpContext != null && filterContext.HttpContext.Request.IsLocal)
				return;

			base.OnAuthorization(filterContext);
		}
	}
}
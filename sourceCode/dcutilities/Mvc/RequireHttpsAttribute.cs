using System;
using System.Web.Mvc;
using DigitallyCreated.Utilities.Mvc.Configuration;


namespace DigitallyCreated.Utilities.Mvc
{
	/// <summary>
	/// Represents an attribute that forces an unsecured HTTP request to be re-sent over HTTPS.
	/// </summary>
	public class RequireHttpsAttribute : FilterAttribute, IAuthorizationFilter
	{
		/// <summary>
		/// Called when authorization is required.
		/// </summary>
		/// <param name="filterContext">The filter context.</param>
		public virtual void OnAuthorization(AuthorizationContext filterContext)
		{
			if (filterContext == null)
				throw new ArgumentNullException("filterContext");

			if (DcuMvcConfigurationSection.GetSection().RequireHttps.IsEnabled == false)
				return;

			if (filterContext.HttpContext.Request.IsSecureConnection) 
				return;

			if (!String.Equals(filterContext.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
				throw new InvalidOperationException("The requested resource can only be accessed via SSL.");

			string url = "https://" + filterContext.HttpContext.Request.Url.Host + filterContext.HttpContext.Request.RawUrl;
			filterContext.Result = new RedirectResult(url);
		}
	}
}
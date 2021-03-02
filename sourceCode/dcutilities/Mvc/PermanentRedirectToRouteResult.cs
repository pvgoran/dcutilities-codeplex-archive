using System;
using System.Web.Mvc;
using System.Web.Routing;


namespace DigitallyCreated.Utilities.Mvc
{
	/// <summary>
	/// An <see cref="ActionResult"/> that works similar to <see cref="RedirectToRouteResult"/> except that
	/// it sets the HTTP return code to 301 (Permanently Moved) rather than 302 (Found). This
	/// HTTP return code is more suitable for redirects from old URLs to new ones. It will permanently
	/// redirect the user's browser to the specified route.
	/// </summary>
	/// <remarks>
	/// You can use the 
	/// <see cref="PermanentRedirectResultExtensions.PermanentlyRedirectToRoute(System.Web.Mvc.Controller,object)"/> 
	/// extension method (and others) off <see cref="Controller"/> to easily create one of these 
	/// <see cref="ActionResult"/>s
	/// </remarks>
	public class PermanentRedirectToRouteResult : ActionResult
	{
		/// <summary>
		/// The name of the route being redirected to (or null if one was not set)
		/// </summary>
		public string RouteName { get; private set; }

		/// <summary>
		/// The values of the route being redirected to
		/// </summary>
		public RouteValueDictionary RouteValues { get; private set; }


		/// <summary>
		/// Constructor, creates a PermanentRedirectToRouteResult that redirects to the route
		/// specified by <paramref name="routeValues"/>
		/// </summary>
		/// <param name="routeValues">The values of the route to redirect to</param>
		public PermanentRedirectToRouteResult(RouteValueDictionary routeValues)
			: this(null, routeValues)
		{
			
		}


		/// <summary>
		/// Constructor, creates a PermanentRedirectToRouteResult that redirects to the route
		/// specified by <paramref name="routeValues"/> and <paramref name="routeName"/>
		/// </summary>
		/// <param name="routeName">The name of the route to redirect to</param>
		/// <param name="routeValues">The values of the route to redirect to</param>
		public PermanentRedirectToRouteResult(string routeName, RouteValueDictionary routeValues)
		{
			RouteName = routeName;
			RouteValues = routeValues;
		}


		/// <summary>
		/// Enables processing of the result of an action method by a custom type that inherits from 
		/// <see cref="ActionResult"/>.
		/// </summary>
		/// <param name="context">The context within which the result is executed.</param>
		public override void ExecuteResult(ControllerContext context)
		{
			if (context == null)
				throw new ArgumentNullException("context");

			UrlHelper urlHelper = new UrlHelper(context.RequestContext);
			string url = urlHelper.RouteUrl(RouteName, RouteValues);
			if (String.IsNullOrEmpty(url))
				throw new InvalidOperationException("No route in the route table matches the supplied values");

			context.HttpContext.Response.StatusCode = 301;
			context.HttpContext.Response.RedirectLocation = url;
		}
	}
}
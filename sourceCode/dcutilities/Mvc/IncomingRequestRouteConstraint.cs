using System.Web;
using System.Web.Routing;


namespace DigitallyCreated.Utilities.Mvc
{
	/// <summary>
	/// A <see cref="IRouteConstraint"/> that constrains the route to match only incoming requests
	/// (<see cref="RouteDirection.IncomingRequest"/>) and not URL generation 
	/// (<see cref="RouteDirection.UrlGeneration"/>). This is useful for routes that represent
	/// old URLs that you will redirect as it prevents URLs from being generated from these routes
	/// but still allows them to be used by the user.
	/// </summary>
	public class IncomingRequestRouteConstraint : IRouteConstraint
	{
		/// <summary>
		/// Determines whether the URL parameter contains a valid value for this constraint.
		/// </summary>
		/// <returns>
		/// true if the URL parameter contains a valid value; otherwise, false.
		/// </returns>
		/// <param name="httpContext">An object that encapsulates information about the HTTP request.</param>
		/// <param name="route">The object that this constraint belongs to.</param>
		/// <param name="parameterName">The name of the parameter that is being checked.</param>
		/// <param name="values">An object that contains the parameters for the URL.</param>
		/// <param name="routeDirection">
		/// An object that indicates whether the constraint check is being performed when an incoming request
		/// is being handled or when a URL is being generated.
		/// </param>
		public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
		{
			return routeDirection == RouteDirection.IncomingRequest;
		}
	}
}
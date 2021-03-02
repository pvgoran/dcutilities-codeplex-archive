using System.Security.Principal;
using System.Web.Mvc;
using System.Web.Routing;


namespace DigitallyCreated.Utilities.Mvc
{
	/// <summary>
	/// Interface that can be implemented by non-standard Controllers to enable 
	/// <see cref="RouteHelpers.IsCurrentUserAuthorizedForRoute(System.Web.Mvc.UrlHelper,object,System.Web.Mvc.HttpVerbs)"/>
	/// to work.
	/// </summary>
	/// <seealso cref="RouteHelpers.IsCurrentUserAuthorizedForRoute(System.Web.Mvc.UrlHelper,object,System.Web.Mvc.HttpVerbs)"/>
	public interface IActionAuthorizationTestable
	{
		/// <summary>
		/// Checks whether a user is authorized to use a particular action method
		/// </summary>
		/// <param name="user">The user</param>
		/// <param name="routeName">The name of the route (may be null)</param>
		/// <param name="routeValues">The the route values</param>
		/// <param name="verb">The request HTTP verb</param>
		/// <returns>True if the user is authorized, false otherwise</returns>
		bool IsAuthorizedForAction(IPrincipal user, string routeName, RouteValueDictionary routeValues, HttpVerbs verb);
	}
}
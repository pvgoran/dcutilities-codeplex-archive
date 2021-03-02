using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;


namespace DigitallyCreated.Utilities.Mvc
{
	/// <summary>
	/// Extension methods that expose easy creation of <see cref="PermanentRedirectResult"/> and
	/// <see cref="PermanentRedirectToRouteResult"/> <see cref="ActionResult"/>s from any <see cref="Controller"/>
	/// </summary>
	public static class PermanentRedirectResultExtensions
	{
		/// <summary>
		/// Returns a <see cref="PermanentRedirectResult"/> that permanently redirects (HTTP return code 301)
		/// to the specified URL
		/// </summary>
		/// <param name="controller">The controller</param>
		/// <param name="url">The URL to redirect toL</param>
		/// <returns>The <see cref="PermanentRedirectResult"/></returns>
		public static PermanentRedirectResult PermanentlyRedirect(this Controller controller, string url)
		{
			return new PermanentRedirectResult(url);
		}


		/// <summary>
		/// Returns a <see cref="PermanentRedirectToRouteResult"/> that permanently redirects (HTTP return
		/// code 301) to the specified route.
		/// </summary>
		/// <param name="controller">The controller</param>
		/// <param name="routeName">The name of the route to use</param>
		/// <returns>The <see cref="PermanentRedirectToRouteResult"/></returns>
		public static PermanentRedirectToRouteResult PermanentlyRedirectToRoute(this Controller controller, string routeName)
		{
			return controller.PermanentlyRedirectToRoute(routeName, null);
		}


		/// <summary>
		/// Returns a <see cref="PermanentRedirectToRouteResult"/> that permanently redirects (HTTP return
		/// code 301) to the specified route.
		/// </summary>
		/// <param name="controller">The controller</param>
		/// <param name="routeValues">The values of the route to use</param>
		/// <returns>The <see cref="PermanentRedirectToRouteResult"/></returns>
		public static PermanentRedirectToRouteResult PermanentlyRedirectToRoute(this Controller controller, object routeValues)
		{
			return controller.PermanentlyRedirectToRoute(new RouteValueDictionary(routeValues));
		}


		/// <summary>
		/// Returns a <see cref="PermanentRedirectToRouteResult"/> that permanently redirects (HTTP return
		/// code 301) to the specified route.
		/// </summary>
		/// <param name="controller">The controller</param>
		/// <param name="routeValues">The values of the route to use</param>
		/// <returns>The <see cref="PermanentRedirectToRouteResult"/></returns>
		public static PermanentRedirectToRouteResult PermanentlyRedirectToRoute(this Controller controller, RouteValueDictionary routeValues)
		{
			return controller.PermanentlyRedirectToRoute(null, routeValues);
		}


		/// <summary>
		/// Returns a <see cref="PermanentRedirectToRouteResult"/> that permanently redirects (HTTP return
		/// code 301) to the specified route.
		/// </summary>
		/// <param name="controller">The controller</param>
		/// <param name="routeName">The name of the route to use</param>
		/// <param name="routeValues">The values of the route to use</param>
		/// <returns>The <see cref="PermanentRedirectToRouteResult"/></returns>
		public static PermanentRedirectToRouteResult PermanentlyRedirectToRoute(this Controller controller, string routeName, object routeValues)
		{
			return controller.PermanentlyRedirectToRoute(routeName, new RouteValueDictionary(routeValues));
		}


		/// <summary>
		/// Returns a <see cref="PermanentRedirectToRouteResult"/> that permanently redirects (HTTP return
		/// code 301) to the specified route.
		/// </summary>
		/// <param name="controller">The controller</param>
		/// <param name="routeName">The name of the route to use</param>
		/// <param name="routeValues">The values of the route to use</param>
		/// <returns>The <see cref="PermanentRedirectToRouteResult"/></returns>
		public static PermanentRedirectToRouteResult PermanentlyRedirectToRoute(this Controller controller, string routeName, RouteValueDictionary routeValues)
		{
			return new PermanentRedirectToRouteResult(routeName, routeValues);
		}


		/// <summary>
		/// Returns a <see cref="PermanentRedirectToRouteResult"/> that permanently redirects (HTTP return
		/// code 301) to the specified action.
		/// </summary>
		/// <param name="controller">The controller</param>
		/// <param name="actionName">The name of the action to use</param>
		/// <returns>The <see cref="PermanentRedirectToRouteResult"/></returns>
		public static PermanentRedirectToRouteResult PermanentlyRedirectToAction(this Controller controller, string actionName)
		{
			return controller.PermanentlyRedirectToAction(actionName, (RouteValueDictionary)null);
		}


		/// <summary>
		/// Returns a <see cref="PermanentRedirectToRouteResult"/> that permanently redirects (HTTP return
		/// code 301) to the specified action.
		/// </summary>
		/// <param name="controller">The controller</param>
		/// <param name="actionName">The name of the action to use</param>
		/// <param name="routeValues">The values of the route to use</param>
		/// <returns>The <see cref="PermanentRedirectToRouteResult"/></returns>
		public static PermanentRedirectToRouteResult PermanentlyRedirectToAction(this Controller controller, string actionName, object routeValues)
		{
			return controller.PermanentlyRedirectToAction(actionName, new RouteValueDictionary(routeValues));
		}


		/// <summary>
		/// Returns a <see cref="PermanentRedirectToRouteResult"/> that permanently redirects (HTTP return
		/// code 301) to the specified action.
		/// </summary>
		/// <param name="controller">The controller</param>
		/// <param name="actionName">The name of the action to use</param>
		/// <param name="routeValues">The values of the route to use</param>
		/// <returns>The <see cref="PermanentRedirectToRouteResult"/></returns>
		public static PermanentRedirectToRouteResult PermanentlyRedirectToAction(this Controller controller, string actionName, RouteValueDictionary routeValues)
		{
			return controller.PermanentlyRedirectToAction(actionName, null, routeValues);
		}


		/// <summary>
		/// Returns a <see cref="PermanentRedirectToRouteResult"/> that permanently redirects (HTTP return
		/// code 301) to the specified action.
		/// </summary>
		/// <param name="controller">The controller</param>
		/// <param name="actionName">The name of the action to use</param>
		/// <param name="controllerName">The name of the controller that has the specified action</param>
		/// <returns>The <see cref="PermanentRedirectToRouteResult"/></returns>
		public static PermanentRedirectToRouteResult PermanentlyRedirectToAction(this Controller controller, string actionName, string controllerName)
		{
			return controller.PermanentlyRedirectToAction(actionName, controllerName, null);
		}


		/// <summary>
		/// Returns a <see cref="PermanentRedirectToRouteResult"/> that permanently redirects (HTTP return
		/// code 301) to the specified action.
		/// </summary>
		/// <param name="controller">The controller</param>
		/// <param name="actionName">The name of the action to use</param>
		/// <param name="controllerName">The name of the controller that has the specified action</param>
		/// <param name="routeValues">The values of the route to use</param>
		/// <returns>The <see cref="PermanentRedirectToRouteResult"/></returns>
		public static PermanentRedirectToRouteResult PermanentlyRedirectToAction(this Controller controller, string actionName, string controllerName, object routeValues)
		{
			return controller.PermanentlyRedirectToAction(actionName, controllerName, new RouteValueDictionary(routeValues));
		}


		/// <summary>
		/// Returns a <see cref="PermanentRedirectToRouteResult"/> that permanently redirects (HTTP return
		/// code 301) to the specified action.
		/// </summary>
		/// <param name="controller">The controller</param>
		/// <param name="actionName">The name of the action to use</param>
		/// <param name="controllerName">The name of the controller that has the specified action</param>
		/// <param name="routeValues">The values of the route to use</param>
		/// <returns>The <see cref="PermanentRedirectToRouteResult"/></returns>
		public static PermanentRedirectToRouteResult PermanentlyRedirectToAction(this Controller controller, string actionName, string controllerName, RouteValueDictionary routeValues)
		{
			RouteValueDictionary routeValueDictionary = new RouteValueDictionary();
			
			//Use the implicit route values
			if (controller.RouteData != null)
			{
				if (controller.RouteData.Values.ContainsKey("action"))
					routeValueDictionary["action"] = controller.RouteData.Values["action"];

				if (controller.RouteData.Values.ContainsKey("controller"))
					routeValueDictionary["controller"] = controller.RouteData.Values["controller"];
			}
			
			foreach (KeyValuePair<string, object> keyValuePair in routeValues)
				routeValueDictionary[keyValuePair.Key] = keyValuePair.Value;

			if (actionName != null)
				routeValueDictionary["action"] = actionName;
			if (controllerName != null)
				routeValueDictionary["controller"] = controllerName;

			return new PermanentRedirectToRouteResult(routeValueDictionary);
		}
	}
}
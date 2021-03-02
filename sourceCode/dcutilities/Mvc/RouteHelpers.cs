using System;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;


namespace DigitallyCreated.Utilities.Mvc
{
	/// <summary>
	/// Methods that help with working with ASP.NET MVC routes
	/// </summary>
	public static class RouteHelpers
	{
		/// <summary>
		/// Creates a route to the current page
		/// </summary>
		/// <param name="url">The <see cref="UrlHelper"/></param>
		/// <returns>The route</returns>
		public static RouteValueDictionary RouteToCurrentPage(this UrlHelper url)
		{
			RouteValueDictionary routeValues = new RouteValueDictionary(url.RequestContext.RouteData.Values);
			NameValueCollection queryString = url.RequestContext.HttpContext.Request.QueryString;

			foreach (string key in queryString.Cast<string>())
			{
				routeValues[key] = queryString[key];
			}

			return routeValues;
		}


		/// <summary>
		/// Removes a particular component of the route from the route dictionary.
		/// </summary>
		/// <remarks>
		/// This method performs the same function as <see cref="RouteValueDictionary.Remove"/>
		/// but in a fluent-syntax manner, allowing you to chain the method calls
		/// </remarks>
		/// <param name="routeValues">The <see cref="RouteValueDictionary"/></param>
		/// <param name="key">The key to remove</param>
		/// <returns><paramref name="routeValues"/> for fluent syntax</returns>
		public static RouteValueDictionary Exclude(this RouteValueDictionary routeValues, string key)
		{
			routeValues.Remove(key);
			return routeValues;
		}


		/// <summary>
		/// Adds a particular component of the route to the route dictionary.
		/// </summary>
		/// <remarks>
		/// This method performs the same function as <see cref="RouteValueDictionary.Add"/>
		/// but in a fluent-syntax manner, allowing you to chain the method calls
		/// </remarks>
		/// <param name="routeValues">The <see cref="RouteValueDictionary"/></param>
		/// <param name="key">The key to add</param>
		/// <param name="value">The value to add</param>
		/// <returns><paramref name="routeValues"/> for fluent syntax</returns>
		public static RouteValueDictionary Include(this RouteValueDictionary routeValues, string key, object value)
		{
			routeValues.Add(key, value);
			return routeValues;
		}


		/// <summary>
		/// Adds a particular component of the route to the route dictionary, replacing any existing
		/// component with the same key (if it exists).
		/// </summary>
		/// <param name="routeValues">The <see cref="RouteValueDictionary"/></param>
		/// <param name="key">The key to add</param>
		/// <param name="value">The value to add</param>
		/// <returns><paramref name="routeValues"/> for fluent syntax</returns>
		public static RouteValueDictionary Replace(this RouteValueDictionary routeValues, string key, object value)
		{
			routeValues[key] = value;
			return routeValues;
		}


		/// <summary>
		/// Returns the virtual file path that corresponds to the specified physical file path.
		/// </summary>
		/// <param name="server">The <see cref="HttpServerUtilityBase"/></param>
		/// <param name="physicalPath">The physical file path</param>
		/// <returns>The virtual file path</returns>
		/// <exception cref="ArgumentException">
		/// If <paramref name="physicalPath"/> does not lie within the current application's virtual directory
		/// structure.
		/// </exception>
		public static string MapPhysicalToVirtualPath(this HttpServerUtilityBase server, string physicalPath)
		{
			string virtualRoot = server.MapPath("~");
			physicalPath = physicalPath.Trim();

			if (physicalPath.IndexOf(virtualRoot, StringComparison.InvariantCultureIgnoreCase) != 0)
				throw new ArgumentException("physicalPath does not lie within the current application's virtual directory structure");

			StringBuilder builder = new StringBuilder(physicalPath);
			builder.Remove(0, virtualRoot.Length);
			builder.Replace(@"\", "/");
			builder.Insert(0, "/");
			builder.ToString();

			return builder.ToString();
		}


		/// <summary>
		/// Returns the virtual file path that corresponds to the specified physical file path.
		/// </summary>
		/// <param name="server">The <see cref="HttpServerUtility"/></param>
		/// <param name="physicalPath">The physical file path</param>
		/// <returns>The virtual file path</returns>
		/// <exception cref="ArgumentException">
		/// If <paramref name="physicalPath"/> does not lie within the current application's virtual directory
		/// structure.
		/// </exception>
		public static string MapPhysicalToVirtualPath(this HttpServerUtility server, string physicalPath)
		{
			return new HttpServerUtilityWrapper(server).MapPhysicalToVirtualPath(physicalPath);
		}


		/// <summary>
		/// Checks whether the current user is authorized to access the page at the end of the specified route.
		/// </summary>
		/// <remarks>
		///	<para>
		/// <b>WARNING:</b> This method can be brittle as ASP.NET MVC doesn't really support the finding of an
		/// action method outside of an HTTP Request. This method mocks a fake request in order to determine
		/// authorization rights. If you get a <see cref="NotImplementedException"/>, you're
		/// probably doing something unexpected and unsupported with these mock request objects. In this case,
		/// you can either submit a patch to fix it, or stop using this method.
		/// </para>
		/// <para>
		/// This method requires you to be either using normal <see cref="Controller"/> derived controllers
		/// that use the default <see cref="ControllerActionInvoker"/> as their <see cref="IActionInvoker"/>,
		/// or that your controllers implement <see cref="IActionAuthorizationTestable"/>.
		/// </para>
		/// </remarks>
		/// <param name="url">The <see cref="UrlHelper"/></param>
		/// <param name="routeValues">The route values</param>
		/// <param name="requestVerb">The verb you will make the proposed request with</param>
		/// <returns>True if the user is authorized to view that page, or false if not.</returns>
		/// <exception cref="NotImplementedException">
		/// If you do something unexpected with the mock request objects in your controller and its filter attributes.
		/// </exception>
		/// <exception cref="NotSupportedException">
		/// If the controller for the specified route is not derived from <see cref="Controller"/> and does not
		/// use the default <see cref="ControllerActionInvoker"/> as its <see cref="IActionInvoker"/>, OR the controller
		/// does not implement <see cref="IActionAuthorizationTestable"/>.
		/// </exception>
		[Obsolete("Too unstable for production use. Will be removed in future versions")]
		public static bool IsCurrentUserAuthorizedForRoute(this UrlHelper url, object routeValues, HttpVerbs requestVerb)
		{
			return url.IsCurrentUserAuthorizedForRoute(new RouteValueDictionary(routeValues), requestVerb);
		}


		/// <summary>
		/// Checks whether the current user is authorized to access the page at the end of the specified route.
		/// </summary>
		/// <remarks>
		///	<para>
		/// <b>WARNING:</b> This method can be brittle as ASP.NET MVC doesn't really support the finding of an
		/// action method outside of an HTTP Request. This method mocks a fake request in order to determine
		/// authorization rights. If you get a <see cref="NotImplementedException"/>, you're
		/// probably doing something unexpected and unsupported with these mock request objects. In this case,
		/// you can either submit a patch to fix it, or stop using this method.
		/// </para>
		/// <para>
		/// This method requires you to be either using normal <see cref="Controller"/> derived controllers
		/// that use the default <see cref="ControllerActionInvoker"/> as their <see cref="IActionInvoker"/>,
		/// or that your controllers implement <see cref="IActionAuthorizationTestable"/>.
		/// </para>
		/// </remarks>
		/// <param name="url">The <see cref="UrlHelper"/></param>
		/// <param name="routeValues">The route values</param>
		/// <param name="requestVerb">The verb you will make the proposed request with</param>
		/// <returns>True if the user is authorized to view that page, or false if not.</returns>
		/// <exception cref="NotImplementedException">
		/// If you do something unexpected with the mock request objects in your controller and its filter attributes.
		/// </exception>
		/// <exception cref="NotSupportedException">
		/// If the controller for the specified route is not derived from <see cref="Controller"/> and does not
		/// use the default <see cref="ControllerActionInvoker"/> as its <see cref="IActionInvoker"/>, OR the controller
		/// does not implement <see cref="IActionAuthorizationTestable"/>.
		/// </exception>
		[Obsolete("Too unstable for production use. Will be removed in future versions")]
		public static bool IsCurrentUserAuthorizedForRoute(this UrlHelper url, RouteValueDictionary routeValues, HttpVerbs requestVerb)
		{
			return url.IsCurrentUserAuthorizedForRoute(null, routeValues, requestVerb);
		}


		/// <summary>
		/// Checks whether the current user is authorized to access the page at the end of the specified route.
		/// </summary>
		/// <remarks>
		///	<para>
		/// <b>WARNING:</b> This method can be brittle as ASP.NET MVC doesn't really support the finding of an
		/// action method outside of an HTTP Request. This method mocks a fake request in order to determine
		/// authorization rights. If you get a <see cref="NotImplementedException"/>, you're
		/// probably doing something unexpected and unsupported with these mock request objects. In this case,
		/// you can either submit a patch to fix it, or stop using this method.
		/// </para>
		/// <para>
		/// This method requires you to be either using normal <see cref="Controller"/> derived controllers
		/// that use the default <see cref="ControllerActionInvoker"/> as their <see cref="IActionInvoker"/>,
		/// or that your controllers implement <see cref="IActionAuthorizationTestable"/>.
		/// </para>
		/// </remarks>
		/// <param name="url">The <see cref="UrlHelper"/></param>
		/// <param name="routeName">The name of the route to use</param>
		/// <param name="routeValues">The route values</param>
		/// <param name="requestVerb">The verb you will make the proposed request with</param>
		/// <returns>True if the user is authorized to view that page, or false if not.</returns>
		/// <exception cref="NotImplementedException">
		/// If you do something unexpected with the mock request objects in your controller and its filter attributes.
		/// </exception>
		/// <exception cref="NotSupportedException">
		/// If the controller for the specified route is not derived from <see cref="Controller"/> and does not
		/// use the default <see cref="ControllerActionInvoker"/> as its <see cref="IActionInvoker"/>, OR the controller
		/// does not implement <see cref="IActionAuthorizationTestable"/>.
		/// </exception>
		[Obsolete("Too unstable for production use. Will be removed in future versions")]
		public static bool IsCurrentUserAuthorizedForRoute(this UrlHelper url, string routeName, RouteValueDictionary routeValues, HttpVerbs requestVerb)
		{
			string controllerName, actionName;
			
			if (routeValues.ContainsKey("controller"))
				controllerName = (string)routeValues["controller"];
			else
				controllerName = url.RequestContext.RouteData.GetRequiredString("controller");

			if (routeValues.ContainsKey("action"))
				actionName = (string)routeValues["action"];
			else
				actionName = url.RequestContext.RouteData.GetRequiredString("action");
			
			IControllerFactory controllerFactory = ControllerBuilder.Current.GetControllerFactory();
			FakeHttpContext httpContext = new FakeHttpContext(url.RouteUrl(routeName, routeValues), url.RequestContext.HttpContext.User, requestVerb);
			RequestContext requestContext = new RequestContext(httpContext, url.RouteCollection.GetRouteData(httpContext));
			IController controller = controllerFactory.CreateController(requestContext, controllerName);
			if (controller == null)
				throw new InvalidOperationException("The ControllerFactory did not return a Controller!");
			try
			{
				IActionAuthorizationTestable testableController = controller as IActionAuthorizationTestable;
				Controller controllerImpl = controller as Controller;
				if (testableController == null && controllerImpl != null && controllerImpl.ActionInvoker.GetType() == typeof(ControllerActionInvoker))
					return new ExposedControllerActionInvoker().IsAuthorizedForAction(new ControllerContext(requestContext, controllerImpl), actionName);

				if (testableController == null)
					throw new NotSupportedException("Cannot look up the action method called by the route specified.\r\nThe ControllerFactory did not create an IController inherited from type Controller that uses a ControllerActionInvoker.\r\nTo overcome this, implement the IActionAuthorizationTestable interface on your controllers.");

				return testableController.IsAuthorizedForAction(httpContext.User, routeName, routeValues, requestVerb);
			}
			finally
			{
				controllerFactory.ReleaseController(controller);
			}
		}


		/// <summary>
		/// Mocked <see cref="HttpContextBase"/>
		/// </summary>
		private class FakeHttpContext : HttpContextBase
		{
			private IPrincipal _User;
			private readonly HttpRequestBase _Request;
			private readonly HttpResponseBase _Response;


			public FakeHttpContext(string url, IPrincipal user, HttpVerbs verb)
			{
				_User = user;
				_Request = new FakeHttpRequest(url, verb);
				_Response = new FakeHttpResponse();
			}


			public override HttpRequestBase Request { get { return _Request; } }

			public override IPrincipal User
			{
				get { return _User; }
				set { _User = value; }
			}

			public override HttpResponseBase Response
			{
				get { return _Response; }
			}
		}


		/// <summary>
		/// Mocked <see cref="HttpRequestBase"/>
		/// </summary>
		private class FakeHttpRequest : HttpRequestBase
		{
			private readonly HttpVerbs _Verb;
			private readonly string _Url;


			public FakeHttpRequest(string url, HttpVerbs verb)
			{
				_Verb = verb;
				_Url = "~" + url;
			}


			public override string PathInfo
			{
				get { return String.Empty; }
			}

			public override string AppRelativeCurrentExecutionFilePath
			{
				get { return _Url; }
			}

			public override string HttpMethod
			{
				get { return _Verb.ToString(); }
			}
		}


		/// <summary>
		/// Mocked <see cref="HttpResponseBase"/>
		/// </summary>
		private class FakeHttpResponse : HttpResponseBase
		{
			private HttpCachePolicyBase _Cache;


			public FakeHttpResponse()
			{
				_Cache = new FakeHttpCachePolicy();
			}

			public override HttpCachePolicyBase Cache
			{
				get { return _Cache; }
			}
		}


		/// <summary>
		/// Mocked <see cref="HttpCachePolicyBase"/>
		/// </summary>
		private class FakeHttpCachePolicy : HttpCachePolicyBase
		{
			public override void SetProxyMaxAge(TimeSpan delta)
			{
			}

			public override void AddValidationCallback(HttpCacheValidateHandler handler, object data)
			{
			}
		}


		/// <summary>
		/// Hacked <see cref="ControllerActionInvoker"/> to allow the choosing of an action method authorization testing
		/// to be done by
		/// <see cref="RouteHelpers.IsCurrentUserAuthorizedForRoute(System.Web.Mvc.UrlHelper,object,System.Web.Mvc.HttpVerbs)"/>
		/// </summary>
		private class ExposedControllerActionInvoker : ControllerActionInvoker
		{
			public bool IsAuthorizedForAction(ControllerContext controllerContext, string actionName)
			{
				ControllerDescriptor controllerDescriptor = GetControllerDescriptor(controllerContext);
				ActionDescriptor actionDescriptor = FindAction(controllerContext, controllerDescriptor, actionName);

				//If the action method can't be found, say yes to authorization... any link will 404 anyway
				if (actionDescriptor == null)
					return true;

				FilterInfo filters = GetFilters(controllerContext, actionDescriptor);
				AuthorizationContext authorizationContext = InvokeAuthorizationFilters(controllerContext, filters.AuthorizationFilters, actionDescriptor);
				return authorizationContext.Result == null;
			}
		}
	}
}
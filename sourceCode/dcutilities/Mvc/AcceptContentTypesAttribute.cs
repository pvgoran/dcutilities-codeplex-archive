using System;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Linq;


namespace DigitallyCreated.Utilities.Mvc
{
	/// <summary>
	/// Test whether the request's (<see cref="HttpRequest.ContentType"/>) is any of the specified content types.
	/// If so, the action method is allowed to be selected for binding.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public class AcceptContentTypesAttribute : ActionMethodSelectorAttribute
	{
		private readonly string[] _ContentTypes;


		/// <summary>
		/// The content types that the action method is allowed to accept
		/// </summary>
		public string[] ContentTypes
		{
			get { return _ContentTypes; }
		}


		/// <summary>
		/// Constructor, creates a <see cref="AcceptContentTypesAttribute"/>
		/// </summary>
		/// <param name="contentTypes">The content types that the action method is allowed to accept</param>
		public AcceptContentTypesAttribute(params string[] contentTypes)
		{
			if (contentTypes == null)
				throw new ArgumentNullException("contentTypes");
			if (contentTypes.Length == 0)
				throw new ArgumentException("You must specify at least one content type", "contentTypes");

			_ContentTypes = contentTypes;
		}


		/// <summary>
		/// Determines whether the specified <see cref="ControllerContext"/> can be bound to the current action
		/// method.
		/// </summary>
		/// <param name="controllerContext">The controller context.</param>
		/// <param name="methodInfo">The current action method's <see cref="MethodInfo"/></param>
		/// <returns>
		/// True if the current request can be bound to the specified action method; otherwise, false.
		/// </returns>
		public override bool IsValidForRequest(ControllerContext controllerContext, MethodInfo methodInfo)
		{
			return ContentTypes.Contains(controllerContext.HttpContext.Request.ContentType, StringComparer.InvariantCultureIgnoreCase);
		}
	}
}
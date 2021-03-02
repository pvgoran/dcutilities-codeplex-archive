using System;
using System.Reflection;
using System.Web;
using System.Web.Mvc;


namespace DigitallyCreated.Utilities.Mvc
{
	/// <summary>
	/// Tests whether the request has the specified parameter set (from <see cref="HttpRequestBase.Params"/> or 
	/// <see cref="HttpRequestBase.Files"/>) and only allows binding to the action method if this is NOT so.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public class RequestMustNotHaveParamAttribute : ActionMethodSelectorAttribute
	{
		private readonly string _Name;


		/// <summary>
		/// Constructor, creates a <see cref="RequestMustNotHaveParamAttribute"/>
		/// </summary>
		/// <param name="paramName">
		/// The name of the parameter that must be set (from <see cref="HttpRequestBase.Params"/> or 
		/// <see cref="HttpRequestBase.Files"/>)
		/// </param>
		/// <exception cref="ArgumentException"><paramref name="paramName"/> must not be null or empty</exception>
		public RequestMustNotHaveParamAttribute(string paramName)
		{
			if (String.IsNullOrEmpty(paramName))
				throw new ArgumentException("paramName must not be null or empty", paramName);

			_Name = paramName;
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
			return controllerContext.HttpContext.Request.Params[_Name] == null &&
			       controllerContext.HttpContext.Request.Files[_Name] == null;
		}
	}
}
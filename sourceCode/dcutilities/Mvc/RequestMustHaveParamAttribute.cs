using System;
using System.Reflection;
using System.Web;
using System.Web.Mvc;


namespace DigitallyCreated.Utilities.Mvc
{
	/// <summary>
	/// Tests whether the request has the specified parameter set (from <see cref="HttpRequestBase.Params"/> or 
	/// <see cref="HttpRequestBase.Files"/>) and optionally set with a specific value, and only allows binding to
	/// the action method if this is so.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public class RequestMustHaveParamAttribute : ActionMethodSelectorAttribute
	{
		private readonly string _Name;


		/// <summary>
		/// The value the parameter must be, or null if you don't care what the value is
		/// </summary>
		public string Value { get; set; }


		/// <summary>
		/// Constructor, creates a <see cref="RequestMustHaveParamAttribute"/>
		/// </summary>
		/// <param name="paramName">
		/// The name of the parameter that must be set (from <see cref="HttpRequestBase.Params"/> or 
		/// <see cref="HttpRequestBase.Files"/>)
		/// </param>
		/// <exception cref="ArgumentException"><paramref name="paramName"/> must not be null or empty</exception>
		public RequestMustHaveParamAttribute(string paramName)
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
			if (controllerContext.HttpContext.Request.Params[_Name] == null && controllerContext.HttpContext.Request.Files[_Name] == null)
				return false;

			if (Value != null)
				return controllerContext.HttpContext.Request.Params[_Name] == Value;

			return true;
		}
	}
}
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;


namespace DigitallyCreated.Utilities.Mvc
{
	/// <summary>
	/// This <see cref="ActionResult"/> serialises objects to JSON to the response using a 
	/// <see cref="JavaScriptSerializer"/>.
	/// </summary>
	/// <remarks>
	///	This is a better version of <see cref="System.Web.Mvc.JsonResult"/> because it allows you to
	///	register <see cref="JavaScriptConverter"/>s with the underlying <see cref="JavaScriptSerializer"/>,
	/// which gives you control over how your objects are serialised.
	/// </remarks>
	public class JsonResult : ActionResult
	{
		/// <summary>
		/// The content encoding to use for the response stream
		/// </summary>
		public Encoding ContentEncoding { get; set; }

		/// <summary>
		/// The MIME type of the content in the response stream (application/json is used by default)
		/// </summary>
		public string ContentType { get; set; }

		/// <summary>
		/// The data to serialise to the response stream in JSON
		/// </summary>
		public object Data { get; set; }

		/// <summary>
		/// The custom <see cref="JavaScriptConverter"/>s to add to the <see cref="JavaScriptSerializer"/>
		/// </summary>
		public IEnumerable<JavaScriptConverter> CustomConverters { get; set; }


		/// <summary>
		/// Constructor, creates a <see cref="JsonResult"/>
		/// </summary>
		public JsonResult()
		{
			ContentType = "application/json";
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

			HttpResponseBase response = context.HttpContext.Response;

			if (String.IsNullOrEmpty(ContentType) == false)
				response.ContentType = ContentType;
			else
				response.ContentType = "application/json";

			if (ContentEncoding != null)
				response.ContentEncoding = ContentEncoding;

			if (Data == null) 
				return;

			JavaScriptSerializer serializer = new JavaScriptSerializer();
			if (CustomConverters != null)
				serializer.RegisterConverters(CustomConverters);
			response.Write(serializer.Serialize(Data));
		}
	}
}
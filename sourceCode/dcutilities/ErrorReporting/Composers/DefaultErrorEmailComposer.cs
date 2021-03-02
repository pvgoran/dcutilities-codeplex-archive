using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using DigitallyCreated.Utilities.ErrorReporting.DetailProviders;
using DigitallyCreated.Utilities.ErrorReporting.TypeRenderers;


namespace DigitallyCreated.Utilities.ErrorReporting.Composers
{
	/// <summary>
	/// This class implements <see cref="IErrorEmailComposer"/> and is able
	/// to create the subject and body of an error report email
	/// </summary>
	public class DefaultErrorEmailComposer : IErrorEmailComposer
	{
		private readonly ICollection<IDetailProvider> _DetailProviders;
		private readonly IList<ITypeRenderer> _TypeRenderers;


		/// <summary>
		/// Indicates whether the email bodies that are created by this composer
		/// are HTML or plain text.
		/// </summary>
		public bool IsHtmlEmail
		{
			get { return true; }
		}


		/// <summary>
		/// Collection in which the <see cref="IDetailProvider"/> objects that
		/// this composer can use should be placed.
		/// </summary>
		public ICollection<IDetailProvider> DetailProviders
		{
			get { return _DetailProviders; }
		}


		/// <summary>
		/// An <see cref="IDetailProvider"/> that will only be used by this composer
		/// in the case that none of the detail providers in <see cref="IErrorEmailComposer.DetailProviders"/>
		/// can provide details for an object
		/// </summary>
		public IDetailProvider FallbackDetailProvider { get; set; }


		/// <summary>
		/// List in which the <see cref="ITypeRenderer"/> objects that this
		/// composer can use should be placed.
		/// </summary>
		/// <remarks>
		/// The first type renderer found in this list that can render the detail
		/// is used. This means renderers for more specific types should be placed 
		/// higher up in the list than less specific types. For example, a renderer
		/// for an Apple class should be placed above a renderer for a Fruit class,
		/// otherwise Apples will be always be rendered as Fruit.
		/// </remarks>
		public IList<ITypeRenderer> TypeRenderers
		{
			get { return _TypeRenderers; }
		}


		/// <summary>
		/// Constructor, creates an <see cref="DefaultErrorEmailComposer"/> object
		/// </summary>
		public DefaultErrorEmailComposer()
		{
			_DetailProviders = new List<IDetailProvider>();
			_TypeRenderers = new List<ITypeRenderer>();

			_TypeRenderers.Add(new HtmlNullTypeRenderer());
			_TypeRenderers.Add(new HtmlEncodedEnumTypeRenderer());
			_TypeRenderers.Add(new HtmlEncodedStringTypeRenderer());
			_TypeRenderers.Add(new BooleanTypeRenderer());
			_TypeRenderers.Add(new AlphanumericTypeRenderer());
			_TypeRenderers.Add(new HtmlDictionaryTypeRenderer());
			_TypeRenderers.Add(new HtmlNameValueCollectionTypeRenderer());
			_TypeRenderers.Add(new HtmlEnumerableTypeRenderer());
			_TypeRenderers.Add(new ObjectTypeRenderer());

			_DetailProviders.Add(new HttpExceptionDetailProvider());
			_DetailProviders.Add(new ExceptionDetailProvider());

			FallbackDetailProvider = new ReflectionFallbackDetailProvider();
		}


		/// <summary>
		/// Creates the body of an error report email
		/// </summary>
		/// <param name="errorObject">
		/// An object that describes the error (for example, an exception) or null
		/// </param>
		/// <param name="severity">The severity of the error</param>
		/// <param name="title">The title of the error</param>
		/// <param name="message">The programmer's message, or null if one wasn't specified</param>
		/// <param name="extraErrorDetails">
		/// A collection of extra details about the error where the key in the <see cref="KeyValuePair{TKey,TValue}"/> 
		/// is the detail's name (ie Current Page) and the value is the object that is the detail (ie a string
		/// with the page URL).
		/// </param>
		/// <returns>The email body</returns>
		public string ComposeEmailBody(object errorObject, ErrorSeverity severity, string title, string message, IEnumerable<KeyValuePair<string, object>> extraErrorDetails)
		{
			StringBuilder builder = new StringBuilder();

			builder.Append(@"
			<!DOCTYPE html PUBLIC '-//W3C//DTD XHTML 1.0 Strict//EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd'>
			<html xmlns='http://www.w3.org/1999/xhtml'>
			<head>
				<title>Onset Website Error Reporting</title>
			</head>
			<body>
			");

			if (errorObject is Exception)
				WriteException(builder, (Exception)errorObject, severity, title, message, extraErrorDetails);
			else
				WriteError(builder, errorObject, severity, title, message, extraErrorDetails);

			builder.Append(@"
			</body>
			</html>
			");

			return builder.ToString();
		}


		/// <summary>
		/// Creates the subject of an error report email
		/// </summary>
		/// <param name="errorObject">
		/// An object that describes the error (for example, an exception) or null
		/// </param>
		/// <param name="severity">The severity of the error</param>
		/// <param name="title">The title of the error</param>
		/// <param name="message">The programmer's message, or null if one wasn't specified</param>
		/// <returns>The email subject</returns>
		public string ComposeEmailSubject(object errorObject, ErrorSeverity severity, string title, string message)
		{
			return "[" + severity + "] " + title;
		}


		/// <summary>
		/// Writes an error's HTML
		/// </summary>
		/// <param name="builder">The <see cref="StringBuilder"/> to write into</param>
		/// <param name="errorObject"></param>
		/// <param name="severity">The severity of the error</param>
		/// <param name="title">The title of the error</param>
		/// <param name="message">The programmer's message, or null if one wasn't specified</param>
		/// <param name="extraErrorDetails">
		/// A collection of extra details about the error where the key in the <see cref="KeyValuePair{TKey,TValue}"/> 
		/// is the detail's name (ie Current Page) and the value is the object that is the detail (ie a string
		/// with the page URL).
		/// </param>
		private void WriteError(StringBuilder builder, object errorObject, ErrorSeverity severity, string title, string message, IEnumerable<KeyValuePair<string, object>> extraErrorDetails)
		{
			ErrorTableRenderer.RenderTableStart(builder);
			ErrorTableRenderer.RenderTableHeading(builder, b => b.Append(HttpUtility.HtmlEncode(title)));
			ErrorTableRenderer.RenderKeyValueRow(builder, b => b.Append("Object Type"), b => b.Append(errorObject.GetType().FullName));

			RenderDetails(builder, UtilityMethods.GetDetailsForObject(errorObject, DetailProviders, FallbackDetailProvider));

			ErrorTableRenderer.RenderKeyValueRow(builder, b => b.Append("Severity"), b => b.Append(severity.ToString()));
			if (message != null)
				ErrorTableRenderer.RenderKeyValueRow(builder, b => b.Append("Programmer Message"), b => b.Append(HttpUtility.HtmlEncode(message)));

			RenderDetails(builder, extraErrorDetails);

			ErrorTableRenderer.RenderTableEnd(builder);
		}


		/// <summary>
		/// Writes an exception's HTML
		/// </summary>
		/// <param name="builder">The <see cref="StringBuilder"/> to write into</param>
		/// <param name="e">The exception</param>
		/// <param name="severity">The severity of the error</param>
		/// <param name="title">The title of the error</param>
		/// <param name="message">The programmer's message, or null if one wasn't specified</param>
		/// <param name="extraErrorDetails">
		/// A collection of extra details about the error where the key in the <see cref="KeyValuePair{TKey,TValue}"/> 
		/// is the detail's name (ie Current Page) and the value is the object that is the detail (ie a string
		/// with the page URL).
		/// </param>
		private void WriteException(StringBuilder builder, Exception e, ErrorSeverity? severity, string title, string message, IEnumerable<KeyValuePair<string, object>> extraErrorDetails)
		{
			ErrorTableRenderer.RenderTableStart(builder);
			ErrorTableRenderer.RenderTableHeading(builder, b => b.Append(HttpUtility.HtmlEncode(title)));
			ErrorTableRenderer.RenderKeyValueRow(builder, b => b.Append("Object Type"), b => b.Append(e.GetType().FullName));

			RenderDetails(builder, UtilityMethods.GetDetailsForObject(e, DetailProviders, FallbackDetailProvider));

			if (severity != null)
				ErrorTableRenderer.RenderKeyValueRow(builder, b => b.Append("Severity"), b => b.Append(severity.ToString()));
			if (message != null)
				ErrorTableRenderer.RenderKeyValueRow(builder, b => b.Append("Programmer Message"), b => b.Append(HttpUtility.HtmlEncode(message)));
			if (e.InnerException != null)
				ErrorTableRenderer.RenderKeyValueRow(builder, b => b.Append("Inner Exception"), b => WriteException(b, e.InnerException, null, "Inner Exception", null, new KeyValuePair<string,object>[0]));

			RenderDetails(builder, extraErrorDetails);

			ErrorTableRenderer.RenderKeyValueMultiRow(builder, b => b.Append("Stack Trace"), b => b.Append("<pre>" + e.StackTrace + "</pre>"));
			ErrorTableRenderer.RenderTableEnd(builder);
		}


		/// <summary>
		/// Renders a collection of details
		/// </summary>
		/// <param name="builder">The <see cref="StringBuilder"/> to render to</param>
		/// <param name="details">
		/// A collection of details about the error where the key in the <see cref="KeyValuePair{TKey,TValue}"/> 
		/// is the detail's name (ie Current Page) and the value is the object that is the detail (ie a string
		/// with the page URL).
		/// </param>
		private void RenderDetails(StringBuilder builder, IEnumerable<KeyValuePair<string, object>> details)
		{
			RenderContext<object> baseContext = new RenderContext<object>(null, null, this);
			foreach (KeyValuePair<string, object> detailPair in details)
			{
				ITypeRenderer keyRenderer = UtilityMethods.ChooseTypeRenderer(detailPair.Key, TypeRenderers);
				ITypeRenderer valueRenderer = UtilityMethods.ChooseTypeRenderer(detailPair.Value, TypeRenderers);
				ErrorTableRenderer.RenderKeyValueRow(builder, b => keyRenderer.Render(baseContext.Copy<object>(detailPair.Key, b)), b => valueRenderer.Render(baseContext.Copy(detailPair.Value, b)));
			}
		}
	}
}
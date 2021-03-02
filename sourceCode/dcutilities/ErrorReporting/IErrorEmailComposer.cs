using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DigitallyCreated.Utilities.ErrorReporting
{

	/// <summary>
	/// The class that implements this interface is able to compose an email body and
	/// subject that contains the details of an error. This body can then be sent by 
	/// the <see cref="ErrorReporter"/>.
	/// </summary>
	public interface IErrorEmailComposer
	{
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
		string ComposeEmailBody(object errorObject, ErrorSeverity severity, string title, string message, IEnumerable<KeyValuePair<string, object>> extraErrorDetails);


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
		string ComposeEmailSubject(object errorObject, ErrorSeverity severity, string title, string message);


		/// <summary>
		/// Indicates whether the email bodies that are created by this composer
		/// are HTML or plain text.
		/// </summary>
		bool IsHtmlEmail { get; }


		/// <summary>
		/// Collection in which the <see cref="IDetailProvider"/> objects that
		/// this composer can use should be placed.
		/// </summary>
		ICollection<IDetailProvider> DetailProviders { get; }


		/// <summary>
		/// An <see cref="IDetailProvider"/> that will only be used by this composer
		/// in the case that none of the detail providers in <see cref="DetailProviders"/>
		/// can provide details for an object
		/// </summary>
		IDetailProvider FallbackDetailProvider { get; set; }


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
		IList<ITypeRenderer> TypeRenderers { get; }
	}
}

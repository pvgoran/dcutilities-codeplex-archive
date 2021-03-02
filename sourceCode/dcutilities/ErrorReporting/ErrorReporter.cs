using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using DigitallyCreated.Utilities.ErrorReporting.Composers;
using DigitallyCreated.Utilities.ErrorReporting.Configuration;


namespace DigitallyCreated.Utilities.ErrorReporting
{
	/// <summary>
	/// The Error Reporting class allows unexpected errors and exceptions to be emailed to a specific
	/// email address.
	/// </summary>
	/// <remarks>
	/// <para>
	/// To configure this class, you need to specify some settings in your app.config/web.config file.
	/// Here is an example configuration:
	/// <code><![CDATA[
	/// <configuration>
	///     <configSections>
	///         <section name="errorReporting" type="DigitallyCreated.Utilities.ErrorReporting.Configuration.ErrorReportingConfigurationSection, DigitallyCreated.Utilities.ErrorReporting"/>
	///     </configSections>
	/// 
	///     <errorReporting enabled="true">
	///         <sendFrom address="website@mywebsite.com" displayName="MyWebsite Error Reporting" />
	///         <sendTo address="developers@mywebsite.com" displayName="Developers" />
	///     </errorReporting>
	/// </configuration>
	/// ]]></code>
	/// </para>
	/// <para>
	/// You also need to configure the default .NET SMTP settings. Here is a simple example:
	/// <code><![CDATA[
	/// <configuration>
	///     ...
	///     
	///     <system.net>
	///         <mailSettings>
	///             <smtp deliveryMethod="Network">
	///                 <network defaultCredentials="true" host="mail.mywebsite.com" port="25" />
	///	            </smtp>
	///         </mailSettings>
	///     </system.net>
	/// </configuration>
	/// ]]></code>
	/// </para>
	/// </remarks>
	public class ErrorReporter
	{
		private readonly IErrorEmailComposer _Composer;
		private readonly IList<Predicate<object>> _IgnoreRules;


		/// <summary>
		/// This list contains rules that evaluate whether an error object should be
		/// ignored (and not reported). If any of the rules in this list return
		/// true, the exception will be ignored.
		/// </summary>
		public IList<Predicate<object>> IgnoreRules
		{
			get { return _IgnoreRules; }
		}


		/// <summary>
		/// Constructor, creates a <see cref="ErrorReporter"/> object that uses a
		/// <see cref="DefaultErrorEmailComposer"/> as its <see cref="IErrorEmailComposer"/>
		/// </summary>
		public ErrorReporter()
			: this(new DefaultErrorEmailComposer())
		{
		}


		/// <summary>
		/// Constructor, creates a <see cref="ErrorReporter"/> object that renders its
		/// emails using a specific <see cref="IErrorEmailComposer"/>
		/// </summary>
		/// <param name="composer">The Composer with which to render emails</param>
		public ErrorReporter(IErrorEmailComposer composer)
		{
			_Composer = composer;
			_IgnoreRules = new List<Predicate<object>>();
			ErrorReportingConfigurationSection.GetSection(); //Causes a check for the presence of configuration
		}


		/// <summary>
		/// Report an ad-hoc, non-exception error
		/// </summary>
		/// <param name="severity">The severity of the error</param>
		/// <param name="title">
		/// The title of the error (for example, Missing Configuration File)
		/// </param>
		/// <param name="message">
		/// An optional message that could provide extra details about the error
		/// </param>
		public void ReportError(ErrorSeverity severity, string title, string message)
		{
			ReportError(null, severity, message, message);
		}


		/// <summary>
		/// Report an error that involves an exception
		/// </summary>
		/// <param name="errorObject">
		/// An object that describes the error (for example, an exception) or null
		/// </param>
		/// <param name="severity">The severity of the error</param>
		/// <param name="title">
		/// The title of the error (for example, Unhandled Exception)
		/// </param>
		/// <param name="message">
		/// An optional message that could provide extra details about the error
		/// </param>
		public void ReportError(object errorObject, ErrorSeverity severity, string title, string message)
		{
			ReportError(errorObject, severity, title, message, new KeyValuePair<string, object>[0]);
		}


		/// <summary>
		/// Report an error that involves an exception
		/// </summary>
		/// <param name="errorObject">
		/// An object that describes the error (for example, an exception) or null
		/// </param>
		/// <param name="severity">The severity of the error</param>
		/// <param name="title">
		/// The title of the error (for example, Unhandled Exception)
		/// </param>
		/// <param name="message">
		/// An optional message that could provide extra details about the error
		/// </param>
		/// <param name="extraErrorDetails">
		/// A collection of extra details about the error where the key in the <see cref="KeyValuePair{TKey,TValue}"/> 
		/// is the detail's name (ie Current Page) and the value is the object that is the detail (ie a string
		/// with the page URL).
		/// </param>
		public void ReportError(object errorObject, ErrorSeverity severity, string title, string message, IEnumerable<KeyValuePair<string, object>> extraErrorDetails)
		{
			ErrorReportingConfigurationSection settings = ErrorReportingConfigurationSection.GetSection();

			if (settings.IsEnabled == false)
				return;

			if (errorObject != null)
			{
				//Check to see whether any of the Ignore rules match the exception
				foreach (Predicate<object> rule in _IgnoreRules)
				{
					if (rule(errorObject))
						return; //Matched, ignore this exception
				}
			}

			MailAddress from = new MailAddress(settings.SendFromEmail.Address, settings.SendFromEmail.DisplayName);
			MailAddress to = new MailAddress(settings.SendToEmail.Address, settings.SendToEmail.DisplayName);

			MailMessage msg = new MailMessage(from, to);
			msg.IsBodyHtml = _Composer.IsHtmlEmail;
			msg.Body = _Composer.ComposeEmailBody(errorObject, severity, title, message, extraErrorDetails);
			msg.Subject = _Composer.ComposeEmailSubject(errorObject, severity, title, message);

			SmtpClient smtp = new SmtpClient();
			smtp.Send(msg);
		}
	}
}

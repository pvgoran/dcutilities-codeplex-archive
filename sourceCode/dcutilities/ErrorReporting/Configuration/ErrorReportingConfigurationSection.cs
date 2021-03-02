using System;
using System.Configuration;
using System.Web;
using System.Web.Configuration;


namespace DigitallyCreated.Utilities.ErrorReporting.Configuration
{
	/// <summary>
	/// The <see cref="ConfigurationSection"/> for Error Reporting
	/// </summary>
	public class ErrorReportingConfigurationSection : ConfigurationSection
	{
		private const string SECTION_NAME = "errorReporting";
		private static ErrorReportingConfigurationSection _Section;
		private readonly static object _Lock = new object();


		/// <summary>
		/// Whether or not error reports should be sent. Can be used as a global off switch
		/// </summary>
		[ConfigurationProperty("enabled", DefaultValue = true, IsRequired = true)]
		public bool IsEnabled
		{
			get { return (bool)this["enabled"]; }
			set { this["enabled"] = value; }
		}


		/// <summary>
		/// The details of the email address from which the error reports should be sent
		/// </summary>
		[ConfigurationProperty("sendFrom", IsRequired = true)]
		public EmailAddressConfigurationElement SendFromEmail
		{
			get { return (EmailAddressConfigurationElement)this["sendFrom"]; }
			set { this["sendFrom"] = value; } 
		}


		/// <summary>
		/// The details of the email address from which the error reports should be sent to
		/// </summary>
		[ConfigurationProperty("sendTo", IsRequired = true)]
		public EmailAddressConfigurationElement SendToEmail
		{
			get { return (EmailAddressConfigurationElement)this["sendTo"]; }
			set { this["sendTo"] = value; }
		}

		
		/// <summary>
		/// Gets the configuration section from the web.config or app.config
		/// </summary>
		/// <returns>The configuration section</returns>
		public static ErrorReportingConfigurationSection GetSection()
		{
			if (_Section == null)
			{
				lock (_Lock)
				{
					if (_Section == null)
					{
						string configFilename;
						if (HttpContext.Current == null)
						{
							_Section = ConfigurationManager.GetSection(SECTION_NAME) as ErrorReportingConfigurationSection;
							configFilename = ".config";
						}
						else
						{
							_Section = WebConfigurationManager.GetSection(SECTION_NAME) as ErrorReportingConfigurationSection;
							configFilename = "web.config";
						}

						if (_Section == null)
							throw new ConfigurationErrorsException(String.Format("The <{0}> section is not defined in your {1} file!", SECTION_NAME, configFilename));
					}
				}
			}

			return _Section;
		}
	}
}
using System;
using System.Configuration;


namespace DigitallyCreated.Utilities.Bcl.Configuration
{
	/// <summary>
	/// The <see cref="ConfigurationSection"/> for <see cref="ICrypto"/> classes
	/// </summary>
	public class CryptoConfigurationSection : ConfigurationSection
	{
		private const string SECTION_NAME = "crypto";
		private static CryptoConfigurationSection _Section;
		private static readonly object _Lock = new object();


		/// <summary>
		/// The password. Must be at least 8 characters long
		/// </summary>
		[ConfigurationProperty("password", IsRequired = true)]
		public string Password
		{
			get { return (string)this["password"]; }
			set { this["password"] = value; }
		}


		/// <summary>
		/// Gets the configuration section from the web.config or app.config
		/// </summary>
		/// <returns>The configuration section</returns>
		public static CryptoConfigurationSection GetSection()
		{
			if (_Section == null)
			{
				lock (_Lock)
				{
					if (_Section == null)
					{
						_Section = ConfigurationManager.GetSection(SECTION_NAME) as CryptoConfigurationSection;

						if (_Section == null)
							throw new ConfigurationErrorsException(String.Format("The <{0}> section is not defined in your .config file!", SECTION_NAME));
					}
				}
			}

			return _Section;
		}
	}
}
using System.Configuration;


namespace DigitallyCreated.Utilities.ErrorReporting.Configuration
{
	/// <summary>
	/// The <see cref="ConfigurationElement"/> that contains information about an email address
	/// </summary>
	public class EmailAddressConfigurationElement : ConfigurationElement
	{
		/// <summary>
		/// The email address
		/// </summary>
		[ConfigurationProperty("address", IsKey = true, IsRequired = true)]
		public string Address
		{
			get { return (string)this["address"]; }
			set { this["address"] = value; }
		}


		/// <summary>
		/// The display name to use with the email address (ie. the address owner's real name)
		/// </summary>
		[ConfigurationProperty("displayName", IsRequired = false)]
		public string DisplayName
		{
			get { return (string)this["displayName"]; }
			set { this["displayName"] = value; }
		}
	}
}
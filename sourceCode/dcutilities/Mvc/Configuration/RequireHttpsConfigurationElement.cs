using System.Configuration;


namespace DigitallyCreated.Utilities.Mvc.Configuration
{
	/// <summary>
	/// The <see cref="ConfigurationElement"/> for the <see cref="RequireHttpsAttribute"/> and 
	/// <see cref="RequireRemoteHttpsAttribute"/>
	/// </summary>
	public class RequireHttpsConfigurationElement : ConfigurationElement
	{
		/// <summary>
		/// Whether or not the <see cref="RequireHttpsAttribute"/> and <see cref="RequireRemoteHttpsAttribute"/>
		/// are enabled or not.
		/// </summary>
		[ConfigurationProperty("enabled", DefaultValue = true, IsRequired = true)]
		public bool IsEnabled
		{
			get { return (bool)this["enabled"]; }
			set { this["enabled"] = value; }
		}
	}
}
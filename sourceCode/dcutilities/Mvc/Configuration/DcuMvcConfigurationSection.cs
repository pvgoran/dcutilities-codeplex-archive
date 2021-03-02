using System;
using System.Configuration;
using System.Web;
using System.Web.Configuration;


namespace DigitallyCreated.Utilities.Mvc.Configuration
{
	/// <summary>
	/// The configuation section for the Mvc project
	/// </summary>
	public class DcuMvcConfigurationSection : ConfigurationSection
	{
		private const string SECTION_NAME = "dcuMvc";
		private static DcuMvcConfigurationSection _Section;
		private readonly static object _Lock = new object();


		/// <summary>
		/// The <see cref="RequireHttpsConfigurationElement"/>
		/// </summary>
		[ConfigurationProperty("requireHttps", IsRequired = false)]
		public RequireHttpsConfigurationElement RequireHttps
		{
			get
			{
				if (this["requireHttps"] == null)
				{
					lock (_Lock)
					{
						if (this["requireHttps"] == null)
						{
							this["requireHttps"] = new RequireHttpsConfigurationElement();
						}
					}
					
				}

				return (RequireHttpsConfigurationElement)this["requireHttps"];
			}

			set { this["requireHttps"] = value; }
		}


		/// <summary>
		/// Gets the configuration section from the web.config or app.config
		/// </summary>
		/// <returns>The configuration section</returns>
		public static DcuMvcConfigurationSection GetSection()
		{
			if (_Section == null)
			{
				lock (_Lock)
				{
					if (_Section == null)
					{
						if (HttpContext.Current == null)
							_Section = ConfigurationManager.GetSection(SECTION_NAME) as DcuMvcConfigurationSection;
						else
							_Section = WebConfigurationManager.GetSection(SECTION_NAME) as DcuMvcConfigurationSection;
						
						if (_Section == null)
							return new DcuMvcConfigurationSection();
					}
				}
			}

			return _Section;
		}
	}
}
using System;
using System.Configuration;

namespace DigitallyCreated.Utilities.BbCode.Configuration
{
	/// <summary>
	/// The main <see cref="ConfigurationSection"/> that allows you to configure the <see cref="BbCodeRenderer"/>
	/// </summary>
	public class BbCodeConfigurationSection : ConfigurationSection
	{
		private const string SECTION_NAME = "bbCode";
		private static BbCodeConfigurationSection _Section;
		private static readonly object _Lock = new object();


		/// <summary>
		/// The collection of service interfaces that are used to register service interfaces to the
		/// container extension's configuration
		/// </summary>
		[ConfigurationProperty("tagDefinitionSets")]
		[ConfigurationCollection(typeof(TagDefinitionSetConfigurationElementCollection), AddItemName = "tagDefinitionSet")]
		public TagDefinitionSetConfigurationElementCollection TagDefinitionSets
		{
			get
			{
				return (TagDefinitionSetConfigurationElementCollection)this["tagDefinitionSets"];
			}
		}


		/// <summary>
		/// Gets the configuration section from the application's configuration file
		/// </summary>
		/// <returns>The configuration section</returns>
		public static BbCodeConfigurationSection GetSection()
		{
			if (_Section == null)
			{
				lock (_Lock)
				{
					if (_Section == null)
					{
						_Section = ConfigurationManager.GetSection(SECTION_NAME) as BbCodeConfigurationSection;
						if (_Section == null)
							throw new ConfigurationErrorsException(String.Format("The <{0}> section is not defined in your .config file!", SECTION_NAME));
					}
				}
			}

			return _Section;
		}
	}
}
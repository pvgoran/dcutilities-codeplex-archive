using System;
using System.Configuration;
using System.Xml;
using Microsoft.Practices.Unity.Configuration;


namespace DigitallyCreated.Utilities.Unity.Configuration
{
	/// <summary>
	/// A <see cref="CustomChannelFactoryFactoryConfigurationElement"/> is encountered under a
	/// <see cref="IChannelFactoryFactory"/> and is able to create a specific
	/// <see cref="ServiceInterfaceConfigurationElement"/> for use.
	/// </summary>
	public class CustomChannelFactoryFactoryConfigurationElement : TypeResolvingConfigurationElement
	{
		/// <summary>
		/// The type of the <see cref="CustomChannelFactoryFactoryConfigurationElement"/> that should
		/// be used to deserialize the element
		/// </summary>
		[ConfigurationProperty("cfgElementType", IsRequired = true)]
		public string ConfigElementType
		{
			get { return (string)base["cfgElementType"]; }
			set { base["cfgElementType"] = value; }
		}

		/// <summary>
		/// Deserialises the element
		/// </summary>
		/// <param name="reader">The <see cref="XmlReader"/> to read from</param>
		public void DeserializeElement(XmlReader reader)
		{
			base.DeserializeElement(reader, false);
		}


		/// <summary>
		/// When implemented in a subclass, creates a <see cref="IChannelFactoryFactory"/>
		/// </summary>
		/// <returns>An <see cref="IChannelFactoryFactory"/></returns>
		/// <exception cref="NotImplementedException">
		/// Always throws. Override this method in your subclass.
		/// </exception>
		public virtual IChannelFactoryFactory CreateChannelFactoryFactory()
		{
			throw new NotImplementedException("You must inherit from CustomChannelFactoryFactoryConfigurationElement and override this method");
		}
	}
}
using System;
using System.Configuration;
using System.Xml;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;


namespace DigitallyCreated.Utilities.Unity.Configuration
{
	/// <summary>
	/// A <see cref="ConfigurationElement"/> that holds information from the configuration file about a
	/// service interface type that should be registered with the <see cref="WcfProxyContainerExtension"/>
	/// </summary>
	public class ServiceInterfaceConfigurationElement : TypeResolvingConfigurationElement
	{
		/// <summary>
		/// The name of the mapping. Can be null to specify the default (unnamed) mapping
		/// </summary>
		[ConfigurationProperty("name", IsRequired = false, DefaultValue = null)]
		public string Name
		{
			get { return (string)this["name"]; }
			set { this["name"] = value; }
		}


		/// <summary>
		/// The service interface type name
		/// </summary>
		[ConfigurationProperty("type", IsRequired = true)]
		public string InterfaceTypeName
		{
			get { return (string)this["type"]; }
			set { this["type"] = value; }
		}


		/// <summary>
		/// The service interface type
		/// </summary>
		public Type InterfaceType
		{
			get
			{
				if (String.IsNullOrEmpty(InterfaceTypeName))
					return null;
				return TypeResolver.ResolveType(InterfaceTypeName);
			}
		}


		/// <summary>
		/// The type to register the service interface under in Unity's name
		/// </summary>
		[ConfigurationProperty("registerAsType", IsRequired = false)]
		public string RegisterAsTypeName
		{
			get { return (string)this["registerAsType"]; }
			set { this["registerAsType"] = value; }
		}


		/// <summary>
		/// The type to register the service interface under in Unity
		/// </summary>
		public Type RegisterAsType
		{
			get
			{
				if (String.IsNullOrEmpty(RegisterAsTypeName))
					return null;
				return TypeResolver.ResolveType(RegisterAsTypeName);
			}
		}


		/// <summary>
		/// The name of the endpoint configuration to use on the channel
		/// </summary>
		[ConfigurationProperty("endpointConfiguration")]
		public string EndpointConfigurationName
		{
			get { return (string)this["endpointConfiguration"]; }
			set { this["endpointConfiguration"] = value; }
		}


		/// <summary>
		/// The config element that can create the appropriate <see cref="IChannelFactoryFactory"/>
		/// </summary>
		/// <remarks>
		/// Deliberately not annotated with <see cref="ConfigurationPropertyAttribute"/> because
		/// any custom element needs to be caught and handled by <see cref="OnDeserializeUnrecognizedElement"/>
		/// </remarks>
		public CustomChannelFactoryFactoryConfigurationElement CustomChannelFactoryFactory { get; set; }


		/// <summary>
		/// Called when an unrecognised element is found. In this case, we're expecting to see
		/// a customChannelFactoryFactory element which we'll load manually.
		/// </summary>
		/// <param name="elementName">The name of the unknown subelement.</param>
		/// <param name="reader">The <see cref="XmlReader"/> being used for deserialization.</param>
		/// <returns>true when an unknown element is encountered while deserializing; otherwise, false.</returns>
		protected override bool OnDeserializeUnrecognizedElement(string elementName, XmlReader reader)
		{
			if (elementName == "customChannelFactoryFactory")
			{
				string typeName = reader.GetAttribute("cfgElementType");
				if (String.IsNullOrEmpty(typeName) == false)
				{
					CustomChannelFactoryFactoryConfigurationElement element = (CustomChannelFactoryFactoryConfigurationElement)Activator.CreateInstance(Type.GetType(typeName));
					element.DeserializeElement(reader);
					CustomChannelFactoryFactory = element;
					return true;
				}
			}

			return base.OnDeserializeUnrecognizedElement(elementName, reader);
		}


		/// <summary>
		/// Configures the Unity container extension with the information found in this object
		/// </summary>
		/// <param name="container">The Unity container</param>
		public void Configure(IUnityContainer container)
		{
			string name = String.IsNullOrEmpty(Name) ? null : Name;

			bool endpointConfSet = String.IsNullOrEmpty(EndpointConfigurationName) == false;
			bool customChannelFfSet = CustomChannelFactoryFactory != null;

			if (endpointConfSet == customChannelFfSet) //Shorthand for both true OR both false
				throw new ConfigurationErrorsException("You must set either the endpointConfiguration attribute or a customChannelFactoryFactory element (choose one or the other).");

			IChannelFactoryFactory channelFactoryFactory;
			if (String.IsNullOrEmpty(EndpointConfigurationName) == false)
				channelFactoryFactory = new EndpointConfigurationChannelFactoryFactory(EndpointConfigurationName);
			else if (CustomChannelFactoryFactory != null)
				channelFactoryFactory = CustomChannelFactoryFactory.CreateChannelFactoryFactory();
			else
				throw new Exception("Neither endpointConfiguration or ChannelFactoryFactory set. This should not happen.");

			if (RegisterAsType == null)
				container.Configure<IWcfProxyExtensionConfiguration>().RegisterType(InterfaceType, name, channelFactoryFactory);
			else
			{
				if (RegisterAsType.IsAssignableFrom(InterfaceType) == false)
					throw new ConfigurationErrorsException(String.Format("registerAsType ({0}) must be assignable from type ({1}})", RegisterAsType, InterfaceType));

				container.Configure<IWcfProxyExtensionConfiguration>().RegisterType(InterfaceType, RegisterAsType, name, channelFactoryFactory);
			}
				
		}
	}
}
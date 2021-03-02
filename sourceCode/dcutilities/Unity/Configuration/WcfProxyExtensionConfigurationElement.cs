using System;
using System.Configuration;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;


namespace DigitallyCreated.Utilities.Unity.Configuration
{
	/// <summary>
	/// A <see cref="ConfigurationElement"/> that holds information in the configuration file about the
	/// configuration of the <see cref="WcfProxyContainerExtension"/>
	/// </summary>
	public class WcfProxyExtensionConfigurationElement : UnityContainerExtensionConfigurationElement
	{
		/// <summary>
		/// The collection of service interfaces that are used to register service interfaces to the
		/// container extension's configuration
		/// </summary>
		[ConfigurationProperty("serviceInterfaces")]
		[ConfigurationCollection(typeof(ServiceInterfaceConfigurationElementCollection), AddItemName = "serviceInterface")]
		public ServiceInterfaceConfigurationElementCollection ServiceInterfaces
		{
			get
			{
				ServiceInterfaceConfigurationElementCollection serviceInterfaces = (ServiceInterfaceConfigurationElementCollection)base["serviceInterfaces"];
				serviceInterfaces.TypeResolver = TypeResolver;
				return serviceInterfaces;
			}
		}

		/// <summary>
		/// The name of the type of <see cref="IChannelManager"/> to use. This can be left out and
		/// a default <see cref="IChannelManager"/> will be used.
		/// </summary>
		[ConfigurationProperty("channelManagerType", IsRequired = false, DefaultValue = null)]
		public string ChannelManagerTypeName
		{
			get { return (string)this["channelManagerType"]; }
			set { this["channelManagerType"] = value; }
		}

		/// <summary>
		/// Gets the Type of <see cref="IChannelManager"/> to use
		/// </summary>
		public Type ChannelManagerType
		{
			get
			{
				if (String.IsNullOrEmpty(ChannelManagerTypeName))
					return null;
				
				return TypeResolver.ResolveType(ChannelManagerTypeName);
			}
		}


		/// <summary>
		/// Configures the Unity container extension with the information found in this object
		/// </summary>
		/// <param name="container">The Unity Container</param>
		public override void Configure(IUnityContainer container)
		{
			Type channelManagerType = ChannelManagerType;
			if (channelManagerType != null)
			{
				if (typeof(IChannelManager).IsAssignableFrom(channelManagerType) == false)
					throw new ConfigurationErrorsException("channelManagerType must be an IChannelManager");
				if (channelManagerType.GetConstructor(new Type[0]) == null)
					throw new ConfigurationErrorsException("channelManagerType must have a default constructor");

				container.Configure<IWcfProxyExtensionConfiguration>().SetChannelManager((IChannelManager)Activator.CreateInstance(channelManagerType));
			}

			foreach (ServiceInterfaceConfigurationElement serviceInterface in ServiceInterfaces)
			{
				serviceInterface.Configure(container);
			}
		}
	}
}
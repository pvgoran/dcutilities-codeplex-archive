using System;
using System.ServiceModel;
using Microsoft.Practices.Unity;


namespace DigitallyCreated.Utilities.Unity
{
	/// <summary>
	/// Configuration interface for <see cref="WcfProxyContainerExtension"/>. Use
	/// <see cref="IUnityContainer.Configure{TConfigurator}"/> to get a object implements this
	/// interface in order to configure the <see cref="WcfProxyContainerExtension"/>
	/// </summary>
	public interface IWcfProxyExtensionConfiguration : IUnityContainerExtensionConfigurator
	{
		/// <summary>
		/// Registers a service interface type with the container so when 
		/// <see cref="IUnityContainer.Resolve{T}()"/> is called with this type, a channel proxy
		/// class will be returned.
		/// </summary>
		/// <typeparam name="TServiceInterfaceType">The service interface type</typeparam>
		/// <param name="channelFactoryFactory">
		/// The <see cref="IChannelFactoryFactory"/> to use to create the appropriate 
		/// <see cref="ChannelFactory{TChannel}"/> with which to create WCF channels.
		/// </param>
		/// <returns>This object</returns>
		IWcfProxyExtensionConfiguration RegisterType<TServiceInterfaceType>(IChannelFactoryFactory channelFactoryFactory);

		/// <summary>
		/// Registers a service interface type with the container so when 
		/// <see cref="IUnityContainer.Resolve{T}()"/> is called with this type, a channel proxy
		/// class will be returned.
		/// </summary>
		/// <param name="serviceInterfaceType">The service interface type</param>
		/// <param name="channelFactoryFactory">
		/// The <see cref="IChannelFactoryFactory"/> to use to create the appropriate 
		/// <see cref="ChannelFactory{TChannel}"/> with which to create WCF channels.
		/// </param>
		/// <returns>This object</returns>
		IWcfProxyExtensionConfiguration RegisterType(Type serviceInterfaceType, IChannelFactoryFactory channelFactoryFactory);

		/// <summary>
		/// Registers a service interface type with the container so when 
		/// <see cref="IUnityContainer.Resolve{T}()"/> is called with this type, a channel proxy
		/// class will be returned.
		/// </summary>
		/// <typeparam name="TServiceInterfaceType">The service interface type</typeparam>
		/// <param name="name">The name of the mapping</param>
		/// <param name="channelFactoryFactory">
		/// The <see cref="IChannelFactoryFactory"/> to use to create the appropriate 
		/// <see cref="ChannelFactory{TChannel}"/> with which to create WCF channels.
		/// </param>
		/// <returns>This object</returns>
		IWcfProxyExtensionConfiguration RegisterType<TServiceInterfaceType>(string name, IChannelFactoryFactory channelFactoryFactory);

		/// <summary>
		/// Registers a service interface type with the container so when 
		/// <see cref="IUnityContainer.Resolve{T}()"/> is called with this type, a channel proxy
		/// class will be returned.
		/// </summary>
		/// <param name="serviceInterfaceType">The service interface type</param>
		/// <param name="name">The name of the mapping</param>
		/// <param name="channelFactoryFactory">
		/// The <see cref="IChannelFactoryFactory"/> to use to create the appropriate 
		/// <see cref="ChannelFactory{TChannel}"/> with which to create WCF channels.
		/// </param>
		/// <returns>This object</returns>
		IWcfProxyExtensionConfiguration RegisterType(Type serviceInterfaceType, string name, IChannelFactoryFactory channelFactoryFactory);

		/// <summary>
		/// Sets the <see cref="IChannelManager"/> that is used to create channel objects
		/// </summary>
		/// <remarks>
		/// By default, the container is configured to use <see cref="ChannelManager"/> as its 
		/// <see cref="IChannelManager"/>
		/// </remarks>
		/// <param name="channelManager">The channel manager to use</param>
		/// <returns>This object</returns>
		IWcfProxyExtensionConfiguration SetChannelManager(IChannelManager channelManager);


		/// <summary>
		/// Registers a service interface type with the container so when 
		/// <see cref="IUnityContainer.Resolve{T}()"/> is called with this type, a channel proxy
		/// class will be returned.
		/// </summary>
		/// <typeparam name="TServiceInterfaceType">The service interface type</typeparam>
		/// <typeparam name="TRegisterAsType">
		/// The type to register the service interface as. The type must be assignable from 
		/// <typeparamref name="TServiceInterfaceType"/>. This will be the type that you request from Unity
		/// and therefore is normally either the same as <typeparamref name="TServiceInterfaceType"/>
		/// or a super type.
		/// </typeparam>
		/// <param name="channelFactoryFactory">
		/// The <see cref="IChannelFactoryFactory"/> to use to create the appropriate 
		/// <see cref="ChannelFactory{TChannel}"/> with which to create WCF channels.
		/// </param>
		/// <returns>This object</returns>
		IWcfProxyExtensionConfiguration RegisterType<TServiceInterfaceType, TRegisterAsType>(IChannelFactoryFactory channelFactoryFactory);


		/// <summary>
		/// Registers a service interface type with the container so when 
		/// <see cref="IUnityContainer.Resolve{T}()"/> is called with this type, a channel proxy
		/// class will be returned.
		/// </summary>
		/// <param name="serviceInterfaceType">The service interface type</param>
		/// <param name="registerAsType">
		/// The type to register the service interface as. The type must be assignable from 
		/// <paramref name="serviceInterfaceType"/>. This will be the type that you request from Unity
		/// and therefore is normally either the same as <paramref name="serviceInterfaceType"/>
		/// or a super type.
		/// </param>
		/// <param name="channelFactoryFactory">
		/// The <see cref="IChannelFactoryFactory"/> to use to create the appropriate 
		/// <see cref="ChannelFactory{TChannel}"/> with which to create WCF channels.
		/// </param>
		/// <returns>This object</returns>
		IWcfProxyExtensionConfiguration RegisterType(Type serviceInterfaceType, Type registerAsType, IChannelFactoryFactory channelFactoryFactory);


		/// <summary>
		/// Registers a service interface type with the container so when 
		/// <see cref="IUnityContainer.Resolve{T}()"/> is called with this type, a channel proxy
		/// class will be returned.
		/// </summary>
		/// <typeparam name="TServiceInterfaceType">The service interface type</typeparam>
		/// <typeparam name="TRegisterAsType">
		/// The type to register the service interface as. The type must be assignable from 
		/// <typeparamref name="TServiceInterfaceType"/>. This will be the type that you request from Unity
		/// and therefore is normally either the same as <typeparamref name="TServiceInterfaceType"/>
		/// or a super type.
		/// </typeparam>
		/// <param name="name">The name of the mapping</param>
		/// <param name="channelFactoryFactory">
		/// The <see cref="IChannelFactoryFactory"/> to use to create the appropriate 
		/// <see cref="ChannelFactory{TChannel}"/> with which to create WCF channels.
		/// </param>
		/// <returns>This object</returns>
		IWcfProxyExtensionConfiguration RegisterType<TServiceInterfaceType, TRegisterAsType>(string name, IChannelFactoryFactory channelFactoryFactory);


		/// <summary>
		/// Registers a service interface type with the container so when 
		/// <see cref="IUnityContainer.Resolve{T}()"/> is called with this type, a channel proxy class will be
		/// returned.
		/// </summary>
		/// <param name="serviceInterfaceType">The service interface type</param>
		/// <param name="registerAsType">
		/// The type to register the service interface as. The type must be assignable from 
		/// <paramref name="serviceInterfaceType"/>. This will be the type that you request from Unity
		/// and therefore is normally either the same as <paramref name="serviceInterfaceType"/>
		/// or a super type.
		/// </param>
		/// <param name="name">The name of the mapping</param>
		/// <param name="channelFactoryFactory">
		/// The <see cref="IChannelFactoryFactory"/> to use to create the appropriate 
		/// <see cref="ChannelFactory{TChannel}"/> with which to create WCF channels.
		/// </param>
		/// <returns>This object</returns>
		IWcfProxyExtensionConfiguration RegisterType(Type serviceInterfaceType, Type registerAsType, string name, IChannelFactoryFactory channelFactoryFactory);
	}
}
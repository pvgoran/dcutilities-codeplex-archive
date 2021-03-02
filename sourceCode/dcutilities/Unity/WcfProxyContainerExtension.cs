using System;
using System.Linq;
using System.ServiceModel;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;


namespace DigitallyCreated.Utilities.Unity
{
	/// <summary>
	/// The WcfProxyContainerExtension extends the Unity Container by enabling it to create WCF channel objects
	/// when asked to resolve a service interface.
	/// </summary>
	/// <remarks>
	/// <para>
	/// To use this extension you must register it with your Unity container. To do this in configuration XML
	/// you do this inside the extensions tag inside the container tag:
	/// <code><![CDATA[
	///	<extensions>
	///	    <add type="Onset.Web.OnsetWebsite.UI.Unity.WcfProxyContainerExtension, Onset.Web.OnsetWebsite.UI"/>
	/// <extensions>
	/// ]]></code>
	/// To do it in code you go:
	/// <code><![CDATA[
	/// myUnityContainer.AddNewExtension<WcfProxyContainerExtension>();
	/// ]]></code>
	/// </para>
	/// <para>
	/// Once it is registered, you need to tell it about your service interfaces. You can do this by
	/// using one of the <c>RegisterType</c> methods on the <see cref="IWcfProxyExtensionConfiguration"/>.
	/// For example:
	/// <code><![CDATA[
	/// myUnityContainer.Configure<IWcfProxyExtensionConfiguration>().RegisterType<IMyServiceInterface>(new EndpointConfigurationChannelFactoryFactory("myServiceEndpointConfiguration"));
	/// ]]></code>
	/// You can do it in XML configuration instead by adding to the extensionConfig tag inside the container tag:
	/// <code><![CDATA[
	/// <extensionConfig>
	///     <add name="WcfProxyContainerExtensionConfig" 
	///          type="DigitallyCreated.Utilities.Unity.WcfProxyExtensionConfigurationElement, DigitallyCreated.Utilities.Unity">
	///         <serviceInterfaces>
	///				<serviceInterface type="MyNamespace.IMyServiceInterface, MyAssembly"
	///                               endpointConfiguration="myServiceEndpointConfiguration" />
	///         </serviceInterfaces>
	///	    </add>
	/// </extensionConfig>
	/// ]]></code>
	/// </para>
	/// </remarks>
	public class WcfProxyContainerExtension : UnityContainerExtension, IWcfProxyExtensionConfiguration
	{
		/// <summary>
		/// Initialises the container with this extension's functionality.
		/// </summary>
		protected override void Initialize()
		{
			//Add the default IChannelManager
			SetChannelManager(new ChannelManager());
		}


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
		public IWcfProxyExtensionConfiguration RegisterType<TServiceInterfaceType>(IChannelFactoryFactory channelFactoryFactory)
		{
			return RegisterType(typeof(TServiceInterfaceType), channelFactoryFactory);
		}


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
		public IWcfProxyExtensionConfiguration RegisterType<TServiceInterfaceType, TRegisterAsType>(IChannelFactoryFactory channelFactoryFactory)
		{
			return RegisterType(typeof(TServiceInterfaceType), typeof(TRegisterAsType), channelFactoryFactory);
		}


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
		public IWcfProxyExtensionConfiguration RegisterType(Type serviceInterfaceType, IChannelFactoryFactory channelFactoryFactory)
		{
			return RegisterType(serviceInterfaceType, (string)null, channelFactoryFactory);
		}


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
		public IWcfProxyExtensionConfiguration RegisterType(Type serviceInterfaceType, Type registerAsType, IChannelFactoryFactory channelFactoryFactory)
		{
			return RegisterType(serviceInterfaceType, registerAsType, null, channelFactoryFactory);
		}


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
		public IWcfProxyExtensionConfiguration RegisterType<TServiceInterfaceType>(string name, IChannelFactoryFactory channelFactoryFactory)
		{
			return RegisterType(typeof(TServiceInterfaceType), name, channelFactoryFactory);
		}


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
		public IWcfProxyExtensionConfiguration RegisterType(Type serviceInterfaceType, string name, IChannelFactoryFactory channelFactoryFactory)
		{
			return RegisterType(serviceInterfaceType, serviceInterfaceType, name, channelFactoryFactory);
		}


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
		public IWcfProxyExtensionConfiguration RegisterType<TServiceInterfaceType, TRegisterAsType>(string name, IChannelFactoryFactory channelFactoryFactory)
		{
			return RegisterType(typeof(TServiceInterfaceType), typeof(TRegisterAsType), name, channelFactoryFactory);
		}


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
		public IWcfProxyExtensionConfiguration RegisterType(Type serviceInterfaceType, Type registerAsType, string name, IChannelFactoryFactory channelFactoryFactory)
		{
			WcfProxyBuildPlanPolicy policy = new WcfProxyBuildPlanPolicy(channelFactoryFactory, serviceInterfaceType);
			Context.Policies.Set<IBuildPlanPolicy>(policy, new NamedTypeBuildKey(registerAsType, name));
			return this;
		}


		/// <summary>
		/// Sets the <see cref="IChannelManager"/> that is used to create channel objects
		/// </summary>
		/// <remarks>
		/// By default, the container is configured to use <see cref="ChannelManager"/> as its 
		/// <see cref="IChannelManager"/>
		/// </remarks>
		/// <param name="channelManager">The channel manager to use</param>
		/// <returns>This object</returns>
		public IWcfProxyExtensionConfiguration SetChannelManager(IChannelManager channelManager)
		{
			IChannelManager oldChannelManager = Context.Lifetime.OfType<IChannelManager>().SingleOrDefault();
			if (oldChannelManager != null)
				Context.Lifetime.Remove(oldChannelManager);

			Context.Lifetime.Add(channelManager);
			return this;
		}
	}
}
using System;
using System.Linq;
using System.ServiceModel;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;


namespace DigitallyCreated.Utilities.Unity
{
	/// <summary>
	/// This <see cref="IBuildPlanPolicy"/> allows the <see cref="WcfProxyContainerExtension"/> to create WCF
	/// proxies for a particular interface when that interface is configured to be built by using this policy
	/// </summary>
	/// <remarks>
	/// This policy is looked for by the <see cref="BuildPlanStrategy"/> that is configured into the ObjectBuilder
	/// by <see cref="UnityDefaultStrategiesExtension"/>. The strategy will use this policy (if found for the current
	/// type being constructed) to create the object instance. The <see cref="WcfProxyContainerExtension"/>
	/// allows the user to register this policy against a particular type to construct, so that it will be used
	/// to construct that type.
	/// </remarks>
	public class WcfProxyBuildPlanPolicy : IBuildPlanPolicy
	{
		private readonly IChannelFactoryFactory _ChannelFactoryFactory;
		private readonly Type _ServiceInterfaceType;


		/// <summary>
		/// Creates a WcfProxyBuildPlanPolicy that will create a channel with the specified endpoint
		/// </summary>
		/// <param name="channelFactoryFactory">
		/// The <see cref="IChannelFactoryFactory"/> to use to create the appropriate 
		/// <see cref="ChannelFactory{TChannel}"/>.
		/// </param>
		/// <param name="serviceInterfaceType"></param>
		public WcfProxyBuildPlanPolicy(IChannelFactoryFactory channelFactoryFactory, Type serviceInterfaceType)
		{
			_ChannelFactoryFactory = channelFactoryFactory;
			_ServiceInterfaceType = serviceInterfaceType;
		}


		/// <summary>
		/// Creates an instance of the build plan's type by creating a WCF channel that implements it
		/// </summary>
		/// <param name="context">Context used to build up the object.</param>
		public void BuildUp(IBuilderContext context)
		{
			if (context.Existing == null)
			{
				IChannelManager channelManager = context.Lifetime.OfType<IChannelManager>().Single();

				if (BuildKey.GetType(context.BuildKey).IsAssignableFrom(_ServiceInterfaceType) == false)
					throw new Exception("A WcfProxyBuildPlanPolicy has been paired with an incorrect BuildKey. Its build key type is not assignable from the service interface type.");

				context.Existing = channelManager.CreateChannel(_ServiceInterfaceType, _ChannelFactoryFactory);
			}
		}
	}
}
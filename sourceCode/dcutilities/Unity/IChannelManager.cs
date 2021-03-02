using System;
using System.ServiceModel;


namespace DigitallyCreated.Utilities.Unity
{
	/// <summary>
	/// An IChannelManager is able to create and return channels for a specified service interface
	/// </summary>
	public interface IChannelManager
	{
		/// <summary>
		/// Creates a WCF channel object for the specified service interface
		/// </summary>
		/// <param name="ofType">
		/// The service interface type (used as the type parameter for <see cref="ChannelFactory{TChannel}"/>
		/// </param>
		/// <param name="channelFactoryFactory">
		/// The <see cref="IChannelFactoryFactory"/> to use to create the appropriate 
		/// <see cref="ChannelFactory{TChannel}"/>.
		/// </param>
		/// <returns>The channel</returns>
		object CreateChannel(Type ofType, IChannelFactoryFactory channelFactoryFactory);
	}
}
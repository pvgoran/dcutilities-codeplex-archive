using System;
using System.ServiceModel;
using DigitallyCreated.Utilities.Unity.Configuration;


namespace DigitallyCreated.Utilities.Unity
{
	/// <summary>
	/// An <see cref="IChannelFactoryFactory"/> is able to create <see cref="ChannelFactory{TChannel}"/>
	/// for a specified type. It is used by a <see cref="IChannelManager"/> to create
	/// <see cref="ChannelFactory{TChannel}"/> objects with which WCF channels can be made.
	/// </summary>
	/// <remarks>
	/// <para>
	/// A <see cref="IChannelFactoryFactory"/> is a good place to perform configuration of a 
	/// <see cref="ChannelFactory{TChannel}"/> (ie specify and customise the endpoint configuration
	/// it will use, etc).
	/// </para>
	/// <para>
	/// Generally, when implementing this interface, you will also want to create a configuration
	/// class that derives from <see cref="CustomChannelFactoryFactoryConfigurationElement"/>
	/// that allows users to choose and configure your <see cref="IChannelFactoryFactory"/>
	/// from the Unity XML configuration file.
	/// </para>
	/// </remarks>
	public interface IChannelFactoryFactory
	{
		/// <summary>
		/// Creates a <see cref="ChannelFactory{TChannel}"/> where the generic type is
		/// the type specified by <paramref name="channelType"/>.
		/// </summary>
		/// <param name="channelType">
		/// The type of the <see cref="ChannelFactory{TChannel}"/> to create
		/// </param>
		/// <returns>A <see cref="ChannelFactory{TChannel}"/> instance</returns>
		ChannelFactory CreateChannelFactory(Type channelType);
	}
}
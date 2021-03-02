using System;
using System.ServiceModel;


namespace DigitallyCreated.Utilities.Unity
{
	/// <summary>
	/// This <see cref="IChannelFactoryFactory"/> is able to create <see cref="ChannelFactory{TChannel}"/>
	/// objects configured with an endpoint configuration specified in the application's configuration 
	/// file.
	/// </summary>
	public class EndpointConfigurationChannelFactoryFactory : IChannelFactoryFactory
	{
		private readonly string _EndpointConfigurationName;


		/// <summary>
		/// Constructor, creates an <see cref="EndpointConfigurationChannelFactoryFactory"/>
		/// that creates <see cref="ChannelFactory{TChannel}"/> configured with the 
		/// specified endpoint configuration
		/// </summary>
		/// <param name="endpointConfigurationName">The name of the endpoint configuration</param>
		public EndpointConfigurationChannelFactoryFactory(string endpointConfigurationName)
		{
			_EndpointConfigurationName = endpointConfigurationName;
		}


		/// <summary>
		/// Creates a <see cref="ChannelFactory{TChannel}"/> where the generic type is
		/// the type specified by <paramref name="channelType"/>.
		/// </summary>
		/// <param name="channelType">
		/// The type of the <see cref="ChannelFactory{TChannel}"/> to create
		/// </param>
		/// <returns>A <see cref="ChannelFactory{TChannel}"/> instance</returns>
		public virtual ChannelFactory CreateChannelFactory(Type channelType)
		{
			Type genericFactoryType = typeof(ChannelFactory<>).MakeGenericType(channelType);
			return (ChannelFactory)Activator.CreateInstance(genericFactoryType, _EndpointConfigurationName);
		}
	}
}
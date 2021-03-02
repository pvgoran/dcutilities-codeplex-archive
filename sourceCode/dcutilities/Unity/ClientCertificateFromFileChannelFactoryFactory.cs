using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Description;


namespace DigitallyCreated.Utilities.Unity
{
	/// <summary>
	/// This <see cref="IChannelFactoryFactory"/> is able to create <see cref="ChannelFactory{TChannel}"/>
	/// objects that are configured with a specific endpoint configuration and are also
	/// configured to use a specific specified certificate from the file system.
	/// </summary>
	public class ClientCertificateFromFileChannelFactoryFactory : EndpointConfigurationChannelFactoryFactory
	{
		private readonly X509Certificate2 _Certificate;


		/// <summary>
		/// Constructor, creates a <see cref="ClientCertificateFromFileChannelFactoryFactory"/>
		/// that creates <see cref="ChannelFactory{TChannel}"/> objects that are configured
		/// with the specified endpoint configuration and configured to use the certificate
		/// found in the specified file (and decrypted with the specified password).
		/// </summary>
		/// <param name="endpointConfigurationName">The name of the endpoint configuration</param>
		/// <param name="certificateFilename">The filename of the certificate (.pfx) file to open</param>
		/// <param name="certificatePassword">The password to use to open the certificate file</param>
		public ClientCertificateFromFileChannelFactoryFactory(string endpointConfigurationName, string certificateFilename, string certificatePassword)
			: base(endpointConfigurationName)
		{
			if (certificateFilename.StartsWith(@"~\") || certificateFilename.StartsWith("~/"))
				certificateFilename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, certificateFilename.Substring(2));

			if (File.Exists(certificateFilename) == false)
				throw new ArgumentException("certificateFilename must be a existing file");

			_Certificate = new X509Certificate2(certificateFilename, certificatePassword);
		}


		/// <summary>
		/// Creates a <see cref="ChannelFactory{TChannel}"/> where the generic type is
		/// the type specified by <paramref name="channelType"/>.
		/// </summary>
		/// <param name="channelType">
		/// The type of the <see cref="ChannelFactory{TChannel}"/> to create
		/// </param>
		/// <returns>A <see cref="ChannelFactory{TChannel}"/> instance</returns>
		public override ChannelFactory CreateChannelFactory(Type channelType)
		{
			ChannelFactory channelFactory = base.CreateChannelFactory(channelType);
			ClientCredentials credentials = channelFactory.Endpoint.Behaviors.Find<ClientCredentials>();
			if (credentials == null)
			{
				credentials = new ClientCredentials();
				channelFactory.Endpoint.Behaviors.Add(credentials);
			}
			credentials.ClientCertificate.Certificate = _Certificate;
			return channelFactory;
		}
	}
}
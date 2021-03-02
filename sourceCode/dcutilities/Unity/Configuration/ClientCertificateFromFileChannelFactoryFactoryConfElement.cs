using System;
using System.Configuration;
using System.IO;


namespace DigitallyCreated.Utilities.Unity.Configuration
{
	/// <summary>
	/// The <see cref="ConfigurationElement"/> that is able to create a 
	/// <see cref="ClientCertificateFromFileChannelFactoryFactory"/>.
	/// </summary>
	public class ClientCertificateFromFileChannelFactoryFactoryConfElement : CustomChannelFactoryFactoryConfigurationElement
	{
		/// <summary>
		/// The filename of the certificate to use (a .pfx). You can prepend "~/" to the front of
		/// the filename and the <see cref="AppDomain.BaseDirectory"/> will be <see cref="Path.Combine(string,string)"/>d
		/// to the front.
		/// </summary>
		[ConfigurationProperty("certFilename", IsRequired = true)]
		public string CertificateFilename
		{
			get { return (string)base["certFilename"]; }
			set { base["certFilename"] = value; }
		}


		/// <summary>
		/// The password to use when unlocking the specified certificate file
		/// </summary>
		[ConfigurationProperty("certPassword", IsRequired = true)]
		public string CertificatePassword
		{
			get { return (string)base["certPassword"]; }
			set { base["certPassword"] = value; }
		}


		/// <summary>
		/// The name of the endpoint configuration to use on the channel
		/// </summary>
		[ConfigurationProperty("endpointConfiguration", IsRequired = true)]
		public string EndpointConfigurationName
		{
			get { return (string)this["endpointConfiguration"]; }
			set { this["endpointConfiguration"] = value; }
		}


		/// <summary>
		/// Creates a <see cref="ClientCertificateFromFileChannelFactoryFactory"/> using the settings
		/// provided.
		/// </summary>
		/// <returns>A <see cref="ClientCertificateFromFileChannelFactoryFactory"/></returns>
		public override IChannelFactoryFactory CreateChannelFactoryFactory()
		{
			return new ClientCertificateFromFileChannelFactoryFactory(EndpointConfigurationName, CertificateFilename, CertificatePassword);
		}
	}
}
using System.Configuration;
using Microsoft.Practices.Unity.Configuration;


namespace DigitallyCreated.Utilities.Unity.Configuration
{
	/// <summary>
	/// A <see cref="ConfigurationElementCollection"/> that holds a collection of 
	/// <see cref="ServiceInterfaceConfigurationElement"/>s
	/// </summary>
	public class ServiceInterfaceConfigurationElementCollection : TypeResolvingConfigurationElementCollection
	{
		/// <summary>
		/// Resolves a <see cref="ServiceInterfaceConfigurationElement"/> by name
		/// </summary>
		/// <param name="name">The name to look up</param>
		/// <returns>The element</returns>
		public new ServiceInterfaceConfigurationElement this[string name]
		{
			get { return (ServiceInterfaceConfigurationElement)Get(name); }
		}

		/// <summary>
		/// Get or set a <see cref="ServiceInterfaceConfigurationElement"/> by index
		/// </summary>
		/// <param name="index">The index to get or set at</param>
		/// <returns>The element</returns>
		public ServiceInterfaceConfigurationElement this[int index]
		{
			get { return (ServiceInterfaceConfigurationElement)Get(index); }
			set
			{
				if (BaseGet(index) != null)
					BaseRemoveAt(index);
				BaseAdd(index, value);
			}
		}


		/// <summary>
		/// When overridden in a derived class, creates a new <see cref="ConfigurationElement"/>.
		/// </summary>
		/// <returns>
		/// A new <see cref="ConfigurationElement"/>.
		/// </returns>
		protected override ConfigurationElement CreateNewElement()
		{
			return new ServiceInterfaceConfigurationElement();
		}


		/// <summary>
		/// Gets the element key for a specified configuration element when overridden in a derived class.
		/// </summary>
		/// <returns>
		/// An <see cref="object"/> that acts as the key for the specified <see cref="ConfigurationElement"/>.
		/// </returns>
		/// <param name="element">
		/// The <see cref="ConfigurationElement"/> to return the key for. 
		/// </param>
		protected override object GetElementKey(ConfigurationElement element)
		{
			ServiceInterfaceConfigurationElement typeElement = (ServiceInterfaceConfigurationElement)element;
			if (typeElement.Name == null)
			{
				return typeElement.InterfaceTypeName;
			}
			return (typeElement.Name + ":" + typeElement.InterfaceTypeName);
		}
	}
}
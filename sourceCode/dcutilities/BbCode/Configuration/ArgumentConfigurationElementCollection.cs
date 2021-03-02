using System.Configuration;


namespace DigitallyCreated.Utilities.BbCode.Configuration
{
	/// <summary>
	/// A <see cref="ConfigurationElementCollection"/> that contains a number of arguments to pass to a constructor
	/// </summary>
	public class ArgumentConfigurationElementCollection : ConfigurationElementCollection
	{
		/// <summary>
		/// When overridden in a derived class, creates a new <see cref="ConfigurationElement"/>.
		/// </summary>
		/// <returns>
		/// A new <see cref="ConfigurationElement"/>.
		/// </returns>
		protected override ConfigurationElement CreateNewElement()
		{
			return new ArgumentConfigurationElement();
		}


		/// <summary>
		/// Gets the element key for a specified configuration element when overridden in a derived class.
		/// </summary>
		/// <returns>
		/// An <see cref="object"/> that acts as the key for the specified <see cref="ConfigurationElement"/>.
		/// </returns>
		/// <param name="element">The <see cref="ConfigurationElement"/> to return the key for. </param>
		protected override object GetElementKey(ConfigurationElement element)
		{
			ArgumentConfigurationElement argElem = (ArgumentConfigurationElement)element;
			return argElem.UniqueId;
		}
	}
}
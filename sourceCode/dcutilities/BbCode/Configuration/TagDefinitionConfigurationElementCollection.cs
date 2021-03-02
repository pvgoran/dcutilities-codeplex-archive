using System.Configuration;

namespace DigitallyCreated.Utilities.BbCode.Configuration
{
	/// <summary>
	/// A <see cref="ConfigurationElementCollection"/> that contains a number of tag definitions
	/// </summary>
	public class TagDefinitionConfigurationElementCollection : ConfigurationElementCollection
	{
		/// <summary>
		/// The name of this tag definition set
		/// </summary>
		[ConfigurationProperty("name", IsRequired = true)]
		public string Name
		{
			get { return (string)base["name"]; }
			set { base["name"] = value; }
		}


		/// <summary>
		/// Resolves a <see cref="TagDefinitionConfigurationElement"/> by name
		/// </summary>
		/// <param name="name">The name to look up</param>
		/// <returns>The element</returns>
		public new TagDefinitionConfigurationElement this[string name]
		{
			get { return (TagDefinitionConfigurationElement)BaseGet(name); }
		}

		/// <summary>
		/// Get or set a <see cref="TagDefinitionConfigurationElement"/> by index
		/// </summary>
		/// <param name="index">The index to get or set at</param>
		/// <returns>The element</returns>
		public TagDefinitionConfigurationElement this[int index]
		{
			get { return (TagDefinitionConfigurationElement)BaseGet(index); }
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
		/// </returns>>
		protected override ConfigurationElement CreateNewElement()
		{
			return new TagDefinitionConfigurationElement();
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
			TagDefinitionConfigurationElement tagDefElement = (TagDefinitionConfigurationElement)element;
			return tagDefElement.TagDefinitionTypeName;
		}
	}
}
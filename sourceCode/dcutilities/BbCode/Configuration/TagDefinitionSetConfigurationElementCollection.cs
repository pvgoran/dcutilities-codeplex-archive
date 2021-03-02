using System;
using System.Configuration;
using System.Linq;

namespace DigitallyCreated.Utilities.BbCode.Configuration
{
	/// <summary>
	/// A <see cref="ConfigurationElementCollection"/> that contains named groups (sets) of tag definitions
	/// </summary>
	public class TagDefinitionSetConfigurationElementCollection : ConfigurationElementCollection
	{
		/// <summary>
		/// The default set's name
		/// </summary>
		[ConfigurationProperty("default", IsRequired = true)]
		public string DefaultSetValue
		{
			get { return (string)base["default"]; }
			set { base["default"] = value; } 
		}

		/// <summary>
		/// The default set of <see cref="ITagDefinition"/>s
		/// </summary>
		public TagDefinitionConfigurationElementCollection DefaultSet
		{
			get
			{
				if (String.IsNullOrEmpty(DefaultSetValue))
					throw new ConfigurationErrorsException("The default attribute on the tagDefinitionSets element must be set");
				TagDefinitionConfigurationElementCollection element =
					this.Cast<TagDefinitionConfigurationElementCollection>().Where(e => e.Name == DefaultSetValue).FirstOrDefault();
				if (element == null)
					throw new ConfigurationErrorsException("The default attribute on the tagDefinitionSets element must be set to an existing tag definition set");
				return element;
			}
		}

		/// <summary>
		/// Resolves a <see cref="TagDefinitionSetConfigurationElementCollection"/> by name
		/// </summary>
		/// <param name="name">The name to look up</param>
		/// <returns>The element</returns>
		public new TagDefinitionConfigurationElementCollection this[string name]
		{
			get { return (TagDefinitionConfigurationElementCollection)BaseGet(name); }
		}

		/// <summary>
		/// Get or set a <see cref="TagDefinitionSetConfigurationElementCollection"/> by index
		/// </summary>
		/// <param name="index">The index to get or set at</param>
		/// <returns>The element</returns>
		public TagDefinitionConfigurationElementCollection this[int index]
		{
			get { return (TagDefinitionConfigurationElementCollection)BaseGet(index); }
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
			return new TagDefinitionConfigurationElementCollection();
		}


		/// <summary>
		/// Gets the element key for a specified configuration element when overridden in a derived class.
		/// </summary>
		/// <returns>
		/// An <see cref="Object"/> that acts as the key for the specified <see cref="ConfigurationElement"/>.
		/// </returns>
		/// <param name="element">The <see cref="ConfigurationElement"/> to return the key for. </param>
		protected override object GetElementKey(ConfigurationElement element)
		{
			TagDefinitionConfigurationElementCollection tagDefElement = (TagDefinitionConfigurationElementCollection)element;
			return tagDefElement.Name;
		}
	}
}
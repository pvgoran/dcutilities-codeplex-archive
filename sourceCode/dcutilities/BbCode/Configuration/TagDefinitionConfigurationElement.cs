using System;
using System.Configuration;
using System.Linq;


namespace DigitallyCreated.Utilities.BbCode.Configuration
{
	/// <summary>
	/// A <see cref="ConfigurationElement"/> that defines a particular concrete <see cref="ITagDefinition"/>
	/// </summary>
	public class TagDefinitionConfigurationElement : ConfigurationElement
	{
		/// <summary>
		/// The name of the <see cref="Type"/> of the tag definition
		/// </summary>
		[ConfigurationProperty("type", IsRequired = true)]
		public string TagDefinitionTypeName
		{
			get { return (string)this["type"]; }
			set { this["type"] = value; }
		}


		/// <summary>
		/// An optional collection of arguments to pass into the tag definition's constructor
		/// </summary>
		[ConfigurationProperty("constructor")]
		[ConfigurationCollection(typeof(TagDefinitionSetConfigurationElementCollection), AddItemName = "argument")]
		public ArgumentConfigurationElementCollection ConstructorArguments
		{
			get { return (ArgumentConfigurationElementCollection)this["constructor"]; }
		}


		/// <summary>
		/// The <see cref="Type"/> of the tag definition
		/// </summary>
		public Type TagDefinitionType
		{
			get
			{
				if (String.IsNullOrEmpty(TagDefinitionTypeName))
					throw new ConfigurationErrorsException("tagDefinition type attribute must be set.");
				Type type = Type.GetType(TagDefinitionTypeName, true);
				if (typeof(ITagDefinition).IsAssignableFrom(type) == false)
					throw new ConfigurationErrorsException("tagDefinition type attribute must be set to a class that implements ITagDefinition");
				return type;
			}
		}


		/// <summary>
		/// Creates an instance of the tag definition specified by this configuration element
		/// </summary>
		/// <returns>The tag definition instance</returns>
		public ITagDefinition CreateTagDefinition()
		{
			if (ConstructorArguments == null || ConstructorArguments.Count == 0)
				return (ITagDefinition)Activator.CreateInstance(TagDefinitionType);

			object[] args = ConstructorArguments.Cast<ArgumentConfigurationElement>().Select(elem => elem.Value).ToArray();
			return (ITagDefinition)Activator.CreateInstance(TagDefinitionType, args);
		}
	}
}
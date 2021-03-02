using System;
using System.Configuration;
using System.Linq;
using DigitallyCreated.Utilities.Bcl;


namespace DigitallyCreated.Utilities.BbCode.Configuration
{
	/// <summary>
	/// A <see cref="ConfigurationElement"/> that allows you to define an argument that will be passed to 
	/// a constructor.
	/// </summary>
	public class ArgumentConfigurationElement : ConfigurationElement
	{
		private static readonly Type[] _AcceptableTypes = new[]
		                                                  	{
		                                                  		typeof(sbyte), typeof(byte), typeof(char),
		                                                  		typeof(short), typeof(ushort), typeof(int),
		                                                  		typeof(uint), typeof(long), typeof(ulong),
		                                                  		typeof(float), typeof(double), typeof(decimal),
		                                                  		typeof(string), typeof(DateTime), typeof(bool)
		                                                  	};

		/// <summary>
		/// Contains a unique GUID for each instance of this class. Ensures that you can have multiple
		/// instances of this element in a collection with the same type and value and still treat them
		/// as distinct instances.
		/// </summary>
		public Guid UniqueId { get; private set; }


		/// <summary>
		/// The name of the <see cref="Type"/> of the object that this argument is
		/// </summary>
		[ConfigurationProperty("type", IsRequired = true)]
		public string TypeName
		{
			get { return (string)base["type"]; }
			set { base["type"] = value; }
		}


		/// <summary>
		/// The <see cref="Type"/> of the object that this argument is
		/// </summary>
		public Type Type
		{
			get
			{
				if (String.IsNullOrEmpty(TypeName))
					throw new ConfigurationErrorsException("tagDefinition type attribute must be set.");
				Type type = Type.GetType(TypeName, true);
				if (_AcceptableTypes.Any(t => t == type) == false)
					throw new ConfigurationErrorsException("argument type attribute must be an integral type, floating point type, decimal, bool, string or DateTime");

				//If they've omitted the Value attribute, they want null, so ensure its a nullable type
				if (ValueString == null && type != typeof(string))
					type = typeof(Nullable<>).MakeGenericType(type);
				
				return type;
			}
		}


		/// <summary>
		/// The string value of the argument
		/// </summary>
		[ConfigurationProperty("value", IsRequired = false)]
		public string ValueString
		{
			get { return (string)base["value"]; }
			set { base["value"] = value; }
		}


		/// <summary>
		/// The strongly typed value of the argument
		/// </summary>
		public object Value
		{
			get
			{
				if (Type == typeof(string))
					return ValueString;

				//Return nullable type
				if (ValueString == null)
					return null;

				if (Type == typeof(sbyte))
					return Convert.ToSByte(ValueString);
				if (Type == typeof(byte))
					return Convert.ToByte(ValueString);
				if (Type == typeof(char))
					return Convert.ToChar(ValueString);
				if (Type == typeof(short))
					return Convert.ToInt16(ValueString);
				if (Type == typeof(ushort))
					return Convert.ToUInt16(ValueString);
				if (Type == typeof(int))
					return Convert.ToInt32(ValueString);
				if (Type == typeof(uint))
					return Convert.ToUInt32(ValueString);
				if (Type == typeof(long))
					return Convert.ToInt64(ValueString);
				if (Type == typeof(ulong))
					return Convert.ToUInt64(ValueString);
				if (Type == typeof(float))
					return Convert.ToSingle(ValueString);
				if (Type == typeof(double))
					return Convert.ToDouble(ValueString);
				if (Type == typeof(decimal))
					return Convert.ToDecimal(ValueString);
				if (Type == typeof(DateTime))
					return Convert.ToDateTime(ValueString);
				if (Type == typeof(bool))
					return Convert.ToBoolean(ValueString);
				
				throw new IllegalStateException("Unexpected type encountered");
			}
		}


		/// <summary>
		/// Constructor, creates a new <see cref="ArgumentConfigurationElement"/>
		/// </summary>
		public ArgumentConfigurationElement()
		{
			UniqueId = Guid.NewGuid();
		}
	}
}
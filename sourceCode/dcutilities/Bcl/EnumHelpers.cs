using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;


namespace DigitallyCreated.Utilities.Bcl
{
	/// <summary>
	/// Helper methods for working with enums 
	/// </summary>
	public static class EnumHelpers
	{
		/// <summary>
		/// Gets all the values and display names of all the enum constants on the specified enum type
		/// </summary>
		/// <remarks>
		/// The display name for an enum constant is the value of 
		/// <see cref="DisplayNameAttribute.DisplayName"/> from the <see cref="FieldDisplayNameAttribute"/> that has
		/// been applied to the enum constant. If the <see cref="FieldDisplayNameAttribute"/> has not been applied
		/// to the constant, the constant's name is used instead.
		/// </remarks>
		/// <typeparam name="TEnum">The enum type</typeparam>
		/// <typeparam name="TUnderlyingType">
		/// The type underlying the enum (this is <see cref="Int32"/> by default for Visual Basic and C#)
		/// </typeparam>
		/// <returns>
		/// <see cref="KeyValuePair{TKey,TValue}"/>s where the <see cref="KeyValuePair{TKey,TValue}.Key"/> is the
		/// display name and the <see cref="KeyValuePair{TKey,TValue}.Value"/> is the value of the enum constant
		/// </returns>
		/// <exception cref="ArgumentException">
		/// If <typeparamref name="TEnum"/> is not an enum type, or if <typeparamref name="TUnderlyingType"/> is not
		/// the underlying type of <typeparamref name="TEnum"/>.
		/// </exception>
		public static IEnumerable<KeyValuePair<string, TUnderlyingType>> GetValuesAndDisplayNames<TEnum, TUnderlyingType>()
		{
			Type type = typeof(TEnum);
			if (type.IsEnum == false)
				throw new ArgumentException("The specified TEnum type argument must be an enum type");
			if (typeof(TUnderlyingType) != Enum.GetUnderlyingType(type))
				throw new ArgumentException("The specified TUnderlyingType type argument must be the type underlying the specified enum type (TEnum)");

			return
				from field in type.GetFields(BindingFlags.Static | BindingFlags.Public)
				let attribute = field.GetCustomAttributes(typeof(FieldDisplayNameAttribute), true).Cast<FieldDisplayNameAttribute>().FirstOrDefault()
				select new KeyValuePair<string, TUnderlyingType>(attribute == null ? field.Name : attribute.DisplayName, (TUnderlyingType)field.GetValue(null));
		}


		/// <summary>
		/// Gets the display names of all the enum constants on the specified enum type
		/// </summary>
		/// <remarks>
		/// The display name for an enum constant is the value of 
		/// <see cref="DisplayNameAttribute.DisplayName"/> from the <see cref="FieldDisplayNameAttribute"/> that has
		/// been applied to the enum constant. If the <see cref="FieldDisplayNameAttribute"/> has not been applied
		/// to the constant, the constant's name is used instead.
		/// </remarks>
		/// <typeparam name="TEnum">The enum type</typeparam>
		/// <returns>The display names</returns>
		/// <exception cref="ArgumentException">If <typeparamref name="TEnum"/> is not an enum type</exception>
		public static IEnumerable<string> GetDisplayNames<TEnum>()
		{
			return
				from field in typeof(TEnum).GetFields(BindingFlags.Static | BindingFlags.Public)
				let attribute = field.GetCustomAttributes(typeof(FieldDisplayNameAttribute), true).Cast<FieldDisplayNameAttribute>().FirstOrDefault()
				select attribute == null ? field.Name : attribute.DisplayName;
		}


		/// <summary>
		/// Gets the display name of the specified enum value
		/// </summary>
		/// <remarks>
		/// The display name for an enum constant is the value of 
		/// <see cref="DisplayNameAttribute.DisplayName"/> from the <see cref="FieldDisplayNameAttribute"/> that has
		/// been applied to the enum constant. If the <see cref="FieldDisplayNameAttribute"/> has not been applied
		/// to the constant, the constant's name is used instead.
		/// </remarks>
		/// <typeparam name="TEnum">The enum type</typeparam>
		/// <param name="enum">The enum value</param>
		/// <returns>
		/// The display name, or <see langword="null"/> if the specified value is not associated to a constant
		/// </returns>
		/// <exception cref="ArgumentException">If <typeparamref name="TEnum"/> is not an enum type</exception>
		public static string GetDisplayNameForValue<TEnum>(TEnum @enum)
		{
			Type type = typeof(TEnum);
			if (type.IsEnum == false)
				throw new ArgumentException("The specified TEnum type argument must be an enum type");

			string name = Enum.GetName(typeof(TEnum), @enum);
			if (name == null)
				return null;

			FieldInfo field = type.GetField(name, BindingFlags.Static | BindingFlags.Public);
			FieldDisplayNameAttribute attribute = field.GetCustomAttributes(typeof(FieldDisplayNameAttribute), true).Cast<FieldDisplayNameAttribute>().FirstOrDefault();
			return attribute == null ? name : attribute.DisplayName;
		}


		/// <summary>
		/// Gets the value associated with the enum constant that bears the specified display name
		/// </summary>
		/// <remarks>
		/// The display name for an enum constant is the value of 
		/// <see cref="DisplayNameAttribute.DisplayName"/> from the <see cref="FieldDisplayNameAttribute"/> that has
		/// been applied to the enum constant. If the <see cref="FieldDisplayNameAttribute"/> has not been applied
		/// to the constant, the constant's name is used instead.
		/// </remarks>
		/// <typeparam name="TEnum">The enum type</typeparam>
		/// <typeparam name="TUnderlyingType">
		/// The type underlying the enum (this is <see cref="Int32"/> by default for Visual Basic and C#)
		/// </typeparam>
		/// <param name="displayName">The display name</param>
		/// <returns>The value</returns>
		/// <exception cref="ArgumentException">
		/// If <typeparamref name="TEnum"/> is not an enum type, or if <typeparamref name="TUnderlyingType"/> is not
		/// the underlying type of <typeparamref name="TEnum"/>, or if no enum constant has the specified display
		/// name
		/// </exception>
		/// <exception cref="ArgumentNullException">
		/// If <paramref name="displayName"/> is <see langword="null"/>
		/// </exception>
		public static TUnderlyingType GetValueForDisplayName<TEnum, TUnderlyingType>(string displayName)
		{
			Type type = typeof(TEnum);
			if (type.IsEnum == false)
				throw new ArgumentException("The specified TEnum type argument must be an enum type");
			if (typeof(TUnderlyingType) != Enum.GetUnderlyingType(type))
				throw new ArgumentException("The specified TUnderlyingType type argument must be the type underlying the specified enum type (TEnum)");
			if (displayName == null)
				throw new ArgumentNullException("displayName");

			try
			{
				return
					(from field in type.GetFields(BindingFlags.Static | BindingFlags.Public)
					 let attribute = field.GetCustomAttributes(typeof(FieldDisplayNameAttribute), true).Cast<FieldDisplayNameAttribute>().FirstOrDefault()
					 where (attribute != null && attribute.DisplayName.EqualsIgnoreCase(displayName)) || field.Name.EqualsIgnoreCase(displayName)
					 select (TUnderlyingType)field.GetValue(null)).First();
			}
			catch (InvalidOperationException)
			{
				throw new ArgumentException("No enum constant has the display name specified", "displayName");
			}
		}
	}
}
using System;
using System.ComponentModel;


namespace DigitallyCreated.Utilities.Bcl
{
	/// <summary>
	/// A version of the <see cref="DisplayNameAttribute"/> that can be used on fields.
	/// This is particularly useful for annotating enum members.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public class FieldDisplayNameAttribute : DisplayNameAttribute
	{
		/// <summary>
		/// Default constructor, sets the display name to <see cref="String.Empty"/>
		/// </summary>
		public FieldDisplayNameAttribute()
		{
		}


		/// <summary>
		/// Constructor, sets the display name to <paramref name="displayName"/>
		/// </summary>
		/// <param name="displayName">The display name of the field</param>
		public FieldDisplayNameAttribute(string displayName)
			: base(displayName)
		{
		}
	}
}
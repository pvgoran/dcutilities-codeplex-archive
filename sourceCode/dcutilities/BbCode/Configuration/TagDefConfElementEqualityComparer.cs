using System;
using System.Collections.Generic;

namespace DigitallyCreated.Utilities.BbCode.Configuration
{
	/// <summary>
	/// An <see cref="IEqualityComparer{T}"/> that compares <see cref="TagDefinitionConfigurationElement"/>s
	/// </summary>
	internal class TagDefConfElementEqualityComparer : IEqualityComparer<TagDefinitionConfigurationElement>
	{
		public static TagDefConfElementEqualityComparer Instance = new TagDefConfElementEqualityComparer();

		private TagDefConfElementEqualityComparer()
		{
		}


		/// <summary>
		/// Determines whether the specified objects are equal.
		/// </summary>
		/// <returns>
		/// true if the specified objects are equal; otherwise, false.
		/// </returns>
		/// <param name="x">The first object to compare.</param>
		/// <param name="y">The second object to compare.</param>
		public bool Equals(TagDefinitionConfigurationElement x, TagDefinitionConfigurationElement y)
		{
			return x.TagDefinitionType.Equals(y.TagDefinitionType);
		}


		/// <summary>
		/// Returns a hash code for the specified object.
		/// </summary>
		/// <returns>
		/// A hash code for the specified object.
		/// </returns>
		/// <param name="obj">The <see cref="object"/> for which a hash code is to be returned.</param>
		/// <exception cref="ArgumentNullException">
		/// The type of <paramref name="obj"/> is a reference type and <paramref name="obj"/> is null.
		/// </exception>
		public int GetHashCode(TagDefinitionConfigurationElement obj)
		{
			return obj.TagDefinitionType.GetHashCode();
		}
	}
}
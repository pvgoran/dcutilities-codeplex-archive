using System.Collections.Generic;


namespace DigitallyCreated.Utilities.BbCode
{
	/// <summary>
	/// Defines a common set of methods for reading from an in-memory parsed version of a BBCode string.
	/// It defines a set of <see cref="ITagInstance"/>s which represent tags found on the render string,
	/// which can be viewed with the <see cref="GetRenderString()"/> methods.
	/// </summary>
	/// <seealso cref="IRenderContext"/>
	/// <seealso cref="IWhitespaceRemovalContext"/>
	public interface IBbStringContext
	{
		/// <summary>
		/// A read only list of <see cref="ITagInstance"/>s found in the input BBCode string. The tags are ordered
		/// by the <see cref="CharRange.StartAt"/> property of their <see cref="ITagInstance.CharRange"/>
		/// </summary>
		IList<ITagInstance> Tags { get; }


		/// <summary>
		/// Gets the entire render string as it looks at this point in time. This means it may be in a partially
		/// rendered (or not rendered at all) form.
		/// </summary>
		/// <returns>The whole render string</returns>
		string GetRenderString();

		/// <summary>
		/// Gets a subset of the render string as it looks at this point in time. This means it may be in a partially
		/// rendered (or not rendered at all) form.
		/// </summary>
		/// <param name="startIndex">The index to start at</param>
		/// <param name="length">The length of the substring to return</param>
		/// <returns>The subset of the render string</returns>
		string GetRenderString(int startIndex, int length);

		/// <summary>
		/// Gets the subset of the render string that is between the specified <see cref="ITagInstance"/>s as it
		/// looks at this point in time. This means i may be in a partially rendering (or not rendered at all) form.
		/// </summary>
		/// <param name="afterTag">The first <see cref="ITagInstance"/></param>
		/// <param name="untilTag">The second <see cref="ITagInstance"/></param>
		/// <returns>The subset of the render string</returns>
		string GetRenderString(ITagInstance afterTag, ITagInstance untilTag);

		/// <summary>
		/// Returns an <see cref="IEnumerable{T}"/> of the tags including and following the specified 
		/// <paramref name="tagInstance"/> in the ordered sequence of tags defined by <see cref="Tags"/>.
		/// </summary>
		/// <param name="tagInstance">The <see cref="ITagInstance"/> to return the tags after</param>
		/// <returns>
		/// The <see cref="ITagInstance"/>s starting with and after <paramref name="tagInstance"/> 
		/// from the <see cref="Tags"/> list.
		/// </returns>
		IEnumerable<ITagInstance> GetTagsOnwardsFrom(ITagInstance tagInstance);

		/// <summary>
		/// Returns an <see cref="IEnumerable{T}"/> of the tags following the specified 
		/// <paramref name="tagInstance"/> in the ordered sequence of tags defined by <see cref="Tags"/>.
		/// </summary>
		/// <param name="tagInstance">The <see cref="ITagInstance"/> to return the tags after</param>
		/// <returns>
		/// The <see cref="ITagInstance"/>s after (and not including) <paramref name="tagInstance"/> 
		/// from the <see cref="Tags"/> list.
		/// </returns>
		IEnumerable<ITagInstance> GetTagsAfter(ITagInstance tagInstance);
	}
}
using System.Collections.Generic;


namespace DigitallyCreated.Utilities.BbCode
{
	/// <summary>
	/// Defines a set of methods that allow <see cref="ITagDefinition"/>s to render their changes to the render
	/// string in a controlled fashion. Insertions and deletions of character ranges from the render string
	/// automatically cause updates in the <see cref="IBbStringContext.Tags"/> and the 
	/// <see cref="HtmlEscapedRanges"/>.
	/// </summary>
	/// <seealso cref="IBbStringContext"/>
	public interface IRenderContext : IBbStringContext
	{
		/// <summary>
		/// Defines a series of <see cref="CharRange"/> that have already been HTML escaped (or simply contain
		/// HTML tags you don't want to be escaped). The series is ordered by <see cref="CharRange.StartAt"/>
		/// </summary>
		IEnumerable<CharRange> HtmlEscapedRanges { get; }

		/// <summary>
		/// Returns whether the rendering result is cacheable (ie. it will always return the same
		/// output given the same <see cref="ITagDefinition"/>s and the same input).
		/// </summary>
		/// <remarks>
		/// <see cref="ITagDefinition"/>s should call <see cref="RegisterRenderCacheability"/> if they wish
		/// to register that they are rendering something that is not cacheable. If one 
		/// <see cref="ITagDefinition"/> registers that it's doing non-cacheable stuff, it makes the entire
		/// render string uncacheable.
		/// </remarks>
		bool IsCacheable { get; }

		/// <summary>
		/// Should be called by <see cref="ITagDefinition"/>s that render things that are not cacheable. For
		/// example, an <see cref="ITagDefinition"/> that reads from a database and renders something produces
		/// non-cacheable output (as the output would change if the database changed).
		/// </summary>
		/// <remarks>
		/// If an <see cref="ITagDefinition"/> does not call this method, it is assumed that its output is
		/// cacheable.
		/// </remarks>
		/// <param name="cacheable">Whether or not the <see cref="ITagDefinition"/>'s render is cacheable</param>
		void RegisterRenderCacheability(bool cacheable);

		/// <summary>
		/// Replaces the range of characters defined by the <paramref name="tagInstance"/> with the specified
		/// <paramref name="replacementText"/>.
		/// </summary>
		/// <remarks>
		/// If the <paramref name="replacementText"/> is longer or shorter than the range of characters defined by
		/// the <paramref name="tagInstance"/>, the <see cref="IBbStringContext.Tags"/> and 
		/// <see cref="HtmlEscapedRanges"/> are automatically shifted to reflect the changes.
		/// </remarks>
		/// <param name="tagInstance">The <see cref="ITagInstance"/> whose character range will be replaced</param>
		/// <param name="replacementText">The replacement text</param>
		/// <param name="registerAsHtmlEscaped">
		/// Whether or not to register the rendered character range as an escaped character range (see
		/// <see cref="HtmlEscapedRanges"/>).
		/// </param>
		void RenderTag(ITagInstance tagInstance, string replacementText, bool registerAsHtmlEscaped);

		/// <summary>
		/// Replaces the specified range of characters with the specified <paramref name="replacementText"/>. Note
		/// that the range of characters cannot overlap with a range of characters that is owned by a 
		/// <see cref="ITagInstance"/> from <see cref="IBbStringContext.Tags"/>.
		/// </summary>
		/// <param name="startAtIndex">The index of the first character in the range</param>
		/// <param name="length">The length of the character range</param>
		/// <param name="replacementText">The replacement text</param>
		/// <param name="registerAsHtmlEscaped">
		/// Whether or not to register the rendered character range as an escaped character range (see
		/// <see cref="HtmlEscapedRanges"/>).
		/// </param>
		void RenderNonTagRange(int startAtIndex, int length, string replacementText, bool registerAsHtmlEscaped);
	}
}
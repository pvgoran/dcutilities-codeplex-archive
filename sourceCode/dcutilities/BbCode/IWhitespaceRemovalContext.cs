namespace DigitallyCreated.Utilities.BbCode
{
	/// <summary>
	/// Defines a set of methods that allow <see cref="ITagDefinition"/>s to remove whitespace around their 
	/// <see cref="ITagInstance"/>s in a controlled fashion. Insertions and deletions of character ranges from
	/// the render string automatically cause updates in the <see cref="IBbStringContext.Tags"/>.
	/// </summary>
	/// <seealso cref="IBbStringContext"/>
	public interface IWhitespaceRemovalContext : IBbStringContext
	{
		/// <summary>
		/// Removes up to a number of newlines (and whitespace between them) that are found before the specified 
		/// <paramref name="tagInstance"/>.
		/// </summary>
		/// <remarks>
		/// If <paramref name="numberOfNewlines"/> is higher than the actual number of newlines before the
		/// <paramref name="tagInstance"/>, only the actual number of newlines will be removed.
		/// </remarks>
		/// <param name="tagInstance">The tag instance to remove whitespace from before</param>
		/// <param name="numberOfNewlines">The number of newlines to remove</param>
		void RemoveWhitespaceBeforeTag(ITagInstance tagInstance, int numberOfNewlines);

		/// <summary>
		/// Removes up to a number of newlines (and whitespace between them) that are found after the specified 
		/// <paramref name="tagInstance"/>.
		/// </summary>
		/// <remarks>
		/// If <paramref name="numberOfNewlines"/> is higher than the actual number of newlines after the
		/// <paramref name="tagInstance"/>, only the actual number of newlines will be removed.
		/// </remarks>
		/// <param name="tagInstance">The tag instance to remove whitespace from after</param>
		/// <param name="numberOfNewlines">The number of newlines to remove</param>
		void RemoveWhitespaceAfterTag(ITagInstance tagInstance, int numberOfNewlines);
	}
}
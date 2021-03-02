using System.Collections.Generic;


namespace DigitallyCreated.Utilities.BbCode
{
	/// <summary>
	/// A <see cref="ITagDefinition"/> defines a type of BBCode tag and is able to identify instances of the tag
	/// (<see cref="ITagInstance"/>s), remove any necessary whitespace around it and render the tag to XHTML.
	/// </summary>
	/// <seealso cref="ITagInstance"/>
	public interface ITagDefinition
	{
		/// <summary>
		/// Identifies all instances of tags belonging to this definition in the specified string 
		/// (<paramref name="input"/>). The tags that are identified are allowed to be semantically invalid (removal
		/// of errant tags happens as a separate process).
		/// </summary>
		/// <param name="input">The input string</param>
		/// <returns>The tag instances found, in any order</returns>
		IEnumerable<ITagInstance> IdentifyTagInstances(string input);


		/// <summary>
		/// Causes the <see cref="ITagDefinition"/> to remove any whitespace it sees fit that is around its
		/// identified <see cref="ITagInstance"/>s. An <see cref="ITagDefinition"/> does not need to remove
		/// any whitespace, if it doesn't want to.
		/// </summary>
		/// <remarks>
		/// This method is always called before <see cref="Render"/> is called on any <see cref="ITagDefinition"/>.
		/// The order in which this method is called across the different <see cref="ITagDefinition"/>s is undefined.
		/// </remarks>
		/// <param name="context">
		/// The <see cref="IWhitespaceRemovalContext"/> contains all <see cref="ITagInstance"/>s, the input text,
		/// and methods that allow for the removal of whitespace.
		/// </param>
		void RemoveWhitespace(IWhitespaceRemovalContext context);


		/// <summary>
		/// Causes the <see cref="ITagDefinition"/> to render all of its <see cref="ITagInstance"/>s into XHTML.
		/// </summary>
		/// <remarks>
		/// This method is always called after <see cref="RemoveWhitespace"/> is called on any 
		/// <see cref="ITagDefinition"/>. The order in which this method is called across the different 
		/// <see cref="ITagDefinition"/>s is undefined.
		/// </remarks>
		/// <param name="context">
		/// The <see cref="IRenderContext"/> contains all the <see cref="ITagInstance"/>s, the input text,
		/// and methods that allow for the rendering of <see cref="ITagInstance"/>s.
		/// </param>
		void Render(IRenderContext context);


		/// <summary>
		/// Makes a closing tag instance for the specified <see cref="IOpenTagInstance"/>. Used to create closing
		/// tags for unclosed tag pairs during tag validation.
		/// </summary>
		/// <param name="openTagInstance">The <see cref="IOpenTagInstance"/> that needs a closing tag</param>
		/// <param name="firstCharIndex">The index of where the first character of the close tag must be</param>
		/// <param name="tagText">
		/// Returns the text of the tag, which will be added to the input string by the validator
		/// </param>
		/// <returns>The <see cref="ICloseTagInstance"/></returns>
		ICloseTagInstance MakeCloseTagFor(IOpenTagInstance openTagInstance, int firstCharIndex, out string tagText);
	}
}
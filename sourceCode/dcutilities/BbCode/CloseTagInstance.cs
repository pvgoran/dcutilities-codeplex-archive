using System.Collections.Generic;
using System.Linq;


namespace DigitallyCreated.Utilities.BbCode
{
	/// <summary>
	/// The default concrete implementation of <see cref="ICloseTagInstance"/> that represents a single closing
	/// BBCode tag.
	/// </summary>
	/// <seealso cref="ICloseTagInstance"/>
	public class CloseTagInstance : TagInstance, ICloseTagInstance
	{
		private readonly IEnumerable<IVetoRule> _CloseTagVetoRules;
		

		/// <summary>
		/// Constructor, creates a <see cref="CloseTagInstance"/>
		/// </summary>
		/// <param name="charRange">The range of characters that the tag falls under</param>
		/// <param name="parentDefinition">
		/// The <see cref="ITagDefinition"/> that created this <see cref="ITagInstance"/>
		/// </param>
		/// <param name="rendersToInlineElement">
		/// Whether or not the tag will render to an inline XHTML element (such as a span tag) or not (such as a
		/// blockquote tag).
		/// </param>
		/// <param name="closeTagVetoRules">
		/// A collection of <see cref="IVetoRule"/>s that are used by the <see cref="CheckIfValidClose"/> method to
		/// determine tag validity.
		/// </param>
		/// <param name="attributes">Any attributes set on the tag (ie. tag metadata)</param>
		public CloseTagInstance(CharRange charRange, ITagDefinition parentDefinition, bool rendersToInlineElement, IEnumerable<IVetoRule> closeTagVetoRules, IEnumerable<KeyValuePair<string, object>> attributes)
			: base(charRange, parentDefinition, rendersToInlineElement, attributes)
		{
			_CloseTagVetoRules = closeTagVetoRules;
		}


		/// <summary>
		/// Checks if the current tag is valid in its current location. For example, the 
		/// <see cref="ICloseTagInstance"/> should check the <see cref="ValidationContext.OpenTagStack"/> and ensure
		/// that the topmost open tag is the matching open tag for the close tag.
		/// </summary>
		/// <param name="context">The <see cref="ValidationContext"/></param>
		/// <returns>
		/// True if the tag is valid, false if it is not (which causes it and its associated 
		/// <see cref="IOpenTagInstance"/> to be removed and therefore ignored during rendering)
		/// </returns>
		public bool CheckIfValidClose(ValidationContext context)
		{
			return context.OpenTagStack.Peek().ParentDefinition == ParentDefinition &&
			       _CloseTagVetoRules.Any(r => r.CheckForVeto(this, context)) == false;
		}
	}
}
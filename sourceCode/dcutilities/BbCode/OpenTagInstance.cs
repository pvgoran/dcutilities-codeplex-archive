using System.Collections.Generic;
using System.Linq;


namespace DigitallyCreated.Utilities.BbCode
{
	/// <summary>
	/// The default concrete implementation of <see cref="IOpenTagInstance"/> that represents a single opening
	/// BBCode tag.
	/// </summary>
	/// <seealso cref="IOpenTagInstance"/>
	public class OpenTagInstance : TagInstance, IOpenTagInstance
	{
		private readonly OpenTagVetoRulesSet _OpenTagVetoRulesSet;

		/// <summary>
		/// The closing tag instance for this opening tag
		/// </summary>
		public ICloseTagInstance CloseTag { get; set; }


		/// <summary>
		/// Constructor, creates a <see cref="OpenTagInstance"/>
		/// </summary>
		/// <param name="charRange">The range of characters that the tag falls under</param>
		/// <param name="parentDefinition">
		/// The <see cref="ITagDefinition"/> that created this <see cref="ITagInstance"/>
		/// </param>
		/// <param name="rendersToInlineElement">
		/// Whether or not the tag will render to an inline XHTML element (such as a span tag) or not (such as a
		/// blockquote tag).
		/// </param>
		/// <param name="openTagVetoRulesSet">
		/// Collections of <see cref="IVetoRule"/>s that are used by the <see cref="CheckForVetoAgainstAnotherTag"/>
		/// and <see cref="CheckForSelfVeto"/> methods to determine tag validity.
		/// </param>
		/// <param name="attributes">Any attributes set on the tag (ie. tag metadata)</param>
		public OpenTagInstance(CharRange charRange, ITagDefinition parentDefinition, bool rendersToInlineElement, OpenTagVetoRulesSet openTagVetoRulesSet, IEnumerable<KeyValuePair<string, object>> attributes)
			: base(charRange, parentDefinition, rendersToInlineElement, attributes)
		{
			_OpenTagVetoRulesSet = openTagVetoRulesSet;
		}


		/// <summary>
		/// Asks this <see cref="IOpenTagInstance"/> whether the specified <paramref name="tagInstance"/>is allowed
		/// to be inside it (ie before this open tag's close tag occurs).
		/// </summary>
		/// <param name="tagInstance">
		/// The <see cref="IOpenTagInstance"/> which is trying to reside before the this tag's
		/// <see cref="ICloseTagInstance"/>.
		/// </param>
		/// <param name="context">The <see cref="ValidationContext"/></param>
		/// <returns>
		/// True if this tag objects to the specified <paramref name="tagInstance"/> (which causes the
		/// <paramref name="tagInstance"/> to be removed and therefore ignored during rendering), 
		/// false if it is okay for the <paramref name="tagInstance"/> to be there.
		/// </returns>
		public bool CheckForVetoAgainstAnotherTag(IOpenTagInstance tagInstance, ValidationContext context)
		{
			return _OpenTagVetoRulesSet.OtherTagVetoRules.Any(r => r.CheckForVeto(tagInstance, context));
		}


		/// <summary>
		/// Looks at the current <see cref="ValidationContext"/> and sees whether it is okay if the
		/// current tag instance resides where it does. For example, the <see cref="IOpenTagInstance"/> may 
		/// check the  <see cref="ValidationContext.OpenTagStack"/> and see whether it is allowed to be inside
		/// any of the tags that are currently on the stack.
		/// </summary>
		/// <param name="context">The <see cref="ValidationContext"/></param>
		/// <returns>
		/// True if this tag objects to its own location (which causes it to be removed and therefore ignored
		/// during rendering), false if it is okay for it to be where it is.
		/// </returns>
		public bool CheckForSelfVeto(ValidationContext context)
		{
			return _OpenTagVetoRulesSet.SelfVetoRules.Any(r => r.CheckForVeto(this, context));
		}
	}
}
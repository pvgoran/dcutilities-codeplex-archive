using System.Collections.Generic;
using System.Text.RegularExpressions;


namespace DigitallyCreated.Utilities.BbCode
{
	/// <summary>
	/// An <see cref="ITagInstance"/> is an instance of a tag that is identified in an input string by a 
	/// <see cref="ITagDefinition"/>. It defines the things that are common to both opening and closing tags.
	/// Tags are generally either <see cref="IOpenTagInstance"/>s or <see cref="ICloseTagInstance"/>s, but a self
	/// closing tag may implement both of those interfaces.
	/// </summary>
	/// <seealso cref="IOpenTagInstance"/>
	/// <seealso cref="ICloseTagInstance"/>
	public interface ITagInstance
	{
		/// <summary>
		/// The range of characters that the tag falls under
		/// </summary>
		CharRange CharRange { get; }

		/// <summary>
		/// Whether or not the tag has been rendered yet
		/// </summary>
		bool Rendered { get; set; }

		/// <summary>
		/// Whether or not the tag will render to an inline XHTML element (such as a span tag)
		/// or not (such as a blockquote tag).
		/// </summary>
		/// <remarks>
		/// This is used during tag validation to ensure XHTML validity.
		/// </remarks>
		bool RendersToInlineElement { get; }

		/// <summary>
		/// The <see cref="ITagDefinition"/> that created this <see cref="ITagInstance"/>
		/// </summary>
		ITagDefinition ParentDefinition { get; }

		/// <summary>
		/// A read-only dictionary that contains any attributes set on the tag (ie. tag metadata).
		/// </summary>
		/// <remarks>
		/// This is populated during tag instance creation with data taken from parsing the tag
		/// </remarks>
		IDictionary<string, object> Attributes { get; }
	}
}
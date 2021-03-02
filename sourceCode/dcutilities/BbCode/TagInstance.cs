using System.Collections.Generic;
using DigitallyCreated.Utilities.Bcl;


namespace DigitallyCreated.Utilities.BbCode
{
	/// <summary>
	/// Abstract base class that implements <see cref="ITagInstance"/> and represents a BBCode tag
	/// </summary>
	/// <seealso cref="ITagInstance"/>
	public abstract class TagInstance : ITagInstance
	{
		private readonly ITagDefinition _ParentDefinition;
		private readonly CharRange _CharRange;
		private readonly bool _RendersToInlineElement;
		private readonly IDictionary<string, object> _Attributes;


		/// <summary>
		/// Whether or not the tag will render to an inline XHTML element (such as a span tag)
		/// or not (such as a blockquote tag).
		/// </summary>
		/// <remarks>
		/// This is used during tag validation to ensure XHTML validity.
		/// </remarks>
		public bool RendersToInlineElement { get { return _RendersToInlineElement; } }

		/// <summary>
		/// The range of characters that the tag falls under
		/// </summary>
		public CharRange CharRange { get { return _CharRange; } }

		/// <summary>
		/// The <see cref="ITagDefinition"/> that created this <see cref="ITagInstance"/>
		/// </summary>
		public ITagDefinition ParentDefinition { get { return _ParentDefinition; } }

		/// <summary>
		/// A read-only dictionary that contains any attributes set on the tag (ie. tag metadata).
		/// </summary>
		/// <remarks>
		/// This is populated during tag instance creation with data taken from parsing the tag
		/// </remarks>
		public IDictionary<string, object> Attributes { get { return _Attributes; } }

		/// <summary>
		/// Whether or not the tag has been rendered yet
		/// </summary>
		public bool Rendered { get; set; }


		/// <summary>
		/// Constructor, initialises a <see cref="TagInstance"/>
		/// </summary>
		/// <param name="charRange">The range of characters that the tag falls under</param>
		/// <param name="parentDefinition">
		/// The <see cref="ITagDefinition"/> that created this <see cref="ITagInstance"/>
		/// </param>
		/// <param name="rendersToInlineElement">
		/// Whether or not the tag will render to an inline XHTML element (such as a span tag) or not (such as a
		/// blockquote tag).
		/// </param>
		/// <param name="attributes">Any attributes set on the tag (ie. tag metadata)</param>
		protected TagInstance(CharRange charRange, ITagDefinition parentDefinition, bool rendersToInlineElement, IEnumerable<KeyValuePair<string, object>> attributes)
		{
			_CharRange = charRange;
			_RendersToInlineElement = rendersToInlineElement;
			_ParentDefinition = parentDefinition;
			_Attributes = new Dictionary<string, object>();
			_Attributes.AddAll(attributes);
			_Attributes = new ReadOnlyDictionary<string, object>(_Attributes);
		}
	}
}
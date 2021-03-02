using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;


namespace DigitallyCreated.Utilities.BbCode
{
	/// <summary>
	/// This class is an <see cref="ITagDefinition"/> and is a skeleton abstract class that implements 
	/// functionality common to most BBCode tag pairs.
	/// </summary>
	/// <remarks>
	/// In a lot of cases the <see cref="SimpleTagDefinition"/> is a better class to inherit from for your
	/// tag definitions. Use this class only if <see cref="SimpleTagDefinition"/> is too restrictive and/or
	/// doesn't suit.
	/// </remarks>
	/// <seealso cref="ITagDefinition"/>
	/// <seealso cref="SimpleTagDefinition"/>
	public abstract class OpenCloseTagDefinition : ITagDefinition
	{
		private readonly Regex _OpenTagRegex;
		private readonly Regex _CloseTagRegex;
		private readonly string _CloseTag;
		private readonly OpenTagVetoRulesSet _OpenTagVetoRulesSet;
		private readonly IEnumerable<IVetoRule> _CloseTagVetoRules;
		private readonly bool _RendersToInlineElement;

		/// <summary>
		/// The <see cref="Regex"/> that is used to find <see cref="IOpenTagInstance"/>s
		/// </summary>
		protected Regex OpenTagRegex { get { return _OpenTagRegex; } }

		/// <summary>
		/// The <see cref="Regex"/> that is used to find <see cref="ICloseTagInstance"/>s
		/// </summary>
		protected Regex CloseTagRegex { get { return _CloseTagRegex; } }

		/// <summary>
		/// The BBCode close tag for this tag definition
		/// </summary>
		protected string CloseTag { get { return _CloseTag; } }

		/// <summary>
		/// Collections of <see cref="IVetoRule"/>s that are used by the 
		/// <see cref="IOpenTagInstance.CheckForVetoAgainstAnotherTag"/> and 
		/// <see cref="IOpenTagInstance.CheckForSelfVeto"/> methods to determine tag validity.
		/// </summary>
		protected OpenTagVetoRulesSet OpenTagVetoRulesSet { get { return _OpenTagVetoRulesSet; } }

		/// <summary>
		/// A collection of <see cref="IVetoRule"/>s that are used by the 
		/// <see cref="ICloseTagInstance.CheckIfValidClose"/> method to determine tag validity.
		/// </summary>
		protected IEnumerable<IVetoRule> CloseTagVetoRules { get { return _CloseTagVetoRules; } }

		/// <summary>
		/// Whether or not the tags will render to a inline XHTML elements (such as a span tags) or not (such as
		/// blockquote tags).
		/// </summary>
		protected bool RendersToInlineElement { get { return _RendersToInlineElement; } }


		/// <summary>
		/// Constructor, creates an <see cref="OpenCloseTagDefinition"/>
		/// </summary>
		/// <param name="openTagRegex">
		/// The <see cref="Regex"/> that is used to find <see cref="IOpenTagInstance"/>s
		/// </param>
		/// <param name="closeTagRegex">
		/// The <see cref="Regex"/> that is used to find <see cref="ICloseTagInstance"/>s
		/// </param>
		/// <param name="closeTag">The BBCode close tag for this tag definition</param>
		/// <param name="openTagVetoRulesSet">
		/// Collections of <see cref="IVetoRule"/>s that are used by the 
		/// <see cref="IOpenTagInstance.CheckForVetoAgainstAnotherTag"/> and 
		/// <see cref="IOpenTagInstance.CheckForSelfVeto"/> methods to determine tag validity.
		/// </param>
		/// <param name="closeTagVetoRules">
		/// A collection of <see cref="IVetoRule"/>s that are used by the 
		/// <see cref="ICloseTagInstance.CheckIfValidClose"/> method to determine tag validity.
		/// </param>
		/// <param name="rendersToInlineElement">
		/// Whether or not the tags will render to a inline XHTML elements (such as a span tags) or not (such as
		/// blockquote tags).
		/// </param>
		protected OpenCloseTagDefinition(Regex openTagRegex, Regex closeTagRegex, string closeTag, OpenTagVetoRulesSet openTagVetoRulesSet, IEnumerable<IVetoRule> closeTagVetoRules, bool rendersToInlineElement)
		{
			_OpenTagRegex = openTagRegex;
			_CloseTagVetoRules = closeTagVetoRules;
			_CloseTagRegex = closeTagRegex;
			_CloseTag = closeTag;
			_OpenTagVetoRulesSet = openTagVetoRulesSet;
			_RendersToInlineElement = rendersToInlineElement;
		}


		/// <summary>
		/// Identifies all <see cref="OpenTagInstance"/>s found in <paramref name="input"/> using 
		/// <see cref="OpenTagRegex"/>. The identified instances do not necessarily create valid BBCode.
		/// </summary>
		/// <param name="input">The input BBcode string</param>
		/// <returns>An enumeration of <see cref="OpenTagInstance"/>s found</returns>
		protected virtual IEnumerable<OpenTagInstance> IdentifyOpenTagInstances(string input)
		{
			return _OpenTagRegex.Matches(input)
				.Cast<Match>()
				.Select(m => new OpenTagInstance(new CharRange(m.Index, m.Length), this, _RendersToInlineElement, _OpenTagVetoRulesSet, ReadTagAttributesFromMatch(m, true)));
		}


		/// <summary>
		/// Identifies all <see cref="CloseTagInstance"/>s found in <paramref name="input"/> using 
		/// <see cref="CloseTagRegex"/>. The identified instances do not necessarily create valid BBCode.
		/// </summary>
		/// <param name="input">The input BBCode string</param>
		/// <returns>An enumeration of <see cref="CloseTagInstance"/>s found</returns>
		protected virtual IEnumerable<CloseTagInstance> IdentifyCloseTagInstances(string input)
		{
			return _CloseTagRegex.Matches(input)
				.Cast<Match>()
				.Select(m => new CloseTagInstance(new CharRange(m.Index, m.Length), this, _RendersToInlineElement, _CloseTagVetoRules, ReadTagAttributesFromMatch(m, false)));
		}


		/// <summary>
		/// If overridden in a derived class, reads tag attributes from the regular expression <see cref="Match"/>
		/// of the tag. By default, it returns an empty enumeration.
		/// </summary>
		/// <param name="match">The tags's <see cref="Match"/></param>
		/// <param name="openTag">
		/// True if the match is a match on an open tag, false if it's a match on a close tag
		/// </param>
		/// <returns>
		/// <see cref="KeyValuePair{TKey,TValue}"/> pairs where the key is the name of attribute, and the value is
		/// the value of the attribute
		/// </returns>
		protected virtual IEnumerable<KeyValuePair<string, object>> ReadTagAttributesFromMatch(Match match, bool openTag)
		{
			return Enumerable.Empty<KeyValuePair<string, object>>();
		}


		/// <summary>
		/// Identifies all instances of tags belonging to this definition in the specified string 
		/// (<paramref name="input"/>). The tags that are identified are allowed to be semantically invalid (removal
		/// of errant tags happens as a separate process).
		/// </summary>
		/// <remarks>
		/// This method uses <see cref="IdentifyOpenTagInstances"/> and <see cref="IdentifyCloseTagInstances"/>s to
		/// do its job.
		/// </remarks>
		/// <param name="input">The input string</param>
		/// <returns>The tag instances found, in any order</returns>
		public virtual IEnumerable<ITagInstance> IdentifyTagInstances(string input)
		{
			return IdentifyOpenTagInstances(input).Concat<ITagInstance>(IdentifyCloseTagInstances(input));
		}


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
		public virtual ICloseTagInstance MakeCloseTagFor(IOpenTagInstance openTagInstance, int firstCharIndex, out string tagText)
		{
			tagText = _CloseTag;
			return new CloseTagInstance(new CharRange(firstCharIndex, _CloseTag.Length), this, _RendersToInlineElement, _CloseTagVetoRules, Enumerable.Empty<KeyValuePair<string, object>>());
		}


		/// <summary>
		/// Causes the <see cref="ITagDefinition"/> to remove any whitespace it sees fit that is around its
		/// identified <see cref="ITagInstance"/>s. An <see cref="ITagDefinition"/> does not need to remove any
		/// whitespace, if it doesn't want to.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This method is always called before <see cref="ITagDefinition.Render"/> is called on any 
		/// <see cref="ITagDefinition"/>. The order in which this method is called across the different 
		/// <see cref="ITagDefinition"/>s is undefined.
		/// </para>
		/// <para>
		/// This method does nothing in its default implementation. Inheritors should override this method
		/// if they want to perform whitespace removal.
		/// </para>
		/// </remarks>
		/// <param name="context">
		/// The <see cref="IWhitespaceRemovalContext"/> contains all <see cref="ITagInstance"/>s, the input text,
		/// and methods that allow for the removal of whitespace.
		/// </param>
		public virtual void RemoveWhitespace(IWhitespaceRemovalContext context)
		{
			//Do nothing by default. Override if you need to perform something here.
		}


		/// <summary>
		/// Causes the <see cref="ITagDefinition"/> to render all of its <see cref="ITagInstance"/>s into XHTML.
		/// </summary>
		/// <remarks>
		/// This method is always called after <see cref="ITagDefinition.RemoveWhitespace"/> is called on any 
		/// <see cref="ITagDefinition"/>. The order in which this method is called across the different 
		/// <see cref="ITagDefinition"/>s is undefined.
		/// </remarks>
		/// <param name="context">
		/// The <see cref="IRenderContext"/> contains all the <see cref="ITagInstance"/>s, the input text,
		/// and methods that allow for the rendering of <see cref="ITagInstance"/>s.
		/// </param>
		public abstract void Render(IRenderContext context);
	}
}
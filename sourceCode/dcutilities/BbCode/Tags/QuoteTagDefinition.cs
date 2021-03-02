using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using System.Linq;


namespace DigitallyCreated.Utilities.BbCode.Tags
{
	/// <summary>
	/// The <see cref="ITagDefinition"/> for BBCode quote tags ([quote][/quote]).
	/// </summary>
	/// <seealso cref="ITagDefinition"/>
	public class QuoteTagDefinition : OpenCloseTagDefinition
	{
		/// <summary>
		/// The BBCode quote close tag
		/// </summary>
		public new const string CloseTag = "[/quote]";

		private const string REPLACEMENT_OPEN_TAG = "<blockquote>";
		private const string REPLACEMENT_CLOSE_TAG = "</blockquote>";
		private const string AUTHOR_TAG_FORMAT_STRING = "<p class=\"QuoteAuthor\">{0} wrote:</p>";
		private const string TEXT_WRAP_OPEN_TAG = "<p>";
		private const string TEXT_WRAP_CLOSE_TAG = "</p>";
		private static readonly Regex _OpenTagRegex = new Regex(@"\[quote(?:=""(?<username>.+?)"")?\]");
		private static readonly Regex _CloseTagRegex = new Regex(@"\[/quote\]");
		private static readonly Regex _WhitespaceRegex = new Regex(@"^\s*$");
		private static readonly OpenTagVetoRulesSet _OpenTagVetoRulesSet = new OpenTagVetoRulesSet
		{
			OtherTagVetoRules = new IVetoRule[] { },
			SelfVetoRules = new IVetoRule[] { new MustNotNestInInlineElement() },
		};


		/// <summary>
		/// Constructor, creates a <see cref="QuoteTagDefinition"/>.
		/// </summary>
		public QuoteTagDefinition()
			: base(_OpenTagRegex, _CloseTagRegex, CloseTag, _OpenTagVetoRulesSet, Enumerable.Empty<IVetoRule>(), false)
		{
		}


		/// <summary>
		/// Reads tag attributes from the regular expression <see cref="Match"/> of the tag.
		/// </summary>
		/// <param name="match">The tags's <see cref="Match"/></param>
		/// <param name="openTag">
		/// True if the match is a match on an open tag, false if it's a match on a close tag
		/// </param>
		/// <returns>
		/// <see cref="KeyValuePair{TKey,TValue}"/> pairs where the key is the name of attribute, and the value is
		/// the value of the attribute
		/// </returns>
		protected override IEnumerable<KeyValuePair<string, object>> ReadTagAttributesFromMatch(Match match, bool openTag)
		{
			if (openTag == false)
				yield break;

			Group group = match.Groups["username"];
			if (group.Success)
				yield return new KeyValuePair<string, object>("Username", group.Value);
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
		public override void RemoveWhitespace(IWhitespaceRemovalContext context)
		{
			IEnumerable<IOpenTagInstance> tagInstances = context.Tags.Where(t => t.ParentDefinition == this)
																	 .OfType<IOpenTagInstance>();

			foreach (IOpenTagInstance openTag in tagInstances)
			{
				ICloseTagInstance closeTag = openTag.CloseTag;

				context.RemoveWhitespaceBeforeTag(openTag, 1);
				context.RemoveWhitespaceAfterTag(openTag, 1);
				context.RemoveWhitespaceBeforeTag(closeTag, 1);
				context.RemoveWhitespaceAfterTag(closeTag, 2);
			}
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
		public override void Render(IRenderContext context)
		{
			IEnumerable<IOpenTagInstance> tagInstances = context.Tags.Where(t => t.ParentDefinition == this)
																	 .OfType<IOpenTagInstance>();

			foreach (IOpenTagInstance openTag in tagInstances)
			{
				ICloseTagInstance closeTag = openTag.CloseTag;

				string replacementOpenTag = REPLACEMENT_OPEN_TAG;
				if (openTag.Attributes.ContainsKey("Username"))
					replacementOpenTag += String.Format(AUTHOR_TAG_FORMAT_STRING, WebUtility.HtmlEncode((string)openTag.Attributes["Username"]));
				context.RenderTag(openTag, replacementOpenTag, true);
				
				//Wrap all content that is not inside an block element tag in P tags
				ITagInstance afterTag = openTag;
				while (afterTag != null)
				{
					//Find the next block element tag nested inside openTag
					IOpenTagInstance untilTag = context.GetTagsAfter(afterTag)
													   .TakeWhile(t => t != closeTag) //Take until the close tag
													   .WhereStackDepthIs(0) //Only tags immediately nested inside openTag
													   .Where(t => t.RendersToInlineElement == false)
													   .OfType<IOpenTagInstance>()
													   .FirstOrDefault();

					if (untilTag != null)
					{
						WrapTextBetweenTagsInPTags(afterTag, untilTag, context);
						afterTag = untilTag.CloseTag;
					}
					else
					{
						WrapTextBetweenTagsInPTags(afterTag, closeTag, context);
						afterTag = null;
					}
				}

				context.RenderTag(closeTag, REPLACEMENT_CLOSE_TAG, true);
			}

			context.RegisterRenderCacheability(true);
		}


		/// <summary>
		/// Wraps text between <paramref name="afterTag"/> and <paramref name="untilTag"/> in paragraph
		/// XHTML tags
		/// </summary>
		/// <param name="afterTag">The tag after which the text follows</param>
		/// <param name="untilTag">The tag before which the text lies</param>
		/// <param name="context">The <see cref="RenderContext"/></param>
		private void WrapTextBetweenTagsInPTags(ITagInstance afterTag, ITagInstance untilTag, IRenderContext context)
		{
			Match whitespaceMatch = _WhitespaceRegex.Match(context.GetRenderString(afterTag, untilTag));
			if (whitespaceMatch.Success)
				return; //No need to wrap whitespace in a P tag

			context.RenderNonTagRange(afterTag.CharRange.StartAt + afterTag.CharRange.Length, 0, TEXT_WRAP_OPEN_TAG, true);
			context.RenderNonTagRange(untilTag.CharRange.StartAt, 0, TEXT_WRAP_CLOSE_TAG, true);
		}
	}
}
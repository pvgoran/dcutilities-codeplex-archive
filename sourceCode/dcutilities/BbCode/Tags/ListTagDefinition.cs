using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using DigitallyCreated.Utilities.Bcl;


namespace DigitallyCreated.Utilities.BbCode.Tags
{
	/// <summary>
	/// The <see cref="ITagDefinition"/> for BBCode list tags ([list][/list]) as well as the contained list item
	/// tag ([*]).
	/// </summary>
	/// <seealso cref="ITagDefinition"/>
	public sealed class ListTagDefinition : ITagDefinition
	{
		/// <summary>
		/// The BBCode list close tag
		/// </summary>
		public const string CloseTag = "[/list]";


		private const string REPLACEMENT_UOLIST_OPEN_TAG = "<ul>";
		private const string REPLACEMENT_UOLIST_CLOSE_TAG = "</ul>";
		private const string REPLACEMENT_OLIST_OPEN_TAG = "<ol>";
		private const string REPLACEMENT_OLIST_CLOSE_TAG = "</ol>";
		private const string REPLACEMENT_LIST_ITEM_OPEN_TAG = "<li>";
		private const string REPLACEMENT_LIST_ITEM_CLOSE_TAG = "</li>";
		private static readonly Regex _OpenTagRegEx = new Regex(@"\[list(?:=(?:(?<ordered>o(?:rdered)?)|(?<unordered>u(?:nordered)?)))?\]");
		private static readonly Regex _CloseTagRegex = new Regex(@"\[/list\]");
		private static readonly Regex _ListItemTagRegex = new Regex(@"\[\*]");
		private static readonly Regex _WhitespaceRegex = new Regex(@"^\s*$");
		private static readonly OpenTagVetoRulesSet _OpenTagVetoRulesSet = new OpenTagVetoRulesSet
		{
			OtherTagVetoRules = new IVetoRule[] { },
			SelfVetoRules = new IVetoRule[] { new MustNotNestInInlineElement() },
		};


		/// <summary>
		/// Identifies all instances of tags belonging to this definition in the specified string 
		/// (<paramref name="input"/>). The tags that are identified are allowed to be semantically invalid (removal
		/// of errant tags happens as a separate process).
		/// </summary>
		/// <param name="input">The input string</param>
		/// <returns>The tag instances found, in any order</returns>
		public IEnumerable<ITagInstance> IdentifyTagInstances(string input)
		{
			IEnumerable<OpenTagInstance> openTags = _OpenTagRegEx.Matches(input)
				.Cast<Match>()
				.Select(m => new OpenTagInstance(new CharRange(m.Index, m.Length), this, false, _OpenTagVetoRulesSet, ReadTagAttributesFromMatch(m)));

			IEnumerable<CloseTagInstance> closeTags = _CloseTagRegex.Matches(input)
				.Cast<Match>()
				.Select(m => new CloseTagInstance(new CharRange(m.Index, m.Length), this, false, Enumerable.Empty<IVetoRule>(), Enumerable.Empty<KeyValuePair<string, object>>()));

			IEnumerable<ListItemTagInstance> listTags = _ListItemTagRegex.Matches(input)
				.Cast<Match>()
				.Select(m => new ListItemTagInstance(new CharRange(m.Index, m.Length), this));

			return openTags.Concat<ITagInstance>(closeTags).Concat(listTags);
		}


		/// <summary>
		/// Reads tag attributes from the regular expression <see cref="Match"/> of the open tag.
		/// </summary>
		/// <param name="match">The tags's <see cref="Match"/></param>
		/// <returns>
		/// <see cref="KeyValuePair{TKey,TValue}"/> pairs where the key is the name of attribute, and the value is
		/// the value of the attribute
		/// </returns>
		private IEnumerable<KeyValuePair<string, object>> ReadTagAttributesFromMatch(Match match)
		{
			yield return new KeyValuePair<string, object>("Ordered", match.Groups["ordered"].Success);
		}


		/// <summary>
		/// Causes the <see cref="ITagDefinition"/> to remove any whitespace it sees fit that is around its
		/// identified <see cref="ITagInstance"/>s. An <see cref="ITagDefinition"/> does not need to remove
		/// any whitespace, if it doesn't want to.
		/// </summary>
		/// <remarks>
		/// This method is always called before <see cref="ITagDefinition.Render"/> is called on any <see cref="ITagDefinition"/>.
		/// The order in which this method is called across the different <see cref="ITagDefinition"/>s is undefined.
		/// </remarks>
		/// <param name="context">
		/// The <see cref="IWhitespaceRemovalContext"/> contains all <see cref="ITagInstance"/>s, the input text,
		/// and methods that allow for the removal of whitespace.
		/// </param>
		public void RemoveWhitespace(IWhitespaceRemovalContext context)
		{
			IEnumerable<IOpenTagInstance> tagInstances = context.Tags.Where(t => t.ParentDefinition == this)
																	 .OfType<OpenTagInstance>();

			foreach (IOpenTagInstance openTag in tagInstances)
			{
				ICloseTagInstance closeTag = openTag.CloseTag;

				IList<ListItemTagInstance> listItemTags = context.GetTagsAfter(openTag)
																 .TakeWhile(t => t != closeTag)
																 .WhereStackDepthIs(0)
																 .OfType<ListItemTagInstance>()
																 .ToList();

				//Don't remove whitespace if there's only whitespace in between the open and close tags
				//because the tag won't be rendered
				if (listItemTags.Any() == false &&
					_WhitespaceRegex.Match(context.GetRenderString(openTag, closeTag)).Success)
					continue;

				context.RemoveWhitespaceBeforeTag(openTag, 1);
				context.RemoveWhitespaceAfterTag(openTag, 1);
				context.RemoveWhitespaceBeforeTag(openTag.CloseTag, 1);
				context.RemoveWhitespaceAfterTag(openTag.CloseTag, 2);
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
		public void Render(IRenderContext context)
		{
			IEnumerable<IOpenTagInstance> tagInstances = context.Tags.Where(t => t.ParentDefinition == this)
																	 .OfType<OpenTagInstance>();

			foreach (IOpenTagInstance openTag in tagInstances)
			{
				ICloseTagInstance closeTag = openTag.CloseTag;

				IList<ListItemTagInstance> listItemTags = context.GetTagsAfter(openTag)
				                                                 .TakeWhile(t => t != closeTag)
				                                                 .WhereStackDepthIs(0)
				                                                 .OfType<ListItemTagInstance>()
				                                                 .ToList();

				string replacementOpenTag;
				string replacementCloseTag;
				ChooseReplacementTags(openTag, out replacementOpenTag, out replacementCloseTag);

				if (listItemTags.Any() == false)
				{
					//Render only if there's not only whitespace in between the open and close tags
					if (_WhitespaceRegex.Match(context.GetRenderString(openTag, closeTag)).Success == false)
					{
						context.RenderTag(openTag, replacementOpenTag + REPLACEMENT_LIST_ITEM_OPEN_TAG, true);
						context.RenderTag(closeTag, REPLACEMENT_LIST_ITEM_CLOSE_TAG + replacementCloseTag, true);
					}
				}
				else
				{
					//If there's content between the open tag and the first list item tag, wrap that in a li tag too
					ListItemTagInstance firstListItemTag = listItemTags.First();
					if (_WhitespaceRegex.Match(context.GetRenderString(openTag, firstListItemTag)).Success)
					{
						context.RenderTag(openTag, replacementOpenTag, true);
						context.RenderNonTagRange(openTag.CharRange.StartAt + openTag.CharRange.Length, firstListItemTag.CharRange.StartAt - (openTag.CharRange.StartAt + openTag.CharRange.Length), String.Empty, false);
						context.RenderTag(firstListItemTag, REPLACEMENT_LIST_ITEM_OPEN_TAG, true);
					}
					else
					{
						context.RenderTag(openTag, replacementOpenTag + REPLACEMENT_LIST_ITEM_OPEN_TAG, true);
						context.RenderTag(firstListItemTag, REPLACEMENT_LIST_ITEM_CLOSE_TAG + REPLACEMENT_LIST_ITEM_OPEN_TAG, true);
					}

					foreach (ListItemTagInstance listItemTagInstance in listItemTags.Skip(1))
						context.RenderTag(listItemTagInstance, REPLACEMENT_LIST_ITEM_CLOSE_TAG + REPLACEMENT_LIST_ITEM_OPEN_TAG, true);

					context.RenderTag(closeTag, REPLACEMENT_LIST_ITEM_CLOSE_TAG + replacementCloseTag, true);
				}
			}

			context.RegisterRenderCacheability(true);
		}


		/// <summary>
		/// Chooses the replacement open and close tags to use for rendering based off whether we are
		/// rendering an ordered or unordered list.
		/// </summary>
		/// <param name="openTag">The <see cref="IOpenTagInstance"/></param>
		/// <param name="replacementOpenTag">Returns the replacement open tag to use</param>
		/// <param name="replacementCloseTag">Returns the replacement close tag to use</param>
		private void ChooseReplacementTags(IOpenTagInstance openTag, out string replacementOpenTag, out string replacementCloseTag)
		{
			if ((bool)openTag.Attributes["Ordered"])
			{
				replacementOpenTag = REPLACEMENT_OLIST_OPEN_TAG;
				replacementCloseTag = REPLACEMENT_OLIST_CLOSE_TAG;
			}
			else
			{
				replacementOpenTag = REPLACEMENT_UOLIST_OPEN_TAG;
				replacementCloseTag = REPLACEMENT_UOLIST_CLOSE_TAG;
			}
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
		public ICloseTagInstance MakeCloseTagFor(IOpenTagInstance openTagInstance, int firstCharIndex, out string tagText)
		{
			tagText = CloseTag;
			return new CloseTagInstance(new CharRange(firstCharIndex, CloseTag.Length), this, false, Enumerable.Empty<IVetoRule>(), Enumerable.Empty<KeyValuePair<string, object>>());
		}


		/// <summary>
		/// A special tag instance that is a self-closing tag (both a <see cref="IOpenTagInstance"/> and a
		/// <see cref="ICloseTagInstance"/> at the same time) that represents a list item tag instance
		/// </summary>
		private class ListItemTagInstance : IOpenTagInstance, ICloseTagInstance
		{
			private static readonly OpenTagVetoRulesSet _ListItemVetoRulesSet = new OpenTagVetoRulesSet
			{
				OtherTagVetoRules = new IVetoRule[] { },
				SelfVetoRules = new IVetoRule[] { new MustNestInListTagVetoRule() },
			};

			private readonly CharRange _CharRange;
			private readonly ITagDefinition _ParentDefinition;
			private readonly IDictionary<string, object> _Attributes;


			public CharRange CharRange { get { return _CharRange; } }
			public bool RendersToInlineElement { get { return false; } }
			public bool Rendered { get; set; }
			public ITagDefinition ParentDefinition { get { return _ParentDefinition; } }
			public IDictionary<string, object> Attributes { get { return _Attributes; } }
			public ICloseTagInstance CloseTag { get; set; }


			public ListItemTagInstance(CharRange charRange, ITagDefinition parentDefinition)
			{
				_CharRange = charRange;
				_ParentDefinition = parentDefinition;
				_Attributes = new ReadOnlyDictionary<string, object>(new Dictionary<string, object>());
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
				return true;
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
				return _ListItemVetoRulesSet.OtherTagVetoRules.Any(r => r.CheckForVeto(tagInstance, context));
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
				return _ListItemVetoRulesSet.SelfVetoRules.Any(r => r.CheckForVeto(this, context));
			}
		}


		/// <summary>
		/// A veto rules that is used for <see cref="ListItemTagInstance"/>s to ensure they always reside
		/// directly within a list tag pair
		/// </summary>
		private class MustNestInListTagVetoRule : IVetoRule
		{
			public bool CheckForVeto(ITagInstance tagInstance, ValidationContext context)
			{
				return context.OpenTagStack.Any() == false || context.OpenTagStack.Peek().ParentDefinition != tagInstance.ParentDefinition;
			}
		}
	}
}
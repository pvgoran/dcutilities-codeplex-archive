using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DigitallyCreated.Utilities.Bcl;


namespace DigitallyCreated.Utilities.BbCode.Tags
{
	/// <summary>
	/// The <see cref="ITagDefinition"/> for BBCode flash tags ([flash][/flash]).
	/// </summary>
	/// <seealso cref="ITagDefinition"/>
	public class FlashTagDefinition : OpenCloseTagDefinition
	{
		/// <summary>
		/// The BBCode flash close tag
		/// </summary>
		public new const string CloseTag = "[/flash]";


		private const int MIN_SIZE = 1;
		private const string REPLACEMENT_TAG_FORMAT_STRING = "<object type=\"application/x-shockwave-flash\" style=\"width:{1}px; height:{2}px;\" data=\"{0}\"><param name=\"movie\" value=\"{0}\" /><param name=\"allowfullscreen\" value=\"true\" /></object>";
		private static readonly Regex _OpenTagRegex = new Regex(@"\[flash=(?<width>\d+),(?<height>\d+)\]");
		private static readonly Regex _CloseTagRegex = new Regex(@"\[/flash\]");
		private static readonly OpenTagVetoRulesSet _VetoRulesSet = new OpenTagVetoRulesSet
		{
			OtherTagVetoRules = new[] { new MustNotNestAnyTagsInMeExceptVetoRule() },
			SelfVetoRules = new IVetoRule[] { new MustHaveValidSizesVetoRule() },
		};
		private readonly int _MinWidth;
		private readonly int _MaxWidth;
		private readonly int _MinHeight;
		private readonly int _MaxHeight;


		/// <summary>
		/// Constructor, creates a <see cref="FlashTagDefinition"/>, where the flash control is allowed
		/// to be any size the user inputs
		/// </summary>
		public FlashTagDefinition()
			: this(MIN_SIZE, int.MaxValue, MIN_SIZE, int.MaxValue)
		{

		}


		/// <summary>
		/// Constructor, creates a <see cref="FlashTagDefinition"/>.
		/// </summary>
		/// <param name="minWidth">
		/// The minimum acceptable width of the flash control (must not be less than 1)
		/// </param>
		/// <param name="maxWidth">The maximum acceptable width of the flash control</param>
		/// <param name="minHeight">
		/// The minimum acceptable height of the flash control (must not be less than 1)
		/// </param>
		/// <param name="maxHeight">The maximum acceptable width of the flash control</param>
		public FlashTagDefinition(int minWidth, int maxWidth, int minHeight, int maxHeight)
			: base(_OpenTagRegex, _CloseTagRegex, CloseTag, _VetoRulesSet, Enumerable.Empty<IVetoRule>(), true)
		{
			if (minWidth < MIN_SIZE)
				throw new ArgumentException("minWidth must not be less than " + MIN_SIZE, "minWidth");
			if (minHeight < MIN_SIZE)
				throw new ArgumentException("minHeight must not be less than " + MIN_SIZE, "minHeight");
			if (maxWidth < minWidth)
				throw new ArgumentException("maxWidth must not be less than minWidth", "maxWidth");
			if (maxHeight < minWidth)
				throw new ArgumentException("maxHeight must not be less than minHeight", "maxHeight");

			_MinWidth = minWidth;
			_MaxWidth = maxWidth;
			_MinHeight = minHeight;
			_MaxHeight = maxHeight;
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

			yield return new KeyValuePair<string, object>("Width", Int32.Parse(match.Groups["width"].Value));
			yield return new KeyValuePair<string, object>("Height", Int32.Parse(match.Groups["height"].Value));
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

				string url = context.GetRenderString(openTag, closeTag);

				//If the url isn't a real URL, don't create a tag and skip it instead
				Match urlMatch = RegularExpressions.Url.Match(url);
				if (urlMatch.Success == false || urlMatch.Index != 0 || urlMatch.Length != url.Length)
					continue;

				string tag;
				int width = (int)openTag.Attributes["Width"];
				int height = (int)openTag.Attributes["Height"];
				tag = String.Format(REPLACEMENT_TAG_FORMAT_STRING, url, width, height);

				context.RenderTag(openTag, tag, true);

				//Remove the containing text and the end tag as the open tag is the only tag now
				int indexAfterOpenTag = openTag.CharRange.StartAt + openTag.CharRange.Length;
				context.RenderNonTagRange(indexAfterOpenTag, closeTag.CharRange.StartAt - indexAfterOpenTag, String.Empty, false);
				context.RenderTag(closeTag, String.Empty, false);
			}

			context.RegisterRenderCacheability(true);
		}


		/// <summary>
		/// Veto rule that ensure the flash control has valid sizes set on its <see cref="IOpenTagInstance"/>
		/// </summary>
		private class MustHaveValidSizesVetoRule : IVetoRule
		{
			/// <summary>
			/// Makes an assessment as to whether <paramref name="tagInstance"/> is a valid tag
			/// </summary>
			/// <param name="tagInstance">The <see cref="ITagInstance"/></param>
			/// <param name="context">The <see cref="ValidationContext"/></param>
			/// <returns>True if the tag is not valid should be vetoed, false otherwise</returns>
			public bool CheckForVeto(ITagInstance tagInstance, ValidationContext context)
			{
				FlashTagDefinition flashTagDef = tagInstance.ParentDefinition as FlashTagDefinition;
				if (flashTagDef == null || tagInstance is IOpenTagInstance == false)
					throw new IllegalStateException("MustHaveValidSizesVetoRule did not receive an img open tag");

				int width = (int)tagInstance.Attributes["Width"];
				int height = (int)tagInstance.Attributes["Height"];

				return (width < flashTagDef._MinWidth || width > flashTagDef._MaxWidth) ||
					   (height < flashTagDef._MinHeight || height > flashTagDef._MaxHeight);
			}
		}
	}
}
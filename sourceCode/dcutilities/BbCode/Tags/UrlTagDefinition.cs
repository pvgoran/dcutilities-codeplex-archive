using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using DigitallyCreated.Utilities.Bcl;


namespace DigitallyCreated.Utilities.BbCode.Tags
{
	/// <summary>
	/// The <see cref="ITagDefinition"/> for BBCode URL tags ([url][/url]).
	/// </summary>
	/// <seealso cref="ITagDefinition"/>
	public class UrlTagDefinition : OpenCloseTagDefinition
	{
		/// <summary>
		/// The BBCode URL close tag
		/// </summary>
		public new const string CloseTag = "[/url]";


		private const string REPLACEMENT_OPEN_TAG_FORMAT_STRING = "<a href=\"{0}\">";
		private const string REPLACEMENT_CLOSE_TAG = "</a>";
		private static readonly Regex _OpenTagRegex = new Regex(@"\[url(?:=(?<url>(?!]).+?))?\]");
		private static readonly Regex _CloseTagRegex = new Regex(@"\[/url\]");
		private static readonly OpenTagVetoRulesSet _VetoRulesSet = new OpenTagVetoRulesSet
		{
			OtherTagVetoRules = new[] { new MustNotNestAnyTagsInMeExceptVetoRule(typeof(ImageTagDefinition)) },
			SelfVetoRules = new IVetoRule[] { },
		};


		/// <summary>
		/// Constructor, creates a <see cref="UrlTagDefinition"/>.
		/// </summary>
		public UrlTagDefinition()
			: base(_OpenTagRegex, _CloseTagRegex, CloseTag, _VetoRulesSet, Enumerable.Empty<IVetoRule>(), true)
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

			Group group = match.Groups["url"];
			if (group.Success)
				yield return new KeyValuePair<string, object>("URL", group.Value);
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

				string text = context.GetRenderString(openTag, closeTag);
				string href = openTag.Attributes.ContainsKey("URL") ? (string)openTag.Attributes["URL"] : text;

				//If the href isn't a real URL, don't create a tag and skip it instead
				Match urlMatch = RegularExpressions.Url.Match(href);
				if (urlMatch.Success == false || urlMatch.Index != 0 || urlMatch.Length != href.Length)
					continue;

				string tag = String.Format(REPLACEMENT_OPEN_TAG_FORMAT_STRING, WebUtility.HtmlEncode(href));
				context.RenderTag(openTag, tag, true);

				context.RenderTag(closeTag, REPLACEMENT_CLOSE_TAG, true);
			}

			context.RegisterRenderCacheability(true);
		}
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DigitallyCreated.Utilities.Bcl;


namespace DigitallyCreated.Utilities.BbCode.Tags
{
	/// <summary>
	/// The <see cref="ITagDefinition"/> for BBCode size tags ([size][/size]).
	/// </summary>
	/// <seealso cref="ITagDefinition"/>
	public class SizeTagDefinition : SimpleTagDefinition
	{
		/// <summary>
		/// The BBCode bold open tag
		/// </summary>
		public const string OpenTag = "[size]";

		/// <summary>
		/// The BBCode bold close tag
		/// </summary>
		public new const string CloseTag = "[/size]";


		private const string REPLACEMENT_OPEN_TAG_FORMAT_STRING = "<span style=\"font-size:{0}%\">";
		private const int MIN_SIZE = 1;
		private const int MAX_SIZE = 200;
		private static readonly Regex _OpenTagRegEx = new Regex(@"\[size=(?<size>\d+)\]");
		private static readonly Regex _CloseTagRegEx = new Regex(@"\[/size\]");
		private static readonly OpenTagVetoRulesSet _VetoRulesSet = new OpenTagVetoRulesSet
		{
			OtherTagVetoRules = new[] { new MustNotSelfNestVetoRule<SizeTagDefinition>() },
			SelfVetoRules = new[] { new MustHaveValidSizeVetoRule() },
		};
		private readonly int _MinSize;
		private readonly int _MaxSize;


		/// <summary>
		/// Constructor, creates a <see cref="SizeTagDefinition"/> that only allows sizes between 1% and 200%.
		/// </summary>
		public SizeTagDefinition()
			: this(MIN_SIZE, MAX_SIZE)
		{
		}


		/// <summary>
		/// Constructor, creates a <see cref="SizeTagDefinition"/>.
		/// </summary>
		/// <param name="minSize">
		/// The minimum percentage size the user is allowed to input (must not be less than 1)
		/// </param>
		/// <param name="maxSize">
		/// The maximum percentage size the user is allowed to input
		/// </param>
		public SizeTagDefinition(int minSize, int maxSize)
			: base(_OpenTagRegEx, ReplacementOpenTagFactoryFunction, _CloseTagRegEx, m => "</span>", CloseTag, _VetoRulesSet, Enumerable.Empty<IVetoRule>(), true, true)
		{
			if (minSize < MIN_SIZE)
				throw new ArgumentException("minSize must not be less than " + MIN_SIZE, "minSize");
			if (maxSize < minSize)
				throw new ArgumentException("maxSize must not be less than minSize", "maxSize");

			_MinSize = minSize;
			_MaxSize = maxSize;
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

			yield return new KeyValuePair<string, object>("Size", Int32.Parse(match.Groups["size"].Value));
		}


		/// <summary>
		/// Replacement function that writes the rendered open tag for size BBcode open tags
		/// </summary>
		/// <param name="openTagInstance">The <see cref="IOpenTagInstance"/></param>
		/// <returns>The rendered XHTML tag</returns>
		private static string ReplacementOpenTagFactoryFunction(IOpenTagInstance openTagInstance)
		{
			int size = (int)openTagInstance.Attributes["Size"];
			return String.Format(REPLACEMENT_OPEN_TAG_FORMAT_STRING, size);
		}


		/// <summary>
		/// Veto rule which ensures that the user's inputted size is within the valid ranges
		/// </summary>
		private class MustHaveValidSizeVetoRule : IVetoRule
		{
			/// <summary>
			/// Makes an assessment as to whether <paramref name="tagInstance"/> is a valid tag
			/// </summary>
			/// <param name="tagInstance">The <see cref="ITagInstance"/></param>
			/// <param name="context">The <see cref="ValidationContext"/></param>
			/// <returns>True if the tag is not valid should be vetoed, false otherwise</returns>
			public bool CheckForVeto(ITagInstance tagInstance, ValidationContext context)
			{
				int size = (int)tagInstance.Attributes["Size"];
				SizeTagDefinition sizeTagDef = (SizeTagDefinition)tagInstance.ParentDefinition;

				return size > sizeTagDef._MaxSize || size < sizeTagDef._MinSize;
			}
		}
	}
}
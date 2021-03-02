using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DigitallyCreated.Utilities.Bcl;


namespace DigitallyCreated.Utilities.BbCode.Tags
{
	/// <summary>
	/// The <see cref="ITagDefinition"/> for BBCode colour tags ([colour][/colour] or [color][/color]).
	/// </summary>
	/// <seealso cref="ITagDefinition"/>
	public class ColourTagDefinition : SimpleTagDefinition
	{
		#region Valid Colour Names

		private static readonly ISet<string> _ColourNames = new HashSet<string>
		{
			"aliceblue",
			"antiquewhite",
			"aqua",
			"aquamarine",
			"azure",
			"beige",
			"bisque",
			"black",
			"blanchedalmond",
			"blue",
			"blueviolet",
			"brown",
			"burlywood",
			"cadetblue",
			"chartreuse",
			"chocolate",
			"coral",
			"cornflowerblue",
			"cornsilk",
			"crimson",
			"cyan",
			"darkblue",
			"darkcyan",
			"darkgoldenrod",
			"darkgray",
			"darkgreen",
			"darkkhaki",
			"darkmagenta",
			"darkolivegreen",
			"darkorange",
			"darkorchid",
			"darkred",
			"darksalmon",
			"darkseagreen",
			"darkslateblue",
			"darkslategray",
			"darkturquoise",
			"darkviolet",
			"deeppink",
			"deepskyblue",
			"dimgray",
			"dodgerblue",
			"firebrick",
			"floralwhite",
			"forestgreen",
			"fuchsia",
			"gainsboro",
			"ghostwhite",
			"gold",
			"goldenrod",
			"gray",
			"green",
			"greenyellow",
			"honeydew",
			"hotpink",
			"indianred ",
			"indigo ",
			"ivory",
			"khaki",
			"lavender",
			"lavenderblush",
			"lawngreen",
			"lemonchiffon",
			"lightblue",
			"lightcoral",
			"lightcyan",
			"lightgoldenrodyellow",
			"lightgrey",
			"lightgreen",
			"lightpink",
			"lightsalmon",
			"lightseagreen",
			"lightskyblue",
			"lightslategray",
			"lightsteelblue",
			"lightyellow",
			"lime",
			"limegreen",
			"linen",
			"magenta",
			"maroon",
			"mediumaquamarine",
			"mediumblue",
			"mediumorchid",
			"mediumpurple",
			"mediumseagreen",
			"mediumslateblue",
			"mediumspringgreen",
			"mediumturquoise",
			"mediumvioletred",
			"midnightblue",
			"mintcream",
			"mistyrose",
			"moccasin",
			"navajowhite",
			"navy",
			"oldlace",
			"olive",
			"olivedrab",
			"orange",
			"orangered",
			"orchid",
			"palegoldenrod",
			"palegreen",
			"paleturquoise",
			"palevioletred",
			"papayawhip",
			"peachpuff",
			"peru",
			"pink",
			"plum",
			"powderblue",
			"purple",
			"red",
			"rosybrown",
			"royalblue",
			"saddlebrown",
			"salmon",
			"sandybrown",
			"seagreen",
			"seashell",
			"sienna",
			"silver",
			"skyblue",
			"slateblue",
			"slategray",
			"snow",
			"springgreen",
			"steelblue",
			"tan",
			"teal",
			"thistle",
			"tomato",
			"turquoise",
			"violet",
			"wheat",
			"white",
			"whitesmoke",
			"yellow",
			"yellowgreen",
		};

		#endregion


		/// <summary>
		/// The BBCode colour close tag
		/// </summary>
		public new const string CloseTag = "[/colour]";


		private const string REPLACEMENT_OPEN_TAG_FORMAT_STRING = "<span style=\"color:{0}\">";
		private static readonly Regex _OpenTagRegex = new Regex(@"\[colou?r=(?:(?<hex>\#[A-Fa-f0-9]{6})|(?<name>[A-Za-z]+))\]");
		private static readonly Regex _CloseTagRegex = new Regex(@"\[/colou?r\]");
		private static readonly OpenTagVetoRulesSet _VetoRulesSet = new OpenTagVetoRulesSet
		{
			OtherTagVetoRules = new IVetoRule[] { new MustNotSelfNestVetoRule<ColourTagDefinition>() },
			SelfVetoRules = new IVetoRule[] { new ValidColourNameVetoRule() },
		};



		/// <summary>
		/// Constructor, creates a <see cref="ColourTagDefinition"/>.
		/// </summary>
		public ColourTagDefinition()
			: base(_OpenTagRegex, ReplacementOpenTagFactoryFunction, _CloseTagRegex, m => "</span>", CloseTag, _VetoRulesSet, Enumerable.Empty<IVetoRule>(), true, true)
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

			Group group = match.Groups["hex"];
			if (group.Success)
				yield return new KeyValuePair<string, object>("Hex", group.Value);
			else
				yield return new KeyValuePair<string, object>("Name", match.Groups["name"].Value);
		}


		/// <summary>
		/// Replacement function that writes the rendered open tag for colour BBcode open tags
		/// </summary>
		/// <param name="openTagInstance">The <see cref="ITagInstance"/></param>
		/// <returns>The rendered XHTML tag</returns>
		private static string ReplacementOpenTagFactoryFunction(IOpenTagInstance openTagInstance)
		{
			string colour;
			if (openTagInstance.Attributes.ContainsKey("Hex"))
				colour = (string)openTagInstance.Attributes["Hex"];
			else
				colour = (string)openTagInstance.Attributes["Name"];

			return String.Format(REPLACEMENT_OPEN_TAG_FORMAT_STRING, colour);
		}


		/// <summary>
		/// Ensures the colour <see cref="IOpenTagInstance"/> contain a valid colour name if they contain a colour
		/// name and not a RGB hex.
		/// </summary>
		private class ValidColourNameVetoRule : IVetoRule
		{
			/// <summary>
			/// Makes an assessment as to whether <paramref name="tagInstance"/> is a valid tag
			/// </summary>
			/// <param name="tagInstance">The <see cref="ITagInstance"/></param>
			/// <param name="context">The <see cref="ValidationContext"/></param>
			/// <returns>True if the tag is not valid should be vetoed, false otherwise</returns>
			public bool CheckForVeto(ITagInstance tagInstance, ValidationContext context)
			{
				if (tagInstance.Attributes.ContainsKey("Hex"))
					return false;

				return _ColourNames.Contains(((string)tagInstance.Attributes["Name"]).ToLower()) == false;
			}
		}
	}
}
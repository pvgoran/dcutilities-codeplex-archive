using System;
using System.Linq;
using System.Text.RegularExpressions;


namespace DigitallyCreated.Utilities.BbCode.Tags
{
	/// <summary>
	/// The <see cref="ITagDefinition"/> for BBCode escape tags ([esc][/esc]).
	/// </summary>
	/// <seealso cref="ITagDefinition"/>
	public class EscapeTagDefinition : SimpleTagDefinition
	{
		/// <summary>
		/// The BBCode escape open tag
		/// </summary>
		public const string OpenTag = "[esc]";

		/// <summary>
		/// The BBCode escape close tag
		/// </summary>
		public new const string CloseTag = "[/esc]";


		private static readonly Regex _OpenTagRegex = new Regex(@"\[esc\]");
		private static readonly Regex _CloseTagRegex = new Regex(@"\[/esc\]");
		private static readonly OpenTagVetoRulesSet _OpenTagVetoRulesSet = new OpenTagVetoRulesSet
		{
			OtherTagVetoRules = new IVetoRule[] { new MustNotNestAnyTagsInMeExceptVetoRule() },
			SelfVetoRules = Enumerable.Empty<IVetoRule>(),
		};


		/// <summary>
		/// Constructor, creates a <see cref="EscapeTagDefinition"/>.
		/// </summary>
		public EscapeTagDefinition()
			: base(_OpenTagRegex, t => String.Empty, _CloseTagRegex, t => String.Empty, CloseTag, _OpenTagVetoRulesSet, Enumerable.Empty<IVetoRule>(), true, true)
		{
		}
	}
}
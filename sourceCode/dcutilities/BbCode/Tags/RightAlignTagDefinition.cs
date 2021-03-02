using System.Linq;
using System.Text.RegularExpressions;


namespace DigitallyCreated.Utilities.BbCode.Tags
{
	/// <summary>
	/// The <see cref="ITagDefinition"/> for BBCode right align tags ([right][/right]).
	/// </summary>
	/// <seealso cref="ITagDefinition"/>
	public class RightAlignTagDefinition : SimpleTagDefinition
	{
		/// <summary>
		/// The BBCode right align open tag
		/// </summary>
		public const string OpenTag = "[right]";

		/// <summary>
		/// The BBCode right align close tag
		/// </summary>
		public new const string CloseTag = "[/right]";


		private static readonly Regex _OpenTagRegEx = new Regex(@"\[right\]");
		private static readonly Regex _CloseTagRegEx = new Regex(@"\[/right\]");
		private static readonly OpenTagVetoRulesSet _OpenTagVetoRulesSet = new OpenTagVetoRulesSet
		{
		    OtherTagVetoRules = new IVetoRule[] { new MustNotSelfNestVetoRule<LeftAlignTagDefinition>(), new MustNotSelfNestVetoRule<RightAlignTagDefinition>(), new MustNotSelfNestVetoRule<CentreAlignTagDefinition>() },
		    SelfVetoRules = new IVetoRule[] { new MustNotNestInInlineElement() },
		};


		/// <summary>
		/// Constructor, creates a <see cref="RightAlignTagDefinition"/>.
		/// </summary>
		public RightAlignTagDefinition()
			: base(_OpenTagRegEx, t => "<div style=\"text-align:right\">", _CloseTagRegEx, t => "</div>", CloseTag, _OpenTagVetoRulesSet, Enumerable.Empty<IVetoRule>(), false, true)
		{
		}
	}
}
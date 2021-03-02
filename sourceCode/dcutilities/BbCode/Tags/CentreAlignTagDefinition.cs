using System.Linq;
using System.Text.RegularExpressions;


namespace DigitallyCreated.Utilities.BbCode.Tags
{
	/// <summary>
	/// The <see cref="ITagDefinition"/> for BBCode centre align tags ([centre][/centre] or [center][/center]).
	/// </summary>
	/// <seealso cref="ITagDefinition"/>
	public class CentreAlignTagDefinition : SimpleTagDefinition
	{
		/// <summary>
		/// The BBCode centre align open tag
		/// </summary>
		public const string OpenTag = "[centre]";

		/// <summary>
		/// The BBCode centre align close tag
		/// </summary>
		public new const string CloseTag = "[/centre]";


		private static readonly Regex _OpenTagRegEx = new Regex(@"\[(?:(?:centre)|(?:center))\]");
		private static readonly Regex _CloseTagRegEx = new Regex(@"\[/(?:(?:centre)|(?:center))\]");
		private static readonly OpenTagVetoRulesSet _OpenTagVetoRulesSet = new OpenTagVetoRulesSet
		{
		    OtherTagVetoRules = new IVetoRule[] { new MustNotSelfNestVetoRule<LeftAlignTagDefinition>(), new MustNotSelfNestVetoRule<RightAlignTagDefinition>(), new MustNotSelfNestVetoRule<CentreAlignTagDefinition>() },
		    SelfVetoRules = new IVetoRule[] { new MustNotNestInInlineElement() },
		};


		/// <summary>
		/// Constructor, creates a <see cref="CentreAlignTagDefinition"/>.
		/// </summary>
		public CentreAlignTagDefinition()
			: base(_OpenTagRegEx, t => "<div style=\"text-align:center\">", _CloseTagRegEx, t => "</div>", CloseTag, _OpenTagVetoRulesSet, Enumerable.Empty<IVetoRule>(), false, true)
		{
		}
	}
}
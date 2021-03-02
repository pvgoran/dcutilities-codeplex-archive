using System.Linq;
using System.Text.RegularExpressions;


namespace DigitallyCreated.Utilities.BbCode.Tags
{
	/// <summary>
	/// The <see cref="ITagDefinition"/> for BBCode left align tags ([left][/left]).
	/// </summary>
	/// <seealso cref="ITagDefinition"/>
	public class LeftAlignTagDefinition : SimpleTagDefinition
	{
		/// <summary>
		/// The BBCode left align open tag
		/// </summary>
		public const string OpenTag = "[left]";

		/// <summary>
		/// The BBCode left align close tag
		/// </summary>
		public new const string CloseTag = "[/left]";


		private static readonly Regex _OpenTagRegEx = new Regex(@"\[left\]");
		private static readonly Regex _CloseTagRegEx = new Regex(@"\[/left\]");
		private static readonly OpenTagVetoRulesSet _OpenTagVetoRulesSet = new OpenTagVetoRulesSet
		{
			OtherTagVetoRules = new IVetoRule[] { new MustNotSelfNestVetoRule<LeftAlignTagDefinition>(), new MustNotSelfNestVetoRule<RightAlignTagDefinition>(), new MustNotSelfNestVetoRule<CentreAlignTagDefinition>() },
			SelfVetoRules = new IVetoRule[] { new MustNotNestInInlineElement() },
		};


		/// <summary>
		/// Constructor, creates a <see cref="LeftAlignTagDefinition"/>.
		/// </summary>
		public LeftAlignTagDefinition()
			: base(_OpenTagRegEx, t => "<div style=\"text-align:left\">", _CloseTagRegEx, t => "</div>", CloseTag, _OpenTagVetoRulesSet, Enumerable.Empty<IVetoRule>(), false, true)
		{
		}
	}
}
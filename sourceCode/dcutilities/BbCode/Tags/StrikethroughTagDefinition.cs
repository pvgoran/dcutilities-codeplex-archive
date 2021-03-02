using System.Linq;
using System.Text.RegularExpressions;


namespace DigitallyCreated.Utilities.BbCode.Tags
{
	/// <summary>
	/// The <see cref="ITagDefinition"/> for BBCode strikethrough tags ([s][/s]).
	/// </summary>
	/// <seealso cref="ITagDefinition"/>
	public class StrikethroughTagDefinition : SimpleTagDefinition
	{
		/// <summary>
		/// The BBCode strikethrough open tag
		/// </summary>
		public const string OpenTag = "[s]";

		/// <summary>
		/// The BBCode strikethrough close tag
		/// </summary>
		public new const string CloseTag = "[/s]";


		private static readonly Regex _OpenTagRegEx = new Regex(@"\[s\]");
		private static readonly Regex _CloseTagRegEx = new Regex(@"\[/s\]");
		private static readonly OpenTagVetoRulesSet _VetoRulesSet = new OpenTagVetoRulesSet
		{
			OtherTagVetoRules = new[] { new MustNotSelfNestVetoRule<StrikethroughTagDefinition>() },
			SelfVetoRules = new IVetoRule[] { },
		};


		/// <summary>
		/// Constructor, creates a <see cref="StrikethroughTagDefinition"/>.
		/// </summary>
		public StrikethroughTagDefinition()
			: base(_OpenTagRegEx, m => "<span style=\"text-decoration:line-through\">", _CloseTagRegEx, m => "</span>", CloseTag, _VetoRulesSet, Enumerable.Empty<IVetoRule>(), true, true)
		{
		}
	}
}
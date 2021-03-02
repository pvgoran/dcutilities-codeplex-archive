using System.Linq;
using System.Text.RegularExpressions;


namespace DigitallyCreated.Utilities.BbCode.Tags
{
	/// <summary>
	/// The <see cref="ITagDefinition"/> for BBCode underline tags ([u][/u]).
	/// </summary>
	/// <seealso cref="ITagDefinition"/>
	public class UnderlineTagDefinition : SimpleTagDefinition
	{
		/// <summary>
		/// The BBCode underline open tag
		/// </summary>
		public const string OpenTag = "[u]";

		/// <summary>
		/// The BBCode underline close tag
		/// </summary>
		public new const string CloseTag = "[/u]";
		private static readonly Regex _OpenTagRegEx = new Regex(@"\[u\]");
		private static readonly Regex _CloseTagRegEx = new Regex(@"\[/u\]");
		private static readonly OpenTagVetoRulesSet _VetoRulesSet = new OpenTagVetoRulesSet
		{
			OtherTagVetoRules = new[] { new MustNotSelfNestVetoRule<UnderlineTagDefinition>() },
			SelfVetoRules = new IVetoRule[] { },
		};


		/// <summary>
		/// Constructor, creates a <see cref="UnderlineTagDefinition"/>.
		/// </summary>
		public UnderlineTagDefinition()
			: base(_OpenTagRegEx, m => "<span style=\"text-decoration:underline\">", _CloseTagRegEx, m => "</span>", CloseTag, _VetoRulesSet, Enumerable.Empty<IVetoRule>(), true, true)
		{
		}
	}
}
using System.Linq;
using System.Text.RegularExpressions;


namespace DigitallyCreated.Utilities.BbCode.Tags
{
	/// <summary>
	/// The <see cref="ITagDefinition"/> for BBCode bold tags ([b][/b]).
	/// </summary>
	/// <seealso cref="ITagDefinition"/>
	public class BoldTagDefinition : SimpleTagDefinition
	{
		/// <summary>
		/// The BBCode bold open tag
		/// </summary>
		public const string OpenTag = "[b]";

		/// <summary>
		/// The BBCode bold close tag
		/// </summary>
		public new const string CloseTag = "[/b]";

		private static readonly Regex _OpenTagRegEx = new Regex(@"\[b\]");
		private static readonly Regex _CloseTagRegEx = new Regex(@"\[/b\]");
		private static readonly OpenTagVetoRulesSet _OpenTagVetoRulesSet = new OpenTagVetoRulesSet
		{
		    OtherTagVetoRules = new[] { new MustNotSelfNestVetoRule<BoldTagDefinition>() },
		    SelfVetoRules = new IVetoRule[] { },
		};


		/// <summary>
		/// Constructor, creates a <see cref="BoldTagDefinition"/>.
		/// </summary>
		public BoldTagDefinition()
			: base(_OpenTagRegEx, m => "<strong>", _CloseTagRegEx, m => "</strong>", CloseTag, _OpenTagVetoRulesSet, Enumerable.Empty<IVetoRule>(), true, true)
		{
		}
	}
}
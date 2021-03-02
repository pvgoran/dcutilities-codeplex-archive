using System.Linq;
using System.Text.RegularExpressions;


namespace DigitallyCreated.Utilities.BbCode.Tags
{
	/// <summary>
	/// The <see cref="ITagDefinition"/> for BBCode italics tags ([i][/i]).
	/// </summary>
	/// <seealso cref="ITagDefinition"/>
	public class ItalicsTagDefinition : SimpleTagDefinition
	{
		/// <summary>
		/// The BBCode italics open tag
		/// </summary>
		public const string OpenTag = "[i]";

		/// <summary>
		/// The BBCode italics open tag
		/// </summary>
		public new const string CloseTag = "[/i]";


		private static readonly Regex _OpenTagRegEx = new Regex(@"\[i\]");
		private static readonly Regex _CloseTagRegEx = new Regex(@"\[/i\]");
		private static readonly OpenTagVetoRulesSet _OpenTagVetoRulesSet = new OpenTagVetoRulesSet
		{
			OtherTagVetoRules = new[] { new MustNotSelfNestVetoRule<ItalicsTagDefinition>() },
			SelfVetoRules = new IVetoRule[] {},
		};


		/// <summary>
		/// Constructor, creates a <see cref="ItalicsTagDefinition"/>.
		/// </summary>
		public ItalicsTagDefinition()
			: base(_OpenTagRegEx, m => "<em>", _CloseTagRegEx, m => "</em>", CloseTag, _OpenTagVetoRulesSet, Enumerable.Empty<IVetoRule>(), true, true)
		{
		}
	}
}
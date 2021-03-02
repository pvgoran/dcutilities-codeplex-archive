using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;


namespace DigitallyCreated.Utilities.BbCode.Tags
{
	/// <summary>
	/// The <see cref="ITagDefinition"/> for BBCode code tags ([code][/code]).
	/// </summary>
	/// <seealso cref="ITagDefinition"/>
	public class CodeTagDefinition : SimpleTagDefinition
	{
		/// <summary>
		/// The BBCode code open tag
		/// </summary>
		public const string OpenTag = "[code]";

		/// <summary>
		/// The BBCode code close tag
		/// </summary>
		public new const string CloseTag = "[/code]";
		private static readonly Regex _OpenTagRegEx = new Regex(@"\[code\]");
		private static readonly Regex _CloseTagRegEx = new Regex(@"\[/code\]");
		private static readonly OpenTagVetoRulesSet _VetoRulesSet = new OpenTagVetoRulesSet
		{
			OtherTagVetoRules = new[] { new MustNotSelfNestVetoRule<CodeTagDefinition>() },
			SelfVetoRules = new IVetoRule[] { },
		};


		/// <summary>
		/// Constructor, creates a <see cref="CodeTagDefinition"/>.
		/// </summary>
		public CodeTagDefinition()
			: base(_OpenTagRegEx, m => "<pre>", _CloseTagRegEx, m => "</pre>", CloseTag, _VetoRulesSet, Enumerable.Empty<IVetoRule>(), false, true)
		{
		}


		/// <summary>
		/// Causes the <see cref="ITagDefinition"/> to remove any whitespace it sees fit that is around its
		/// identified <see cref="ITagInstance"/>s. An <see cref="ITagDefinition"/> does not need to remove any
		/// whitespace, if it doesn't want to.
		/// </summary>
		/// <remarks>
		/// This method is always called before <see cref="ITagDefinition.Render"/> is called on any 
		/// <see cref="ITagDefinition"/>. The order in which this method is called across the different 
		/// <see cref="ITagDefinition"/>s is undefined.
		/// </remarks>
		/// <param name="context">
		/// The <see cref="IWhitespaceRemovalContext"/> contains all <see cref="ITagInstance"/>s, the input text,
		/// and methods that allow for the removal of whitespace.
		/// </param>
		public override void RemoveWhitespace(IWhitespaceRemovalContext context)
		{
			IEnumerable<IOpenTagInstance> tagInstances = context.Tags.Where(t => t.ParentDefinition == this)
																	 .OfType<IOpenTagInstance>();

			foreach (IOpenTagInstance openTag in tagInstances)
			{
				context.RemoveWhitespaceBeforeTag(openTag, 1);
				context.RemoveWhitespaceAfterTag(openTag, 1);
				context.RemoveWhitespaceBeforeTag(openTag.CloseTag, 1);
				context.RemoveWhitespaceAfterTag(openTag.CloseTag, 2);
			}
		}
	}
}
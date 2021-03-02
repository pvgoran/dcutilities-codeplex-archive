using System.Linq;


namespace DigitallyCreated.Utilities.BbCode
{
	/// <summary>
	/// This <see cref="IVetoRule"/> enforces that the tag should not be nested inside any tag pair that
	/// will render to an inline XHTML element. This enforces XHTML standards.
	/// </summary>
	public class MustNotNestInInlineElement : IVetoRule
	{
		/// <summary>
		/// Makes an assessment as to whether <paramref name="tagInstance"/> is a valid tag
		/// </summary>
		/// <param name="tagInstance">The <see cref="ITagInstance"/></param>
		/// <param name="context">The <see cref="ValidationContext"/></param>
		/// <returns>True if the tag is not valid should be vetoed, false otherwise</returns>
		public bool CheckForVeto(ITagInstance tagInstance, ValidationContext context)
		{
			return context.OpenTagStack.Any() && context.OpenTagStack.Peek().RendersToInlineElement;
		}
	}
}
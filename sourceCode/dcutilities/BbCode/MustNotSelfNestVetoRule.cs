namespace DigitallyCreated.Utilities.BbCode
{
	/// <summary>
	/// This <see cref="IVetoRule"/> enforces that a tag cannot nest inside itself
	/// </summary>
	/// <typeparam name="T">The <see cref="ITagDefinition"/> that creates the tag instances</typeparam>
	public class MustNotSelfNestVetoRule<T> : IVetoRule
		where T : ITagDefinition
	{
		/// <summary>
		/// Makes an assessment as to whether <paramref name="tagInstance"/> is a valid tag
		/// </summary>
		/// <param name="tagInstance">The <see cref="ITagInstance"/></param>
		/// <param name="context">The <see cref="ValidationContext"/></param>
		/// <returns>True if the tag is not valid should be vetoed, false otherwise</returns>
		public bool CheckForVeto(ITagInstance tagInstance, ValidationContext context)
		{
			return tagInstance.ParentDefinition is T;
		}
	}
}
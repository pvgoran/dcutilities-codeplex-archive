namespace DigitallyCreated.Utilities.BbCode
{
	/// <summary>
	/// Represents a rule that inspects a <see cref="ITagInstance"/> and a <see cref="ValidationContext"/>
	/// and makes an assessment as to whether that tag is valid
	/// </summary>
	public interface IVetoRule
	{
		/// <summary>
		/// Makes an assessment as to whether <paramref name="tagInstance"/> is a valid tag
		/// </summary>
		/// <param name="tagInstance">The <see cref="ITagInstance"/></param>
		/// <param name="context">The <see cref="ValidationContext"/></param>
		/// <returns>True if the tag is not valid should be vetoed, false otherwise</returns>
		bool CheckForVeto(ITagInstance tagInstance, ValidationContext context);
	}
}

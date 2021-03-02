namespace DigitallyCreated.Utilities.BbCode
{
	/// <summary>
	/// This type of <see cref="ITagInstance"/> represents a closing BBCode tag.
	/// </summary>
	public interface ICloseTagInstance : ITagInstance
	{
		/// <summary>
		/// Checks if the current tag is valid in its current location. For example, the 
		/// <see cref="ICloseTagInstance"/> should check the <see cref="ValidationContext.OpenTagStack"/> and ensure
		/// that the topmost open tag is the matching open tag for the close tag.
		/// </summary>
		/// <param name="context">The <see cref="ValidationContext"/></param>
		/// <returns>
		/// True if the tag is valid, false if it is not (which causes it and its associated 
		/// <see cref="IOpenTagInstance"/> to be removed and therefore ignored during rendering)
		/// </returns>
		bool CheckIfValidClose(ValidationContext context);
	}
}
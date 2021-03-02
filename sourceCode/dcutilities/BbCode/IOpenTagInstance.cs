namespace DigitallyCreated.Utilities.BbCode
{
	/// <summary>
	/// This type of <see cref="ITagInstance"/> represents an opening BBCode tag.
	/// </summary>
	public interface IOpenTagInstance : ITagInstance
	{
		/// <summary>
		/// The closing tag instance for this opening tag
		/// </summary>
		ICloseTagInstance CloseTag { get; set; }

		/// <summary>
		/// Asks this <see cref="IOpenTagInstance"/> whether the specified <paramref name="tagInstance"/>is allowed
		/// to be inside it (ie before this open tag's close tag occurs).
		/// </summary>
		/// <param name="tagInstance">
		/// The <see cref="IOpenTagInstance"/> which is trying to reside before the this tag's
		/// <see cref="ICloseTagInstance"/>.
		/// </param>
		/// <param name="context">The <see cref="ValidationContext"/></param>
		/// <returns>
		/// True if this tag objects to the specified <paramref name="tagInstance"/> (which causes the
		/// <paramref name="tagInstance"/> to be removed and therefore ignored during rendering), 
		/// false if it is okay for the <paramref name="tagInstance"/> to be there.
		/// </returns>
		bool CheckForVetoAgainstAnotherTag(IOpenTagInstance tagInstance, ValidationContext context);

		/// <summary>
		/// Looks at the current <see cref="ValidationContext"/> and sees whether it is okay if the
		/// current tag instance resides where it does. For example, the <see cref="IOpenTagInstance"/> may 
		/// check the  <see cref="ValidationContext.OpenTagStack"/> and see whether it is allowed to be inside
		/// any of the tags that are currently on the stack.
		/// </summary>
		/// <param name="context">The <see cref="ValidationContext"/></param>
		/// <returns>
		/// True if this tag objects to its own location (which causes it to be removed and therefore ignored
		/// during rendering), false if it is okay for it to be where it is.
		/// </returns>
		bool CheckForSelfVeto(ValidationContext context);
	}
}
namespace DigitallyCreated.Utilities.Mvc
{
	/// <summary>
	/// The types of temporary information box that can be displayed. This is used in conjunction with the 
	/// <see cref="EyeCandyHtmlHelpers.TempInfoBox"/> method.
	/// </summary>
	public enum TempInfoBoxType
	{
		/// <summary>
		/// An error box
		/// </summary>
		Error,
		
		/// <summary>
		/// A warning box
		/// </summary>
		Warning,

		/// <summary>
		/// An information box
		/// </summary>
		Info,

		/// <summary>
		/// A success box
		/// </summary>
		Success
	}
}
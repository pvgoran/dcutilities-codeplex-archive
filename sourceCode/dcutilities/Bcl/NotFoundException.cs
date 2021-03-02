using System;


namespace DigitallyCreated.Utilities.Bcl
{
	/// <summary>
	/// If something was not found, this exception will be thrown.
	/// </summary>
	public class NotFoundException : Exception
	{
		/// <summary>
		/// The name of the entity that was not found in a form that is suitable for
		/// display to the user (ie OrderEntity -> Order)
		/// </summary>
		public string FriendlyEntityName { get; private set; }

		/// <summary>
		/// The unique ID that describes the entity that was not found
		/// </summary>
		public string EntityID { get; private set; }


		/// <summary>
		/// Creates a new <see cref="NotFoundException"/> and auto-creates the <see cref="Exception.Message"/>
		/// </summary>
		/// <param name="friendlyEntityName">
		/// The name of the entity that was not found in a form that is suitable for
		/// display to the user (ie OrderEntity -> Order)
		/// </param>
		/// <param name="entityID">The unique ID that describes the entity that was not found</param>
		public NotFoundException(string friendlyEntityName, string entityID)
			: base(String.Format("The {0} (ID: {1}) was not found.", friendlyEntityName, entityID))
		{
			FriendlyEntityName = friendlyEntityName;
			EntityID = entityID;
		}


		/// <summary>
		/// Creates a new <see cref="NotFoundException"/> with both
		/// a manually created message and an inner exception
		/// </summary>
		/// <param name="message">A message detailing the error that occurred</param>
		/// <param name="friendlyEntityName">The name
		/// The name of the entity that was not found in a form that is suitable for
		/// display to the user (ie OrderEntity -> Order)
		/// </param>
		/// <param name="entityID">The unique ID that describes the entity that was not found</param>
		/// <param name="innerException">The exception that was the cause of this exception</param>
		public NotFoundException(string friendlyEntityName, string entityID, string message, Exception innerException)
			: base(message, innerException)
		{
			FriendlyEntityName = friendlyEntityName;
			EntityID = entityID;
		}
	}
}
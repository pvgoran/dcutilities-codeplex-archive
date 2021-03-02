using System;
using System.Diagnostics;


namespace DigitallyCreated.Utilities.BbCode
{
	/// <summary>
	/// Defines a mutable character range
	/// </summary>
	[DebuggerDisplay("StartAt = {StartAt}, Length = {Length}")]
	public class CharRange : IComparable<CharRange>
	{
		/// <summary>
		/// The character index to start the range at
		/// </summary>
		public int StartAt { get; set; }

		/// <summary>
		/// The length of the range
		/// </summary>
		public int Length { get; set; }


		/// <summary>
		/// Constructor, creates a <see cref="CharRange"/>
		/// </summary>
		/// <param name="startAt">The character index to start the range at</param>
		/// <param name="length">The length of the range</param>
		public CharRange(int startAt, int length)
		{
			StartAt = startAt;
			Length = length;
		}


		/// <summary>
		/// Compares the current object with another object of the same type.
		/// </summary>
		/// <remarks>
		/// Compares only by <see cref="StartAt"/> and ignores <see cref="Length"/>
		/// </remarks>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>
		/// A value that indicates the relative order of the objects being compared. 
		/// </returns>
		public int CompareTo(CharRange other)
		{
			return StartAt.CompareTo(other.StartAt);
		}
	}
}
namespace DigitallyCreated.Utilities.Bcl.Linq
{

	/// <summary>
	/// A match up pair class used by the <see cref="LinqExtensions.MatchUp{TFirst,TSecond}"/> method
	/// </summary>
	/// <typeparam name="TFirst">The type of the first data member</typeparam>
	/// <typeparam name="TSecond">The type of the second data member</typeparam>
	public class MatchPair<TFirst, TSecond>
	{
		/// <summary>
		/// A value from the first enumeration
		/// </summary>
		public TFirst First { get; private set; }

		/// <summary>
		/// A value from the second enumeration
		/// </summary>
		public TSecond Second { get; private set; }

		/// <summary>
		/// Whether or not <see cref="First"/> is set (has a value in it). This is useful
		/// when matching value types (where nulls don't exist).
		/// </summary>
		public bool IsFirstSet { get; private set; }

		/// <summary>
		/// Whether or not <see cref="Second"/> is set (has a value in it). This is useful
		/// when matching value types (where nulls don't exist).
		/// </summary>
		public bool IsSecondSet { get; private set; }


		/// <summary>
		/// Constructor, creates a MatchPair
		/// </summary>
		/// <param name="first">The value of the <see cref="First"/> data member</param>
		/// <param name="isFirstSet">The value of the <see cref="IsFirstSet"/> data member</param>
		/// <param name="second">The value of the <see cref="Second"/> data member</param>
		/// <param name="isSecondSet">The value of the <see cref="IsSecondSet"/> data member</param>
		public MatchPair(TFirst first, bool isFirstSet, TSecond second, bool isSecondSet)
		{
			First = first;
			Second = second;
			IsFirstSet = isFirstSet;
			IsSecondSet = isSecondSet;
		}
	}
}
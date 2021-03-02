using System;
using System.Collections.Generic;
using System.Linq;


namespace DigitallyCreated.Utilities.Bcl.Linq
{
	/// <summary>
	/// Defines some extensions to LINQ
	/// </summary>
	public static class LinqExtensions
	{
		/// <summary>
		/// Takes a first sequence of objects and a second sequence of objects and returns a sequence which
		/// contains pairs where objects from the first sequence are matched against objects from second
		/// sequence. Objects that are in one sequence by not in the other are also returned in a pair, but
		/// they are paired with an unset value (check <see cref="MatchPair{TFirst,TSecond}.IsFirstSet"/> and 
		/// <see cref="MatchPair{TFirst,TSecond}.IsSecondSet"/>.
		/// </summary>
		/// <remarks>
		/// <para>
		/// When comparing the lists {A,B,C} and {B,C,D} you will get the following match up:
		/// </para>
		/// <list type="table">
		///		<listheader>
		///			<term><see cref="MatchPair{TFirst,TSecond}.First"/></term>
		///			<description><see cref="MatchPair{TFirst,TSecond}.Second"/></description>
		///		</listheader>
		///		<item>
		///			<term>A</term>
		///			<description> </description>
		///		</item>
		///		<item>
		///			<term>B</term>
		///			<description>B</description>
		///		</item>
		///		<item>
		///			<term>C</term>
		///			<description>C</description>
		///		</item>
		///		<item>
		///			<term></term>
		///			<description>D</description>
		///		</item>
		/// </list>
		/// <para>
		/// If you require a match up between more than one sequence look at <see cref="IFederable{T}"/>
		/// and <see cref="FederatorExtensions"/>.
		/// </para>
		/// </remarks>
		/// <typeparam name="TFirst">The type contained in the first sequence</typeparam>
		/// <typeparam name="TSecond">The type contained in the second sequence</typeparam>
		/// <param name="firsts">The first sequence</param>
		/// <param name="seconds">The second sequence</param>
		/// <param name="matcher">
		/// A comparison function that can determine whether two objects should be matched
		/// </param>
		/// <returns>The matched pairs</returns>
		public static IEnumerable<MatchPair<TFirst, TSecond>> MatchUp<TFirst, TSecond>(this IEnumerable<TFirst> firsts, IEnumerable<TSecond> seconds, Func<TFirst, TSecond, bool> matcher)
		{
			IList<MatchPair<TFirst, TSecond>> matchUp = new List<MatchPair<TFirst, TSecond>>();

			foreach (TFirst first in firsts)
			{
				matchUp.Add(new MatchPair<TFirst, TSecond>(first, true, default(TSecond), false));
			}

			foreach (TSecond second in seconds)
			{
				TSecond localSecond = second; //Unnecessary in this case, but it suppresses the "access to modified closure" warning

				var indexType = matchUp.Select((m, i) => new { Match = m, Index = i })
				                       .Where(t => t.Match.IsFirstSet && t.Match.IsSecondSet == false && matcher(t.Match.First, localSecond))
				                       .FirstOrDefault();

				if (indexType == null)
					matchUp.Add(new MatchPair<TFirst, TSecond>(default(TFirst), false, localSecond, true));
				else
					matchUp[indexType.Index] = new MatchPair<TFirst, TSecond>(indexType.Match.First, true, localSecond, true);
			}

			return matchUp;
		}


		/// <summary>
		/// Takes a first sequence of objects and a second sequence of objects and returns a sequence which
		/// contains pairs where objects from the first sequence are matched against objects from second
		/// sequence. Objects that are in one sequence by not in the other are also returned in a pair, but
		/// they are paired with an unset value (check <see cref="MatchPair{TFirst,TSecond}.IsFirstSet"/> and 
		/// <see cref="MatchPair{TFirst,TSecond}.IsSecondSet"/>. The sequences will be matched by their
		/// natural equality (<see cref="object.Equals(object)"/>).
		/// </summary>
		/// <remarks>
		/// <para>
		/// When comparing the lists {A,B,C} and {B,C,D} you will get the following match up:
		/// </para>
		/// <list type="table">
		///		<listheader>
		///			<term><see cref="MatchPair{TFirst,TSecond}.First"/></term>
		///			<description><see cref="MatchPair{TFirst,TSecond}.Second"/></description>
		///		</listheader>
		///		<item>
		///			<term>A</term>
		///			<description> </description>
		///		</item>
		///		<item>
		///			<term>B</term>
		///			<description>B</description>
		///		</item>
		///		<item>
		///			<term>C</term>
		///			<description>C</description>
		///		</item>
		///		<item>
		///			<term></term>
		///			<description>D</description>
		///		</item>
		/// </list>
		/// <para>
		/// If you require a match up between more than one sequence look at <see cref="IFederable{T}"/>
		/// and <see cref="FederatorExtensions"/>.
		/// </para>
		/// </remarks>
		/// <typeparam name="T">The type contained in the both sequences</typeparam>
		/// <param name="firsts">The first sequence</param>
		/// <param name="seconds">The second sequence</param>
		/// <returns>The matched pairs</returns>
		public static IEnumerable<MatchPair<T, T>> MatchUp<T>(this IEnumerable<T> firsts, IEnumerable<T> seconds)
		{
			return firsts.MatchUp(seconds, GetNaturalEqualityComparerFunction<T>());
		}


		/// <summary>
		/// Gets a comparer function that can compare references type or value types using their
		/// <see cref="object.Equals(object)"/> method and supports nulls.
		/// </summary>
		/// <typeparam name="T">The type to compare</typeparam>
		/// <returns>The comparer function</returns>
		public static Func<T, T, bool> GetNaturalEqualityComparerFunction<T>()
		{
			Func<T, T, bool> matcher;

			if (typeof(T).IsValueType)
				matcher = (f, s) => f.Equals(s);
			else
				matcher = (f, s) =>
				{
					object of = f, os = s;
					return (of == null && os == null) || (of != null && f.Equals(s));
				};

			return matcher;
		}
	}
}
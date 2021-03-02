using System;
using System.Collections.Generic;


namespace DigitallyCreated.Utilities.Bcl
{
	/// <summary>
	/// An <see cref="IEqualityComparer{T}"/> that instead of comparing <typeparamref name="T"/>, it compares
	/// a member on <typeparamref name="T"/> instead. This is useful for comparing objects based off of 
	/// one of their properties, for example.
	/// </summary>
	/// <typeparam name="T">The type to compare the member of</typeparam>
	/// <typeparam name="TMember">The type of the member</typeparam>
	public class MemberEqualityComparer<T, TMember> : EqualityComparer<T>
	{
		private readonly Func<T, TMember> _MemberSelector;
		private readonly IEqualityComparer<TMember> _MemberEqualityComparer;


		/// <summary>
		/// Constructor, creates a <see cref="MemberEqualityComparer{T,TMember}"/>
		/// </summary>
		/// <param name="memberSelector">A function that returns the member off of the object</param>
		/// <param name="memberEqualityComparer">
		/// The <see cref="IEqualityComparer{T}"/> to compared the member with
		/// </param>
		public MemberEqualityComparer(Func<T, TMember> memberSelector, IEqualityComparer<TMember> memberEqualityComparer)
		{
			_MemberSelector = memberSelector;
			_MemberEqualityComparer = memberEqualityComparer;
		}

		/// <summary>
		/// Constructor, creates a <see cref="MemberEqualityComparer{T,TMember}"/>, where the member
		/// is compared using the default <see cref="EqualityComparer{T}"/> for that type (ie
		/// <see cref="EqualityComparer{T}.Default"/>)
		/// </summary>
		/// <param name="memberSelector">A function that returns the member off of the object</param>
		public MemberEqualityComparer(Func<T, TMember> memberSelector)
			: this(memberSelector, EqualityComparer<TMember>.Default)
		{
			
		}


		/// <summary>
		/// Determines whether two objects of type <typeparamref name="T"/> are equal.
		/// </summary>
		/// <returns>
		/// true if the specified objects are equal; otherwise, false.
		/// </returns>
		/// <param name="x">The first object to compare.</param>
		/// <param name="y">The second object to compare.</param>
		public override bool Equals(T x, T y)
		{
			return _MemberEqualityComparer.Equals(_MemberSelector(x), _MemberSelector(y));
		}


		/// <summary>
		/// Serves as a hash function for the specified object for hashing algorithms and data structures, such as a
		/// hash table.
		/// </summary>
		/// <returns>
		/// A hash code for the specified object.
		/// </returns>
		/// <param name="obj">The object for which to get a hash code.</param>
		/// <exception cref="ArgumentNullException">
		/// The type of <paramref name="obj"/> is a reference type and <paramref name="obj"/> is null.
		/// </exception>
		public override int GetHashCode(T obj)
		{
			return _MemberEqualityComparer.GetHashCode(_MemberSelector(obj));
		}
	}
}
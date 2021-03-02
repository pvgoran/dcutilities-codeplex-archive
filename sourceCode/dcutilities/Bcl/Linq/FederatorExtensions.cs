using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;


namespace DigitallyCreated.Utilities.Bcl.Linq
{
	/// <summary>
	/// Adds extension methods that join the Federator into LINQ.
	/// </summary>
	public static class FederatorExtensions
	{
		/// <summary>
		/// Starts federated iteration sequence construction. See <see cref="IFederable{T}"/> for more
		/// information about sequence federation.
		/// </summary>
		/// <typeparam name="TSource">The type in the sequences being federated</typeparam>
		/// <param name="sequence1">The first sequence</param>
		/// <param name="sequence2">The second sequence</param>
		/// <returns>An <see cref="IFederable{T}"/> that you can add more sequences to or federate</returns>
		public static IFederable<TSource> FederateWith<TSource>(this IEnumerable<TSource> sequence1, IEnumerable<TSource> sequence2)
		{
			return new Federator<TSource>(sequence1).FederateWith(sequence2);
		}
		

		/// <summary>
		/// Causes a federation between two sequences to occur where the sequences are federated by comparing
		/// them using their natural equality (<see cref="object.Equals(object)"/>). See 
		/// <see cref="IFederable{T}"/> for more information about sequence federation.
		/// </summary>
		/// <remarks>
		/// This is shorthand for calling <see cref="FederateWith{TSource}"/> and then calling 
		/// <see cref="IFederable{T}.Federate"/>
		/// </remarks>
		/// <typeparam name="TSource">The type in the sequences being federated</typeparam>
		/// <param name="sequence1">The first sequence</param>
		/// <param name="sequence2">The second sequence</param>
		/// <returns>
		/// An <see cref="IGrouping{TKey,TElement}"/> where <see cref="IGrouping{TKey,TElement}.Key"/> is each
		/// unique item across all sequences and the contents of each <see cref="IGrouping{TKey,TElement}"/>
		/// are <see cref="FederatedGroupItem{T}"/>s whose presences indicate which sequences include 
		/// the <see cref="IGrouping{TKey,TElement}.Key"/>.
		/// </returns>
		public static IEnumerable<IGrouping<TSource, FederatedGroupItem<TSource>>> Federate<TSource>(this IEnumerable<TSource> sequence1, IEnumerable<TSource> sequence2)
		{
			return new Federator<TSource>(sequence1).FederateWith(sequence2).Federate();
		}


		/// <summary>
		/// Causes a federation between two sequences to occur where the sequences are federated by comparing
		/// the natural equality (<see cref="object.Equals(object)"/>) of the keys selected with 
		/// <paramref name="keySelector"/>. See <see cref="IFederable{T}"/> for more information about sequence
		/// federation.
		/// </summary>
		/// <remarks>
		/// This is shorthand for calling <see cref="FederateWith{TSource}"/> and then calling
		/// <see cref="IFederable{T}.Federate{TKey}(System.Func{T,TKey})"/>
		/// </remarks>
		/// <typeparam name="TSource">The type in the sequences being federated</typeparam>
		/// <typeparam name="TKey">The type of the key selected with <paramref name="keySelector"/></typeparam>
		/// <param name="sequence1">The first sequence</param>
		/// <param name="sequence2">The second sequence</param>
		/// <param name="keySelector">
		/// A function that extracts the key for each element in the sequences
		/// </param>
		/// <returns>
		/// An <see cref="IGrouping{TKey,TElement}"/> where <see cref="IGrouping{TKey,TElement}.Key"/> is each
		/// unique key (as selected by <paramref name="keySelector"/>) across all sequences. The contents of each
		/// <see cref="IGrouping{TKey,TElement}"/> are <see cref="FederatedGroupItem{T}"/>s whose presences
		/// indicate which sequences include the <see cref="IGrouping{TKey,TElement}.Key"/> and also contain
		/// the exact item from the sequence that contained the <see cref="IGrouping{TKey,TElement}.Key"/>.
		/// </returns>
		public static IEnumerable<IGrouping<TKey, FederatedGroupItem<TSource>>> Federate<TSource, TKey>(this IEnumerable<TSource> sequence1, IEnumerable<TSource> sequence2, Func<TSource, TKey> keySelector)
		{
			return new Federator<TSource>(sequence1).FederateWith(sequence2).Federate(keySelector);
		}


		/// <summary>
		/// Causes a federation between two sequences to occur where the sequences are federated by comparing
		/// the keys selected with <paramref name="keySelector"/> with the equality comparison function defined
		/// by <paramref name="keyComparer"/>. See <see cref="IFederable{T}"/> for more information about
		/// sequence federation.
		/// </summary>
		/// <remarks>
		/// This is shorthand for calling <see cref="FederateWith{TSource}"/> and then calling
		/// <see cref="IFederable{T}.Federate{TKey}(System.Func{T,TKey},System.Collections.Generic.IEqualityComparer{TKey})"/>
		/// </remarks>
		/// <typeparam name="TSource">The type in the sequences being federated</typeparam>
		/// <typeparam name="TKey">The type of the key selected with <paramref name="keySelector"/></typeparam>
		/// <param name="sequence1">The first sequence</param>
		/// <param name="sequence2">The second sequence</param>
		/// <param name="keySelector">
		/// A function that extracts the key for each element in the sequences
		/// </param>
		/// <param name="keyComparer">
		/// A <see cref="IEqualityComparer{T}"/> that can compare the selected keys
		/// </param>
		/// <returns>
		/// An <see cref="IGrouping{TKey,TElement}"/> where <see cref="IGrouping{TKey,TElement}.Key"/> is each
		/// unique key (as selected by <paramref name="keySelector"/>) across all sequences. The contents of each
		/// <see cref="IGrouping{TKey,TElement}"/> are <see cref="FederatedGroupItem{T}"/>s whose presences
		/// indicate which sequences include the <see cref="IGrouping{TKey,TElement}.Key"/> and also contain
		/// the exact item from the sequence that contained the <see cref="IGrouping{TKey,TElement}.Key"/>.
		/// </returns>
		public static IEnumerable<IGrouping<TKey, FederatedGroupItem<TSource>>> Federate<TSource, TKey>(this IEnumerable<TSource> sequence1, IEnumerable<TSource> sequence2, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> keyComparer)
		{
			return new Federator<TSource>(sequence1).FederateWith(sequence2).Federate(keySelector, keyComparer);
		}
	}


	/// <summary>
	/// An IFederable is an object that allows you construct and execute a federated iteration sequence.  A
	/// federated iteration sequence is where you can iterate over multiple sequences at the same time in a
	/// form that groups together items from the different sequences that are equal and have them returned
	/// together, knowing exactly which sequence each item originally came from.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The verb "federate" comes from the term "federation" which means to "bring together" or "unite".
	/// Federating multiple sequences brings them together into a form where you can iterate over all the
	/// sequences together while still retaining knowledge about which items you are seeing belong to which
	/// federated sequence.
	/// </para>
	/// <para>
	/// For example, if you had the following sequences <c>{A, B, C}</c>, <c>{B, C, D}</c>, <c>{C, D, E}</c> 
	/// and you federated them in that order you would be returned the following groups (though maybe not
	/// in this order):
	/// </para>
	/// <div>
	///	<table>
	///		<tr>
	///			<th><see cref="IGrouping{TKey,TElement}.Key"/></th>
	///			<th><see cref="FederatedGroupItem{T}.SequenceIndex"/></th>
	///			<th><see cref="FederatedGroupItem{T}.Item"/></th>
	///		</tr>
	///		<tr><td>A</td><td>0</td><td>A</td></tr>
	///		<tr><td>B</td><td>0</td><td>B</td></tr>
	///		<tr><td>B</td><td>1</td><td>B</td></tr>
	///		<tr><td>C</td><td>0</td><td>C</td></tr>
	/// 	<tr><td>C</td><td>1</td><td>C</td></tr>
	/// 	<tr><td>C</td><td>2</td><td>C</td></tr>
	/// 	<tr><td>D</td><td>1</td><td>D</td></tr>
	/// 	<tr><td>D</td><td>2</td><td>D</td></tr>
	/// 	<tr><td>E</td><td>2</td><td>E</td></tr>
	/// </table>
	/// </div>
	/// <para>
	/// Note that the rows above with the same key are returned together in the one 
	/// <see cref="IGrouping{TKey,TElement}"/> object.
	/// </para>
	/// <para>
	/// A more complex example puts the Keys to better use (as they were the same at the Items in the previous
	/// example). If you had the following People objects in the following sequences:
	/// <c>{ {Name = Daniel, Id = 1}, {Name = Dwain, Id = 2} }</c>, 
	/// <c>{ {Name = Daniel, Id = 3}, {Name = Sean, Id = 4} }</c> and <c>{ {Name = Dwain, Id = 5} }</c>
	/// federated together in that order you would be returned the following groups (though maybe not
	/// in this order):
	/// </para>
	/// <div>
	///	<table>
	///		<tr>
	///			<th><see cref="IGrouping{TKey,TElement}.Key"/></th>
	///			<th><see cref="FederatedGroupItem{T}.SequenceIndex"/></th>
	///			<th><see cref="FederatedGroupItem{T}.Item"/></th>
	///		</tr>
	///		<tr><td>Daniel</td><td>0</td><td>{Name = Daniel, Id = 1}</td></tr>
	///		<tr><td>Daniel</td><td>1</td><td>{Name = Daniel, Id = 3}</td></tr>
	///		<tr><td>Dwain</td><td>0</td><td>{Name = Dwain, Id = 2}</td></tr>
	///		<tr><td>Dwain</td><td>2</td><td>{Name = Dwain, Id = 5}</td></tr>
	/// 	<tr><td>Sean</td><td>1</td><td>{Name = Sean, Id = 4}</td></tr>
	/// </table>
	/// </div>
	/// <para>
	/// To use <see cref="IFederable{T}"/> you create one by setting up a federation between two sequences
	/// by calling <see cref="FederatorExtensions.FederateWith{TSource}"/>. You can then add more sequences
	/// to be federated by calling <see cref="FederateWith"/>. The order in which the sequences are added 
	/// to the <see cref="IFederable{T}"/> counts, as each sequence is identified by its index (which is 0
	/// for the first sequence added, 1 for the second, and so on). When you have selected all the sequences to
	/// federate, you execute the federating by calling <see cref="Federate"/> or one of its overloads.
	/// You will be returned a sequence of <see cref="IGrouping{TKey,TElement}"/> objects that contain
	/// <see cref="FederatedGroupItem{T}"/>.
	/// </para>
	/// </remarks>
	/// <typeparam name="T">The type in the sequences being federated</typeparam>
	public interface IFederable<T>
	{
		/// <summary>
		/// Returns a read only collection of the sequences currently configured to be federated by
		/// this <see cref="IFederable{T}"/>.
		/// </summary>
		/// <remarks>
		/// This property is only useful for those wishing to extend <see cref="IFederable{T}"/> 
		/// functionality.
		/// </remarks>
		IList<IEnumerable<T>> Sequences { get; }

		/// <summary>
		/// Adds <paramref name="sourceSeq"/> to the list of sequences to be federated.
		/// </summary>
		/// <param name="sourceSeq">The sequence to add to the list of sequences to be federated.</param>
		/// <returns>This object</returns>
		IFederable<T> FederateWith(IEnumerable<T> sourceSeq);


		/// <summary>
		/// Causes a federation between the sequences to occur where the sequences are federated by comparing
		/// them using their natural equality (<see cref="object.Equals(object)"/>). See 
		/// <see cref="IFederable{T}"/> for more information about sequence federation.
		/// </summary>
		/// <returns>
		/// An <see cref="IGrouping{TKey,TElement}"/> where <see cref="IGrouping{TKey,TElement}.Key"/> is each
		/// unique item across all sequences and the contents of each <see cref="IGrouping{TKey,TElement}"/>
		/// are <see cref="FederatedGroupItem{T}"/>s whose presences indicate which sequences include 
		/// the <see cref="IGrouping{TKey,TElement}.Key"/>.
		/// </returns>
		IEnumerable<IGrouping<T, FederatedGroupItem<T>>> Federate();


		/// <summary>
		/// Causes a federation between two sequences to occur where the sequences are federated by comparing
		/// the natural equality (<see cref="object.Equals(object)"/>) of the keys selected with 
		/// <paramref name="keySelector"/>. See <see cref="IFederable{T}"/> for more information about sequence
		/// federation.
		/// </summary>
		/// <typeparam name="TKey">The type of the key selected with <paramref name="keySelector"/></typeparam>
		/// <param name="keySelector">
		/// A function that extracts the key for each element in the sequences
		/// </param>
		/// <returns>
		/// An <see cref="IGrouping{TKey,TElement}"/> where <see cref="IGrouping{TKey,TElement}.Key"/> is each
		/// unique key (as selected by <paramref name="keySelector"/>) across all sequences. The contents of each
		/// <see cref="IGrouping{TKey,TElement}"/> are <see cref="FederatedGroupItem{T}"/>s whose presences
		/// indicate which sequences include the <see cref="IGrouping{TKey,TElement}.Key"/> and also contain
		/// the exact item from the sequence that contained the <see cref="IGrouping{TKey,TElement}.Key"/>.
		/// </returns>
		IEnumerable<IGrouping<TKey, FederatedGroupItem<T>>> Federate<TKey>(Func<T, TKey> keySelector);


		/// <summary>
		/// Causes a federation between two sequences to occur where the sequences are federated by comparing
		/// the keys selected with <paramref name="keySelector"/> with the equality comparison function defined
		/// by <paramref name="keyComparer"/>. See <see cref="IFederable{T}"/> for more information about
		/// sequence federation.
		/// </summary>
		/// <typeparam name="TKey">The type of the key selected with <paramref name="keySelector"/></typeparam>
		/// <param name="keySelector">
		/// A function that extracts the key for each element in the sequences
		/// </param>
		/// <param name="keyComparer">
		/// A <see cref="IEqualityComparer{T}"/> that can compare the selected keys
		/// </param>
		/// <returns>
		/// An <see cref="IGrouping{TKey,TElement}"/> where <see cref="IGrouping{TKey,TElement}.Key"/> is each
		/// unique key (as selected by <paramref name="keySelector"/>) across all sequences. The contents of each
		/// <see cref="IGrouping{TKey,TElement}"/> are <see cref="FederatedGroupItem{T}"/>s whose presences
		/// indicate which sequences include the <see cref="IGrouping{TKey,TElement}.Key"/> and also contain
		/// the exact item from the sequence that contained the <see cref="IGrouping{TKey,TElement}.Key"/>.
		/// </returns>
		IEnumerable<IGrouping<TKey, FederatedGroupItem<T>>> Federate<TKey>(Func<T, TKey> keySelector, IEqualityComparer<TKey> keyComparer);
	}


	/// <summary>
	/// Implementation of <see cref="IFederable{T}"/>. See <see cref="IFederable{T}"/> for more information.
	/// </summary>
	/// <typeparam name="T">The type in the sequences being federated</typeparam>
	/// <seealso cref="IFederable{T}"/>
	internal class Federator<T> : IFederable<T>
	{
		private readonly IList<IEnumerable<T>> _Sequences;
		private IList<IEnumerable<T>> _ReadOnlySequences;

		/// <summary>
		/// Returns a read only collection of the sequences currently configured to be federated by
		/// this <see cref="IFederable{T}"/>.
		/// </summary>
		/// <remarks>
		/// This property is only useful for those wishing to extend <see cref="IFederable{T}"/> 
		/// functionality.
		/// </remarks>
		public IList<IEnumerable<T>> Sequences
		{
			get
			{
				//Lazy load the read only collection, so we don't waste an object allocation
				//on somethat that'll almost never be used
				if (_ReadOnlySequences == null)
					_ReadOnlySequences = new ReadOnlyCollection<IEnumerable<T>>(_Sequences);

				return _ReadOnlySequences;
			}
		}

		/// <summary>
		/// Constructor, creates a new Federator
		/// </summary>
		/// <param name="sequence">The first sequence to be federated</param>
		public Federator(IEnumerable<T> sequence)
		{
			_Sequences = new List<IEnumerable<T>>();
			_Sequences.Add(sequence);
		}


		/// <summary>
		/// Adds <paramref name="sourceSeq"/> to the list of sequences to be federated.
		/// </summary>
		/// <param name="sourceSeq">The sequence to add to the list of sequences to be federated.</param>
		/// <returns>This object</returns>
		public IFederable<T> FederateWith(IEnumerable<T> sourceSeq)
		{
			_Sequences.Add(sourceSeq);
			return this;
		}


		/// <summary>
		/// Causes a federation between the sequences to occur where the sequences are federated by comparing
		/// them using their natural equality (<see cref="object.Equals(object)"/>). See 
		/// <see cref="IFederable{T}"/> for more information about sequence federation.
		/// </summary>
		/// <returns>
		/// An <see cref="IGrouping{TKey,TElement}"/> where <see cref="IGrouping{TKey,TElement}.Key"/> is each
		/// unique item across all sequences and the contents of each <see cref="IGrouping{TKey,TElement}"/>
		/// are <see cref="FederatedGroupItem{T}"/>s whose presences indicate which sequences include 
		/// the <see cref="IGrouping{TKey,TElement}.Key"/>.
		/// </returns>
		public IEnumerable<IGrouping<T, FederatedGroupItem<T>>> Federate()
		{
			return Federate(i => i);
		}


		/// <summary>
		/// Causes a federation between two sequences to occur where the sequences are federated by comparing
		/// the natural equality (<see cref="object.Equals(object)"/>) of the keys selected with 
		/// <paramref name="keySelector"/>. See <see cref="IFederable{T}"/> for more information about sequence
		/// federation.
		/// </summary>
		/// <typeparam name="TKey">The type of the key selected with <paramref name="keySelector"/></typeparam>
		/// <param name="keySelector">
		/// A function that extracts the key for each element in the sequences
		/// </param>
		/// <returns>
		/// An <see cref="IGrouping{TKey,TElement}"/> where <see cref="IGrouping{TKey,TElement}.Key"/> is each
		/// unique key (as selected by <paramref name="keySelector"/>) across all sequences. The contents of each
		/// <see cref="IGrouping{TKey,TElement}"/> are <see cref="FederatedGroupItem{T}"/>s whose presences
		/// indicate which sequences include the <see cref="IGrouping{TKey,TElement}.Key"/> and also contain
		/// the exact item from the sequence that contained the <see cref="IGrouping{TKey,TElement}.Key"/>.
		/// </returns>
		public IEnumerable<IGrouping<TKey, FederatedGroupItem<T>>> Federate<TKey>(Func<T, TKey> keySelector)
		{
			return Federate(keySelector, EqualityComparer<TKey>.Default);
		}


		/// <summary>
		/// Causes a federation between two sequences to occur where the sequences are federated by comparing
		/// the keys selected with <paramref name="keySelector"/> with the equality comparison function defined
		/// by <paramref name="keyComparer"/>. See <see cref="IFederable{T}"/> for more information about
		/// sequence federation.
		/// </summary>
		/// <typeparam name="TKey">The type of the key selected with <paramref name="keySelector"/></typeparam>
		/// <param name="keySelector">
		/// A function that extracts the key for each element in the sequences
		/// </param>
		/// <param name="keyComparer">
		/// A <see cref="IEqualityComparer{T}"/> that can compare the selected keys
		/// </param>
		/// <returns>
		/// An <see cref="IGrouping{TKey,TElement}"/> where <see cref="IGrouping{TKey,TElement}.Key"/> is each
		/// unique key (as selected by <paramref name="keySelector"/>) across all sequences. The contents of each
		/// <see cref="IGrouping{TKey,TElement}"/> are <see cref="FederatedGroupItem{T}"/>s whose presences
		/// indicate which sequences include the <see cref="IGrouping{TKey,TElement}.Key"/> and also contain
		/// the exact item from the sequence that contained the <see cref="IGrouping{TKey,TElement}.Key"/>.
		/// </returns>
		public IEnumerable<IGrouping<TKey, FederatedGroupItem<T>>> Federate<TKey>(Func<T, TKey> keySelector, IEqualityComparer<TKey> keyComparer)
		{
			IEnumerable<FederatedGroupItem<T>> concatedSequences = _Sequences[0].Select(item => new FederatedGroupItem<T>(0, item));
			for (int i = 1; i < _Sequences.Count; i++)
			{
				int capturedI = i;
				concatedSequences = concatedSequences.Concat(_Sequences[i].Select(item => new FederatedGroupItem<T>(capturedI, item)));
			}

			return concatedSequences.GroupBy(fgi => keySelector(fgi.Item), keyComparer);
		}
	}


	/// <summary>
	/// Represents an item from one of the sequences being federated by an <see cref="IFederable{T}"/>.
	/// It notes which sequence the item came from by way of its index, which is derived from the order
	/// in which the sequences are added to the <see cref="IFederable{T}"/>.
	/// </summary>
	/// <typeparam name="T">The type contained in the sequences being federated</typeparam>
	public class FederatedGroupItem<T>
	{
		private readonly int _SequenceIndex;
		private readonly T _Item;

		/// <summary>
		/// The item from one of the sequences that was federated
		/// </summary>
		public T Item
		{
			get { return _Item; }
		}

		/// <summary>
		/// The index of the sequence from which <see cref="Item"/> is from. The index is derived from the
		/// order in which the sequences are added to the <see cref="IFederable{T}"/>.
		/// </summary>
		public int SequenceIndex
		{
			get { return _SequenceIndex; }
		}


		/// <summary>
		/// Constructor, creates a FederatedGroupItem
		/// </summary>
		/// <param name="sequenceIndex">The sequence index number</param>
		/// <param name="item">The item</param>
		public FederatedGroupItem(int sequenceIndex, T item)
		{
			_SequenceIndex = sequenceIndex;
			_Item = item;
		}
	}
}
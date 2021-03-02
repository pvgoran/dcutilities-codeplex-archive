using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;


namespace DigitallyCreated.Utilities.Bcl
{
	/// <summary>
	/// A wrapper for an <see cref="IDictionary{TKey,TValue}"/> through which all writing to the wrapped 
	/// <see cref="IDictionary{TKey,TValue}"/> is prevented.
	/// </summary>
	/// <remarks>
	/// Because this is a wrapper, changes made to the underlying dictionary are reflected by the read-only
	/// dictionary.
	/// </remarks>
	/// <typeparam name="TKey">The type of keys in the dictionary</typeparam>
	/// <typeparam name="TValue">The type of values in the dictionary</typeparam>
	[Serializable]
	[ComVisible(false)]
	[DebuggerTypeProxy(typeof(ReadOnlyDictionaryDebuggerTypeProxy<,>))]
	[DebuggerDisplay("Count = {Count}")]
	public class ReadOnlyDictionary<TKey, TValue> : IDictionary<TKey, TValue>
	{
		private readonly IDictionary<TKey, TValue> _Dictionary;


		/// <summary>
		/// Constructor, creates a <see cref="ReadOnlyDictionary{TKey,TValue}"/>
		/// </summary>
		/// <param name="dictionary">The dictionary to wrap</param>
		/// <exception cref="ArgumentNullException">If <paramref name="dictionary"/> is null</exception>
		public ReadOnlyDictionary(IDictionary<TKey, TValue> dictionary)
		{
			if (dictionary == null)
				throw new ArgumentNullException("dictionary");

			_Dictionary = dictionary;
		}


		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>
		/// A <see cref="IEnumerator{T}"/> that can be used to iterate through the collection.
		/// </returns>
		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return _Dictionary.GetEnumerator();
		}


		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>
		/// An <see cref="IEnumerator"/> object that can be used to iterate through the collection.
		/// </returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}


		/// <summary>
		/// This method is not supported by the <see cref="ReadOnlyDictionary{TKey,TValue}"/>
		/// </summary>
		/// <param name="item">The object to add to the <see cref="ICollection{T}"/>.</param>
		/// <exception cref="NotSupportedException">The <see cref="ICollection{T}"/> is read-only.</exception>
		void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
		{
			throw new NotSupportedException("The dictionary is read-only");
		}


		/// <summary>
		/// This method is not supported by the <see cref="ReadOnlyDictionary{TKey,TValue}"/>
		/// </summary>
		/// <exception cref="NotSupportedException">The <see cref="ICollection{T}"/> is read-only.</exception>
		void ICollection<KeyValuePair<TKey, TValue>>.Clear()
		{
			throw new NotSupportedException("The dictionary is read-only");
		}


		/// <summary>
		/// Determines whether the <see cref="ICollection{T}"/> contains a specific value.
		/// </summary>
		/// <param name="item">The object to locate in the <see cref="ICollection{T}"/>.</param>
		/// <returns>
		/// true if <paramref name="item"/> is found in the <see cref="ICollection{T}"/>; otherwise, false.
		/// </returns>
		public bool Contains(KeyValuePair<TKey, TValue> item)
		{
			return _Dictionary.Contains(item);
		}


		/// <summary>
		/// Copies the elements of the <see cref="ICollection{T}"/> to an <see cref="Array"/>, starting at a
		/// particular <see cref="T:System.Array"/> index.
		/// </summary>
		/// <param name="array">
		/// The one-dimensional <see cref="Array"/> that is the destination of the elements copied from 
		/// <see cref="ICollection{T}"/>. The <see cref="Array"/> must have zero-based indexing.
		/// </param>
		/// <param name="arrayIndex">
		/// The zero-based index in <paramref name="array"/> at which copying begins.
		/// </param>
		/// <exception cref="ArgumentNullException"><paramref name="array"/> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than 0.</exception>
		/// <exception cref="ArgumentException">
		/// <paramref name="array"/> is multidimensional. -or- The number of elements in the source 
		/// <see cref="ICollection{T}"/> is greater than the available space from <paramref name="arrayIndex"/> to
		/// the end of the destination <paramref name="array"/>
		/// </exception>
		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			_Dictionary.CopyTo(array, arrayIndex);
		}


		/// <summary>
		/// This method is not supported by the <see cref="ReadOnlyDictionary{TKey,TValue}"/>
		/// </summary>
		/// <param name="item">The object to remove from the <see cref="ICollection{T}"/>.</param>
		/// <returns>
		/// true if <paramref name="item"/> was successfully removed from the <see cref="ICollection{T}"/>;
		/// otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original
		/// <see cref="ICollection{T}"/>.
		/// </returns>
		/// <exception cref="NotSupportedException">The <see cref="ICollection{T}"/> is read-only.</exception>
		bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
		{
			throw new NotSupportedException("The dictionary is read-only");
		}


		/// <summary>
		/// Gets the number of elements contained in the <see cref="ICollection{T}"/>.
		/// </summary>
		/// <returns>
		/// The number of elements contained in the <see cref="ICollection{T}"/>.
		/// </returns>
		public int Count
		{
			get { return _Dictionary.Count; }
		}

		/// <summary>
		/// Gets a value indicating whether the <see cref="ICollection{T}"/> is read-only.
		/// </summary>
		/// <returns>
		/// Always returns true.
		/// </returns>
		public bool IsReadOnly
		{
			get { return true; }
		}


		/// <summary>
		/// Determines whether the <see cref="IDictionary{TKey,TValue}"/> contains an element with the specified key.
		/// </summary>
		/// <param name="key">The key to locate in the <see cref="IDictionary{TKey,TValue}"/>.</param>
		/// <returns>
		/// true if the <see cref="IDictionary{TKey,TValue}"/> contains an element with the key; otherwise, false.
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="key"/> is null.</exception>
		public bool ContainsKey(TKey key)
		{
			return _Dictionary.ContainsKey(key);
		}


		/// <summary>
		/// This method is not supported by the <see cref="ReadOnlyDictionary{TKey,TValue}"/>
		/// </summary>
		/// <param name="key">The object to use as the key of the element to add.</param>
		/// <param name="value">The object to use as the value of the element to add.</param>
		/// <exception cref="NotSupportedException">
		/// The <see cref="IDictionary{TKey,TValue}"/> is read-only.
		/// </exception>
		void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
		{
			throw new NotSupportedException("The dictionary is read-only");
		}


		/// <summary>
		/// This method is not supported by the <see cref="ReadOnlyDictionary{TKey,TValue}"/>
		/// </summary>
		/// <param name="key">The key of the element to remove.</param>
		/// <returns>
		/// true if the element is successfully removed; otherwise, false. This method also returns false if 
		/// <paramref name="key"/> was not found in the original <see cref="IDictionary{TKey,TValue}"/>.
		/// </returns>
		/// <exception cref="T:System.NotSupportedException">
		/// The <see cref="IDictionary{TKey,TValue}"/> is read-only.
		/// </exception>
		bool IDictionary<TKey, TValue>.Remove(TKey key)
		{
			throw new NotSupportedException("The dictionary is read-only");
		}


		/// <summary>
		/// Gets the value associated with the specified key.
		/// </summary>
		/// <param name="key">The key whose value to get.</param>
		/// <param name="value">
		/// When this method returns, the value associated with the specified key, if the key is found; otherwise,
		/// the default value for the type of the <paramref name="value"/> parameter. This parameter is passed
		/// uninitialized.
		/// </param>
		/// <returns>
		/// true if the object that implements <see cref="IDictionary{TKey,TValue}"/> contains an element with the
		/// specified key; otherwise, false.
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="key"/> is null.</exception>
		public bool TryGetValue(TKey key, out TValue value)
		{
			return _Dictionary.TryGetValue(key, out value);
		}


		/// <summary>
		/// Gets the element with the specified key.
		/// </summary>
		/// <param name="key">The key of the element to get.</param>
		/// <returns>
		/// The element with the specified key.
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="key"/> is null.</exception>
		/// <exception cref="KeyNotFoundException">
		/// The property is retrieved and <paramref name="key"/> is not found.
		/// </exception>
		public TValue this[TKey key]
		{
			get { return _Dictionary[key]; }
		}


		/// <summary>
		/// Gets the element with the specified key. Setting is not supported by the 
		/// <see cref="ReadOnlyDictionary{TKey,TValue}"/>
		/// </summary>
		/// <param name="key">The key of the element to get or set.</param>
		/// <returns>
		/// The element with the specified key.
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="key"/> is null.</exception>
		/// <exception cref="KeyNotFoundException">
		/// The property is retrieved and <paramref name="key"/> is not found.
		/// </exception>
		/// <exception cref="NotSupportedException">
		/// The property is set and the <see cref="IDictionary{TKey,TValue}"/> is read-only.
		/// </exception>
		TValue IDictionary<TKey, TValue>.this[TKey key]
		{
			get { return _Dictionary[key]; }
			set { throw new NotSupportedException("The dictionary is read-only"); }
		}


		/// <summary>
		/// Gets an <see cref="ICollection{T}"/> containing the keys of the <see cref="IDictionary{TKey,TValue}"/>.
		/// </summary>
		/// <returns>
		/// An <see cref="ICollection{T}"/> containing the keys of the object that implements 
		/// <see cref="IDictionary{TKey,TValue}"/>.
		/// </returns>
		public ICollection<TKey> Keys
		{
			get { return _Dictionary.Keys; }
		}

		/// <summary>
		/// Gets an <see cref="ICollection{T}"/> containing the values in the <see cref="IDictionary{TKey,TValue}"/>.
		/// </summary>
		/// <returns>
		/// An <see cref="ICollection{T}"/> containing the values in the object that implements 
		/// <see cref="IDictionary{TKey,TValue}"/>.
		/// </returns>
		public ICollection<TValue> Values
		{
			get { return _Dictionary.Values; }
		}
	}


	/// <summary>
	/// Debugging type proxy for <see cref="IDictionary{TKey,TValue}"/>
	/// </summary>
	/// <typeparam name="TKey">The type of keys in the dictionary</typeparam>
	/// <typeparam name="TValue">The type of values in the dictionary</typeparam>
	internal sealed class ReadOnlyDictionaryDebuggerTypeProxy<TKey, TValue>
	{
		private readonly IDictionary<TKey, TValue> _Dictionary;

		public ReadOnlyDictionaryDebuggerTypeProxy(IDictionary<TKey, TValue> dictionary)
		{
			if (dictionary == null)
				throw new ArgumentNullException("dictionary");

			_Dictionary = dictionary;
		}

		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		public KeyValuePair<TKey, TValue>[] Items
		{
			get
			{
				KeyValuePair<TKey, TValue>[] array = new KeyValuePair<TKey, TValue>[_Dictionary.Count];
				_Dictionary.CopyTo(array, 0);
				return array;
			}
		}
	}
}
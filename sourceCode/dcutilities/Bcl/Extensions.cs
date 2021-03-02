using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DigitallyCreated.Utilities.Bcl
{
	/// <summary>
	/// Some various extension methods adding miscellaneous functionality to the .NET base class library
	/// </summary>
	public static class Extensions
	{
		/// <summary>
		/// Adds all the <paramref name="items"/> to the <paramref name="collection"/>.
		/// </summary>
		/// <typeparam name="TCollection">The type of the collection</typeparam>
		/// <typeparam name="TAdd">
		/// The type to add to the collection (equal to or a subtype of <typeparamref name="TCollection"/>)
		/// </typeparam>
		/// <param name="collection">The collection to add the items to</param>
		/// <param name="items">The items to add</param>
		public static void AddAll<TCollection, TAdd>(this ICollection<TCollection> collection, IEnumerable<TAdd> items)
			where TAdd : TCollection
		{
			if (collection is List<TCollection>)
				((List<TCollection>)collection).AddRange(items.Cast<TCollection>());
			else
			{
				foreach (TAdd item in items)
					collection.Add(item);
			}
		}


		/// <summary>
		/// Removes all the <paramref name="items"/> from the <paramref name="collection"/>.
		/// </summary>
		/// <typeparam name="TCollection">The type of the collection</typeparam>
		/// <typeparam name="TAdd">
		/// The type to add to the collection (equal to or a subtype of <typeparamref name="TCollection"/>)
		/// </typeparam>
		/// <param name="collection">The collection to remove the items from</param>
		/// <param name="items">The items to remove</param>
		public static void RemoveAll<TCollection, TAdd>(this ICollection<TCollection> collection, IEnumerable<TAdd> items)
			where TAdd : TCollection
		{
			foreach (TAdd item in items)
			{
				collection.Remove(item);
			}
		}


		/// <summary>
		/// Checks whether two strings are equal, ignoring case.
		/// </summary>
		/// <param name="firstString">The string</param>
		/// <param name="compareToString">The string to compare with</param>
		/// <returns>True if they are equal case-insensitively, false otherwise</returns>
		public static bool EqualsIgnoreCase(this string firstString, string compareToString)
		{
			return String.Compare(firstString, compareToString, true) == 0;
		}


		/// <summary>
		/// This method allows you to overcome the limitations of the using block
		/// (see http://msdn.microsoft.com/en-us/library/aa355056.aspx for more information) by being
		/// able to catch exceptions in the block and exceptions from the dispose method and if
		/// both occur throw an <see cref="AggregateException"/>.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Note that the semantic of this block is slightly different to a normal using block, since
		/// keywords like return will not "return" the current method; rather they will return the
		/// action delegate you pass to the method.
		/// </para>
		/// <example>
		/// Here is an example of how you could use this:
		/// <code>
		/// new MyWcfClient().SafeUsingBlock(myWcfClient =>
		/// {
		///	    myWcfClient.MyServiceMethod();
		/// });
		/// </code>
		/// </example>
		/// </remarks>
		/// <typeparam name="TDisposable">The <see cref="IDisposable"/> type</typeparam>
		/// <param name="disposable">The disposable object</param>
		/// <param name="action">A function that will run as "using block"</param>
		public static void SafeUsingBlock<TDisposable>(this TDisposable disposable, Action<TDisposable> action)
			where TDisposable : IDisposable
		{
			disposable.SafeUsingBlock(action, d => d);
		}


		/// <summary>
		/// Internal implementation of SafeUsingBlock that enables custom unwrapping of the object
		/// to pass to the safe using block action.
		/// </summary>
		/// <typeparam name="TDisposable">The disposable type</typeparam>
		/// <typeparam name="T">The type passed to the action</typeparam>
		/// <param name="disposable">The disposable object</param>
		/// <param name="action">The type passed to the action</param>
		/// <param name="unwrapper">
		/// The unwrapper function that takes the disposable and returns the object to pass to the action
		/// </param>
		internal static void SafeUsingBlock<TDisposable, T>(this TDisposable disposable, Action<T> action, Func<TDisposable, T> unwrapper)
			where TDisposable : IDisposable
		{
			try
			{
				action(unwrapper(disposable));
			}
			catch (Exception actionException)
			{
				try
				{
					disposable.Dispose();
				}
				catch (Exception disposeException)
				{
					throw new AggregateException(actionException, disposeException);
				}

				throw;
			}

			disposable.Dispose(); //Let it throw on its own
		}
	}
}

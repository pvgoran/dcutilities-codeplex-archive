using System;
using System.Collections.Generic;

namespace DigitallyCreated.Utilities.ErrorReporting
{
	/// <summary>
	/// This class provides a type-safe way for classes to implement 
	/// <see cref="IDetailProvider"/> (ie by inheriting from this class)
	/// </summary>
	/// <typeparam name="T">The type of object that you will providing details for</typeparam>
	public abstract class AbstractDetailProvider<T> : IDetailProvider 
	{
		/// <summary>
		/// Whether or not this detail provider can provide details about the concrete
		/// type of object passed in.
		/// </summary>
		/// <param name="obj">The object that you may want details about</param>
		/// <returns>
		/// True if the detail provider can provide details about the passed in object
		/// </returns>
		public bool CanProvideDetailFor(object obj)
		{
			return obj is T;
		}


		/// <summary>
		/// Gets a dictionary of details about the object where the key is the detail's
		/// name and the key is the object that is the detail.
		/// </summary>
		/// <param name="obj">The object to read the details from</param>
		/// <returns>
		/// A dictionary of details about the object where the key is the detail's
		/// name and the key is the object that is the detail.
		/// </returns>
		/// <remarks>
		/// Use an <see cref="ITypeRenderer"/> to render a detail object
		/// </remarks>
		/// <exception cref="TypeNotSupportedException">
		/// Thrown if this detail provider does not support the type of object you
		/// passed in.
		/// </exception>
		public IDictionary<string, object> GetDetail(object obj)
		{
			if (CanProvideDetailFor(obj))
				return DoGetDetail((T)obj);
			
			throw new TypeNotSupportedException(obj.GetType(), GetType());
		}


		/// <summary>
		/// Gets a dictionary of details about the object where the key is the detail's
		/// name and the key is the object that is the detail.
		/// </summary>
		/// <param name="obj">The object to read the details from</param>
		/// <returns>
		/// A dictionary of details about the object where the key is the detail's
		/// name and the key is the object that is the detail.
		/// </returns>
		protected abstract IDictionary<string, object> DoGetDetail(T obj);
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DigitallyCreated.Utilities.ErrorReporting
{
	/// <summary>
	/// An <see cref="IDetailProvider"/> is able to extract detailed information
	/// from a certain type of object.
	/// </summary>
	/// <remarks>
	/// Note that the provider only returns the detailed information about the concrete object 
	/// type, and not the information from the type's super type. This means that a detail provider
	/// for the object type "MyException" that extends from <see cref="Exception"/> that has a 
	/// special property "MyErrorCode" would only return details about "MyErrorCode" and not about, 
	/// for example, <see cref="Exception.Message"/> (which is a property inherited from 
	/// <see cref="Exception"/>. If you want to display <see cref="Exception.Message"/> you need 
	/// another detail provider for the <see cref="Exception"/> type specifically.
	/// </remarks>
	public interface IDetailProvider
	{
		/// <summary>
		/// Whether or not this detail provider can provide details about the concrete
		/// type of object passed in.
		/// </summary>
		/// <param name="obj">The object that you may want details about</param>
		/// <returns>
		/// True if the detail provider can provide details about the passed in object
		/// </returns>
		bool CanProvideDetailFor(object obj);
		

		/// <summary>
		/// Gets a dictionary of details about the object where the key is the detail's
		/// name and the key is the object that is the detail.
		/// </summary>
		/// <param name="obj">The object to read the details from</param>
		/// <returns>
		/// A dictionary of details about the object where the key is the detail's
		/// name and the value is the object that is the detail.
		/// </returns>
		/// <remarks>
		/// Use an <see cref="ITypeRenderer"/> to render a detail object
		/// </remarks>
		/// <exception cref="TypeNotSupportedException">
		/// Thrown if this detail provider does not support the type of object you
		/// passed in.
		/// </exception>
		IDictionary<string, object> GetDetail(object obj);
	}
}

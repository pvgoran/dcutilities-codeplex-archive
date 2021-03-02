using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;


namespace DigitallyCreated.Utilities.ErrorReporting.DetailProviders
{
	/// <summary>
	/// This <see cref="IDetailProvider"/> is designed to be a fallback detail provider that 
	/// can provide detail on any object by using reflection. It will reflect over all public
	/// properties and include each's value as a detail. It will also evaluate
	/// <see cref="object.ToString"/> and include that as a detail. If the object to
	/// provide details for is null, no details will be returned.
	/// </summary>
	public class ReflectionFallbackDetailProvider : IDetailProvider
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
			return true;
		}


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
		public IDictionary<string, object> GetDetail(object obj)
		{
			IDictionary<string, object> details = new Dictionary<string, object>();

			if (obj == null)
				return details;

			IEnumerable<PropertyInfo> properties = from property in obj.GetType().GetProperties()
												   where property.GetGetMethod() != null
												   where property.GetIndexParameters().Length == 0
			                                       select property;

			foreach (PropertyInfo property in properties)
			{
				try
				{
					object detail = property.GetValue(obj, null);
					details.Add(property.Name, detail);
				}
				catch (Exception)
				{
					continue;
				}
			}

			details.Add("ToString()", obj.ToString());

			return details;
		}
	}
}
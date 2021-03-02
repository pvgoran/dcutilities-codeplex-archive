using System;
using System.Collections.Generic;


namespace DigitallyCreated.Utilities.ErrorReporting
{
	/// <summary>
	/// Utility methods for use by the Error Renderer code
	/// </summary>
	public static class UtilityMethods
	{
		/// <summary>
		/// Gets the details for a object by using a set of <see cref="IDetailProvider"/>s. If none of the
		/// <see cref="IDetailProvider"/>s specified by <paramref name="providers"/> are able to provide
		/// details for the object, then the <paramref name="fallbackDetailProvider"/> is used to provide
		/// details.
		/// </summary>
		/// <param name="forObject">The object to get the details for</param>
		/// <param name="providers">The <see cref="IDetailProvider"/> to use</param>
		/// <param name="fallbackDetailProvider">
		/// The fallback <see cref="IDetailProvider"/> to use if none of the <paramref name="providers"/> can
		/// be used
		/// </param>
		/// <returns>
		/// A collection of details about the object where the key in the <see cref="KeyValuePair{TKey,TValue}"/> 
		/// is the detail's name and the value is the object that is the detail.
		/// </returns>
		public static IEnumerable<KeyValuePair<string, object>> GetDetailsForObject(object forObject, IEnumerable<IDetailProvider> providers, IDetailProvider fallbackDetailProvider)
		{
			bool matched = false;

			foreach (IDetailProvider detailProvider in providers)
			{
				if (detailProvider.CanProvideDetailFor(forObject) == false)
					continue;

				matched = true;
				foreach (KeyValuePair<string, object> detailPair in detailProvider.GetDetail(forObject))
					yield return detailPair;
			}

			if (matched == false)
			{
				if (fallbackDetailProvider.CanProvideDetailFor(forObject))
					foreach (KeyValuePair<string, object> detailPair in fallbackDetailProvider.GetDetail(forObject))
						yield return detailPair;
			}
		}


		/// <summary>
		/// Chooses the first <see cref="ITypeRenderer"/> in <paramref name="typeRenderers"/> that is able to
		/// render <paramref name="obj"/>.
		/// </summary>
		/// <param name="obj">The object to find a <see cref="ITypeRenderer"/> for</param>
		/// <param name="typeRenderers">The type renderers</param>
		/// <returns>The type renderer</returns>
		/// <exception cref="NotSupportedException">If a <see cref="ITypeRenderer"/> can't be found</exception>
		public static ITypeRenderer ChooseTypeRenderer(object obj, IEnumerable<ITypeRenderer> typeRenderers)
		{
			foreach (ITypeRenderer renderer in typeRenderers)
			{
				if (renderer.CanRender(obj))
					return renderer;
			}

			throw new NotSupportedException("Cannot find a type renderer for the type " + obj.GetType().FullName);
		}

	}
}
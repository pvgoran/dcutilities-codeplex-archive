using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Mvc;
using DigitallyCreated.Utilities.Bcl.Linq;


namespace DigitallyCreated.Utilities.Mvc
{
	/// <summary>
	/// This model binder is able to bind a <see cref="Sorter{T}"/>.
	/// </summary>
	/// <remarks>
	/// <para>
	/// To use this binder you must use the <see cref="ModelBinderAttribute"/> on the parameter you want to
	/// be bound with this binder, to cause this binder to be used to do the binding.
	/// </para>
	/// <para>
	/// To get customise how the <see cref="Sorter{T}"/> that is bound is configured, you must subclass
	/// this binder. This allows you to set the sorter's various settings, such as its Translation
	/// Dictionary, the Prefix it uses. You do this by overriding 
	/// <see cref="TranslationDictionary"/> and <see cref="Prefix"/>.
	/// </para>
	/// <para>
	/// You can also customise how columns are sorted when there is no sorting defined (ie. the default
	/// sort order) by overriding <see cref="CreateNewSorter"/>. By default, all properties are added to
	/// the sorter and sorted ascending in alphabetical order.
	/// </para>
	/// <para>
	/// In the case that no sorting settings were provided in the routing data (ie the query parameter was
	/// not defined), this binder has the ability to get the previous sorter used the last time the user
	/// used the action method by getting a copy of the last used sorter out of the ASP.NET session. To
	/// enable this functionality, you must override <see cref="SessionKey"/>and return a unique session
	/// key.
	/// </para>
	/// <para>
	/// If an error occurs when decoding the <see cref="Sorter{T}"/> from a string, the error will be
	/// silently absorbed and a new sorter (from <see cref="CreateNewSorter"/>) returned in its place.
	/// </para>
	/// </remarks>
	/// <typeparam name="T">The type of the sorter to do binding with</typeparam>
	/// <see cref="Sorter{T}"/>
	public class SorterModelBinder<T> : IModelBinder
	{
		/// <summary>
		/// Returns the translation dictionary to use with the Sorter
		/// </summary>
		protected virtual IDictionary<Expression<Func<T, object>>, string> TranslationDictionary
		{
			get { return null; }
		}

		/// <summary>
		/// Returns the prefix to use with the sorter
		/// </summary>
		protected virtual string Prefix
		{
			get { return null; }
		}

		/// <summary>
		/// Returns the key to use to save/load the sorter from the ASP.NET session
		/// </summary>
		protected virtual string SessionKey
		{
			get { return null; }
		}


		/// <summary>
		/// Creates a new sorter with the default sorting options set
		/// </summary>
		/// <returns>A new sorter</returns>
		protected virtual Sorter<T> CreateNewSorter()
		{
			if (TranslationDictionary != null)
				return new Sorter<T>(TranslationDictionary).AddAllProperties(true);

			return new Sorter<T>().AddAllProperties(true);

		}


		/// <summary>
		/// Binds a <see cref="Sorter{T}"/>
		/// </summary>
		/// <param name="controllerContext">The controller context</param>
		/// <param name="bindingContext">The binding context</param>
		/// <returns>The <see cref="Sorter{T}"/></returns>
		public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
		{
			if (bindingContext.ModelType != (typeof(Sorter<T>)))
				return null;

			Sorter<T> sorter = MaterialiseSorter(controllerContext, bindingContext);
			if (SessionKey != null)
				controllerContext.HttpContext.Session[SessionKey] = sorter;

			return sorter;
		}


		/// <summary>
		/// Creates the sorter, loading its data from the route parameters or from the session, or creating
		/// a new sorter
		/// </summary>
		/// <param name="controllerContext">The controller context</param>
		/// <param name="bindingContext">The binding context</param>
		/// <returns>The <see cref="Sorter{T}"/></returns>
		private Sorter<T> MaterialiseSorter(ControllerContext controllerContext, ModelBindingContext bindingContext)
		{
			ValueProviderResult valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

			//If the sorter cannot be gotten from the value provider)
			if (valueProviderResult == null)
			{
				//Try getting the sorter out of the session, if a session key has been set
				if (SessionKey != null && controllerContext.HttpContext.Session[SessionKey] != null)
					return (Sorter<T>)controllerContext.HttpContext.Session[SessionKey];

				//Else return a new sorter
				return CreateNewSorter();
			}

			string encodedString = valueProviderResult.AttemptedValue;

			try
			{
				if (TranslationDictionary == null)
					return ValidateDecodedSorter(Sorter<T>.Decode(Prefix, encodedString));

				return ValidateDecodedSorter(Sorter<T>.Decode(Prefix, TranslationDictionary, encodedString));
			}
			catch (SorterException)
			{
				return CreateNewSorter();
			}
		}


		/// <summary>
		/// Validates a decoded sorter and ensures that the string it was decoded from wasn't tampered with.
		/// If it was (properties that shouldn't be sorted are being sorted, or properties that should be 
		/// sorted aren't being sorted) they will be added and removed as necessary using a sorter created
		/// by <see cref="CreateNewSorter"/> as a template.
		/// </summary>
		/// <param name="decodedSorter">The decoded sorter</param>
		/// <returns>The validated <paramref name="decodedSorter"/></returns>
		private Sorter<T> ValidateDecodedSorter(Sorter<T> decodedSorter)
		{
			Sorter<T> correctSorter = CreateNewSorter();

			IEnumerable<MatchPair<KeyValuePair<LambdaExpression, bool>, KeyValuePair<LambdaExpression, bool>>> matchPairs = 
				decodedSorter.OrderedPropertySelectors
				.MatchUp(correctSorter.OrderedPropertySelectors, (f, s) => decodedSorter.PropertySelectorsEqual(f.Key, s.Key));

			//Find properties that we shouldn't be sorting and remove them
			IEnumerable<LambdaExpression> toRemove = from pair in matchPairs
			                                         where pair.IsSecondSet == false
			                                         select pair.First.Key;
			
			foreach (LambdaExpression propertySelector in toRemove)
				decodedSorter.RemoveProperty(propertySelector);

			//Find properties that we should be sorting and add them
			IEnumerable<KeyValuePair<LambdaExpression, bool>> toAdd = from pair in matchPairs
			                                                          where pair.IsFirstSet == false
			                                                          select pair.Second;

			MethodInfo addPropertyGenericMethod = decodedSorter.GetType().GetMethod("AddProperty");
			foreach (KeyValuePair<LambdaExpression, bool> kvp in toAdd)
			{
				//Use reflection to add the selector to the sorter, since we don't know the exact
				//type the selector will be at compile time, so we need to "dynamically" call
				//the generic AddProperty method. C# 4 dynamic would be great here.
				Type funcType = kvp.Key.GetType().GetGenericArguments()[0]; //Func<T, returnType>
				Type returnType = funcType.GetGenericArguments()[1];
				MethodInfo method = addPropertyGenericMethod.MakeGenericMethod(returnType);
				method.Invoke(decodedSorter, new object[] {kvp.Key, kvp.Value});
			}
			
			return decodedSorter;
		}
	}
}
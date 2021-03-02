using System;
using System.Web.Mvc;


namespace DigitallyCreated.Utilities.Mvc
{
	/// <summary>
	/// The BoolBinder is able to bind a bool and always bind true or false. This is different to the
	/// <see cref="DefaultModelBinder"/> which, when no form data for the bool has been submitted, will
	/// try to bind null to bool (and therefore fail). The BoolBinder will, when no form data has been
	/// submitted (as is the case with an unticked checkbox), bind false.
	/// </summary>
	public class BoolBinder : IModelBinder
	{
		/// <summary>
		/// Binds a <see cref="bool"/>
		/// </summary>
		/// <param name="controllerContext">The controller context</param>
		/// <param name="bindingContext">The binding context</param>
		/// <returns>The <see cref="bool"/></returns>
		public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
		{
			if (bindingContext.ModelType != typeof(bool))
				throw new InvalidOperationException("You cannot bind an object that is not a bool with the BoolBinder");

			ValueProviderResult value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
			if (value == null)
				return false;

			if (value.RawValue is bool)
				return value;

			try {
				return Boolean.Parse(value.AttemptedValue);
			}
			catch (FormatException)
			{
				return false;
			}
		}
	}
}
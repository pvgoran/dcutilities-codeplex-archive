using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using DigitallyCreated.Utilities.Bcl;
using DigitallyCreated.Utilities.Bcl.Configuration;


namespace DigitallyCreated.Utilities.Mvc
{
	/// <summary>
	/// This class contains <see cref="HtmlHelper"/> extension methods that add HTML form rendering
	/// functionality.
	/// </summary>
	public static class FormHtmlHelpers
	{
		/// <summary>
		/// Renders a normal HTML form checkbox.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This is different to the normal 
		/// <see cref="InputExtensions.CheckBox(System.Web.Mvc.HtmlHelper,string,System.Collections.Generic.IDictionary{string,object})"/>
		/// checkbox helper because it does not render an extra hidden field.
		/// </para>
		/// <para>
		/// Because this does not render an extra hidden field, trying to bind the value posted back by this
		/// field to <see cref="bool"/> will fail when the checkbox is unticked. This is because when a checkbox
		/// is unticked, it does not post back any value (as opposed to posting back false). This means that
		/// the <see cref="DefaultModelBinder"/> will try to bind null (no value posted back) to a bool and fail.
		/// To solve this issue, you should use the <see cref="BoolBinder"/> when using checkboxes created by this
		/// method.
		/// </para>
		/// </remarks>
		/// <param name="html">The <see cref="HtmlHelper"/></param>
		/// <param name="name">The form field name</param>
		/// <returns>The HTML</returns>
		public static MvcHtmlString CheckboxStandard(this HtmlHelper html, string name)
		{
			return CheckboxStandard(html, name, null, null);
		}


		/// <summary>
		/// Renders a normal HTML form checkbox.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This is different to the normal 
		/// <see cref="InputExtensions.CheckBox(System.Web.Mvc.HtmlHelper,string,System.Collections.Generic.IDictionary{string,object})"/>
		/// checkbox helper because it does not render an extra hidden field.
		/// </para>
		/// <para>
		/// Because this does not render an extra hidden field, trying to bind the value posted back by this
		/// field to <see cref="bool"/> will fail when the checkbox is unticked. This is because when a checkbox
		/// is unticked, it does not post back any value (as opposed to posting back false). This means that
		/// the <see cref="DefaultModelBinder"/> will try to bind null (no value posted back) to a bool and fail.
		/// To solve this issue, you should use the <see cref="BoolBinder"/> when using checkboxes created by this
		/// method.
		/// </para>
		/// </remarks>
		/// <param name="html">The <see cref="HtmlHelper"/></param>
		/// <param name="name">The form field name</param>
		/// <param name="isChecked">Whether or not the box should be checked</param>
		/// <returns>The HTML</returns>
		public static MvcHtmlString CheckboxStandard(this HtmlHelper html, string name, bool isChecked)
		{
			return CheckboxStandard(html, name, isChecked, null);
		}


		/// <summary>
		/// Renders a normal HTML form checkbox.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This is different to the normal 
		/// <see cref="InputExtensions.CheckBox(System.Web.Mvc.HtmlHelper,string,System.Collections.Generic.IDictionary{string,object})"/>
		/// checkbox helper because it does not render an extra hidden field.
		/// </para>
		/// <para>
		/// Because this does not render an extra hidden field, trying to bind the value posted back by this
		/// field to <see cref="bool"/> will fail when the checkbox is unticked. This is because when a checkbox
		/// is unticked, it does not post back any value (as opposed to posting back false). This means that
		/// the <see cref="DefaultModelBinder"/> will try to bind null (no value posted back) to a bool and fail.
		/// To solve this issue, you should use the <see cref="BoolBinder"/> when using checkboxes created by this
		/// method.
		/// </para>
		/// </remarks>
		/// <param name="html">The <see cref="HtmlHelper"/></param>
		/// <param name="name">The form field name</param>
		/// <param name="htmlAttributes">
		/// An object containing the HTML attributes for the element. The attributes are retrieved via
		/// reflection by examining the properties of the object. Typically created using object initializer
		/// syntax.
		/// </param>
		/// <returns>The HTML</returns>
		public static MvcHtmlString CheckboxStandard(this HtmlHelper html, string name, object htmlAttributes)
		{
			return CheckboxStandard(html, name, null, new RouteValueDictionary(htmlAttributes)); //Uses RouteValueDictionary just for its ability to reflect the properties off an object
		}


		/// <summary>
		/// Renders a normal HTML form checkbox.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This is different to the normal 
		/// <see cref="InputExtensions.CheckBox(System.Web.Mvc.HtmlHelper,string,System.Collections.Generic.IDictionary{string,object})"/>
		/// checkbox helper because it does not render an extra hidden field.
		/// </para>
		/// <para>
		/// Because this does not render an extra hidden field, trying to bind the value posted back by this
		/// field to <see cref="bool"/> will fail when the checkbox is unticked. This is because when a checkbox
		/// is unticked, it does not post back any value (as opposed to posting back false). This means that
		/// the <see cref="DefaultModelBinder"/> will try to bind null (no value posted back) to a bool and fail.
		/// To solve this issue, you should use the <see cref="BoolBinder"/> when using checkboxes created by this
		/// method.
		/// </para>
		/// </remarks>
		/// <param name="html">The <see cref="HtmlHelper"/></param>
		/// <param name="name">The form field name</param>
		/// <param name="isChecked">
		/// Whether or not the box should be checked, or null if the ViewData should be looked in to determine
		/// this
		/// </param>
		/// <param name="htmlAttributes">An object containing the HTML attributes for the element.</param>
		/// <returns>The HTML</returns>
		public static MvcHtmlString CheckboxStandard(this HtmlHelper html, string name, bool? isChecked, IDictionary<string, object> htmlAttributes)
		{
			TagBuilder checkbox = new TagBuilder("input");
			checkbox.MergeAttributes(htmlAttributes);
			checkbox.Attributes["type"] = "checkbox";
			checkbox.Attributes["name"] = name;
			checkbox.Attributes["id"] = name;
			checkbox.Attributes["value"] = "true";

			bool checkedState;

			if (isChecked != null)
				checkedState = isChecked.Value;
			else if (html.ViewData.ContainsKey(name) == false)
				checkedState = false;
			else
			{
				try
				{
					checkedState = Boolean.Parse(html.ViewData[name].ToString());
				}
				catch (FormatException)
				{
					checkedState = false;
				}
			}

			if (checkedState)
				checkbox.Attributes["checked"] = "checked";

			return MvcHtmlString.Create(checkbox.ToString(TagRenderMode.SelfClosing));
		}


		/// <summary>
		/// Renders a normal HTML form checkbox.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This is different to the normal 
		/// <see cref="InputExtensions.CheckBoxFor{TModel}(System.Web.Mvc.HtmlHelper{TModel},System.Linq.Expressions.Expression{System.Func{TModel,bool}})"/>
		/// checkbox helper because it does not render an extra hidden field.
		/// </para>
		/// <para>
		/// Because this does not render an extra hidden field, trying to bind the value posted back by this
		/// field to <see cref="bool"/> will fail when the checkbox is unticked. This is because when a checkbox
		/// is unticked, it does not post back any value (as opposed to posting back false). This means that
		/// the <see cref="DefaultModelBinder"/> will try to bind null (no value posted back) to a bool and fail.
		/// To solve this issue, you should use the <see cref="BoolBinder"/> when using checkboxes created by this
		/// method.
		/// </para>
		/// </remarks>
		/// <param name="html">The <see cref="HtmlHelper"/></param>
		/// <param name="expression">An expression that identifies the model property the checkbox is for</param>
		/// <returns>The HTML</returns>
		public static MvcHtmlString CheckboxStandardFor<TModel>(this HtmlHelper<TModel> html, Expression<Func<TModel, bool>> expression)
		{
			return html.CheckboxStandardFor(expression, null);
		}


		/// <summary>
		/// Renders a normal HTML form checkbox.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This is different to the normal 
		/// <see cref="InputExtensions.CheckBoxFor{TModel}(System.Web.Mvc.HtmlHelper{TModel},System.Linq.Expressions.Expression{System.Func{TModel,bool}})"/>
		/// checkbox helper because it does not render an extra hidden field.
		/// </para>
		/// <para>
		/// Because this does not render an extra hidden field, trying to bind the value posted back by this
		/// field to <see cref="bool"/> will fail when the checkbox is unticked. This is because when a checkbox
		/// is unticked, it does not post back any value (as opposed to posting back false). This means that
		/// the <see cref="DefaultModelBinder"/> will try to bind null (no value posted back) to a bool and fail.
		/// To solve this issue, you should use the <see cref="BoolBinder"/> when using checkboxes created by this
		/// method.
		/// </para>
		/// </remarks>
		/// <param name="html">The <see cref="HtmlHelper"/></param>
		/// <param name="expression">An expression that identifies the model property the checkbox is for</param>
		/// <param name="htmlAttributes">An object containing the HTML attributes for the element.</param>
		/// <returns>The HTML</returns>
		public static MvcHtmlString CheckboxStandardFor<TModel>(this HtmlHelper<TModel> html, Expression<Func<TModel, bool>> expression, object htmlAttributes)
		{
			return html.CheckboxStandardFor(expression, new RouteValueDictionary(htmlAttributes));
		}


		/// <summary>
		/// Renders a normal HTML form checkbox.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This is different to the normal 
		/// <see cref="InputExtensions.CheckBoxFor{TModel}(System.Web.Mvc.HtmlHelper{TModel},System.Linq.Expressions.Expression{System.Func{TModel,bool}})"/>
		/// checkbox helper because it does not render an extra hidden field.
		/// </para>
		/// <para>
		/// Because this does not render an extra hidden field, trying to bind the value posted back by this
		/// field to <see cref="bool"/> will fail when the checkbox is unticked. This is because when a checkbox
		/// is unticked, it does not post back any value (as opposed to posting back false). This means that
		/// the <see cref="DefaultModelBinder"/> will try to bind null (no value posted back) to a bool and fail.
		/// To solve this issue, you should use the <see cref="BoolBinder"/> when using checkboxes created by this
		/// method.
		/// </para>
		/// </remarks>
		/// <param name="html">The <see cref="HtmlHelper"/></param>
		/// <param name="expression">An expression that identifies the model property the checkbox is for</param>
		/// <param name="htmlAttributes">An object containing the HTML attributes for the element.</param>
		/// <returns>The HTML</returns>
		public static MvcHtmlString CheckboxStandardFor<TModel>(this HtmlHelper<TModel> html, Expression<Func<TModel, bool>> expression, IDictionary<string, object> htmlAttributes)
		{
			if (expression == null)
				throw new ArgumentNullException("expression");

			ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
			bool? isChecked = null;
			bool outBool;
			if ((metadata.Model != null) && bool.TryParse(metadata.Model.ToString(), out outBool))
				isChecked = outBool;

			return html.CheckboxStandard(ExpressionHelper.GetExpressionText(expression), isChecked, htmlAttributes);
		}


		/// <summary>
		/// Renders a dropdown from an <see cref="IEnumerable{T}"/> of any type, using selector functions to extract
		/// the <see cref="SelectListItem.Text"/> and <see cref="SelectListItem.Value"/> values from the <typeparamref name="TItem"/>.
		/// </summary>
		/// <typeparam name="TItem">The type of item</typeparam>
		/// <param name="html">The <see cref="HtmlHelper"/></param>
		/// <param name="name">The name of the dropdown to render</param>
		/// <param name="blankFirst">Whether or not to include a blank dropdown item as the first item</param>
		/// <param name="items">The items to put in the dropdown</param>
		/// <param name="textSelector">
		/// A function that returns the <see cref="SelectListItem.Text"/> for a dropdown item
		/// </param>
		/// <param name="valueSelector">
		/// A function that returns the <see cref="SelectListItem.Value"/> for a dropdown item
		/// </param>
		/// <returns>The dropdown HTML</returns>
		public static MvcHtmlString DropDownList<TItem>(this HtmlHelper html, string name, bool blankFirst, IEnumerable<TItem> items, Func<TItem, object> textSelector, Func<TItem, object> valueSelector)
		{
			return html.DropDownList(name, blankFirst, items, textSelector, valueSelector, null, null);
		}


		/// <summary>
		/// Renders a dropdown from an <see cref="IEnumerable{T}"/> of any type, using selector functions to extract
		/// the <see cref="SelectListItem.Text"/> and <see cref="SelectListItem.Value"/> values from the <typeparamref name="TItem"/>.
		/// </summary>
		/// <typeparam name="TItem">The type of item</typeparam>
		/// <param name="html">The <see cref="HtmlHelper"/></param>
		/// <param name="name">The name of the dropdown to render</param>
		/// <param name="blankFirst">Whether or not to include a blank dropdown item as the first item</param>
		/// <param name="items">The items to put in the dropdown</param>
		/// <param name="textSelector">
		/// A function that returns the <see cref="SelectListItem.Text"/> for a dropdown item
		/// </param>
		/// <param name="valueSelector">
		/// A function that returns the <see cref="SelectListItem.Value"/> for a dropdown item
		/// </param>
		/// <param name="selectedValue">
		/// The <see cref="SelectListItem.Value"/> of the item that should be marked as selected (or null)
		/// </param>
		/// <returns>The dropdown HTML</returns>
		public static MvcHtmlString DropDownList<TItem>(this HtmlHelper html, string name, bool blankFirst, IEnumerable<TItem> items, Func<TItem, object> textSelector, Func<TItem, object> valueSelector, string selectedValue)
		{
			return html.DropDownList(name, blankFirst, items, textSelector, valueSelector, selectedValue, null);
		}


		/// <summary>
		/// Renders a dropdown from an <see cref="IEnumerable{T}"/> of any type, using selector functions to extract
		/// the <see cref="SelectListItem.Text"/> and <see cref="SelectListItem.Value"/> values from the <typeparamref name="TItem"/>.
		/// </summary>
		/// <typeparam name="TItem">The type of item</typeparam>
		/// <param name="html">The <see cref="HtmlHelper"/></param>
		/// <param name="name">The name of the dropdown to render</param>
		/// <param name="blankFirst">Whether or not to include a blank dropdown item as the first item</param>
		/// <param name="items">The items to put in the dropdown</param>
		/// <param name="textSelector">
		/// A function that returns the <see cref="SelectListItem.Text"/> for a dropdown item
		/// </param>
		/// <param name="valueSelector">
		/// A function that returns the <see cref="SelectListItem.Value"/> for a dropdown item
		/// </param>
		/// <param name="selectedValue">
		/// The <see cref="SelectListItem.Value"/> of the item that should be marked as selected (or null)
		/// </param>
		/// <param name="htmlAttributes">An object containing the HTML attributes for the element.</param>
		/// <returns>The dropdown HTML</returns>
		public static MvcHtmlString DropDownList<TItem>(this HtmlHelper html, string name, bool blankFirst, IEnumerable<TItem> items, Func<TItem, object> textSelector, Func<TItem, object> valueSelector, string selectedValue, object htmlAttributes)
		{
			return html.DropDownList(name, blankFirst, items, textSelector, valueSelector, selectedValue, new RouteValueDictionary(htmlAttributes));
		}


		/// <summary>
		/// Renders a dropdown from an <see cref="IEnumerable{T}"/> of any type, using selector functions to extract
		/// the <see cref="SelectListItem.Text"/> and <see cref="SelectListItem.Value"/> values from the <typeparamref name="TItem"/>.
		/// </summary>
		/// <typeparam name="TItem">The type of item</typeparam>
		/// <param name="html">The <see cref="HtmlHelper"/></param>
		/// <param name="name">The name of the dropdown to render</param>
		/// <param name="blankFirst">Whether or not to include a blank dropdown item as the first item</param>
		/// <param name="items">The items to put in the dropdown</param>
		/// <param name="textSelector">
		/// A function that returns the <see cref="SelectListItem.Text"/> for a dropdown item
		/// </param>
		/// <param name="valueSelector">
		/// A function that returns the <see cref="SelectListItem.Value"/> for a dropdown item
		/// </param>
		/// <param name="selectedValue">
		/// The <see cref="SelectListItem.Value"/> of the item that should be marked as selected (or null)
		/// </param>
		/// <param name="htmlAttributes">An object containing the HTML attributes for the element.</param>
		/// <returns>The dropdown HTML</returns>
		public static MvcHtmlString DropDownList<TItem>(this HtmlHelper html, string name, bool blankFirst, IEnumerable<TItem> items, Func<TItem, object> textSelector, Func<TItem, object> valueSelector, string selectedValue, IDictionary<string, object> htmlAttributes)
		{
			IEnumerable<SelectListItem> selectListItems = items.Select(p => new SelectListItem
			{
				Text = textSelector(p).ToString(),
				Value = valueSelector(p).ToString(),
				Selected = selectedValue != null ? selectedValue == valueSelector(p).ToString() : false,
			});

			if (blankFirst)
			{
				SelectListItem[] blankFirstArray = new[] { new SelectListItem() };
				selectListItems = blankFirstArray.Concat(selectListItems);
			}

			return html.DropDownList(name, selectListItems, htmlAttributes);
		}


		/// <summary>
		/// Renders a hidden form field that contains a value encrypted with AES. The key for the encryption must
		/// be set in your web.config with the <see cref="CryptoConfigurationSection"/>.
		/// </summary>
		/// <param name="html">The <see cref="HtmlHelper"/></param>
		/// <param name="name">The name of the field</param>
		/// <param name="value">The value to encrypt</param>
		/// <returns>The HTML</returns>
		public static MvcHtmlString EncryptedHidden(this HtmlHelper html, string name, string value)
		{
			return EncryptedHidden(html, name, value, null);
		}


		/// <summary>
		/// Renders a hidden form field that contains a value encrypted with AES. The key for the encryption must
		/// be set in your web.config with the <see cref="CryptoConfigurationSection"/>.
		/// </summary>
		/// <param name="html">The <see cref="HtmlHelper"/></param>
		/// <param name="name">The name of the field</param>
		/// <param name="value">The value to encrypt</param>
		/// <param name="htmlAttributes">An object containing the HTML attributes for the element.</param>
		/// <returns>The HTML</returns>
		public static MvcHtmlString EncryptedHidden(this HtmlHelper html, string name, string value, object htmlAttributes)
		{
			return EncryptedHidden(html, name, value, new RouteValueDictionary(htmlAttributes));
		}


		/// <summary>
		/// Renders a hidden form field that contains a value encrypted with AES. The key for the encryption must
		/// be set in your web.config with the <see cref="CryptoConfigurationSection"/>.
		/// </summary>
		/// <param name="html">The <see cref="HtmlHelper"/></param>
		/// <param name="name">The name of the field</param>
		/// <param name="value">The value to encrypt</param>
		/// <param name="htmlAttributes">An object containing the HTML attributes for the element.</param>
		/// <returns>The HTML</returns>
		public static MvcHtmlString EncryptedHidden(this HtmlHelper html, string name, string value, IDictionary<string, object> htmlAttributes)
		{
			string encryptedValue;
			using (ICrypto crypto = new AesCrypto())
			{
				encryptedValue = crypto.EncryptToBase64(value);
			}

			return html.Hidden(name, encryptedValue, htmlAttributes);
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using DigitallyCreated.Utilities.Bcl.Linq;


namespace DigitallyCreated.Utilities.Mvc
{
	/// <summary>
	/// This helper class provides extensions to <see cref="HtmlHelper"/> when using strongly typed views
	/// of type <see cref="PagedSortedViewModel{T}"/>.
	/// </summary>
	public static class PagedSortedHtmlHelpers
	{
		#region SortableColumnHeader Methods


		/// <summary>
		/// Creates a column header cell (<c>td</c>) that is sortable (the user can click on it to sort
		/// ascending/descending). The current page's route and URL parameters will be used for routing
		/// the sort link.
		/// </summary>
		/// <typeparam name="T">The type being displayed in the view</typeparam>
		/// <typeparam name="TProp">The type of the property whose column header is being created</typeparam>
		/// <param name="html">The <see cref="HtmlHelper{T}"/></param>
		/// <param name="propertySelector">
		/// A lambda expression that returns one of the properties on the object
		/// </param>
		/// <param name="text">The text to display in the column header cell</param>
		/// <returns>The HTML</returns>
		public static MvcHtmlString SortableColumnHeader<T, TProp>(this HtmlHelper<PagedSortedViewModel<T>> html, Expression<Func<T, TProp>> propertySelector, string text)
		{
			return html.SortableColumnHeader(propertySelector, text, null, null, null);
		}



		/// <summary>
		/// Creates a column header cell (<c>td</c>) that is sortable (the user can click on it to sort
		/// ascending/descending). The current page's route and URL parameters will be used for routing
		/// the sort link.
		/// </summary>
		/// <typeparam name="T">The type being displayed in the view</typeparam>
		/// <typeparam name="TAuxModel">The type of the auxiliary model</typeparam>
		/// <typeparam name="TProp">The type of the property whose column header is being created</typeparam>
		/// <param name="html">The <see cref="HtmlHelper{T}"/></param>
		/// <param name="propertySelector">
		/// A lambda expression that returns one of the properties on the object
		/// </param>
		/// <param name="text">The text to display in the column header cell</param>
		/// <returns>The HTML</returns>
		public static MvcHtmlString SortableColumnHeader<T, TAuxModel, TProp>(this HtmlHelper<PagedSortedViewModel<T, TAuxModel>> html, Expression<Func<T, TProp>> propertySelector, string text)
		{
			return html.SortableColumnHeader(propertySelector, text, null, null, null);
		}


		/// <summary>
		/// Creates a column header cell (<c>td</c>) that is sortable (the user can click on it to sort
		/// ascending/descending). The current page's route and URL parameters will be used for routing
		/// the sort link.
		/// </summary>
		/// <typeparam name="T">The type being displayed in the view</typeparam>
		/// <typeparam name="TProp">The type of the property whose column header is being created</typeparam>
		/// <param name="html">The <see cref="HtmlHelper{T}"/></param>
		/// <param name="propertySelector">
		/// A lambda expression that returns one of the properties on the object
		/// </param>
		/// <param name="text">The text to display in the column header cell</param>
		/// <param name="routeValues">
		/// An object whose properties are the routing values to use for routing the the sort link
		/// </param>
		/// <returns>The HTML</returns>
		public static MvcHtmlString SortableColumnHeader<T, TProp>(this HtmlHelper<PagedSortedViewModel<T>> html, Expression<Func<T, TProp>> propertySelector, string text, object routeValues)
		{
			return html.SortableColumnHeader(propertySelector, text, new RouteValueDictionary(routeValues), null, null);
		}


		/// <summary>
		/// Creates a column header cell (<c>td</c>) that is sortable (the user can click on it to sort
		/// ascending/descending). The current page's route and URL parameters will be used for routing
		/// the sort link.
		/// </summary>
		/// <typeparam name="T">The type being displayed in the view</typeparam>
		/// <typeparam name="TAuxModel">The type of the auxiliary model</typeparam>
		/// <typeparam name="TProp">The type of the property whose column header is being created</typeparam>
		/// <param name="html">The <see cref="HtmlHelper{T}"/></param>
		/// <param name="propertySelector">
		/// A lambda expression that returns one of the properties on the object
		/// </param>
		/// <param name="text">The text to display in the column header cell</param>
		/// <param name="routeValues">
		/// An object whose properties are the routing values to use for routing the the sort link
		/// </param>
		/// <returns>The HTML</returns>
		public static MvcHtmlString SortableColumnHeader<T, TAuxModel, TProp>(this HtmlHelper<PagedSortedViewModel<T, TAuxModel>> html, Expression<Func<T, TProp>> propertySelector, string text, object routeValues)
		{
			return html.SortableColumnHeader(propertySelector, text, new RouteValueDictionary(routeValues), null, null);
		}


		/// <summary>
		/// Creates a column header cell (<c>td</c>) that is sortable (the user can click on it to sort
		/// ascending/descending). The current page's route and URL parameters will be used for routing the
		/// sort link.
		/// </summary>
		/// <typeparam name="T">The type being displayed in the view</typeparam>
		/// <typeparam name="TProp">The type of the property whose column header is being created</typeparam>
		/// <param name="html">The <see cref="HtmlHelper{T}"/></param>
		/// <param name="propertySelector">
		/// A lambda expression that returns one of the properties on the object
		/// </param>
		/// <param name="text">The text to display in the column header cell</param>
		/// <param name="routeValues">
		/// An <see cref="RouteValueDictionary"/> which defines the routing values to use for routing the the
		/// sort link
		/// </param>
		/// <returns>The HTML</returns>
		public static MvcHtmlString SortableColumnHeader<T, TProp>(this HtmlHelper<PagedSortedViewModel<T>> html, Expression<Func<T, TProp>> propertySelector, string text, RouteValueDictionary routeValues)
		{
			return html.SortableColumnHeader(propertySelector, text, new RouteValueDictionary(routeValues), null, null);
		}


		/// <summary>
		/// Creates a column header cell (<c>td</c>) that is sortable (the user can click on it to sort
		/// ascending/descending). The current page's route and URL parameters will be used for routing the
		/// sort link.
		/// </summary>
		/// <typeparam name="T">The type being displayed in the view</typeparam>
		/// <typeparam name="TAuxModel">The type of the auxiliary model</typeparam>
		/// <typeparam name="TProp">The type of the property whose column header is being created</typeparam>
		/// <param name="html">The <see cref="HtmlHelper{T}"/></param>
		/// <param name="propertySelector">
		/// A lambda expression that returns one of the properties on the object
		/// </param>
		/// <param name="text">The text to display in the column header cell</param>
		/// <param name="routeValues">
		/// An <see cref="RouteValueDictionary"/> which defines the routing values to use for routing the the
		/// sort link
		/// </param>
		/// <returns>The HTML</returns>
		public static MvcHtmlString SortableColumnHeader<T, TAuxModel, TProp>(this HtmlHelper<PagedSortedViewModel<T, TAuxModel>> html, Expression<Func<T, TProp>> propertySelector, string text, RouteValueDictionary routeValues)
		{
			return html.SortableColumnHeader(propertySelector, text, new RouteValueDictionary(routeValues), null, null);
		}


		/// <summary>
		/// Creates a column header cell (<c>td</c>) that is sortable (the user can click on it to sort
		/// ascending/descending)
		/// </summary>
		/// <typeparam name="T">The type being displayed in the view</typeparam>
		/// <typeparam name="TProp">The type of the property whose column header is being created</typeparam>
		/// <param name="html">The <see cref="HtmlHelper{T}"/></param>
		/// <param name="propertySelector">
		/// A lambda expression that returns one of the properties on the object
		/// </param>
		/// <param name="text">The text to display in the column header cell</param>
		/// <param name="routeValues">
		/// The route values to include in the link that allows sorting. This can be null, which will cause the
		/// current page's route and URL parameters to be used
		/// </param>
		/// <param name="sortRouteValueName">
		/// The route value name used to store the sorting settings string in. This can be null which will
		/// cause the value <c>sort</c> to be used
		/// </param>
		/// <param name="htmlAttributes">Html attributes to apply to the <c>th</c> tag</param>
		/// <returns>The HTML</returns>
		public static MvcHtmlString SortableColumnHeader<T, TProp>(this HtmlHelper<PagedSortedViewModel<T>> html, Expression<Func<T, TProp>> propertySelector, string text, RouteValueDictionary routeValues, string sortRouteValueName, IDictionary<string, object> htmlAttributes)
		{
			return html.SortableColumnHeader(html.ViewData.Model, propertySelector, text, routeValues, sortRouteValueName, htmlAttributes);
		}


		/// <summary>
		/// Creates a column header cell (<c>td</c>) that is sortable (the user can click on it to sort
		/// ascending/descending)
		/// </summary>
		/// <typeparam name="T">The type being displayed in the view</typeparam>
		/// <typeparam name="TAuxModel">The type of the auxiliary model</typeparam>
		/// <typeparam name="TProp">The type of the property whose column header is being created</typeparam>
		/// <param name="html">The <see cref="HtmlHelper{T}"/></param>
		/// <param name="propertySelector">
		/// A lambda expression that returns one of the properties on the object
		/// </param>
		/// <param name="text">The text to display in the column header cell</param>
		/// <param name="routeValues">
		/// The route values to include in the link that allows sorting. This can be null, which will cause the
		/// current page's route and URL parameters to be used
		/// </param>
		/// <param name="sortRouteValueName">
		/// The route value name used to store the sorting settings string in. This can be null which will
		/// cause the value <c>sort</c> to be used
		/// </param>
		/// <param name="htmlAttributes">Html attributes to apply to the <c>th</c> tag</param>
		/// <returns>The HTML</returns>
		public static MvcHtmlString SortableColumnHeader<T, TAuxModel, TProp>(this HtmlHelper<PagedSortedViewModel<T, TAuxModel>> html, Expression<Func<T, TProp>> propertySelector, string text, RouteValueDictionary routeValues, string sortRouteValueName, IDictionary<string, object> htmlAttributes)
		{
			return html.SortableColumnHeader(html.ViewData.Model, propertySelector, text, routeValues, sortRouteValueName, htmlAttributes);
		}


		/// <summary>
		/// Creates a column header cell (<c>td</c>) that is sortable (the user can click on it to sort
		/// ascending/descending)
		/// </summary>
		/// <typeparam name="T">The type being displayed in the view</typeparam>
		/// <typeparam name="TProp">The type of the property whose column header is being created</typeparam>
		/// <param name="html">The <see cref="HtmlHelper{T}"/></param>
		/// <param name="viewData">
		/// The <see cref="PagedSortedViewModel{T}"/> view data from the <see cref="HtmlHelper{T}"/>
		/// </param>
		/// <param name="propertySelector">
		/// A lambda expression that returns one of the properties on the object
		/// </param>
		/// <param name="text">The text to display in the column header cell</param>
		/// <param name="routeValues">
		/// The route values to include in the link that allows sorting. This can be null, which will cause the
		/// current page's route and URL parameters to be used
		/// </param>
		/// <param name="sortRouteValueName">
		/// The route value name used to store the sorting settings string in. This can be null which will
		/// cause the value <c>sort</c> to be used
		/// </param>
		/// <param name="htmlAttributes">Html attributes to apply to the <c>th</c> tag</param>
		/// <returns>The HTML</returns>
		private static MvcHtmlString SortableColumnHeader<T, TProp>(this HtmlHelper html, PagedSortedViewModel<T> viewData, Expression<Func<T, TProp>> propertySelector, string text, RouteValueDictionary routeValues, string sortRouteValueName, IDictionary<string, object> htmlAttributes)
		{
			UrlHelper url = new UrlHelper(html.ViewContext.RequestContext);
			sortRouteValueName = sortRouteValueName ?? "sort";
			routeValues = routeValues ?? url.RouteToCurrentPage();
			htmlAttributes = htmlAttributes ?? new Dictionary<string, object>();

			Sorter<T> sorter = viewData.Sorter;

			bool sortDirection;
			KeyValuePair<LambdaExpression, bool> selectorPair = sorter.OrderedPropertySelectors[0];
			bool isFirstSort = sorter.PropertySelectorsEqual(propertySelector, selectorPair.Key);
			if (isFirstSort)
				sortDirection = !selectorPair.Value;
			else
				sortDirection = true;

			bool currentSortDirection = sorter.GetSortDirectionForProperty(propertySelector);

			RouteValueDictionary routeDictionary = new RouteValueDictionary(routeValues);
			routeDictionary[sortRouteValueName] = sorter.Clone().SortBy(propertySelector, sortDirection).Encode();

			TagBuilder th = new TagBuilder("th");
			TagBuilder a = new TagBuilder("a");

			string linkTitle = String.Format("{0} (Sorted {1})", text, (currentSortDirection ? "Ascending" : "Descending"));
			a.MergeAttribute("title", linkTitle);
			a.MergeAttribute("href", url.RouteUrl(routeDictionary));
			
			th.AddCssClass("Sortable");
			if (isFirstSort) 
				th.AddCssClass("PrimarySortColumn");
			else
				th.AddCssClass("NonPrimarySortColumn");
			if (currentSortDirection) 
				th.AddCssClass("SortedAscending");
			else
				th.AddCssClass("SortedDescending");

			a.InnerHtml = text;
			th.InnerHtml = a.ToString();
			th.MergeAttributes(htmlAttributes);

			return MvcHtmlString.Create(th.ToString());
		}


		#endregion


		#region PagingControls Methods


		/// <summary>
		/// Creates a control that allows moving to the next page, the previous page and shows the current page
		/// and the total number of pages.
		/// </summary>
		/// <typeparam name="T">The type being displayed in the view</typeparam>
		/// <param name="html">The <see cref="HtmlHelper{T}"/></param>
		/// <returns>The HTML</returns>
		public static MvcHtmlString PagingControls<T>(this HtmlHelper<PagedSortedViewModel<T>> html)
		{
			return html.PagingControls(null);
		}


		/// <summary>
		/// Creates a control that allows moving to the next page, the previous page and shows the current page
		/// and the total number of pages.
		/// </summary>
		/// <typeparam name="T">The type being displayed in the view</typeparam>
		/// <typeparam name="TAuxModel">The type of the auxiliary model</typeparam>
		/// <param name="html">The <see cref="HtmlHelper{T}"/></param>
		/// <returns>The HTML</returns>
		public static MvcHtmlString PagingControls<T, TAuxModel>(this HtmlHelper<PagedSortedViewModel<T, TAuxModel>> html)
		{
			return html.PagingControls(null);
		}


		/// <summary>
		/// Creates a control that allows moving to the next page, the previous page and shows the current page
		/// and the total number of pages.
		/// </summary>
		/// <typeparam name="T">The type being displayed in the view</typeparam>
		/// <param name="html">The <see cref="HtmlHelper{T}"/></param>
		/// <param name="pageRouteValueName">
		/// The name of the route value to use to save the page number for the page change links. This can be
		/// left null and the value <c>page</c> will be used.
		/// </param>
		/// <returns>The HTML</returns>
		public static MvcHtmlString PagingControls<T>(this HtmlHelper<PagedSortedViewModel<T>> html, string pageRouteValueName)
		{
			return html.PagingControls(html.ViewData.Model, pageRouteValueName);
		}


		/// <summary>
		/// Creates a control that allows moving to the next page, the previous page and shows the current page
		/// and the total number of pages.
		/// </summary>
		/// <typeparam name="T">The type being displayed in the view</typeparam>
		/// <typeparam name="TAuxModel">The type of the auxiliary model</typeparam>
		/// <param name="html">The <see cref="HtmlHelper{T}"/></param>
		/// <param name="pageRouteValueName">
		/// The name of the route value to use to save the page number for the page change links. This can be
		/// left null and the value <c>page</c> will be used.
		/// </param>
		/// <returns>The HTML</returns>
		public static MvcHtmlString PagingControls<T, TAuxModel>(this HtmlHelper<PagedSortedViewModel<T, TAuxModel>> html, string pageRouteValueName)
		{
			return html.PagingControls(html.ViewData.Model, pageRouteValueName);
		}


		/// <summary>
		/// Creates a control that allows moving to the next page, the previous page and shows the current page
		/// and the total number of pages.
		/// </summary>
		/// <typeparam name="T">The type being displayed in the view</typeparam>
		/// <param name="html">The <see cref="HtmlHelper{T}"/></param>
		/// <param name="viewData">
		/// The <see cref="PagedSortedViewModel{T}"/> view data from the <see cref="HtmlHelper{T}"/>
		/// </param>
		/// <param name="pageRouteValueName">
		/// The name of the route value to use to save the page number for the page change links. This can be
		/// left null and the value <c>page</c> will be used.
		/// </param>
		/// <returns>The HTML</returns>
		private static MvcHtmlString PagingControls<T>(this HtmlHelper html, PagedSortedViewModel<T> viewData, string pageRouteValueName)
		{
			UrlHelper url = new UrlHelper(html.ViewContext.RequestContext);
			pageRouteValueName = pageRouteValueName ?? "page";

			TagBuilder p = new TagBuilder("p");
			StringBuilder builder = new StringBuilder();
			bool showPrevPage = PagedDataHelper.PreviousPageExists(viewData.Page, viewData.TotalPages);
			bool showNextPage = PagedDataHelper.NextPageExists(viewData.Page, viewData.TotalPages);

			if (showPrevPage)
			{
				RouteValueDictionary previousRoute = url.RouteToCurrentPage();
				previousRoute[pageRouteValueName] = viewData.Page - 1;
				builder.Append(html.RouteLink("<< Previous", previousRoute));
			}

			builder.Append(" Page ");
			builder.Append(viewData.Page);
			builder.Append(" of ");
			builder.Append(viewData.TotalPages);
			builder.Append(" ");

			if (showNextPage)
			{
				RouteValueDictionary nextRoute = url.RouteToCurrentPage();
				nextRoute[pageRouteValueName] = viewData.Page + 1;
				builder.Append(html.RouteLink("Next >>", nextRoute));
			}

			p.InnerHtml = builder.ToString();

			return MvcHtmlString.Create(p.ToString());
		}


		#endregion
	}
}
using System;
using System.Linq;


namespace DigitallyCreated.Utilities.Bcl.Linq
{
	/// <summary>
	/// This helper class helps you work with data in a paged manner
	/// </summary>
	public static class PagedDataHelper
	{
		/// <summary>
		/// Gets the total number of pages
		/// </summary>
		/// <param name="totalItems">The total number of items</param>
		/// <param name="pageSize">The page size</param>
		/// <returns>The total number of pages (minimum 1)</returns>
		public static int GetTotalPages(int totalItems, int pageSize)
		{
			if (totalItems == 0)
				return 1;

			int totalPages = totalItems / pageSize;
			if (totalItems % pageSize != 0) totalPages++;
			return totalPages;
		}

		
		/// <summary>
		/// Validates a page number and returns a valid page number
		/// </summary>
		/// <remarks>
		/// This method ensures that the page number is in the correct range  (1 - 
		/// <paramref name="totalPages"/>)
		/// </remarks>
		/// <param name="page">The page number</param>
		/// <param name="totalPages">The total number of pages</param>
		/// <returns>The valid page number (minimum 1)</returns>
		public static int ValidatePage(int page, int totalPages)
		{
			if (page < 1)
				return 1;
			if (page > totalPages)
				return 1;

			return page;
		}


		/// <summary>
		/// Validates a page number and returns a valid page number
		/// </summary>
		/// <remarks>
		/// This method ensures that the page number is in the correct range  (1 - 
		/// <paramref name="totalPages"/>), and null page numbers are changed to page 1.
		/// </remarks>
		/// <param name="page">The page number</param>
		/// <param name="totalPages">The total number of pages</param>
		/// <returns>The valid page number (minimum 1)</returns>
		public static int ValidatePage(int? page, int totalPages)
		{
			if (page == null)
				return 1;

			return ValidatePage(page.Value, totalPages);
		}


		/// <summary>
		/// Validates a page number and returns a valid page number
		/// </summary>
		/// <remarks>
		/// This method ensures that the page number is in the correct range (1 - 
		/// <paramref name="totalPages"/>), non-numeric strings are and and null strings are changed to page 1.
		/// </remarks>
		/// <param name="page">The page number</param>
		/// <param name="totalPages">The total number of pages</param>
		/// <returns>The valid page number (minimum 1)</returns>
		public static int ValidatePage(string page, int totalPages)
		{
			if (page == null)
				return 1;

			int pageInt;
			if (Int32.TryParse(page, out pageInt) == false)
				return 1;

			return ValidatePage(pageInt, totalPages);
		}


		/// <summary>
		/// Returns the page of data specified by skipping and taking
		/// </summary>
		/// <typeparam name="T">The type being queried</typeparam>
		/// <param name="queryable">The <see cref="IQueryable{T}"/></param>
		/// <param name="page">The page number</param>
		/// <param name="pageSize">The page size</param>
		/// <returns>The <see cref="IQueryable{T}"/>, now paged</returns>
		public static IQueryable<T> Page<T>(this IQueryable<T> queryable, int page, int pageSize)
		{
			int skip = (page - 1) * pageSize;

			return queryable.Skip(skip).Take(pageSize);
		}


		/// <summary>
		/// Returns the skip and take values for the specified page number
		/// </summary>
		/// <param name="page">The page number</param>
		/// <param name="pageSize">The page size</param>
		/// <param name="skip">The skip value</param>
		/// <param name="take">The take value</param>
		public static void GetSkipTakeValues(int page, int pageSize, out int skip, out int take)
		{
			skip = (page - 1) * pageSize;
			take = pageSize;
		}


		/// <summary>
		/// Tells you whether there is a next page or not
		/// </summary>
		/// <param name="page">The page number</param>
		/// <param name="totalPages">The total number of pages</param>
		/// <returns>True if there is a next page, false otherwise</returns>
		public static bool NextPageExists(int page, int totalPages)
		{
			return page < totalPages;
		}


		/// <summary>
		/// Tells you whether there is a previous page or not
		/// </summary>
		/// <param name="page">The page number</param>
		/// <param name="totalPages">The total number of pages</param>
		/// <returns>True if there is a previous page, false otherwise</returns>
		public static bool PreviousPageExists(int page, int totalPages)
		{
			return page != 1;
		}
	}
}
using System.Collections;
using System.Collections.Generic;
using DigitallyCreated.Utilities.Bcl.Linq;


namespace DigitallyCreated.Utilities.Mvc
{
	/// <summary>
	/// View model that supports the <see cref="DigitallyCreated.Utilities.Bcl.Linq.Sorter{T}"/> class and
	/// paging of data
	/// </summary>
	/// <typeparam name="T">The type being viewed</typeparam>
	public class PagedSortedViewModel<T> : IEnumerable<T>
	{
		/// <summary>
		/// The model behind this model. This contains only the current
		/// page of data, sorted by <see cref="Sorter"/>.
		/// </summary>
		public IEnumerable<T> Model { get; private set; }

		/// <summary>
		/// The sorter that sorted the <see cref="Model"/>
		/// </summary>
		public Sorter<T> Sorter { get; private set; }

		/// <summary>
		/// The current page
		/// </summary>
		public int Page { get; private set; }

		/// <summary>
		/// The total number of pages of data
		/// </summary>
		public int TotalPages { get; private set; }


		/// <summary>
		/// Constructor, creates a <see cref="PagedSortedViewModel{T}"/>
		/// </summary>
		/// <param name="model">
		/// The model behind this model. This contains only the current page of data, sorted by 
		/// <see cref="Sorter"/>.
		/// </param>
		/// <param name="sorter">The sorter that sorted the <see cref="Model"/></param>
		/// <param name="page">The current page</param>
		/// <param name="totalPages">The total number of pages of data</param>
		public PagedSortedViewModel(IEnumerable<T> model, Sorter<T> sorter, int page, int totalPages)
		{
			Model = model;
			Sorter = sorter;
			TotalPages = totalPages;
			Page = page;
			TotalPages = totalPages;
		}


		/// <summary>
		/// Gets an <see cref="IEnumerator{T}"/> that enumerates over <see cref="Model"/>
		/// </summary>
		/// <returns>Enumerator over <see cref="Model"/></returns>
		public IEnumerator<T> GetEnumerator()
		{
			return Model.GetEnumerator();
		}


		/// <summary>
		/// Gets an <see cref="IEnumerator"/> that enumerates over <see cref="Model"/>
		/// </summary>
		/// <returns>Enumerator over <see cref="Model"/></returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}


	/// <summary>
	/// View model that supports the <see cref="Sorter{T}"/> class and
	/// paging of data, plus an additional set of strongly typed data
	/// </summary>
	/// <typeparam name="T">The type being viewed</typeparam>
	/// <typeparam name="TAuxModel">The type of the auxiliary data</typeparam>
	public class PagedSortedViewModel<T, TAuxModel> : PagedSortedViewModel<T>
	{
		/// <summary>
		/// The additional strongly-typed data
		/// </summary>
		public TAuxModel AuxModel { get; private set; }


		/// <summary>
		/// Constructor, creates a <see cref="PagedSortedViewModel{T, TExtra}"/>
		/// </summary>
		/// <param name="model">
		/// The model behind this model. This contains only the current page of data, sorted by 
		/// <see cref="PagedSortedViewModel{T}.Sorter"/>.
		/// </param>
		/// <param name="sorter">The sorter that sorted the <see cref="PagedSortedViewModel{T}.Model"/></param>
		/// <param name="page">The current page</param>
		/// <param name="totalPages">The total number of pages of data</param>
		/// <param name="auxModel">The auxiliary data</param>
		public PagedSortedViewModel(IEnumerable<T> model, Sorter<T> sorter, int page, int totalPages, TAuxModel auxModel) 
			: base(model, sorter, page, totalPages)
		{
			AuxModel = auxModel;
		}
	}
}
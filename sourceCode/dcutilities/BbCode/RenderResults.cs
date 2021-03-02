namespace DigitallyCreated.Utilities.BbCode
{
	/// <summary>
	/// Contains the results of a BBCode render.
	/// </summary>
	public struct RenderResults
	{
		private readonly string _RenderedString;
		private readonly bool _IsCacheable;


		/// <summary>
		/// The rendered XHTML string
		/// </summary>
		public string RenderedString { get { return _RenderedString; } }

		/// <summary>
		/// Whether or not the <see cref="RenderedString"/> can be cached. You must never cache
		/// if this returns <see langword="false"/>.
		/// </summary>
		public bool IsCacheable { get { return _IsCacheable; } }


		/// <summary>
		/// Constructor, creates a <see cref="RenderResults"/>
		/// </summary>
		/// <param name="renderedString">The rendered XHTML string</param>
		/// <param name="isCacheable">Whether or not the <paramref name="renderedString"/> is cacheable</param>
		public RenderResults(string renderedString, bool isCacheable) 
			: this()
		{
			_RenderedString = renderedString;
			_IsCacheable = isCacheable;
		}
	}
}
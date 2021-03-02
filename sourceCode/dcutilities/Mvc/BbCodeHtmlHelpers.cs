using System.Web.Mvc;
using DigitallyCreated.Utilities.BbCode;

namespace DigitallyCreated.Utilities.Mvc
{
	/// <summary>
	/// Contains HTML helpers that render BBCode using the BBCode library
	/// </summary>
	public static class BbCodeHtmlHelpers
	{
		/// <summary>
		/// Renders the specified BBCode string using the default set of tag definitions defined in the web.config
		/// </summary>
		/// <param name="html">The <see cref="HtmlHelper"/></param>
		/// <param name="bbCode">The BBCode string</param>
		/// <returns>The rendered XHTML</returns>
		public static MvcHtmlString BbCode(this HtmlHelper html, string bbCode)
		{
			return html.BbCode(bbCode, null);
		}


		/// <summary>
		/// Renders the specified BBCode string using the default set of tag definitions defined in the web.config
		/// </summary>
		/// <param name="html">The <see cref="HtmlHelper"/></param>
		/// <param name="bbCode">The BBCode string</param>
		/// <param name="cachedRender">
		/// The cached render of <paramref name="bbCode"/>. This will be returned by this method if it is not null
		/// </param>
		/// <returns>The rendered XHTML</returns>
		public static MvcHtmlString BbCode(this HtmlHelper html, string bbCode, string cachedRender)
		{
			return html.BbCode(bbCode, cachedRender, null);
		}


		/// <summary>
		/// Renders the specified BBCode string using the specified set of tag definitions defined in the web.config
		/// </summary>
		/// <param name="html">The <see cref="HtmlHelper"/></param>
		/// <param name="bbCode">The BBCode string</param>
		/// <param name="cachedRender">
		/// The cached render of <paramref name="bbCode"/>. This will be returned by this method if it is not null
		/// </param>
		/// <param name="configTagDefSetName">
		/// The name of the set of tag definitions to use (defined in the web.config)
		/// </param>
		/// <returns>The rendered XHTML</returns>
		public static MvcHtmlString BbCode(this HtmlHelper html, string bbCode, string cachedRender, string configTagDefSetName)
		{
			if (cachedRender != null)
				return MvcHtmlString.Create(cachedRender);

			BbCodeRenderer bbCodeRenderer = new BbCodeRenderer(configTagDefSetName);
			return MvcHtmlString.Create(bbCodeRenderer.Render(bbCode).RenderedString);
		}
	}
}
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using DigitallyCreated.Utilities.Bcl;


namespace DigitallyCreated.Utilities.Mvc
{
	/// <summary>
	/// Extensions to the <see cref="HtmlHelper"/> class that give develops various pretty eye-candy
	/// functionality.
	/// </summary>
	public static class EyeCandyHtmlHelpers
	{
		/// <summary>
		/// Writes a Temporary Information Box, which is a box that contains a message that is only displayed
		/// if a particular key exists in TempData. This box will stay on screen for 5 seconds and then fade
		/// away and disappear. This is good for "Your Business Object Here was successfully created" sort of
		/// messages.
		/// </summary>
		/// <param name="html">The <see cref="HtmlHelper"/></param>
		/// <param name="message">The message that the box should display</param>
		/// <param name="type">The type of the box</param>
		/// <param name="tempDataKeyName">
		/// The key to look for in the TempData. If this key is not present, the box is not rendered. The 
		/// value of the key's entry is ignored by this method.
		/// </param>
		/// <remarks>
		/// <para>
		/// This method will output XHTML that looks like this:
		/// </para>
		/// <code><![CDATA[
		/// <div id="UniqueAutoGeneratedIdGoesHere" class="TempInfoBox ErrorBox|WarningBox|InfoBox|SuccessBox">
		///	    <p>Your message here.</p>
		/// </div>
		/// <script type="text/javascript">
		///	    jQuery(function() { setTimeout(function() { $("#UniqueAutoGeneratedIdGoesHere").fadeOut("slow"); }, 5000); });
		/// </script>
		/// ]]></code>
		/// <para>
		/// Note that each time you call this method a new unique ID is auto-generated and used on the div.
		/// Also note that all divs will have the <c>TempInfoBox</c> CSS class. They will also have one of
		/// <c>ErrorBox</c>, <c>WarningBox</c>, <c>InfoBox</c>, and <c>SuccessBox</c> as another CSS class
		/// depending on what <see cref="TempInfoBoxType"/> you choose.
		/// </para>
		/// <para>
		/// This HTML is entirely unstyled. You will need to style it to suit your website in your own CSS
		/// file.
		/// </para>
		/// <para>
		/// The fadeout effect is done using jQuery 1.3.2 (this comes with ASP.NET MVC in the standard 
		/// Visual Studio ASP.NET MVC project template). You need to ensure it is included on your page.
		/// </para>
		/// </remarks>
		/// <returns>The HTML</returns>
		public static MvcHtmlString TempInfoBox(this HtmlHelper html, string message, TempInfoBoxType type, string tempDataKeyName)
		{
			if (html.ViewContext.TempData.ContainsKey(tempDataKeyName) == false)
				return MvcHtmlString.Empty;

			//In ASP.NET MVC 2 the temp data item is only removed when after is has been read
			//so read it here, even though we don't care about the value
			object temp = html.ViewContext.TempData[tempDataKeyName];

			string uniqueId = "GUID" + Guid.NewGuid().ToString("N");

			TagBuilder p = new TagBuilder("p");
			p.InnerHtml = html.Encode(message);
			
			TagBuilder div = new TagBuilder("div");
			div.Attributes["id"] = uniqueId;
			div.AddCssClass(type + "Box");
			div.AddCssClass("TempInfoBox");
			div.InnerHtml = p.ToString();

			TagBuilder script = new TagBuilder("script");
			script.Attributes["type"] = "text/javascript";
			script.InnerHtml = "jQuery(function() { setTimeout(function() { $(\"#" + uniqueId + "\").fadeOut(\"slow\"); }, 5000); });";

			StringBuilder output = new StringBuilder();
			output.Append(div.ToString());
			output.Append("\r\n");
			output.Append(script.ToString());

			return MvcHtmlString.Create(output.ToString());
		}


		/// <summary>
		/// Writes a fieldset and legend that collapses when the user clicks on the legend and expands back
		/// into existence when the legend is clicked again. The class is <see cref="IDisposable"/> and
		/// therefore can be used in a using block, and when the using block ends a closing <c>fieldset</c> tag
		/// will be outputted.
		/// </summary>
		/// <remarks>
		/// For more information, see the <see cref="Mvc.CollapsibleFieldset"/> class.
		/// </remarks>
		/// <param name="html">The page's <see cref="HtmlHelper"/></param>
		/// <param name="legendText">The text to display in the legend</param>
		/// <param name="collapsedByDefault">Whether or not the fieldset is collapsed by default</param>
		/// <returns>The HTML</returns>
		public static CollapsibleFieldset CollapsibleFieldset(this HtmlHelper html, string legendText, bool collapsedByDefault)
		{
			return new CollapsibleFieldset(html, legendText, collapsedByDefault);
		}


		/// <summary>
		/// Takes a string, escapes it for HTML use, then turns all line breaks into &lt;br/&gt; tags, and
		/// then wraps each URL found in the text in an a tag with the href attribute set to the URL.
		/// </summary>
		/// <param name="html">The <see cref="HtmlHelper"/></param>
		/// <param name="text">The text to encode and format</param>
		/// <param name="useRelNoFollow">Add the rel="nofollow" attribute to the inserted a tags</param>
		/// <returns>The HTML</returns>
		public static MvcHtmlString EncodeAndInsertBrsAndLinks(this HtmlHelper html, string text, bool useRelNoFollow)
		{
			StringBuilder builder = new StringBuilder();
			MatchCollection matches = RegularExpressions.Url.Matches(text);
			
			Match match;
			for (int i = 0; i < matches.Count; i++)
			{
				Match lastMatch = i > 0 ? matches[i - 1] : null;
				match = matches[i];

				TagBuilder aTag = new TagBuilder("a");
				aTag.Attributes["href"] = match.Value;
				if (useRelNoFollow) aTag.Attributes["rel"] = "nofollow";
				aTag.InnerHtml = match.Value;
				string tag = aTag.ToString();

				//Append the bit that comes before the very first match, if it exists
				if (i == 0 && match.Index > 0)
					builder.Append(html.Encode(text.Substring(0, match.Index)));

				//Append the bit before the linkified URL if it exists
				if (lastMatch != null)
				{
					int startIndex = lastMatch.Index + lastMatch.Length;
					int length = match.Index - startIndex;
					if (length > 0)
						builder.Append(html.Encode(text.Substring(startIndex, length)));
				}

				//Append the linkified URL
				builder.Append(tag);

				//Append the bit that comes after the very last match, if it exists
				if (i == matches.Count - 1 && match.Index + match.Length < text.Length)
					builder.Append(html.Encode(text.Substring(match.Index + match.Length)));
			}

			if (matches.Count == 0)
				builder.Append(html.Encode(text));

			builder.Replace("\r", String.Empty);
			builder.Replace("\n", "<br/>");

			return MvcHtmlString.Create(builder.ToString());
		}
	}
}
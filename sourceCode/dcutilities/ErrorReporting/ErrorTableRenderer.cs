using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;


namespace DigitallyCreated.Utilities.ErrorReporting
{
	/// <summary>
	/// This helper class is used to emit HTML that makes up error tables
	/// </summary>
	public static class ErrorTableRenderer
	{
		/// <summary>
		/// Render the start of an error table
		/// </summary>
		/// <param name="builder">The <see cref="StringBuilder"/> to emit HTML into</param>
		public static void RenderTableStart(StringBuilder builder)
		{
			builder.Append("<table style='font-family:Verdana, Arial, Helvetica, sans-serif; font-size:12px; border:1px black solid;' cellspacing='0'>");
		}


		/// <summary>
		/// Renders the heading row of the table
		/// </summary>
		/// <param name="builder">The <see cref="StringBuilder"/> to emit HTML into</param>
		/// <param name="contentsRenderer">A method that can render the contents of the heading row</param>
		public static void RenderTableHeading(StringBuilder builder, Action<StringBuilder> contentsRenderer)
		{
			builder.Append("<tr style='font-size:20px; font-weight:bold; background-color:#FF3333'>");
			builder.Append("<td colspan='2' style='border:1px black solid;'>");
			contentsRenderer(builder);
			builder.Append("</td></tr>");
		}


		/// <summary>
		/// Renders the heading row of the table
		/// </summary>
		/// <param name="builder">The <see cref="StringBuilder"/> to emit HTML into</param>
		/// <param name="keyRenderer">A method that can render the key</param>
		/// <param name="valueRenderer">A method that can render the value</param>
		public static void RenderTableKeyValueHeading(StringBuilder builder, Action<StringBuilder> keyRenderer, Action<StringBuilder> valueRenderer)
		{
			builder.Append("<tr style='font-weight:bold; background-color:#FF3333'>");
			builder.Append("<td style='font-weight:bold; border:1px black solid;'>");
			keyRenderer(builder);
			builder.Append("</td><td style='border:1px black solid;'>");
			valueRenderer(builder);
			builder.Append("</td></tr>");
		}


		/// <summary>
		/// Renders a row that has two columns. The left one is used for the "key" and the right one
		/// is sued for the "value".
		/// </summary>
		/// <param name="builder">The <see cref="StringBuilder"/> to emit HTML into</param>
		/// <param name="keyRenderer">A method that can render the key</param>
		/// <param name="valueRenderer">A method that can render the value</param>
		public static void RenderKeyValueRow(StringBuilder builder, Action<StringBuilder> keyRenderer, Action<StringBuilder> valueRenderer)
		{
			builder.Append("<tr><td style='font-weight:bold; background-color: #FFA4A4; border:1px black solid;'>");
			keyRenderer(builder);
			builder.Append("</td><td style='border:1px black solid; background-color:white;'>");
			valueRenderer(builder);
			builder.Append("</td></tr>");
		}


		/// <summary>
		/// Renders two rows, the first being a title row containing the "key", the second
		/// containing the "value".
		/// </summary>
		/// <param name="builder">The <see cref="StringBuilder"/> to emit HTML into</param>
		/// <param name="keyRenderer">A method that can render the key</param>
		/// <param name="valueRenderer">A method that can render the value</param>
		public static void RenderKeyValueMultiRow(StringBuilder builder, Action<StringBuilder> keyRenderer, Action<StringBuilder> valueRenderer)
		{
			builder.Append("<tr><td colspan='2' style='font-weight:bold; background-color: #FFA4A4; border:1px black solid;'>");
			keyRenderer(builder);
			builder.Append("</td></tr><tr><td colspan='2' style='border:1px black solid;'>");
			valueRenderer(builder);
			builder.Append("</td></tr>");
		}


		/// <summary>
		/// Renders the end of the error table
		/// </summary>
		/// <param name="builder">The <see cref="StringBuilder"/> to emit HTML into</param>
		public static void RenderTableEnd(StringBuilder builder)
		{
			builder.Append("</table>");
		}
	}
}
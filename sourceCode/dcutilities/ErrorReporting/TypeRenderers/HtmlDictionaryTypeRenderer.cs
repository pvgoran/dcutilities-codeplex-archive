using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;


namespace DigitallyCreated.Utilities.ErrorReporting.TypeRenderers
{
	/// <summary>
	/// The <see cref="HtmlDictionaryTypeRenderer"/> is able to render objects
	/// of type <see cref="IDictionary"/> as an HTML table
	/// </summary>
	public class HtmlDictionaryTypeRenderer : AbstractTypeRenderer<IDictionary>
	{
		/// <summary>
		/// Renders the detail object to a <see cref="StringBuilder"/>
		/// </summary>
		/// <param name="context">The rendering context</param>
		protected override void DoRender(RenderContext<IDictionary> context)
		{
			ErrorTableRenderer.RenderTableStart(context.Builder);
			ErrorTableRenderer.RenderTableKeyValueHeading(context.Builder, b => b.Append("Key"), b => b.Append("Value"));

			foreach (DictionaryEntry entry in context.ObjectToRender)
			{
				ITypeRenderer keyRenderer = UtilityMethods.ChooseTypeRenderer(entry.Key, context.TypeRenderers);
				ITypeRenderer valueRenderer = UtilityMethods.ChooseTypeRenderer(entry.Value, context.TypeRenderers);

				ErrorTableRenderer.RenderKeyValueRow(context.Builder, b => keyRenderer.Render(context.Copy(entry.Key, b)), b => valueRenderer.Render(context.Copy(entry.Value, b)));
			}

			ErrorTableRenderer.RenderTableEnd(context.Builder);
		}
	}
}
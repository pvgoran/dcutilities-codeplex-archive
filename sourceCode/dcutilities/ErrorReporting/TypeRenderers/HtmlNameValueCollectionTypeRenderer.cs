using System.Collections.Specialized;
using System.Text;


namespace DigitallyCreated.Utilities.ErrorReporting.TypeRenderers
{
	/// <summary>
	/// The <see cref="HtmlDictionaryTypeRenderer"/> is able to render objects
	/// of type <see cref="NameValueCollection"/> as an HTML table
	/// </summary>
	public class HtmlNameValueCollectionTypeRenderer : AbstractTypeRenderer<NameValueCollection>
	{
		/// <summary>
		/// Renders the detail object to a <see cref="StringBuilder"/>
		/// </summary>
		/// <param name="context">The rendering context</param>
		protected override void DoRender(RenderContext<NameValueCollection> context)
		{
			ErrorTableRenderer.RenderTableStart(context.Builder);
			ErrorTableRenderer.RenderTableKeyValueHeading(context.Builder, b => b.Append("Key"), b => b.Append("Value"));

			foreach (string key in context.ObjectToRender.AllKeys)
			{
				ITypeRenderer keyRenderer = UtilityMethods.ChooseTypeRenderer(key, context.TypeRenderers);
				ITypeRenderer valueRenderer = UtilityMethods.ChooseTypeRenderer(context.ObjectToRender[key], context.TypeRenderers);

				ErrorTableRenderer.RenderKeyValueRow(context.Builder, b => keyRenderer.Render(context.Copy((object)key, b)), b => valueRenderer.Render(context.Copy((object)context.ObjectToRender[key], b)));
			}

			ErrorTableRenderer.RenderTableEnd(context.Builder);
		}
	}
}
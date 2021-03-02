using System;
using System.Collections;
using System.Text;


namespace DigitallyCreated.Utilities.ErrorReporting.TypeRenderers
{
	/// <summary>
	/// This <see cref="ITypeRenderer"/> is able to render <see cref="IEnumerable"/>s as an HTML table
	/// </summary>
	public class HtmlEnumerableTypeRenderer : AbstractTypeRenderer<IEnumerable>
	{
		/// <summary>
		/// Renders the detail object to a <see cref="StringBuilder"/>
		/// </summary>
		/// <param name="context">The rendering context</param>
		protected override void DoRender(RenderContext<IEnumerable> context)
		{
			ErrorTableRenderer.RenderTableStart(context.Builder);
			ErrorTableRenderer.RenderTableKeyValueHeading(context.Builder, b => b.Append("Index"), b => b.Append("Value"));

			int i = 0;
			foreach (object obj in context.ObjectToRender)
			{
				ITypeRenderer objRenderer = UtilityMethods.ChooseTypeRenderer(obj, context.TypeRenderers);
				
				ErrorTableRenderer.RenderKeyValueRow(context.Builder, b => b.Append(i), b => objRenderer.Render(context.Copy(obj, b)));
				i++;
			}

			ErrorTableRenderer.RenderTableEnd(context.Builder);
		}
	}
}
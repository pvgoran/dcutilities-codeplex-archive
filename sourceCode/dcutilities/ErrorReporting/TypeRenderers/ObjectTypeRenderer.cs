using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;


namespace DigitallyCreated.Utilities.ErrorReporting.TypeRenderers
{
	/// <summary>
	/// This <see cref="ITypeRenderer"/> is a catchall for all objects that renders
	/// that there is no specific renderer available. It renders out the type of object and
	/// the result of the object's <see cref="object.ToString"/> method.
	/// </summary>
	public class ObjectTypeRenderer : AbstractTypeRenderer<object>
	{
		/// <summary>
		/// Renders the detail object to a <see cref="StringBuilder"/>
		/// </summary>
		/// <param name="context">The rendering context</param>
		protected override void DoRender(RenderContext<object> context)
		{
			ErrorTableRenderer.RenderTableStart(context.Builder);
			ErrorTableRenderer.RenderKeyValueRow(context.Builder, b => b.Append("Object Type"), b => b.Append(context.ObjectToRender.GetType().FullName));

			IEnumerable<KeyValuePair<string, object>> details = UtilityMethods.GetDetailsForObject(context.ObjectToRender, context.DetailProviders, context.FallbackDetailProvider);
			foreach (KeyValuePair<string, object> detailPair in details)
			{
				ITypeRenderer keyRenderer = UtilityMethods.ChooseTypeRenderer(detailPair.Key, context.TypeRenderers);
				ITypeRenderer valueRenderer = UtilityMethods.ChooseTypeRenderer(detailPair.Value, context.TypeRenderers);
				ErrorTableRenderer.RenderKeyValueRow(context.Builder, b => keyRenderer.Render(context.Copy<object>(detailPair.Key, b)), b => valueRenderer.Render(context.Copy(detailPair.Value, b)));
			}

			ErrorTableRenderer.RenderTableEnd(context.Builder);
		}
	}
}
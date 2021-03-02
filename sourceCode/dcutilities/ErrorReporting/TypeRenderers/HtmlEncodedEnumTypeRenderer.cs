using System;
using System.Text;
using System.Web;


namespace DigitallyCreated.Utilities.ErrorReporting.TypeRenderers
{
	/// <summary>
	/// This <see cref="ITypeRenderer"/> is able to render enums as HTML encoded text
	/// </summary>
	public class HtmlEncodedEnumTypeRenderer : AbstractTypeRenderer<Enum>
	{
		/// <summary>
		/// Renders the detail object to a <see cref="StringBuilder"/>
		/// </summary>
		/// <param name="context">The rendering context</param>
		protected override void DoRender(RenderContext<Enum> context)
		{
			context.Builder.Append(HttpUtility.HtmlEncode(context.ObjectToRender.ToString()));
		}
	}
}
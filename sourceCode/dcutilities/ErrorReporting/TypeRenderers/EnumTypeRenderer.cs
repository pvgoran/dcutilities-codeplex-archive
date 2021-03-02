using System;
using System.Text;


namespace DigitallyCreated.Utilities.ErrorReporting.TypeRenderers
{
	/// <summary>
	/// This <see cref="ITypeRenderer"/> is able to render enums
	/// </summary>
	public class EnumTypeRenderer : AbstractTypeRenderer<Enum>
	{
		/// <summary>
		/// Renders the detail object to a <see cref="StringBuilder"/>
		/// </summary>
		/// <param name="context">The rendering context</param>
		protected override void DoRender(RenderContext<Enum> context)
		{
			context.Builder.Append(String.Format("{0}.{1}", context.ObjectToRender.GetType().Name, context.ObjectToRender));
		}
	}
}
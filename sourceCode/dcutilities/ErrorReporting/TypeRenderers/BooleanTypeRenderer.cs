using System;
using System.Text;


namespace DigitallyCreated.Utilities.ErrorReporting.TypeRenderers
{
	/// <summary>
	/// This <see cref="ITypeRenderer"/> is able to render <see cref="bool"/>s
	/// </summary>
	public class BooleanTypeRenderer : AbstractTypeRenderer<bool>
	{
		/// <summary>
		/// Renders the detail object to a <see cref="StringBuilder"/>
		/// </summary>
		/// <param name="context">The rendering context</param>
		protected override void DoRender(RenderContext<bool> context)
		{
			context.Builder.Append(context.ObjectToRender.ToString());
		}
	}
}
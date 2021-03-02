using System.Text;


namespace DigitallyCreated.Utilities.ErrorReporting.TypeRenderers
{
	/// <summary>
	/// This <see cref="ITypeRenderer"/> is able to render null values as HTML
	/// </summary>
	public class HtmlNullTypeRenderer : ITypeRenderer
	{
		/// <summary>
		/// Whether or not this renderer can render the object passed in.
		/// </summary>
		/// <param name="obj">The object to check rendering capability for</param>
		/// <returns>True if it can render the object, false otherwise</returns>
		public bool CanRender(object obj)
		{
			return obj == null;
		}


		/// <summary>
		/// Renders the detail object to a <see cref="StringBuilder"/>
		/// </summary>
		/// <param name="context">The rendering context</param>
		/// <exception cref="TypeNotSupportedException">
		/// Thrown if this detail renderer does not support the type of object you
		/// passed in via the context.
		/// </exception>
		public void Render(RenderContext<object> context)
		{
			context.Builder.Append("<em>null</em>");
		}
	}
}
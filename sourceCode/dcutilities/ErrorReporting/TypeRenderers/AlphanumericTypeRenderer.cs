using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;


namespace DigitallyCreated.Utilities.ErrorReporting.TypeRenderers
{
	/// <summary>
	/// This <see cref="ITypeRenderer"/> is able to render any of the built in numeric types
	/// </summary>
	public class AlphanumericTypeRenderer : ITypeRenderer
	{
		/// <summary>
		/// Whether or not this renderer can render the object passed in.
		/// </summary>
		/// <param name="obj">The object to check rendering capability for</param>
		/// <returns>True if it can render the object, false otherwise</returns>
		public bool CanRender(object obj)
		{
			return obj is byte || obj is sbyte || obj is decimal || obj is double || obj is float ||
			       obj is int || obj is uint || obj is long || obj is ulong || obj is short ||
			       obj is ushort || obj is string || obj is char || obj is DateTime ||
			       obj is DateTimeOffset || obj is TimeSpan;
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
			context.Builder.Append(context.ObjectToRender.ToString());
		}
	}
}
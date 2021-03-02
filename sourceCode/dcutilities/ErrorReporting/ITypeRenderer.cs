using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DigitallyCreated.Utilities.ErrorReporting
{
	/// <summary>
	/// An <see cref="ITypeRenderer"/> is able to render a certain type of object. 
	/// </summary>
	public interface ITypeRenderer
	{
		/// <summary>
		/// Whether or not this renderer can render the object passed in.
		/// </summary>
		/// <param name="obj">The object to check rendering capability for</param>
		/// <returns>True if it can render the object, false otherwise</returns>
		bool CanRender(object obj);


		/// <summary>
		/// Renders the detail object to a <see cref="StringBuilder"/>
		/// </summary>
		/// <param name="context">The rendering context</param>
		/// <exception cref="TypeNotSupportedException">
		/// Thrown if this detail renderer does not support the type of object you
		/// passed in via the context.
		/// </exception>
		void Render(RenderContext<object> context);
	}
}

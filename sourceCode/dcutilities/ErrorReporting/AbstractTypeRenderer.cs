using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DigitallyCreated.Utilities.ErrorReporting
{
	/// <summary>
	/// This class provides a type-safe way for classes to implement 
	/// <see cref="ITypeRenderer"/> (ie by inheriting from this class)
	/// </summary>
	/// <typeparam name="T">The type of object that you will support rendering</typeparam>
	public abstract class AbstractTypeRenderer<T> : ITypeRenderer
	{
		/// <summary>
		/// Whether or not this renderer can render the object passed in.
		/// </summary>
		/// <param name="obj">The object to check rendering capability for</param>
		/// <returns>True if it can render the object, false otherwise</returns>
		public bool CanRender(object obj)
		{
			return obj is T;
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
			if (CanRender(context.ObjectToRender))
				DoRender(context.Copy((T)context.ObjectToRender));
			else
				throw new TypeNotSupportedException(context.ObjectToRender.GetType(), GetType());
		}


		/// <summary>
		/// Renders the detail object to a <see cref="StringBuilder"/>
		/// </summary>
		/// <param name="context">The rendering context</param>
		protected abstract void DoRender(RenderContext<T> context);
	}
}

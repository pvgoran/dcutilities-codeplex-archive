using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;


namespace DigitallyCreated.Utilities.ErrorReporting
{
	/// <summary>
	/// This class contains information needed by <see cref="ITypeRenderer"/>s when they render
	/// </summary>
	/// <typeparam name="T">The type of object being rendered</typeparam>
	public class RenderContext<T>
	{
		/// <summary>
		/// The object that the <see cref="ITypeRenderer"/> needs to render
		/// </summary>
		public T ObjectToRender { get; private set; }

		/// <summary>
		/// The <see cref="StringBuilder"/> to append the rendering into
		/// </summary>
		public StringBuilder Builder { get; private set; }

		/// <summary>
		/// A collection of all type renderers registered in the rendering system
		/// </summary>
		public IList<ITypeRenderer> TypeRenderers { get; private set; }

		/// <summary>
		/// A collection of all the detail providers in the rendering system
		/// </summary>
		public ICollection<IDetailProvider> DetailProviders { get; private set; }

		/// <summary>
		/// The fallback detail provider to be used when none of the <see cref="DetailProviders"/>
		/// can provide details
		/// </summary>
		public IDetailProvider FallbackDetailProvider { get; private set; }


		/// <summary>
		/// Constructor, creates a RenderContext
		/// </summary>
		/// <param name="objectToRender">The object to render</param>
		/// <param name="builder">The <see cref="StringBuilder"/> to append the rendering into</param>
		/// <param name="typeRenderers">The type renderers registered in the system</param>
		/// <param name="detailProviders">The detail providers registered in the system</param>
		/// <param name="fallbackDetailProvider">The fallback detail provider</param>
		public RenderContext(T objectToRender, StringBuilder builder, IList<ITypeRenderer> typeRenderers, ICollection<IDetailProvider> detailProviders, IDetailProvider fallbackDetailProvider)
		{
			ObjectToRender = objectToRender;
			Builder = builder;
			TypeRenderers = typeRenderers;
			DetailProviders = detailProviders;
			FallbackDetailProvider = fallbackDetailProvider;
		}

		
		/// <summary>
		/// Constructor, creates a RenderContext that gets its <see cref="ITypeRenderer"/>s,
		/// <see cref="IDetailProvider"/>s, etc from the specified <see cref="IErrorEmailComposer"/>.
		/// </summary>
		/// <param name="objectToRender">The object to render</param>
		/// <param name="builder">The <see cref="StringBuilder"/> to append the rendering into</param>
		/// <param name="composer">The <see cref="IErrorEmailComposer"/> to get data from</param>
		public RenderContext(T objectToRender, StringBuilder builder, IErrorEmailComposer composer)
			: this(objectToRender, builder, new ReadOnlyCollection<ITypeRenderer>(composer.TypeRenderers), new ReadOnlyCollection<IDetailProvider>(new List<IDetailProvider>(composer.DetailProviders)), composer.FallbackDetailProvider)
		{
		}


		/// <summary>
		/// Static constructor, creates a <see cref="RenderContext{T}"/> by copying this 
		/// <see cref="RenderContext{T}"/> but using a different object to render.
		/// </summary>
		/// <typeparam name="TNewType">The type of the new object to render</typeparam>
		/// <param name="objectToRender">The object to render</param>
		/// <returns>The new <see cref="RenderContext{T}"/></returns>
		public RenderContext<TNewType> Copy<TNewType>(TNewType objectToRender)
		{
			return new RenderContext<TNewType>(objectToRender, Builder, TypeRenderers, DetailProviders, FallbackDetailProvider);
		}


		/// <summary>
		/// Static constructor, creates a <see cref="RenderContext{T}"/> by copying this 
		/// <see cref="RenderContext{T}"/> but using a different object to render and 
		/// <see cref="StringBuilder"/>.
		/// </summary>
		/// <typeparam name="TNewType">The type of the new object to render</typeparam>
		/// <param name="objectToRender">The object to render</param>
		/// <param name="builder">The <see cref="StringBuilder"/> to append the rendering into</param>
		/// <returns>The new <see cref="RenderContext{T}"/></returns>
		public RenderContext<TNewType> Copy<TNewType>(TNewType objectToRender, StringBuilder builder)
		{
			return new RenderContext<TNewType>(objectToRender, builder, TypeRenderers, DetailProviders, FallbackDetailProvider);
		}
	}
}
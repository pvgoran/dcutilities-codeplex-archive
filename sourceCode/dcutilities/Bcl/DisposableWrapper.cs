using System;


namespace DigitallyCreated.Utilities.Bcl
{
	/// <summary>
	/// This class wraps an object and is <see cref="IDisposable"/>. If the wrapped object is also 
	/// <see cref="IDisposable"/> (it may not be), then it will be disposed when this wrapper is.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This is useful when you are using a technique like dependency injection and one type that may be
	/// injected is <see cref="IDisposable"/> and the other is not. Wrapping both means that the 
	/// <see cref="IDisposable"/> one is disposed, and the other is treated as if it was 
	/// <see cref="IDisposable"/> but it really isn't.
	/// </para>
	/// <para>
	/// This situation occurs with WCF service clients. The service interface itself isn't 
	/// <see cref="IDisposable"/> but the service client is. So doing dependency injection where the actual
	/// concrete service class is injected or a WCF service proxy client class is injected would result in
	/// you having to know which one you were using in order to dispose the service client correctly,
	/// defeating the purpose of the dependency injection.
	/// </para>
	/// </remarks>
	/// <typeparam name="T">The type being wrapped</typeparam>
	public class DisposableWrapper<T> : IDisposable
	{
		private readonly T _Object;

		/// <summary>
		/// Constructor, creates a <see cref="DisposableWrapper{T}"/>
		/// </summary>
		/// <param name="objectToWrap">The object to wrap</param>
		public DisposableWrapper(T objectToWrap)
		{
			_Object = objectToWrap;
		}


		/// <summary>
		/// The wrapped object
		/// </summary>
		public T Object
		{
			get { return _Object; }
		}


		/// <summary>
		/// If the wrapped object is <see cref="IDisposable"/> this will call
		/// <see cref="IDisposable.Dispose"/>, otherwise this will do nothing.
		/// </summary>
		public void Dispose()
		{
			IDisposable disposable = _Object as IDisposable;
			if (disposable != null)
				disposable.Dispose();
		}
	}


	/// <summary>
	/// Convenient helper class for creating <see cref="DisposableWrapper{T}"/>s
	/// </summary>
	public static class DisposableWrapper
	{
		/// <summary>
		/// Creates a <see cref="DisposableWrapper{T}"/>
		/// </summary>
		/// <remarks>
		/// This method may be more convenient than the <see cref="DisposableWrapper{T}"/> constructor
		/// since it uses type inference to determine the type of disposable wrapper.
		/// </remarks>
		/// <typeparam name="T">The type to use inside the <see cref="DisposableWrapper{T}"/></typeparam>
		/// <param name="objectToWrap">The object to wrap</param>
		/// <returns>The <see cref="DisposableWrapper{T}"/></returns>
		public static DisposableWrapper<T> New<T>(T objectToWrap)
		{
			return new DisposableWrapper<T>(objectToWrap);
		}


		/// <summary>
		/// This method allows you to overcome the limitations of the using block
		/// (see http://msdn.microsoft.com/en-us/library/aa355056.aspx for more information) by being
		/// able to catch exceptions in the block and exceptions from the dispose method and if
		/// both occur throw an <see cref="AggregateException"/>. This overload automatically unwraps
		/// a <see cref="DisposableWrapper{T}"/> for you.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Note that the semantic of this block is slightly different to a normal using block, since
		/// keywords like return will not "return" the current method; rather they will return the
		/// action delegate you pass to the method.
		/// </para>
		/// <example>
		/// Here is an example of how you could use this:
		/// <code>
		/// (DisposableWrapper.SafeUsingBlock(new MyWcfClient(), myWcfClient =>
		/// {
		///	    myWcfClient.MyServiceMethod();
		/// });
		/// </code>
		/// </example>
		/// </remarks>
		/// <typeparam name="T">The type wrapped in the disposable wrapper</typeparam>
		/// <param name="objectToWrap">The object to wrap</param>
		/// <param name="action">A function that will run as "using block"</param>
		public static void SafeUsingBlock<T>(T objectToWrap, Action<T> action)
		{
			new DisposableWrapper<T>(objectToWrap).SafeUsingBlock(action, dw => dw.Object);
		}
	}
}
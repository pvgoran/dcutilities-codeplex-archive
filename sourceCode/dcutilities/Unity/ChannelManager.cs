using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.ServiceModel;
using System.Threading;
using ReaderWriterLock=DigitallyCreated.Utilities.Bcl.Concurrency.ReaderWriterLock;


namespace DigitallyCreated.Utilities.Unity
{
	/// <summary>
	/// The ChannelManager is able to create WCF channel objects for a specified interface. It does this by
	/// using the WCF <see cref="ChannelFactory{TChannel}"/>. It caches an instance of a 
	/// <see cref="ChannelFactory{TChannel}"/> for a particular type on a per thread basis as according to 
	/// <see cref="ChannelFactory{TChannel}"/>'s documentation, it is not thread safe.
	/// </summary>
	/// <remarks>
	/// This class is thread safe.
	/// </remarks>
	public class ChannelManager : IChannelManager
	{
		private readonly ReaderWriterLock _ReaderWriterLock;
		private readonly IDictionary<Thread, IDictionary<Type, IDictionary<IChannelFactoryFactory, CacheItem>>> _Cache;


		/// <summary>
		/// Constructor, creates a ChannelManager
		/// </summary>
		public ChannelManager()
		{
			_ReaderWriterLock = new ReaderWriterLock();
			_Cache = new Dictionary<Thread, IDictionary<Type, IDictionary<IChannelFactoryFactory, CacheItem>>>();
		}
		

		/// <summary>
		/// Creates a WCF channel object for the specified service interface
		/// </summary>
		/// <param name="ofType">
		/// The service interface type (used as the type parameter for <see cref="ChannelFactory{TChannel}"/>
		/// </param>
		/// <param name="channelFactoryFactory">
		/// The <see cref="IChannelFactoryFactory"/> to use to create the appropriate 
		/// <see cref="ChannelFactory{TChannel}"/>.
		/// </param>
		/// <returns>The channel</returns>
		public object CreateChannel(Type ofType, IChannelFactoryFactory channelFactoryFactory)
		{
			CleanCacheOfDeadThreads();

			if (CheckCacheItemExists(ofType, channelFactoryFactory) == false)
				CreateChannelFactory(ofType, channelFactoryFactory);

			object channel;
			try
			{
				using (_ReaderWriterLock.AcquireDisposableRead())
				{
					channel = _Cache[Thread.CurrentThread][ofType][channelFactoryFactory].CreateChannelFunc();
				}
			}
			catch (Exception e)
			{
				//If we're not an expected channel error, throw
				if (e is CommunicationException == false && e is ObjectDisposedException == false && e is TimeoutException == false)
					throw;
					
				//Expected channel error. Toss the original factory and create a new one
				//If the new one fails too, any exception will be thrown as normal
				CreateChannelFactory(ofType, channelFactoryFactory);

				using (_ReaderWriterLock.AcquireDisposableRead())
				{
					channel = _Cache[Thread.CurrentThread][ofType][channelFactoryFactory].CreateChannelFunc();
				}
			}

			return channel;
		}


		/// <summary>
		/// Checks to see whether a channel factory exists in the cache for the current thread
		/// </summary>
		/// <param name="ofType">The service interface type</param>
		/// <param name="channelFactoryFactory">
		/// The <see cref="IChannelFactoryFactory"/> to use to create the appropriate 
		/// <see cref="ChannelFactory{TChannel}"/>.
		/// </param>
		/// <returns>True if there is a channel factory in the cache, false otherwise</returns>
		private bool CheckCacheItemExists(Type ofType, IChannelFactoryFactory channelFactoryFactory)
		{
			using (_ReaderWriterLock.AcquireDisposableRead())
			{
				return _Cache.ContainsKey(Thread.CurrentThread) &&
				       _Cache[Thread.CurrentThread].ContainsKey(ofType) &&
					   _Cache[Thread.CurrentThread][ofType].ContainsKey(channelFactoryFactory);
			}
		}


		/// <summary>
		/// Cleans dead threads' channel factories out of the cache
		/// </summary>
		private void CleanCacheOfDeadThreads()
		{
			using (_ReaderWriterLock.AcquireDisposableRead())
			{
				if (_Cache.Any(kvp => kvp.Key.IsAlive == false) == false)
					return;
			}

			using (_ReaderWriterLock.AcquireDisposableWrite())
			{
				IList<Thread> deadThreads = _Cache.Where(kvp => kvp.Key.IsAlive == false).Select(kvp => kvp.Key).ToList();
				foreach (Thread thread in deadThreads)
					_Cache.Remove(thread);
			}
		}


		/// <summary>
		/// Create a new channel factory and puts it into the cache
		/// </summary>
		/// <param name="ofType">The service interface type</param>
		/// <param name="channelFactoryFactory">
		/// The <see cref="IChannelFactoryFactory"/> to use to create the appropriate 
		/// <see cref="ChannelFactory{TChannel}"/>.
		/// </param>
		private void CreateChannelFactory(Type ofType, IChannelFactoryFactory channelFactoryFactory)
		{
			using (_ReaderWriterLock.AcquireDisposableWrite())
			{
				if (_Cache.ContainsKey(Thread.CurrentThread) == false)
					_Cache.Add(Thread.CurrentThread, new Dictionary<Type, IDictionary<IChannelFactoryFactory, CacheItem>>());

				if (_Cache[Thread.CurrentThread].ContainsKey(ofType) == false)
					_Cache[Thread.CurrentThread].Add(ofType, new Dictionary<IChannelFactoryFactory, CacheItem>());

				ChannelFactory channelFactory = channelFactoryFactory.CreateChannelFactory(ofType);

				CacheItem cacheItem = new CacheItem(channelFactory, CompileCreateChannelFunc(channelFactory));

				_Cache[Thread.CurrentThread][ofType][channelFactoryFactory] = cacheItem;
			}
		}


		/// <summary>
		/// Compiles a delegate that calls the <see cref="ChannelFactory{TChannel}.CreateChannel()"/>
		/// method on the specified channel factory
		/// </summary>
		/// <param name="channelFactory">The channel factory</param>
		/// <returns>The delegate that calls <see cref="ChannelFactory{TChannel}.CreateChannel()"/></returns>
		private Func<object> CompileCreateChannelFunc(ChannelFactory channelFactory)
		{
			Type factoryType = channelFactory.GetType();
			MethodInfo createChannelMethod = factoryType.GetMethod("CreateChannel", new Type[0]);

			ConstantExpression constantExpr = Expression.Constant(channelFactory, factoryType);
			MethodCallExpression methodCallExpr = Expression.Call(constantExpr, createChannelMethod);
			Expression<Func<object>> lambdaExpr = Expression.Lambda<Func<object>>(methodCallExpr);
			return lambdaExpr.Compile();
		}


		/// <summary>
		/// A cache item resides in the cache and holds a 
		/// <see cref="System.ServiceModel.ChannelFactory{TChannel}"/> and a delegate that can call 
		/// <see cref="System.ServiceModel.ChannelFactory{TChannel}.CreateChannel()"/> on that channel
		/// </summary>
		private class CacheItem
		{
			private readonly ChannelFactory _ChannelFactory;
			private readonly Func<object> _CreateChannelFunc;

			public ChannelFactory ChannelFactory
			{
				get { return _ChannelFactory; }
			}

			
			public Func<object> CreateChannelFunc
			{
				get { return _CreateChannelFunc; }
			}


			public CacheItem(ChannelFactory channelFactory, Func<object> createChannelFunc)
			{
				_ChannelFactory = channelFactory;
				_CreateChannelFunc = createChannelFunc;
			}
		}

	}
}
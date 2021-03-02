using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DigitallyCreated.Utilities.Ef
{
	/// <summary>
	/// The CompiledQueryReplicator is a class that allows you to specify a LINQ query once, then
	/// automatically compile different instances of it. This is useful when you need multiple instances
	/// of the same query compiled with different <see cref="System.Data.Objects.MergeOption"/>s, 
	/// for example. Each instance is identified with a particular key of type <typeparamref name="TKey"/>.
	/// </summary>
	/// <remarks>
	/// <para>
	/// If you're using the Entity Framework (LINQ to Entities), it is suggested that you use the 
	/// <see cref="EfCompiledQueryReplicatorFactory{TKey}"/> class to instantiate instances of this
	/// class, as it makes the syntax shorter and automatically provides you with the Entity Framework
	/// compiled query compiler.
	/// </para>
	/// <para>
	/// This class is thread-safe.
	/// </para>
	/// </remarks>
	/// <typeparam name="TQuery">The type of the query</typeparam>
	/// <typeparam name="TKey">The type to use as the key</typeparam>
	public class CompiledQueryReplicator<TQuery, TKey>
	{
		private readonly Expression<TQuery> _QueryExpressionTree;
		private readonly Func<Expression<TQuery>, TQuery> _Compiler;
		private readonly IDictionary<TKey, TQuery> _CompiledQueries;


		/// <summary>
		/// Creates a CompiledQueryReplicator using the specified query that will be compiled with
		/// the specified compiler.
		/// </summary>
		/// <param name="query">The query's expression tree</param>
		/// <param name="compiler">
		/// A delegate that can compile the query expression tree into an actual query
		/// </param>
		public CompiledQueryReplicator(Expression<TQuery> query, Func<Expression<TQuery>, TQuery> compiler)
		{
			_QueryExpressionTree = query;
			_Compiler = compiler;
			_CompiledQueries = new Dictionary<TKey, TQuery>();
		}


		/// <summary>
		/// Returns the compiled query instance associated with the specified key. If
		/// no query instance has been created for the specified key, one will be
		/// and it will be returned.
		/// </summary>
		/// <param name="key">The key</param>
		/// <returns>The compiled query.</returns>
		public TQuery this[TKey key]
		{
			get
			{
				TQuery query;

				lock (_CompiledQueries)
				{
					if (_CompiledQueries.ContainsKey(key) == false)
					{
						query = _Compiler(_QueryExpressionTree);
						_CompiledQueries.Add(key, query);
					}
					else
						query = _CompiledQueries[key];
				}

				return query;
			}
		}
	}
}
using System;
using System.Data.Objects;
using System.Linq.Expressions;


namespace DigitallyCreated.Utilities.Ef
{
	/// <summary>
	/// Factory class that makes it easier to create <see cref="CompiledQueryReplicator{TQuery,TKey}"/>
	/// instances that hold LINQ to Entities compiled queries.
	/// </summary>
	/// <typeparam name="TKey">T
	/// he type to use as the key for the CompiledQueryReplicators created
	/// </typeparam>
	public static class EfCompiledQueryReplicatorFactory<TKey>
	{
		/// <summary>
		/// Creates a <see cref="CompiledQueryReplicator{TQuery,TKey}"/> that compiles LINQ to Entities
		/// queries.
		/// </summary>
		/// <typeparam name="TArg0">
		/// The <see cref="ObjectContext"/> that encapsulates the model connection and metadata.
		/// </typeparam>
		/// <typeparam name="TResult">
		/// The type of the query results returned by executing the delegate returned by the Compile method.
		/// </typeparam>
		/// <param name="query">The LINQ to Entities query expression to compile</param>
		/// <returns>The <see cref="CompiledQueryReplicator{TQuery,TKey}"/> created</returns>
		public static CompiledQueryReplicator<Func<TArg0, TResult>, TKey> Create<TArg0, TResult>(Expression<Func<TArg0, TResult>> query)
			where TArg0 : ObjectContext
		{
			return new CompiledQueryReplicator<Func<TArg0, TResult>, TKey>(query, CompiledQuery.Compile);
		}

		/// <summary>
		/// Creates a <see cref="CompiledQueryReplicator{TQuery,TKey}"/> that compiles LINQ to Entities
		/// queries.
		/// </summary>
		/// <typeparam name="TArg0">
		/// The <see cref="ObjectContext"/> that encapsulates the model connection and metadata.
		/// </typeparam>
		/// <typeparam name="TArg1">
		/// Represents the type of the parameter that has to be passed in when executing the delegate returned
		/// by the Compile method.
		/// </typeparam>
		/// <typeparam name="TResult">
		/// The type of the query results returned by executing the delegate returned by the Compile method.
		/// </typeparam>
		/// <param name="query">The LINQ to Entities query expression to compile</param>
		/// <returns>The <see cref="CompiledQueryReplicator{TQuery,TKey}"/> created</returns>
		public static CompiledQueryReplicator<Func<TArg0, TArg1, TResult>, TKey> Create<TArg0, TArg1, TResult>(Expression<Func<TArg0, TArg1, TResult>> query)
			where TArg0 : ObjectContext
		{
			return new CompiledQueryReplicator<Func<TArg0, TArg1, TResult>, TKey>(query, CompiledQuery.Compile);
		}


		/// <summary>
		/// Creates a <see cref="CompiledQueryReplicator{TQuery,TKey}"/> that compiles LINQ to Entities
		/// queries.
		/// </summary>
		/// <typeparam name="TArg0">
		/// The <see cref="ObjectContext"/> that encapsulates the model connection and metadata.
		/// </typeparam>
		/// <typeparam name="TArg1">
		/// Represents the type of the parameter that has to be passed in when executing the delegate returned
		/// by the Compile method.
		/// </typeparam>
		/// <typeparam name="TArg2">
		/// Represents the type of the parameter that has to be passed in when executing the delegate returned
		/// by the Compile method.
		/// </typeparam>
		/// <typeparam name="TResult">
		/// The type of the query results returned by executing the delegate returned by the Compile method.
		/// </typeparam>
		/// <param name="query">The LINQ to Entities query expression to compile</param>
		/// <returns>The <see cref="CompiledQueryReplicator{TQuery,TKey}"/> created</returns>
		public static CompiledQueryReplicator<Func<TArg0, TArg1, TArg2, TResult>, TKey> Create<TArg0, TArg1, TArg2, TResult>(Expression<Func<TArg0, TArg1, TArg2, TResult>> query)
			where TArg0 : ObjectContext
		{
			return new CompiledQueryReplicator<Func<TArg0, TArg1, TArg2, TResult>, TKey>(query, CompiledQuery.Compile);
		}


		/// <summary>
		/// Creates a <see cref="CompiledQueryReplicator{TQuery,TKey}"/> that compiles LINQ to Entities
		/// queries.
		/// </summary>
		/// <typeparam name="TArg0">
		/// The <see cref="ObjectContext"/> that encapsulates the model connection and metadata.
		/// </typeparam>
		/// <typeparam name="TArg1">
		/// Represents the type of the parameter that has to be passed in when executing the delegate returned
		/// by the Compile method.
		/// </typeparam>
		/// <typeparam name="TArg2">
		/// Represents the type of the parameter that has to be passed in when executing the delegate returned
		/// by the Compile method.
		/// </typeparam>
		/// <typeparam name="TArg3">
		/// Represents the type of the parameter that has to be passed in when executing the delegate returned
		/// by the Compile method.
		/// </typeparam>
		/// <typeparam name="TResult">
		/// The type of the query results returned by executing the delegate returned by the Compile method.
		/// </typeparam>
		/// <param name="query">The LINQ to Entities query expression to compile</param>
		/// <returns>The <see cref="CompiledQueryReplicator{TQuery,TKey}"/> created</returns>
		public static CompiledQueryReplicator<Func<TArg0, TArg1, TArg2, TArg3, TResult>, TKey> Create<TArg0, TArg1, TArg2, TArg3, TResult>(Expression<Func<TArg0, TArg1, TArg2, TArg3, TResult>> query)
			where TArg0 : ObjectContext
		{
			return new CompiledQueryReplicator<Func<TArg0, TArg1, TArg2, TArg3, TResult>, TKey>(query, CompiledQuery.Compile);
		}
	}
}
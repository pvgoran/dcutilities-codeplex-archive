using System;


namespace DigitallyCreated.Utilities.Bcl.ExpressionTrees
{
	/// <summary>
	/// Marker methods that will be rewritten out of the expression trees they are used in by the 
	/// <see cref="ExpressionTreeRewriter"/>
	/// </summary>
	public static class RewriterMarkers
	{
		/// <summary>
		/// Tests if an entity is null by casting its primary key to a nullable type (if it is a value type)
		/// and comparing it with null.
		/// </summary>
		/// <remarks>
		/// This can avoid queries with performance issues when using LINQ to SQL and SQL CE 3.5
		/// </remarks>
		/// <typeparam name="T">The type of the primary key</typeparam>
		/// <param name="entityPrimaryKey">The entity's primary key</param>
		/// <returns>This never returns and will be rewritten out of the expression tree</returns>
		[RewriteUsingRewriterClass(typeof(EntityNullTestRewriter))]
		public static bool EntityNullTest<T>(T entityPrimaryKey)
		{
			throw new InvalidOperationException("Should not be executed. Should be rewritten out of the expression tree.");
		}
	}
}
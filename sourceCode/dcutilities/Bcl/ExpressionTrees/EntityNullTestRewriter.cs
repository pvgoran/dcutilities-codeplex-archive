using System;
using System.Linq.Expressions;


namespace DigitallyCreated.Utilities.Bcl.ExpressionTrees
{
	/// <summary>
	/// Rewrites a method (of form Func&lt;T,bool&gt;) into an expression where the expression of the method's
	/// only parameter is casted to a nullable type (if it is a value type) and then is checked that it is not
	/// equal to null, returning a boolean.
	/// </summary>
	/// <remarks>
	/// This rewriter can help avoid queries with performance issues when using LINQ to SQL and SQL CE 3.5 where
	/// testing an entity for null causes unwanted table scans. To use it, use the 
	/// <see cref="RewriterMarkers.EntityNullTest{T}"/> marker method in your queries.
	/// </remarks>
	/// <example>
	/// For example (where t.ID is an int),
	/// <code>
	/// query.Where(t =&gt; RewriterMarkers.EntityNullTest(t.ID))
	/// </code> 
	/// is rewritten into 
	/// <code>
	/// query.Where(t =&gt; (int?)t.ID != null)
	/// </code>
	/// </example>
	public class EntityNullTestRewriter : IExpressionRewriter
	{
		/// <summary>
		/// Rewrites the <paramref name="methodCallExpression"/> into another expression that can be substituted in
		/// its place
		/// </summary>
		/// <param name="methodCallExpression">The method call expression</param>
		/// <returns>The new expression to use instead of the method call expression</returns>
		public Expression RewriteMethodCall(MethodCallExpression methodCallExpression)
		{
			Expression primaryKeyAccessExpr = methodCallExpression.Arguments[0];

			Type typeToCastTo;
			if (primaryKeyAccessExpr.Type.IsValueType)
				typeToCastTo = typeof(Nullable<>).MakeGenericType(primaryKeyAccessExpr.Type);
			else
				typeToCastTo = primaryKeyAccessExpr.Type;

			UnaryExpression castExpr = Expression.MakeUnary(ExpressionType.Convert, primaryKeyAccessExpr, typeToCastTo);
			BinaryExpression compareToNullExpr = Expression.NotEqual(castExpr, Expression.Constant(null));

			return compareToNullExpr;
		}
	}
}
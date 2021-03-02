using System.Linq.Expressions;


namespace DigitallyCreated.Utilities.Bcl.ExpressionTrees
{
	/// <summary>
	/// An <see cref="IExpressionRewriter"/> is able to rewrite a marker method call into another expression.
	/// It is used by the <see cref="ExpressionTreeRewriter"/>.
	/// </summary>
	public interface IExpressionRewriter
	{
		/// <summary>
		/// Rewrites the <paramref name="methodCallExpression"/> into another expression that can be substituted in
		/// its place
		/// </summary>
		/// <param name="methodCallExpression">The method call expression</param>
		/// <returns>The new expression to use instead of the method call expression</returns>
		Expression RewriteMethodCall(MethodCallExpression methodCallExpression);
	}
}
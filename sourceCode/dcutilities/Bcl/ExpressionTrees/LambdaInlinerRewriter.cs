using System;
using System.Collections.Generic;
using System.Linq.Expressions;


namespace DigitallyCreated.Utilities.Bcl.ExpressionTrees
{
	/// <summary>
	/// The <see cref="LambdaInlinerRewriter"/> inlines the specified lambda expression into the expression tree
	/// it is rewriting at the marker method location.
	/// </summary>
	/// <remarks>
	/// The marker method and the lambda expression must take and return exactly the same parameters.
	/// </remarks>
	public class LambdaInlinerRewriter : IExpressionRewriter
	{
		private readonly LambdaExpression _TreeToInline;


		/// <summary>
		/// Constructor, creates a <see cref="LambdaInlinerRewriter"/>
		/// </summary>
		/// <param name="treeToInline">The lambda expression to inline</param>
		public LambdaInlinerRewriter(LambdaExpression treeToInline)
		{
			_TreeToInline = treeToInline;
		}


		/// <summary>
		/// Rewrites the <paramref name="methodCallExpression"/> into another expression that can be substituted in
		/// its place
		/// </summary>
		/// <param name="methodCallExpression">The method call expression</param>
		/// <returns>The new expression to use instead of the method call expression</returns>
		public Expression RewriteMethodCall(MethodCallExpression methodCallExpression)
		{
			if (methodCallExpression.Type != _TreeToInline.ReturnType)
				throw new InvalidOperationException(String.Format("Cannot inline lambda; the lambda's return type ({0}) is not the marker method return type ({1})", _TreeToInline.Type, methodCallExpression.Type));

			if (methodCallExpression.Arguments.Count != _TreeToInline.Parameters.Count)
				throw new InvalidOperationException(String.Format("Cannot inline lambda; marker method parameter count ({0}) is different to lambda parameter count ({1})", methodCallExpression.Arguments.Count, _TreeToInline.Parameters.Count));

			IDictionary<Expression, Expression> replacements = new Dictionary<Expression, Expression>();
			for (int i = 0; i < methodCallExpression.Arguments.Count; i++)
			{
				if (_TreeToInline.Parameters[i].Type != methodCallExpression.Arguments[i].Type)
					throw new InvalidOperationException(String.Format("Cannot inline lambda; marker method parameter (#{0}) type ({1}) is not the lambda parameter type ({2})", i, methodCallExpression.Arguments[i].Type, _TreeToInline.Parameters[i].Type));

				replacements.Add(_TreeToInline.Parameters[i], methodCallExpression.Arguments[i]);
			}

			return ((LambdaExpression)new ExpressionReplacerVisitor(replacements).Visit(_TreeToInline)).Body;
		}
	}
}
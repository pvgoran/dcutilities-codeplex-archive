using System;
using System.Collections.Generic;
using System.Linq.Expressions;


namespace DigitallyCreated.Utilities.Bcl.ExpressionTrees
{
	/// <summary>
	/// Utility class that contains some expression tree utilities
	/// </summary>
	public static class ExpressionUtil
	{
		/// <summary>
		/// Builds an OR expression tree where the value returned by <paramref name="valueSelector"/> is checked to
		/// see if it is any of the values found in <paramref name="wantedItems"/>.
		/// </summary>
		/// <typeparam name="T">The type from which the value is being selected</typeparam>
		/// <typeparam name="TProp">The type returned by the <paramref name="valueSelector"/> expression</typeparam>
		/// <param name="valueSelector">An expression that returns a value that is checked against</param>
		/// <param name="wantedItems">
		/// The values that are compared to the value returned by <paramref name="valueSelector"/>
		/// </param>
		/// <returns>
		/// The expression tree of a function that takes <typeparamref name="T"/> and returns true if the value
		/// returned by <paramref name="valueSelector"/> is any of the values in <paramref name="wantedItems"/>.
		/// </returns>
		public static Expression<Func<T, bool>> BuildOrExpression<T, TProp>(Expression<Func<T, TProp>> valueSelector, IEnumerable<TProp> wantedItems)
		{
			ParameterExpression inputParam = valueSelector.Parameters[0];

			Expression binaryExpressionTree = null;

			foreach (TProp item in wantedItems)
			{
				ConstantExpression constant = Expression.Constant(item, typeof(TProp));
				BinaryExpression comparison = Expression.Equal(valueSelector.Body, constant);

				if (binaryExpressionTree == null)
					binaryExpressionTree = comparison;
				else
					binaryExpressionTree = Expression.OrElse(binaryExpressionTree, comparison);
			}

			if (binaryExpressionTree == null)
				throw new ArgumentException("wantedItems must not be empty", "wantedItems");

			return Expression.Lambda<Func<T, bool>>(binaryExpressionTree, new[] { inputParam });
		}
	}
}
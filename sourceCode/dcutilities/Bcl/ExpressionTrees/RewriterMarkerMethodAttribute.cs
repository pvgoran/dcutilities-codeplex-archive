using System;


namespace DigitallyCreated.Utilities.Bcl.ExpressionTrees
{
	/// <summary>
	/// Marks a method as a method that should be replaced by the <see cref="ExpressionTreeRewriter"/>.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public abstract class RewriterMarkerMethodAttribute : Attribute
	{
		/// <summary>
		/// Creates an <see cref="IExpressionRewriter"/> that can rewrite out the marker method
		/// this attribute is applied to.
		/// </summary>
		/// <returns>The <see cref="IExpressionRewriter"/></returns>
		public abstract IExpressionRewriter CreateRewriter();
	}
}
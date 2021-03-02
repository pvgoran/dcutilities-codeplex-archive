using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace DigitallyCreated.Utilities.Bcl.ExpressionTrees
{
	/// <summary>
	/// Unit tests to test the <see cref="ExpressionUtil"/> class
	/// </summary>
	[TestClass]
	public class ExpressionUtilTests
	{
		/// <summary>
		/// Tests the <see cref="ExpressionUtil.BuildOrExpression{T,TProp}"/> method
		/// </summary>
		[TestMethod]
		public void TestBuildOrExpression()
		{
			int[] nums = new[] {1, 2, 3, 4, 5};
			int[] wanted = new[] {2, 3, 4};

			List<int> ints = nums.AsQueryable()
				.Where(ExpressionUtil.BuildOrExpression<int, int>(i => i, wanted))
				.ToList();

			Assert.AreEqual(2, ints[0]);
			Assert.AreEqual(3, ints[1]);
			Assert.AreEqual(4, ints[2]);
		}


		/// <summary>
		/// Tests the <see cref="ExpressionUtil.BuildOrExpression{T,TProp}"/> with an empty wanted array
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void TestBuildOrExpressionWithNullWanted()
		{
			int[] nums = new[] { 1, 2, 3, 4, 5 };
			int[] wanted = new int[0];

			nums.AsQueryable()
				.Where(ExpressionUtil.BuildOrExpression<int, int>(i => i, wanted))
				.ToList();
		}
	}
}
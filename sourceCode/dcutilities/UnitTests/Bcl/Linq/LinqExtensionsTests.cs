using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace DigitallyCreated.Utilities.Bcl.Linq
{
	/// <summary>
	/// Tests for the <see cref="LinqExtensions"/> class
	/// </summary>
	[TestClass]
	public class LinqExtensionsTests
	{
		/// <summary>
		/// Tests the <see cref="LinqExtensions.MatchUp{T}"/> method
		/// </summary>
		[TestMethod]
		public void TestMatchUp()
		{
			string[] first = { "a", "b", "c"};
			string[] second = { "b", "c", "d" };

			IList<MatchPair<string, string>> matchPairs = first.MatchUp(second).ToList();

			Assert.AreEqual(4, matchPairs.Count);

			Assert.AreEqual("a", matchPairs[0].First);
			Assert.AreEqual("b", matchPairs[1].First);
			Assert.AreEqual("c", matchPairs[2].First);
			Assert.IsNull(matchPairs[3].First);

			Assert.IsTrue(matchPairs[0].IsFirstSet);
			Assert.IsTrue(matchPairs[1].IsFirstSet);
			Assert.IsTrue(matchPairs[2].IsFirstSet);
			Assert.IsFalse(matchPairs[3].IsFirstSet);

			Assert.IsNull(matchPairs[0].Second);
			Assert.AreEqual("b", matchPairs[1].Second);
			Assert.AreEqual("c", matchPairs[2].Second);
			Assert.AreEqual("d", matchPairs[3].Second);

			Assert.IsFalse(matchPairs[0].IsSecondSet);
			Assert.IsTrue(matchPairs[1].IsSecondSet);
			Assert.IsTrue(matchPairs[2].IsSecondSet);
			Assert.IsTrue(matchPairs[3].IsSecondSet);
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace DigitallyCreated.Utilities.Bcl.Linq
{
	/// <summary>
	/// This class tests the <see cref="FederatorExtensions"/> and the <see cref="Federator{T}"/> classes.
	/// </summary>
	[TestClass]
	public class FederatorExtensionsTests
	{
		/// <summary>
		/// Tests the federation between two string sequences using the default key selector and comparator
		/// </summary>
		[TestMethod]
		public void TestFederationBetweenTwoStringSequences()
		{
			string[] firstSequence = new[] { "A", "B", "C" };
			string[] secondSequence = new[] { "B", "C", "D" };

			IList<IGrouping<string, FederatedGroupItem<string>>> federatedGroups = firstSequence.Federate(secondSequence).OrderBy(g => g.Key).ToList();
			IList<FederatedGroupItem<string>> groupItems;
			
			Assert.AreEqual(4, federatedGroups.Count);

			Assert.AreEqual("A", federatedGroups[0].Key);
			groupItems = federatedGroups[0].OrderBy(gi => gi.Item).ToList();
			Assert.AreEqual(1, groupItems.Count);
			Assert.AreEqual("A", groupItems[0].Item);
			Assert.AreEqual(0, groupItems[0].SequenceIndex);

			Assert.AreEqual("B", federatedGroups[1].Key);
			groupItems = federatedGroups[1].OrderBy(gi => gi.Item).ToList();
			Assert.AreEqual(2, groupItems.Count);
			Assert.AreEqual("B", groupItems[0].Item);
			Assert.AreEqual("B", groupItems[1].Item);
			Assert.AreEqual(0, groupItems[0].SequenceIndex);
			Assert.AreEqual(1, groupItems[1].SequenceIndex);

			Assert.AreEqual("C", federatedGroups[2].Key);
			groupItems = federatedGroups[2].OrderBy(gi => gi.Item).ToList();
			Assert.AreEqual(2, groupItems.Count);
			Assert.AreEqual("C", groupItems[0].Item);
			Assert.AreEqual("C", groupItems[1].Item);
			Assert.AreEqual(0, groupItems[0].SequenceIndex);
			Assert.AreEqual(1, groupItems[1].SequenceIndex);

			Assert.AreEqual("D", federatedGroups[3].Key);
			groupItems = federatedGroups[3].OrderBy(gi => gi.Item).ToList();
			Assert.AreEqual(1, groupItems.Count);
			Assert.AreEqual("D", groupItems[0].Item);
			Assert.AreEqual(1, groupItems[0].SequenceIndex);
		}


		/// <summary>
		/// Tests federation between three sequences of objects where the key selector selects a string property
		/// on the object and the comparator does a case insensitive string comparison.
		/// </summary>
		[TestMethod]
		public void TestFederationBetweenThreeObjectSequences()
		{
			Person[] firstSequence = new[]
			                         	{
											new Person { Name = "Daniel" },
											new Person { Name = "dwain" },
			                         	};

			Person[] secondSequence = new[]
			                         	{
											new Person { Name = "daniel" },
											new Person { Name = "Sean" },
			                         	};

			Person[] thirdSequence = new[]
			                         	{
											new Person { Name = "Dwain" },
			                         	};

			IFederable<Person> federable = firstSequence.FederateWith(secondSequence).FederateWith(thirdSequence);
			IList<IGrouping<string, FederatedGroupItem<Person>>> federatedGroups = federable.Federate(p => p.Name, StringComparer.InvariantCultureIgnoreCase).OrderBy(g => g.Key).ToList();
			IList<FederatedGroupItem<Person>> groupItems;

			Assert.AreEqual(3, federatedGroups.Count);

			Assert.AreEqual("Daniel", federatedGroups[0].Key);
			groupItems = federatedGroups[0].OrderBy(gi => gi.Item.Name).ToList();
			Assert.AreEqual(2, groupItems.Count);
			Assert.AreSame(secondSequence[0], groupItems[0].Item);
			Assert.AreSame(firstSequence[0], groupItems[1].Item);
			Assert.AreEqual(1, groupItems[0].SequenceIndex);
			Assert.AreEqual(0, groupItems[1].SequenceIndex);

			Assert.AreEqual("dwain", federatedGroups[1].Key);
			groupItems = federatedGroups[1].OrderBy(gi => gi.Item.Name).ToList();
			Assert.AreEqual(2, groupItems.Count);
			Assert.AreSame(firstSequence[1], groupItems[0].Item);
			Assert.AreSame(thirdSequence[0], groupItems[1].Item);
			Assert.AreEqual(0, groupItems[0].SequenceIndex);
			Assert.AreEqual(2, groupItems[1].SequenceIndex);

			Assert.AreEqual("Sean", federatedGroups[2].Key);
			groupItems = federatedGroups[2].OrderBy(gi => gi.Item.Name).ToList();
			Assert.AreEqual(1, groupItems.Count);
			Assert.AreSame(secondSequence[1], groupItems[0].Item);
			Assert.AreEqual(1, groupItems[0].SequenceIndex);
		}


		/// <summary>
		/// Helper class used in tests
		/// </summary>
		private class Person
		{
			public string Name;
		}
	}
}
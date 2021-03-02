using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace DigitallyCreated.Utilities.Bcl.Linq
{
	/// <summary>
	/// Tests to test <see cref="Sorter{T}"/>
	/// </summary>
	[TestClass]
	public class SorterTests
	{
		/// <summary>
		/// Gets or sets the test context which provides information about and functionality for the current
		/// test run.
		///</summary>
		public TestContext TestContext { get; set; }


		/// <summary>
		/// Tests simple sorting
		/// </summary>
		[TestMethod]
		public void TestSimpleSorting()
		{
			IList<SortClass> data = CreateSimpleSortData();

			Sorter<SortClass> sorter = new Sorter<SortClass>();
			sorter.AddProperty(x => x.A, true);
			sorter.AddProperty(x => x.B, false);

			List<SortClass> sortedData = data.AsQueryable().OrderBy(sorter).ToList();

			Assert.AreSame(data[3], sortedData[0]);
			Assert.AreSame(data[1], sortedData[1]);
			Assert.AreSame(data[2], sortedData[2]);
			Assert.AreSame(data[0], sortedData[3]);
			Assert.AreSame(data[4], sortedData[4]);
		}


		/// <summary>
		/// Tests simple sorting using a deserialized sorter
		/// </summary>
		[TestMethod]
		public void TestSimpleSortingAfterDeserialization()
		{
			IList<SortClass> data = CreateSimpleSortData();

			Sorter<SortClass> sorter = new Sorter<SortClass>();
			sorter.AddProperty(x => x.A, true);
			sorter.AddProperty(x => x.B, false);

			sorter = SerializeAndDeserializeSorter(sorter);

			List<SortClass> sortedData = data.AsQueryable().OrderBy(sorter).ToList();

			Assert.AreSame(data[3], sortedData[0]);
			Assert.AreSame(data[1], sortedData[1]);
			Assert.AreSame(data[2], sortedData[2]);
			Assert.AreSame(data[0], sortedData[3]);
			Assert.AreSame(data[4], sortedData[4]);
		}


		/// <summary>
		/// Tests cloning functionality
		/// </summary>
		[TestMethod]
		public void TestCloningAndSorting()
		{
			IList<SortClass> data = CreateSimpleSortData();

			Sorter<SortClass> sorter = new Sorter<SortClass>();
			sorter.AddProperty(x => x.A, true);
			sorter.AddProperty(x => x.B, false);
			Sorter<SortClass> clonedSorter = sorter.Clone().SortBy(x => x.B, true);

			List<SortClass> sortedData = data.AsQueryable().OrderBy(clonedSorter).ToList();

			Assert.AreSame(data[3], sortedData[0]);
			Assert.AreSame(data[0], sortedData[1]);
			Assert.AreSame(data[2], sortedData[2]);
			Assert.AreSame(data[1], sortedData[3]);
			Assert.AreSame(data[4], sortedData[4]);

			sortedData = data.AsQueryable().OrderBy(sorter).ToList();

			Assert.AreSame(data[3], sortedData[0]);
			Assert.AreSame(data[1], sortedData[1]);
			Assert.AreSame(data[2], sortedData[2]);
			Assert.AreSame(data[0], sortedData[3]);
			Assert.AreSame(data[4], sortedData[4]);
		}


		/// <summary>
		/// Tests simple decoding functionality
		/// </summary>
		[TestMethod]
		public void TestSimpleDecodingAndSorting()
		{
			IList<SortClass> data = CreateSimpleSortData();

			Sorter<SortClass> sorter = Sorter<SortClass>.Decode(null, "A'B!");
			List<SortClass> sortedData = data.AsQueryable().OrderBy(sorter).ToList();

			Assert.AreSame(data[3], sortedData[0]);
			Assert.AreSame(data[1], sortedData[1]);
			Assert.AreSame(data[2], sortedData[2]);
			Assert.AreSame(data[0], sortedData[3]);
			Assert.AreSame(data[4], sortedData[4]);

			sorter = new Sorter<SortClass>(null, "A", "B!");
			sortedData = data.AsQueryable().OrderBy(sorter).ToList();

			Assert.AreSame(data[3], sortedData[0]);
			Assert.AreSame(data[1], sortedData[1]);
			Assert.AreSame(data[2], sortedData[2]);
			Assert.AreSame(data[0], sortedData[3]);
			Assert.AreSame(data[4], sortedData[4]);
		}


		/// <summary>
		/// Tests decoding with prefixes functionality
		/// </summary>
		[TestMethod]
		public void TestPrefixedDecodingAndSorting()
		{
			IList<SortClass> data = CreateSimpleSortData();

			Sorter<SortClass> sorter = new Sorter<SortClass>("p", "p.A", "p.B!");
			List<SortClass> sortedData = data.AsQueryable().OrderBy(sorter).ToList();

			Assert.AreSame(data[3], sortedData[0]);
			Assert.AreSame(data[1], sortedData[1]);
			Assert.AreSame(data[2], sortedData[2]);
			Assert.AreSame(data[0], sortedData[3]);
			Assert.AreSame(data[4], sortedData[4]);

			sorter = Sorter<SortClass>.Decode("p", "p.A'p.B!");
			sortedData = data.AsQueryable().OrderBy(sorter).ToList();

			Assert.AreSame(data[3], sortedData[0]);
			Assert.AreSame(data[1], sortedData[1]);
			Assert.AreSame(data[2], sortedData[2]);
			Assert.AreSame(data[0], sortedData[3]);
			Assert.AreSame(data[4], sortedData[4]);
		}


		/// <summary>
		/// Tests decoding with translation dictionary functionality using a deserialized sorter
		/// </summary>
		[TestMethod]
		public void TestTranslationDictionaryDecodingAndSortingAfterDeserialization()
		{
			IList<SortClass> data = CreateSimpleSortData();
			IDictionary<Expression<Func<SortClass, object>>, string> translationDictionary = new Dictionary<Expression<Func<SortClass, object>>, string>
			                                                    	{
			                                                    		{s => s.A, "0"},
			                                                    		{s => s.B, "1"},
			                                                    	};

			Sorter<SortClass> sorter = new Sorter<SortClass>("n", translationDictionary, "n.0", "n.1!");
			sorter = SerializeAndDeserializeSorter(sorter);

			List<SortClass> sortedData = data.AsQueryable().OrderBy(sorter).ToList();

			Assert.AreSame(data[3], sortedData[0]);
			Assert.AreSame(data[1], sortedData[1]);
			Assert.AreSame(data[2], sortedData[2]);
			Assert.AreSame(data[0], sortedData[3]);
			Assert.AreSame(data[4], sortedData[4]);

			translationDictionary = new Dictionary<Expression<Func<SortClass, object>>, string>
			                        	{
			                        		{s => s.A, "0"},
			                        	};

			sorter = Sorter<SortClass>.Decode("n", translationDictionary, "n.0'n.B!");
			sortedData = data.AsQueryable().OrderBy(sorter).ToList();

			Assert.AreSame(data[3], sortedData[0]);
			Assert.AreSame(data[1], sortedData[1]);
			Assert.AreSame(data[2], sortedData[2]);
			Assert.AreSame(data[0], sortedData[3]);
			Assert.AreSame(data[4], sortedData[4]);
		}


		/// <summary>
		/// Tests decoding with translation dictionary functionality
		/// </summary>
		[TestMethod]
		public void TestTranslationDictionaryDecodingAndSorting()
		{
			IList<SortClass> data = CreateSimpleSortData();
			IDictionary<Expression<Func<SortClass, object>>, string> translationDictionary = new Dictionary<Expression<Func<SortClass, object>>, string>
			                                                    	{
			                                                    		{s => s.A, "0"},
			                                                    		{s => s.B, "1"},
			                                                    	};

			Sorter<SortClass> sorter = new Sorter<SortClass>("n", translationDictionary, "n.0", "n.1!");
			List<SortClass> sortedData = data.AsQueryable().OrderBy(sorter).ToList();

			Assert.AreSame(data[3], sortedData[0]);
			Assert.AreSame(data[1], sortedData[1]);
			Assert.AreSame(data[2], sortedData[2]);
			Assert.AreSame(data[0], sortedData[3]);
			Assert.AreSame(data[4], sortedData[4]);

			translationDictionary = new Dictionary<Expression<Func<SortClass, object>>, string>
			                        	{
			                        		{s => s.A, "0"},
			                        	};

			sorter = Sorter<SortClass>.Decode("n", translationDictionary, "n.0'n.B!");
			sortedData = data.AsQueryable().OrderBy(sorter).ToList();

			Assert.AreSame(data[3], sortedData[0]);
			Assert.AreSame(data[1], sortedData[1]);
			Assert.AreSame(data[2], sortedData[2]);
			Assert.AreSame(data[0], sortedData[3]);
			Assert.AreSame(data[4], sortedData[4]);
		}


		/// <summary>
		/// Tests the <see cref="Sorter{T}.SortBy"/> functionality
		/// </summary>
		[TestMethod]
		public void TestSortBy()
		{
			IList<SortClass> data = CreateSimpleSortData();
			IDictionary<Expression<Func<SortClass, object>>, string> translationDictionary = new Dictionary<Expression<Func<SortClass, object>>, string>
			                                                    	{
			                                                    		{s => s.A, "0"},
			                                                    		{s => s.B, "1"},
			                                                    	};

			Sorter<SortClass> sorter = new Sorter<SortClass>("n", translationDictionary, "n.0", "n.1!")
				.SortBy(x => x.A, false)
				.SortBy("n.B");

			List<SortClass> sortedData = data.AsQueryable().OrderBy(sorter).ToList();

			Assert.AreSame(data[0], sortedData[0]);
			Assert.AreSame(data[3], sortedData[1]);
			Assert.AreSame(data[2], sortedData[2]);
			Assert.AreSame(data[1], sortedData[3]);
			Assert.AreSame(data[4], sortedData[4]);
		}


		/// <summary>
		/// Tests the <see cref="Sorter{T}.AddAllProperties"/> functionality
		/// </summary>
		[TestMethod]
		public void TestAddAllPropertiesSorter()
		{
			IList<SortClass> data = CreateSimpleSortData();

			Sorter<SortClass> sorter = new Sorter<SortClass>().AddAllProperties(true);
			List<SortClass> sortedData = data.AsQueryable().OrderBy(sorter).ToList();

			Assert.AreSame(data[3], sortedData[0]);
			Assert.AreSame(data[0], sortedData[1]);
			Assert.AreSame(data[2], sortedData[2]);
			Assert.AreSame(data[1], sortedData[3]);
			Assert.AreSame(data[4], sortedData[4]);
		}


		/// <summary>
		/// Tests that using a default sorter should crash
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(SorterException))]
		public void TestDefaultSorter()
		{
			IList<SortClass> data = CreateSimpleSortData();

			Sorter<SortClass> sorter = new Sorter<SortClass>();
			data.AsQueryable().OrderBy(sorter).ToList();
		}


		/// <summary>
		/// Tests encoding
		/// </summary>
		[TestMethod]
		public void TestEncoding()
		{
			Sorter<SortClass> sorter = new Sorter<SortClass>();
			sorter.AddProperty(x => x.A, true);
			sorter.AddProperty(x => x.B, true);

			Assert.AreEqual("A'B", sorter.Encode());

			sorter.SortBy(x => x.A, false).SortBy(x => x.B, false);
			Assert.AreEqual("B!'A!", sorter.Encode());

			sorter.SortBy(x => x.A, true);
			Assert.AreEqual("A'B!", sorter.Encode());
		}


		/// <summary>
		/// Tests encoding with prefixes
		/// </summary>
		[TestMethod]
		public void TestEncodingWithPrefix()
		{
			Sorter<SortClass> sorter = new Sorter<SortClass>("e");
			sorter.AddProperty(x => x.A, true);
			sorter.AddProperty(x => x.B, true);

			Assert.AreEqual("e.A'e.B", sorter.Encode());

			sorter.SortBy(x => x.A, false).SortBy(x => x.B, false);
			Assert.AreEqual("e.B!'e.A!", sorter.Encode());

			sorter.SortBy(x => x.A, true);
			Assert.AreEqual("e.A'e.B!", sorter.Encode());
		}


		/// <summary>
		/// Tests encoding with translation dictionaries
		/// </summary>
		[TestMethod]
		public void TestEncodingWithTranslationDictionary()
		{
			IDictionary<Expression<Func<SortClass, object>>, string> translationDictionary = new Dictionary<Expression<Func<SortClass, object>>, string>
			                                                    	{
			                                                    		{s => s.A, "0"},
			                                                    		{s => s.B, "1"},
			                                                    	};

			Sorter<SortClass> sorter = new Sorter<SortClass>(translationDictionary);
			sorter.AddProperty(x => x.A, true);
			sorter.AddProperty(x => x.B, true);

			Assert.AreEqual("0'1", sorter.Encode());

			sorter.SortBy(x => x.A, false).SortBy(x => x.B, false);
			Assert.AreEqual("1!'0!", sorter.Encode());

			sorter.SortBy(x => x.A, true);
			Assert.AreEqual("0'1!", sorter.Encode());
		}


		/// <summary>
		/// Tests encoding with both prefixes and translation dictionary
		/// </summary>
		[TestMethod]
		public void TestEncodingWithPrefixAndTranslationDictionary()
		{
			IDictionary<Expression<Func<SortClass, object>>, string> translationDictionary = new Dictionary<Expression<Func<SortClass, object>>, string>
			                                                    	{
			                                                    		{s => s.A, "0"},
			                                                    		{s => s.B, "1"},
			                                                    	};

			Sorter<SortClass> sorter = new Sorter<SortClass>("e", translationDictionary);
			sorter.AddProperty(x => x.A, true);
			sorter.AddProperty(x => x.B, true);

			Assert.AreEqual("e.0'e.1", sorter.Encode());

			sorter.SortBy(x => x.A, false).SortBy(x => x.B, false);
			Assert.AreEqual("e.1!'e.0!", sorter.Encode());

			sorter.SortBy(x => x.A, true);
			Assert.AreEqual("e.0'e.1!", sorter.Encode());
		}


		/// <summary>
		/// Tests encoding with both prefixes and translation dictionary using a deserialized sorted
		/// </summary>
		[TestMethod]
		public void TestEncodingWithPrefixAndTranslationDictionaryAfterDeserialization()
		{
			IDictionary<Expression<Func<SortClass, object>>, string> translationDictionary = new Dictionary<Expression<Func<SortClass, object>>, string>
			                                                    	{
			                                                    		{s => s.A, "0"},
			                                                    		{s => s.B, "1"},
			                                                    	};

			Sorter<SortClass> sorter = new Sorter<SortClass>("e", translationDictionary);
			sorter = SerializeAndDeserializeSorter(sorter);

			sorter.AddProperty(x => x.A, true);
			sorter.AddProperty(x => x.B, true);

			Assert.AreEqual("e.0'e.1", sorter.Encode());

			sorter.SortBy(x => x.A, false).SortBy(x => x.B, false);
			Assert.AreEqual("e.1!'e.0!", sorter.Encode());

			sorter.SortBy(x => x.A, true);
			Assert.AreEqual("e.0'e.1!", sorter.Encode());
		}


		/// <summary>
		/// Tests that exceptions are thrown when trying to decode a broken string
		/// </summary>
		[TestMethod]
		public void TestDecodingInvalidString()
		{
			IDictionary<Expression<Func<SortClass, object>>, string> dict = new Dictionary<Expression<Func<SortClass, object>>, string>
			                                                    	{
			                                                    		{s => s.A, "0"},
			                                                    		{s => s.B, "1"},
			                                                    	};

			TestHelper.ExpectException<SorterException>(() => Sorter<SortClass>.Decode(null, "A'D"));
			TestHelper.ExpectException<SorterException>(() => Sorter<SortClass>.Decode(null, "AD"));
			TestHelper.ExpectException<SorterException>(() => Sorter<SortClass>.Decode(null, "!AD"));
			TestHelper.ExpectException<SorterException>(() => Sorter<SortClass>.Decode(null, "a.A'B"));
			TestHelper.ExpectException<SorterException>(() => Sorter<SortClass>.Decode("a", "A!'B"));
			TestHelper.ExpectException<SorterException>(() => Sorter<SortClass>.Decode("a", "a.A!'B"));
			TestHelper.ExpectException<SorterException>(() => Sorter<SortClass>.Decode("a", dict, "a.0!'1"));
			TestHelper.ExpectException<SorterException>(() => Sorter<SortClass>.Decode("a", dict, "a.0!'11"));
			TestHelper.ExpectException<SorterException>(() => Sorter<SortClass>.Decode(null, dict, "a.0!'1"));
		}


		/// <summary>
		/// Tests to ensure various methods, when passed invalid arguments, will crash acceptably
		/// </summary>
		[TestMethod]
		public void TestInvalidConfigurations()
		{
			TestHelper.ExpectException<ArgumentException>(() => new Sorter<SortClass>("a.b"));

			Sorter<SortClass> sorter = new Sorter<SortClass>();

			TestHelper.ExpectException<ArgumentException>(() => sorter.AddProperty(s => s.ToString(), true));
			TestHelper.ExpectException<ArgumentException>(() => sorter.AddProperty(s => sorter.OrderedPropertySelectors, true));

			IDictionary<Expression<Func<SortClass, object>>, string> dict = new Dictionary<Expression<Func<SortClass, object>>, string>
			                                                    	{
			                                                    		{s => s.ToString(), "0"},
			                                                    	};

			TestHelper.ExpectException<ArgumentException>(() => new Sorter<SortClass>(dict));
		}


		/// <summary>
		/// Tests sorting on properties on objects that are inside the sortable object
		/// </summary>
		[TestMethod]
		public void TestDeepSort()
		{
			List<SortClass> data = new List<SortClass>
			               	{
			               		new SortClass("Daniel", "Apple", new SortClass("Microsoft", "Socks")),     //0
			               		new SortClass("Daniel", "Peach", new SortClass("Microsoft", "Socks")),     //1
			               		new SortClass("Daniel", "Orange", new SortClass("Atlassian", "Hat")),      //2
			               		new SortClass("Alex", "Apple", new SortClass("Atlassian", "Pants")),       //3
			               		new SortClass("Dwain", "Strawberry", new SortClass("JetBrains", "Jumper")) //4
			               	};

			Sorter<SortClass> sorter = new Sorter<SortClass>()
												.AddProperty(s => s.C.A, true)
												.AddProperty(s => s.C.B, true)
												.AddProperty(s => s.B, false);

			List<SortClass> sortedData = data.AsQueryable().OrderBy(sorter).ToList();

			Assert.AreSame(data[2], sortedData[0]);
			Assert.AreSame(data[3], sortedData[1]);
			Assert.AreSame(data[4], sortedData[2]);
			Assert.AreSame(data[1], sortedData[3]);
			Assert.AreSame(data[0], sortedData[4]);
		}


		/// <summary>
		/// Tests sorting on properties on objects that are inside the sortable object using a deserialized
		/// sorter
		/// </summary>
		[TestMethod]
		public void TestDeepSortAfterDeserialization()
		{
			List<SortClass> data = new List<SortClass>
			               	{
			               		new SortClass("Daniel", "Apple", new SortClass("Microsoft", "Socks")),     //0
			               		new SortClass("Daniel", "Peach", new SortClass("Microsoft", "Socks")),     //1
			               		new SortClass("Daniel", "Orange", new SortClass("Atlassian", "Hat")),      //2
			               		new SortClass("Alex", "Apple", new SortClass("Atlassian", "Pants")),       //3
			               		new SortClass("Dwain", "Strawberry", new SortClass("JetBrains", "Jumper")) //4
			               	};

			Sorter<SortClass> sorter = new Sorter<SortClass>()
												.AddProperty(s => s.C.A, true)
												.AddProperty(s => s.C.B, true)
												.AddProperty(s => s.B, false);
			sorter = SerializeAndDeserializeSorter(sorter);

			List<SortClass> sortedData = data.AsQueryable().OrderBy(sorter).ToList();

			Assert.AreSame(data[2], sortedData[0]);
			Assert.AreSame(data[3], sortedData[1]);
			Assert.AreSame(data[4], sortedData[2]);
			Assert.AreSame(data[1], sortedData[3]);
			Assert.AreSame(data[0], sortedData[4]);
		}


		/// <summary>
		/// Tests sorting on an int property while using a translation dictionary to test sorting against value
		/// types. This is needed because value typed properties generate a polluted translation dictionary 
		/// property selector because they are auto-boxed to return object.
		/// </summary>
		[TestMethod]
		public void TestIntSortWithTranslationDictionary()
		{
			List<SortClass> data = new List<SortClass>
			                       	{
			                       		new SortClass("Daniel", "Apple", 2),     //0
										new SortClass("Daniel", "Peach", 4),     //1
										new SortClass("Daniel", "Orange", 1),    //2
										new SortClass("Alex", "Apple", 6),       //3
										new SortClass("Dwain", "Strawberry", 0), //4
			                       	};

			IDictionary<Expression<Func<SortClass, object>>, string> dict = new Dictionary<Expression<Func<SortClass, object>>, string>
			                                                    	{
																		{s => s.Z, "2"},
			                                                    		{s => s.A, "0"},
			                                                    		{s => s.B, "1"},
			                                                    	};

			Sorter<SortClass> sorter = new Sorter<SortClass>(dict)
				.AddProperty(s => s.Z, true)
				.AddProperty(s => s.A, true)
				.AddProperty(s => s.B, true);

			List<SortClass> sortedData = data.AsQueryable().OrderBy(sorter).ToList();

			Assert.AreSame(data[4], sortedData[0]);
			Assert.AreSame(data[2], sortedData[1]);
			Assert.AreSame(data[0], sortedData[2]);
			Assert.AreSame(data[1], sortedData[3]);
			Assert.AreSame(data[3], sortedData[4]);
		}


		/// <summary>
		/// Tests sorting on an int property while using a translation dictionary to test sorting against value
		/// types using a deserialized sorter. This is needed because value typed properties generate a
		/// polluted translation dictionary  property selector because they are auto-boxed to return object.
		/// </summary>
		[TestMethod]
		public void TestIntSortWithTranslationDictionaryAfterDeserialization()
		{
			List<SortClass> data = new List<SortClass>
			                       	{
			                       		new SortClass("Daniel", "Apple", 2),     //0
			                       		new SortClass("Daniel", "Peach", 4),     //1
			                       		new SortClass("Daniel", "Orange", 1),    //2
			                       		new SortClass("Alex", "Apple", 6),       //3
			                       		new SortClass("Dwain", "Strawberry", 0), //4
			                       	};

			IDictionary<Expression<Func<SortClass, object>>, string> dict = new Dictionary<Expression<Func<SortClass, object>>, string>
			                                                                	{
			                                                                		{s => s.Z, "2"},
			                                                                		{s => s.A, "0"},
			                                                                		{s => s.B, "1"},
			                                                                	};

			Sorter<SortClass> sorter = new Sorter<SortClass>(dict)
				.AddProperty(s => s.Z, true)
				.AddProperty(s => s.A, true)
				.AddProperty(s => s.B, true);
			sorter = SerializeAndDeserializeSorter(sorter);

			List<SortClass> sortedData = data.AsQueryable().OrderBy(sorter).ToList();

			Assert.AreSame(data[4], sortedData[0]);
			Assert.AreSame(data[2], sortedData[1]);
			Assert.AreSame(data[0], sortedData[2]);
			Assert.AreSame(data[1], sortedData[3]);
			Assert.AreSame(data[3], sortedData[4]);
		}


		/// <summary>
		/// Creates a simple data set to sort
		/// </summary>
		/// <returns></returns>
		private IList<SortClass> CreateSimpleSortData()
		{
			return new List<SortClass>
			       	{
			       		new SortClass("Daniel", "Apple"),    //0
			       		new SortClass("Daniel", "Peach"),    //1
			       		new SortClass("Daniel", "Orange"),   //2
			       		new SortClass("Alex", "Apple"),      //3
			       		new SortClass("Dwain", "Strawberry") //4
			       	};
		}


		/// <summary>
		/// Serializes a sorter the returns the deserialized result
		/// </summary>
		/// <typeparam name="T">The sortable type</typeparam>
		/// <param name="sorter">The sorter</param>
		/// <returns>The deserialized sorter</returns>
		private Sorter<T> SerializeAndDeserializeSorter<T>(Sorter<T> sorter)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(sorter.GetType());
			StringWriter stringWriter = new StringWriter();
			xmlSerializer.Serialize(stringWriter, sorter);
			return (Sorter<T>)xmlSerializer.Deserialize(new StringReader(stringWriter.ToString()));
		}


		/// <summary>
		/// A simple class that can be sorted
		/// </summary>
		public class SortClass
		{
			public string A { get; private set; }
			public string B { get; private set; }
			public SortClass C { get; private set; }
			public int Z { get; private set; }

			public SortClass(string a, string b)
			{
				A = a;
				B = b;
			}

			public SortClass(string a, string b, int z)
				: this(a,b)
			{
				Z = z;
			}

			public SortClass(string a, string b, SortClass c)
				: this(a, b)
			{
				C = c;
			}
		}
	}
}
using DigitallyCreated.Utilities.Ef.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace DigitallyCreated.Utilities.Ef
{
	/// <summary>
	/// Tests to test <see cref="EfUtil"/>
	/// </summary>
	[TestClass]
	public class EfUtilTests
	{
		/// <summary>
		/// Gets or sets the test context which provides information about and functionality for the current
		/// test run.
		///</summary>
		public TestContext TestContext { get; set; }


		/// <summary>
		/// Tests <see cref="EfUtil.ClearNonScalarProperties(System.Data.Objects.DataClasses.EntityObject,bool)"/>
		/// </summary>
		[TestMethod]
		public void TestClearNonScalarPropertiesWithEntityKeyErasure()
		{
			Author author = new Author();
			author.ID = 23;
			author.Name = "Daniel Chambers";
			Book book = new Book();
			book.ID = 12;
			book.Name = "Awesome Book";
			book.Author = author;

			EfUtil.ClearNonScalarProperties(author, true);

			Assert.AreEqual(0, author.Books.Count);
			Assert.AreEqual(0, author.ID);
			Assert.AreEqual("Daniel Chambers", author.Name);

			author = new Author();
			author.ID = 23;
			author.Name = "Daniel Chambers";
			book = new Book();
			book.ID = 12;
			book.Name = "Awesome Book";
			author.Books.Add(book);

			EfUtil.ClearNonScalarProperties(book, true);

			Assert.IsNull(book.Author);
			Assert.AreEqual(0, book.ID);
			Assert.AreEqual("Awesome Book", book.Name);
		}


		/// <summary>
		/// Tests <see cref="EfUtil.ClearNonScalarProperties(System.Data.Objects.DataClasses.EntityObject)"/>
		/// </summary>
		[TestMethod]
		public void TestClearNonScalarProperties()
		{
			Author author = new Author();
			author.ID = 23;
			author.Name = "Daniel Chambers";
			Book book = new Book();
			book.ID = 12;
			book.Name = "Awesome Book";
			book.Author = author;

			EfUtil.ClearNonScalarProperties(author);

			Assert.AreEqual(0, author.Books.Count);
			Assert.AreEqual(23, author.ID);
			Assert.AreEqual("Daniel Chambers", author.Name);

			author = new Author();
			author.ID = 23;
			author.Name = "Daniel Chambers";
			book = new Book();
			book.ID = 12;
			book.Name = "Awesome Book";
			author.Books.Add(book);

			EfUtil.ClearNonScalarProperties(book);

			Assert.IsNull(book.Author);
			Assert.AreEqual(12, book.ID);
			Assert.AreEqual("Awesome Book", book.Name);
		}


		/// <summary>
		/// Tests <see cref="EfUtil.CopyScalarProperties{T}"/>
		/// </summary>
		[TestMethod]
		public void TestCopyScalarProperties()
		{
			Book book = new Book();
			book.ID = 12;
			book.Name = "Awesome Book";
			book.Author = new Author();
			Book otherBook = new Book();

			EfUtil.CopyScalarProperties(book, otherBook, false);

			Assert.AreEqual(0, otherBook.ID);
			Assert.AreEqual(book.Name, otherBook.Name);
			Assert.IsNull(otherBook.Author);

			otherBook = new Book();
			EfUtil.CopyScalarProperties(book, otherBook, true);

			Assert.AreEqual(book.ID, otherBook.ID);
			Assert.AreEqual(book.Name, otherBook.Name);
			Assert.IsNull(otherBook.Author);
		}
	}
}
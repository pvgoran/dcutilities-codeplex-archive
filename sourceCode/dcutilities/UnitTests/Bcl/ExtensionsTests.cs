using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace DigitallyCreated.Utilities.Bcl
{
	/// <summary>
	/// Unit tests for the <see cref="Extensions"/> class
	/// </summary>
	[TestClass]
	public class ExtensionsTests
	{
		/// <summary>
		/// Tests the <see cref="Extensions.SafeUsingBlock{TDisposable}(TDisposable,System.Action{TDisposable})"/> 
		/// method with an exception thrown in the body and in the dispose
		/// </summary>
		[TestMethod]
		public void TestSafeUsingBlockWithAggregateException()
		{
			try
			{
				(new ErrorDisposable()).SafeUsingBlock(myDisposable =>
				{
					throw new Exception("SafeUsingBlock");
				});
			}
			catch (AggregateException e)
			{
				IList<Exception> exceptions = e.InnerExceptions.ToList();

				Assert.AreEqual(2, exceptions.Count);
				Assert.AreEqual("SafeUsingBlock", exceptions[0].Message);
				Assert.AreEqual("Dispose", exceptions[1].Message);
				return;
			}

			Assert.Fail("No exception or incorrect exception thrown");
		}


		/// <summary>
		/// Tests the <see cref="Extensions.SafeUsingBlock{TDisposable}(TDisposable,System.Action{TDisposable})"/> 
		/// method with an exception thrown in the body but not in the dispose
		/// </summary>
		[TestMethod]
		public void TestSafeUsingBlockWithOnlyErrorInBlock()
		{
			NoErrorDisposable errorDispose = new NoErrorDisposable();
			try
			{
				errorDispose.SafeUsingBlock(myDisposable =>
				{
					throw new Exception("SafeUsingBlock");
				});
			}
			catch (Exception e)
			{
				Assert.AreEqual("SafeUsingBlock", e.Message);
				Assert.IsTrue(errorDispose.Disposed);
				return;
			}

			Assert.Fail("No exception or incorrect exception thrown");
		}


		/// <summary>
		/// Tests the <see cref="Extensions.SafeUsingBlock{TDisposable}(TDisposable,System.Action{TDisposable})"/> 
		/// method with an exception thrown in the dispose but not in the body
		/// </summary>
		[TestMethod]
		public void TestSafeUsingBlockWithOnlyErrorInDispose()
		{
			try
			{
				(new ErrorDisposable()).SafeUsingBlock(myDisposable =>
				{
				});
			}
			catch (Exception e)
			{
				Assert.AreEqual("Dispose", e.Message);
				return;
			}

			Assert.Fail("No exception or incorrect exception thrown");
		}

		
		/// <summary>
		/// An <see cref="IDisposable"/> that Disposes successfully
		/// </summary>
		private class NoErrorDisposable : IDisposable
		{
			public bool Disposed { get; private set; }

			public void Dispose()
			{
				Disposed = true;
			}
		}


		/// <summary>
		/// An <see cref="IDisposable"/> that throws an exception when disposing
		/// </summary>
		private class ErrorDisposable : IDisposable
		{
			public void Dispose()
			{
				throw new Exception("Dispose");
			}
		}
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace DigitallyCreated.Utilities
{
	/// <summary>
	/// Helper methods for use in unit tests
	/// </summary>
	static class TestHelper
	{
		/// <summary>
		/// Tests a piece of code to ensure it throws an exception
		/// </summary>
		/// <typeparam name="T">
		/// The type of exception to expect (subclasses of this type that are caught will not cause the test to
		/// fail)
		/// </typeparam>
		/// <param name="action">The code to run</param>
		public static void ExpectException<T>(Action action) where T : Exception
		{
			try
			{
				action();
			}
			catch (T)
			{
				return;
			}

			Assert.Fail(String.Format("Expected exception of type {0} was not caught.", typeof(T).Name));
		}


		/// <summary>
		/// Asserts that two byte arrays contain the same data
		/// </summary>
		/// <param name="expected">The expected data</param>
		/// <param name="actual">The actual data</param>
		public static void AreEqual(byte[] expected, byte[] actual)
		{
			Assert.AreEqual(expected.Length, actual.Length, "The size of the two arrays is different");
			for (int i = 0; i < expected.Length; i++)
			{
				Assert.AreEqual(expected[i], actual[i]);
			}
		}
	}
}

using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace DigitallyCreated.Utilities.Bcl
{
	/// <summary>
	/// Tests the <see cref="Base64StreamReader"/> class
	/// </summary>
	[TestClass]
	public class Base64StreamReaderTests
	{
		private Random _Random;

		/// <summary>
		/// Setup before the tests
		/// </summary>
		[TestInitialize]
		public void SetUp()
		{
			_Random = new Random();
		}

		/// <summary>
		/// Tests the reader with byte arrays of sequential data
		/// </summary>
		[TestMethod]
		public void TestReaderWithSequentialData()
		{
			TestWithSequentialData(2048, 2048);
			TestWithSequentialData(4096, 4096);
			TestWithSequentialData(8192, 8192);
			TestWithSequentialData(128, 128);
			TestWithSequentialData(96, 96);
			TestWithSequentialData(12, 12);
			TestWithSequentialData(79, 79);
			TestWithSequentialData(113, 113);
			TestWithSequentialData(3571, 3571);
		}


		/// <summary>
		/// Tests the reader with byte arrays of random data
		/// </summary>
		[TestMethod]
		public void TestReaderWithRandomData()
		{
			TestWithRandomData(2048, 2048);
			TestWithRandomData(4096, 4096);
			TestWithRandomData(8192, 8192);
			TestWithRandomData(128, 128);
			TestWithRandomData(96, 96);
			TestWithRandomData(12, 12);
			TestWithRandomData(79, 79);
			TestWithRandomData(113, 113);
			TestWithRandomData(3571, 3571);
		}


		/// <summary>
		/// Tests the reader with byte arrays of random data but read in 1 byte chunks so as to beat
		/// the hell out of the internal buffer
		/// </summary>
		[TestMethod]
		public void TestReaderWithRandomDataReadIn1ByteChunks()
		{
			TestWithRandomData(12, 1);
			TestWithRandomData(2048, 1);
			TestWithRandomData(4096, 1);
			TestWithRandomData(8192, 1);
			TestWithRandomData(128, 1);
			TestWithRandomData(96, 1);
			TestWithRandomData(79, 1);
			TestWithRandomData(113, 1);
			TestWithRandomData(3571, 1);
		}


		/// <summary>
		/// Tests the reader with byte arrays of random data but read in 2 byte chunks so as to beat
		/// the hell out of the internal buffer
		/// </summary>
		[TestMethod]
		public void TestReaderWithRandomDataReadIn2ByteChunks()
		{
			TestWithRandomData(2048, 2);
			TestWithRandomData(4096, 2);
			TestWithRandomData(8192, 2);
			TestWithRandomData(128, 2);
			TestWithRandomData(96, 2);
			TestWithRandomData(12, 2);
			TestWithRandomData(79, 2);
			TestWithRandomData(113, 2);
			TestWithRandomData(3571, 2);
		}


		/// <summary>
		/// Tests the reader with byte arrays of random data but read in 3 byte chunks so as to not use
		/// the internal buffer at all
		/// </summary>
		[TestMethod]
		public void TestReaderWithRandomDataReadIn3ByteChunks()
		{
			TestWithRandomData(2048, 3);
			TestWithRandomData(4096, 3);
			TestWithRandomData(8192, 3);
			TestWithRandomData(128, 3);
			TestWithRandomData(96, 3);
			TestWithRandomData(12, 3);
			TestWithRandomData(79, 3);
			TestWithRandomData(113, 3);
			TestWithRandomData(3571, 3);
		}


		/// <summary>
		/// Tests the reader with byte arrays of random data but read in 3 byte chunks so as to use 
		/// the internal buffer as well as read data
		/// </summary>
		[TestMethod]
		public void TestReaderWithRandomDataReadIn4ByteChunks()
		{
			TestWithRandomData(2048, 4);
			TestWithRandomData(4096, 4);
			TestWithRandomData(8192, 4);
			TestWithRandomData(128, 4);
			TestWithRandomData(96, 4);
			TestWithRandomData(12, 4);
			TestWithRandomData(79, 4);
			TestWithRandomData(113, 4);
			TestWithRandomData(3571, 4);
		}


		/// <summary>
		/// Tests the reader with a byte array of sequential data, where the source data
		/// is of a certain size
		/// </summary>
		/// <param name="size">Size of source data in bytes</param>
		/// <param name="readChunkSize">The size in which to read chunks of data from the reader</param>
		private void TestWithSequentialData(int size, int readChunkSize)
		{
			byte[] bytes = new byte[size];
			for (int i = 0; i < bytes.Length; i++)
				bytes[i] = (byte)(i % 256);

			TestReader(bytes, readChunkSize);
		}


		/// <summary>
		/// Tests the reader by taking a byte array, encoding it into base64 then decoding
		/// it with the reader and comparing the original and decoded output.
		/// </summary>
		/// <param name="originalBytes">The data to use</param>
		/// <param name="readChunkSize">Read from the stream in this chunk size</param>
		private void TestReader(byte[] originalBytes, int readChunkSize)
		{
			string base64Str = Convert.ToBase64String(originalBytes, Base64FormattingOptions.InsertLineBreaks);

			StringReader strReader = new StringReader(base64Str);
			Base64StreamReader reader = new Base64StreamReader(strReader);

			byte[] readBytes = new byte[originalBytes.Length];
			int offset = 0;
			int read;
			while ((read = reader.Read(readBytes, offset, Math.Min(readChunkSize, originalBytes.Length - offset))) > 0)
			{
				offset += read;
			}
			Assert.AreEqual(0, reader.Read(new byte[3], 0, 3));

			for (int i = 0; i < originalBytes.Length; i++)
				Assert.AreEqual(originalBytes[i], readBytes[i]);
		}



		/// <summary>
		/// Tests the reader with a byte array of random data, where the source data
		/// is of a certain size
		/// </summary>
		/// <param name="size">Size of source data in bytes</param>
		/// <param name="readChunkSize">The size in which to read chunks of data from the reader</param>
		private void TestWithRandomData(int size, int readChunkSize)
		{
			byte[] originalBytes = new byte[size];
			_Random.NextBytes(originalBytes);

			TestReader(originalBytes, readChunkSize);
		}
	}
}
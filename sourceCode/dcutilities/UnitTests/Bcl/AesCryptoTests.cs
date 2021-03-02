using System;
using System.Security.Cryptography;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace DigitallyCreated.Utilities.Bcl
{
	/// <summary>
	/// Test the <see cref="AesCrypto"/> class
	/// </summary>
	[TestClass]
	public class AesCryptoTests
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
		/// Tests encryption and decryption while using a password
		/// </summary>
		[TestMethod]
		public void TestEncryptWithPassword()
		{
			byte[] data = CreateRandomData(1000);

			byte[] encryptedData;
			string base64EncryptedData;
			using (ICrypto crypto = new AesCrypto("password!"))
			{
				encryptedData = crypto.Encrypt(data);
				base64EncryptedData = crypto.EncryptToBase64(data);
			}
			
			byte[] decryptedData;
			byte[] decryptedDataFromBase64;
			using (ICrypto crypto = new AesCrypto("password!"))
			{
				decryptedData = crypto.Decrypt(encryptedData);
				decryptedDataFromBase64 = crypto.Decrypt(base64EncryptedData);
			}

			TestHelper.AreEqual(data, decryptedData);
			TestHelper.AreEqual(data, decryptedDataFromBase64);
		}


		/// <summary>
		/// Tests encryption and decryption while using a password that is one char short of what is needed
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void TestEncryptWithSevenCharPassword()
		{
			byte[] data = CreateRandomData(1000);

			using (ICrypto crypto = new AesCrypto("1234567"))
			{
				crypto.Encrypt(data);
			}
		}


		/// <summary>
		/// Tests encryption and decryption while using a password and a salt
		/// </summary>
		[TestMethod]
		public void TestEncryptWithPasswordWithSalt()
		{
			byte[] salt = CreateRandomData(128);
			byte[] data = CreateRandomData(1000);


			byte[] encryptedData;
			string base64EncryptedData;
			using (ICrypto crypto = new AesCrypto("password!", salt))
			{
				encryptedData = crypto.Encrypt(data);
				base64EncryptedData = crypto.EncryptToBase64(data);
			}

			byte[] decryptedData;
			byte[] decryptedDataFromBase64;
			using (ICrypto crypto = new AesCrypto("password!", salt))
			{
				decryptedData = crypto.Decrypt(encryptedData);
				decryptedDataFromBase64 = crypto.Decrypt(base64EncryptedData);
			}

			TestHelper.AreEqual(data, decryptedData);
			TestHelper.AreEqual(data, decryptedDataFromBase64);
		}


		/// <summary>
		/// Tests encryption and decryption to and from a string
		/// </summary>
		[TestMethod]
		public void TestEncryptFromString()
		{
			string data = "Some test data";

			byte[] encryptedData;
			string base64EncryptedData;
			using (ICrypto crypto = new AesCrypto("password!"))
			{
				encryptedData = crypto.Encrypt(data);
				base64EncryptedData = crypto.EncryptToBase64(data);
			}

			string decryptedData;
			string decryptedDataFromBase64;
			using (ICrypto crypto = new AesCrypto("password!"))
			{
				decryptedData = crypto.DecryptToString(encryptedData);
				decryptedDataFromBase64 = crypto.DecryptToString(base64EncryptedData);
			}

			Assert.AreEqual(data, decryptedData);
			Assert.AreEqual(data, decryptedDataFromBase64);
		}


		/// <summary>
		/// Tests encryption using the hash and encrypt method
		/// </summary>
		[TestMethod]
		public void TestHashAndEncrypt()
		{
			byte[] data = CreateRandomData(1000);

			byte[] encryptedData;
			string base64EncryptedData;
			using (ICrypto crypto = new AesCrypto("password!"))
			{
				encryptedData = crypto.HashAndEncrypt(data);
				base64EncryptedData = crypto.HashAndEncryptToBase64(data);
			}

			byte[] decryptedData;
			byte[] decryptedDataFromBase64;
			using (ICrypto crypto = new AesCrypto("password!"))
			{
				decryptedData = crypto.DecryptAndCheckHash(encryptedData);
				decryptedDataFromBase64 = crypto.DecryptAndCheckHash(base64EncryptedData);
			}

			TestHelper.AreEqual(data, decryptedData);
			TestHelper.AreEqual(data, decryptedDataFromBase64);
		}


		/// <summary>
		/// Tests encryption using the hash and encrypt method to and from a string
		/// </summary>
		[TestMethod]
		public void TestHashAndEncryptFromString()
		{
			string data = "Some test data";

			byte[] encryptedData;
			string base64EncryptedData;
			using (ICrypto crypto = new AesCrypto("password!"))
			{
				encryptedData = crypto.HashAndEncrypt(data);
				base64EncryptedData = crypto.HashAndEncryptToBase64(data);
			}

			string decryptedData;
			string decryptedDataFromBase64;
			using (ICrypto crypto = new AesCrypto("password!"))
			{
				decryptedData = crypto.DecryptToStringAndCheckHash(encryptedData);
				decryptedDataFromBase64 = crypto.DecryptToStringAndCheckHash(base64EncryptedData);
			}

			Assert.AreEqual(data, decryptedData);
			Assert.AreEqual(data, decryptedDataFromBase64);
		}


		/// <summary>
		/// Tests that tampering with hashed and encrypted data causes an exception upon decryption
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(CryptographicException))]
		public void TestHashAndEncryptTamperingWithData()
		{
			byte[] data = CreateRandomData(1000);

			byte[] encryptedData;
			using (ICrypto crypto = new AesCrypto("password!"))
			{
				encryptedData = crypto.HashAndEncrypt(data);
			}

			//Tamper
			encryptedData[20] = (byte)~(uint)encryptedData[20];

			using (ICrypto crypto = new AesCrypto("password!"))
			{
				crypto.DecryptAndCheckHash(encryptedData);
			}
		}


		/// <summary>
		/// Tests hashing and encrypting with no data
		/// </summary>
		[TestMethod]
		public void TestHashAndEncryptWithNoData()
		{
			byte[] data = new byte[0];

			byte[] encryptedData;
			using (ICrypto crypto = new AesCrypto("password!"))
			{
				encryptedData = crypto.HashAndEncrypt(data);
			}

			byte[] decryptedData;
			using (ICrypto crypto = new AesCrypto("password!"))
			{
				decryptedData = crypto.DecryptAndCheckHash(encryptedData);
			}

			Assert.AreEqual(0, decryptedData.Length);
		}


		/// <summary>
		/// Creates random bytes
		/// </summary>
		/// <param name="size">The number of bytes to create</param>
		/// <returns>The random bytes</returns>
		private byte[] CreateRandomData(int size)
		{
			byte[] bytes = new byte[size];
			_Random.NextBytes(bytes);
			return bytes;
		}
	}
}
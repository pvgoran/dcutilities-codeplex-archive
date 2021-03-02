using System;
using System.Security.Cryptography;
using System.Text;
using DigitallyCreated.Utilities.Bcl.Configuration;


namespace DigitallyCreated.Utilities.Bcl
{
	/// <summary>
	/// The <see cref="AesCrypto"/> object is able to encrypt and decrypt data using the AES algorithm.
	/// It acts as a wrapper over the <see cref="AesManaged"/> crypto object.
	/// </summary>
	/// <seealso cref="AesManaged"/>
	public class AesCrypto : ICrypto
	{
		private readonly AesManaged _AesProvider;
		private ICryptoTransform _Encryptor;
		private ICryptoTransform _Decryptor;

		private ICryptoTransform Encryptor
		{
			get { return _Encryptor ?? (_Encryptor = _AesProvider.CreateEncryptor()); }
		}

		private ICryptoTransform Decryptor
		{
			get { return _Decryptor ?? (_Decryptor = _AesProvider.CreateDecryptor()); }
		}


		/// <summary>
		/// Constructor, creates a new <see cref="AesCrypto"/> using the password specified in your
		/// application configuration file (see <see cref="CryptoConfigurationSection"/>).
		/// </summary>
		public AesCrypto()
			: this(CryptoConfigurationSection.GetSection().Password)
		{
		}


		/// <summary>
		/// Constructor, creates a new <see cref="AesCrypto"/> using the specified password to derive an encryption
		/// key and initialisation vector
		/// </summary>
		/// <param name="password">The password</param>
		public AesCrypto(string password)
			: this(password, GetBytesFromPassword(password))
		{
		}


		/// <summary>
		/// Constructor, creates a new <see cref="AesCrypto"/> using the specified password and salt to derive an
		/// encryption key and initialisation vector
		/// </summary>
		/// <param name="password">The password</param>
		/// <param name="salt">
		/// Some salt data (usually a set of random bytes associated with the password). Must have at least 8 bytes
		/// </param>
		public AesCrypto(string password, byte[] salt)
		{
			_AesProvider = new AesManaged();

			Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, salt);
			_AesProvider.Key = rfc2898DeriveBytes.GetBytes(_AesProvider.KeySize / 8);
			_AesProvider.IV = rfc2898DeriveBytes.GetBytes(_AesProvider.BlockSize / 8);
		}


		/// <summary>
		/// Constructor, creates a new <see cref="AesCrypto"/> with the specified key and initialisation vector
		/// </summary>
		/// <param name="key">The key. Must be 32, 16, or 8 bytes.</param>
		/// <param name="iv">The initialisation vector. Must be 16 bytes.</param>
		public AesCrypto(byte[] key, byte[] iv)
		{
			_AesProvider = new AesManaged
			               	{
			               		IV = iv,
			               		Key = key,
			               	};
		}


		/// <summary>
		/// Gets the bytes from the password and validates its length
		/// </summary>
		/// <param name="password">The password</param>
		/// <returns>The bytes</returns>
		private static byte[] GetBytesFromPassword(string password)
		{
			if (password.Length < 8)
				throw new ArgumentException("password must be at least 8 characters long", "password");

			return Encoding.UTF8.GetBytes(password);
		}


		/// <summary>
		/// Encrypts some byte data and returns the resultant bytes.
		/// </summary>
		/// <param name="data">The data to encrypt</param>
		/// <returns>The encrypted bytes</returns>
		public byte[] Encrypt(byte[] data)
		{
			return Encryptor.TransformFinalBlock(data, 0, data.Length);
		}


		/// <summary>
		/// Encrypts some string data and returns the resultant bytes.
		/// </summary>
		/// <param name="data">The data to encrypt</param>
		/// <returns>The encrypted bytes</returns>
		public byte[] Encrypt(string data)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(data);
			return Encrypt(bytes);
		}


		/// <summary>
		/// Encrypts some byte data and returns the resultant bytes as a base-64 encoded string.
		/// </summary>
		/// <param name="data">The data to encrypt</param>
		/// <returns>The encrypted bytes as a base-64 string</returns>
		public string EncryptToBase64(byte[] data)
		{
			byte[] encryptedBytes = Encrypt(data);
			return Convert.ToBase64String(encryptedBytes);
		}


		/// <summary>
		/// Encrypts some string data and returns the resultant bytes as a base-64 encoded string.
		/// </summary>
		/// <param name="data">The data to encrypt</param>
		/// <returns>The encrypted bytes as a base-64 string</returns>
		public string EncryptToBase64(string data)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(data);
			return EncryptToBase64(bytes);
		}

		/// <summary>
		/// Hashes some byte data and encrypts the data alongside the hash, allowing you to check
		/// the integrity of the data when you decrypt it.
		/// </summary>
		/// <param name="data">The data to encrypt</param>
		/// <returns>The encrypted bytes</returns>
		public byte[] HashAndEncrypt(byte[] data)
		{
			SHA1Managed sha1 = new SHA1Managed();

			byte[] saltedData = Combine(data, _AesProvider.Key);
			byte[] hash = sha1.ComputeHash(saltedData);
			byte[] hashAndData = Combine(hash, data);

			return Encrypt(hashAndData);
		}

		private byte[] Combine(byte[] first, byte[] second)
		{
			int i = 0;
			byte[] combined = new byte[first.Length + second.Length];
			foreach (byte b in first)
			{
				combined[i] = b;
				i++;
			}
			foreach (byte b in second)
			{
				combined[i] = b;
				i++;
			}
			return combined;
		}

		/// <summary>
		/// Hashes some byte data and encrypts the data alongside the hash, allowing you to check
		/// the integrity of the data when you decrypt it.
		/// </summary>
		/// <param name="data">The data to encrypt</param>
		/// <returns>The encrypted bytes</returns>
		public byte[] HashAndEncrypt(string data)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(data);
			return HashAndEncrypt(bytes);
		}

		/// <summary>
		/// Hashes some byte data and encrypts the data alongside the hash, allowing you to check
		/// the integrity of the data when you decrypt it. The resultant bytes are returned as
		/// a base-64 encoded string.
		/// </summary>
		/// <param name="data">The data to encrypt</param>
		/// <returns>The encrypted bytes as a base-64 string</returns>
		public string HashAndEncryptToBase64(byte[] data)
		{
			byte[] bytes = HashAndEncrypt(data);
			return Convert.ToBase64String(bytes);
		}

		/// <summary>
		/// Hashes some byte data and encrypts the data alongside the hash, allowing you to check
		/// the integrity of the data when you decrypt it. The resultant bytes are returned as
		/// a base-64 encoded string.
		/// </summary>
		/// <param name="data">The data to encrypt</param>
		/// <returns>The encrypted bytes as a base-64 string</returns>
		public string HashAndEncryptToBase64(string data)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(data);
			return HashAndEncryptToBase64(bytes);
		}


		/// <summary>
		/// Decrypts some encrypted byte data and returns the decrypted bytes
		/// </summary>
		/// <param name="encryptedData">The encrypted data</param>
		/// <returns>The decrypted bytes</returns>
		public byte[] Decrypt(byte[] encryptedData)
		{
			return Decryptor.TransformFinalBlock(encryptedData, 0, encryptedData.Length);
		}


		/// <summary>
		/// Decrypts some encrypted byte data and returns the decrypted bytes
		/// </summary>
		/// <param name="encryptedDataBase64String">The encrypted data as a base-64 encoded string</param>
		/// <returns>The decrypted bytes</returns>
		public byte[] Decrypt(string encryptedDataBase64String)
		{
			byte[] bytes = Convert.FromBase64String(encryptedDataBase64String);
			return Decryptor.TransformFinalBlock(bytes, 0, bytes.Length);
		}


		/// <summary>
		/// Decrypts some encrypted byte data and returns the decrypted string
		/// </summary>
		/// <param name="encryptedData">The encrypted data</param>
		/// <returns>The decrypted string</returns>
		public string DecryptToString(byte[] encryptedData)
		{
			return Encoding.UTF8.GetString(Decrypt(encryptedData));
		}


		/// <summary>
		/// Decrypts some encrypted byte data and returns the decrypted string
		/// </summary>
		/// <param name="encryptedDataBase64String">The encrypted data as a base-64 encoded string</param>
		/// <returns>The decrypted string</returns>
		public string DecryptToString(string encryptedDataBase64String)
		{
			return Encoding.UTF8.GetString(Decrypt(encryptedDataBase64String));
		}

		/// <summary>
		/// Decrypts some encrypted byte data and checks the hash to ensure the data has not been tampered with
		/// and returns the decrypted bytes.
		/// </summary>
		/// <remarks>
		/// This decrypts the output of <see cref="ICrypto.HashAndEncrypt(byte[])"/>
		/// </remarks>
		/// <param name="encryptedData">The encrypted data</param>
		/// <returns>The decrypted bytes</returns>
		/// <exception cref="CryptographicException">
		/// If the hash verification failed and therefore possible data tampering was detected.
		/// </exception>
		public byte[] DecryptAndCheckHash(byte[] encryptedData)
		{
			SHA1Managed sha1 = new SHA1Managed();
			byte[] hashAndData = Decrypt(encryptedData);

			if (hashAndData.Length < 20)
				throw new CryptographicException("Hash verification failed; possible data tampering detected.");

			byte[] data = new byte[hashAndData.Length - 20];
			for (int i = 20; i < hashAndData.Length; i++)
			{
				data[i - 20] = hashAndData[i];
			}

			byte[] saltedData = Combine(data, _AesProvider.Key);
			byte[] expectedHash = sha1.ComputeHash(saltedData);

			for (int i = 0; i < 20; i++)
			{
				if (hashAndData[i] != expectedHash[i])
					throw new CryptographicException("Hash verification failed; possible data tampering detected.");
			}

			return data;
		}

		/// <summary>
		/// Decrypts some encrypted byte data and checks the hash to ensure the data has not been tampered with
		/// and returns the decrypted bytes.
		/// </summary>
		/// <remarks>
		/// This decrypts the output of <see cref="ICrypto.HashAndEncrypt(byte[])"/>
		/// </remarks>
		/// <param name="encryptedDataBase64String">The encrypted data as a base-64 encoded string</param>
		/// <returns>The decrypted bytes</returns>
		/// <exception cref="CryptographicException">
		/// If the hash verification failed and therefore possible data tampering was detected.
		/// </exception>
		public byte[] DecryptAndCheckHash(string encryptedDataBase64String)
		{
			byte[] bytes = Convert.FromBase64String(encryptedDataBase64String);
			return DecryptAndCheckHash(bytes);
		}

		/// <summary>
		/// Decrypts some encrypted byte data and checks the hash to ensure the data has not been tampered with
		/// and returns the decrypted string.
		/// </summary>
		/// <remarks>
		/// This decrypts the output of <see cref="ICrypto.HashAndEncrypt(byte[])"/>
		/// </remarks>
		/// <param name="encryptedData">The encrypted data</param>
		/// <returns>The decrypted string</returns>
		/// <exception cref="CryptographicException">
		/// If the hash verification failed and therefore possible data tampering was detected.
		/// </exception>
		public string DecryptToStringAndCheckHash(byte[] encryptedData)
		{
			return Encoding.UTF8.GetString(DecryptAndCheckHash(encryptedData));
		}

		/// <summary>
		/// Decrypts some encrypted byte data and checks the hash to ensure the data has not been tampered with
		/// and returns the decrypted string.
		/// </summary>
		/// <remarks>
		/// This decrypts the output of <see cref="ICrypto.HashAndEncrypt(byte[])"/>
		/// </remarks>
		/// <param name="encryptedDataBase64String">The encrypted data as a base-64 encoded string</param>
		/// <returns>The decrypted string</returns>
		/// <exception cref="CryptographicException">
		/// If the hash verification failed and therefore possible data tampering was detected.
		/// </exception>
		public string DecryptToStringAndCheckHash(string encryptedDataBase64String)
		{
			byte[] bytes = Convert.FromBase64String(encryptedDataBase64String);
			return Encoding.UTF8.GetString(DecryptAndCheckHash(bytes));
		}


		/// <summary>
		/// Releases the underlying cryptography objects
		/// </summary>
		public void Dispose()
		{
			if (_Encryptor != null)
			{
				_Encryptor.Dispose();
				_Encryptor = null;
			}
			if (_Decryptor != null)
			{
				_Decryptor.Dispose();
				_Decryptor = null;
			}
			if (_AesProvider != null)
			{
				_AesProvider.Clear(); //Also Dispose()s the object
			}
		}
	}
}
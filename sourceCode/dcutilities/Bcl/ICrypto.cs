using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;


namespace DigitallyCreated.Utilities.Bcl
{
	/// <summary>
	/// Defines an object that is able to encrypt and decrypt data.
	/// </summary>
	public interface ICrypto : IDisposable
	{
		/// <summary>
		/// Encrypts some byte data and returns the resultant bytes.
		/// </summary>
		/// <param name="data">The data to encrypt</param>
		/// <returns>The encrypted bytes</returns>
		byte[] Encrypt(byte[] data);

		/// <summary>
		/// Encrypts some string data and returns the resultant bytes.
		/// </summary>
		/// <param name="data">The data to encrypt</param>
		/// <returns>The encrypted bytes</returns>
		byte[] Encrypt(string data);

		/// <summary>
		/// Encrypts some byte data and returns the resultant bytes as a base-64 encoded string.
		/// </summary>
		/// <param name="data">The data to encrypt</param>
		/// <returns>The encrypted bytes as a base-64 string</returns>
		string EncryptToBase64(byte[] data);

		/// <summary>
		/// Encrypts some string data and returns the resultant bytes as a base-64 encoded string.
		/// </summary>
		/// <param name="data">The data to encrypt</param>
		/// <returns>The encrypted bytes as a base-64 string</returns>
		string EncryptToBase64(string data);

		/// <summary>
		/// Hashes some byte data and encrypts the data alongside the hash, allowing you to check
		/// the integrity of the data when you decrypt it.
		/// </summary>
		/// <param name="data">The data to encrypt</param>
		/// <returns>The encrypted bytes</returns>
		byte[] HashAndEncrypt(byte[] data);

		/// <summary>
		/// Hashes some byte data and encrypts the data alongside the hash, allowing you to check
		/// the integrity of the data when you decrypt it.
		/// </summary>
		/// <param name="data">The data to encrypt</param>
		/// <returns>The encrypted bytes</returns>
		byte[] HashAndEncrypt(string data);

		/// <summary>
		/// Hashes some byte data and encrypts the data alongside the hash, allowing you to check
		/// the integrity of the data when you decrypt it. The resultant bytes are returned as
		/// a base-64 encoded string.
		/// </summary>
		/// <param name="data">The data to encrypt</param>
		/// <returns>The encrypted bytes as a base-64 string</returns>
		string HashAndEncryptToBase64(byte[] data);

		/// <summary>
		/// Hashes some byte data and encrypts the data alongside the hash, allowing you to check
		/// the integrity of the data when you decrypt it. The resultant bytes are returned as
		/// a base-64 encoded string.
		/// </summary>
		/// <param name="data">The data to encrypt</param>
		/// <returns>The encrypted bytes as a base-64 string</returns>
		string HashAndEncryptToBase64(string data);

		/// <summary>
		/// Decrypts some encrypted byte data and returns the decrypted bytes
		/// </summary>
		/// <param name="encryptedData">The encrypted data</param>
		/// <returns>The decrypted bytes</returns>
		byte[] Decrypt(byte[] encryptedData);

		/// <summary>
		/// Decrypts some encrypted byte data and returns the decrypted bytes
		/// </summary>
		/// <param name="encryptedDataBase64String">The encrypted data as a base-64 encoded string</param>
		/// <returns>The decrypted bytes</returns>
		byte[] Decrypt(string encryptedDataBase64String);

		/// <summary>
		/// Decrypts some encrypted byte data and returns the decrypted string
		/// </summary>
		/// <param name="encryptedData">The encrypted data</param>
		/// <returns>The decrypted string</returns>
		string DecryptToString(byte[] encryptedData);

		/// <summary>
		/// Decrypts some encrypted byte data and returns the decrypted string
		/// </summary>
		/// <param name="encryptedDataBase64String">The encrypted data as a base-64 encoded string</param>
		/// <returns>The decrypted string</returns>
		string DecryptToString(string encryptedDataBase64String);

		/// <summary>
		/// Decrypts some encrypted byte data and checks the hash to ensure the data has not been tampered with
		/// and returns the decrypted bytes.
		/// </summary>
		/// <remarks>
		/// This decrypts the output of <see cref="HashAndEncrypt(byte[])"/>
		/// </remarks>
		/// <param name="encryptedData">The encrypted data</param>
		/// <returns>The decrypted bytes</returns>
		/// <exception cref="CryptographicException">
		/// If the hash verification failed and therefore possible data tampering was detected.
		/// </exception>
		byte[] DecryptAndCheckHash(byte[] encryptedData);

		/// <summary>
		/// Decrypts some encrypted byte data and checks the hash to ensure the data has not been tampered with
		/// and returns the decrypted bytes.
		/// </summary>
		/// <remarks>
		/// This decrypts the output of <see cref="HashAndEncrypt(byte[])"/>
		/// </remarks>
		/// <param name="encryptedDataBase64String">The encrypted data as a base-64 encoded string</param>
		/// <returns>The decrypted bytes</returns>
		/// <exception cref="CryptographicException">
		/// If the hash verification failed and therefore possible data tampering was detected.
		/// </exception>
		byte[] DecryptAndCheckHash(string encryptedDataBase64String);

		/// <summary>
		/// Decrypts some encrypted byte data and checks the hash to ensure the data has not been tampered with
		/// and returns the decrypted string.
		/// </summary>
		/// <remarks>
		/// This decrypts the output of <see cref="HashAndEncrypt(byte[])"/>
		/// </remarks>
		/// <param name="encryptedData">The encrypted data</param>
		/// <returns>The decrypted string</returns>
		/// <exception cref="CryptographicException">
		/// If the hash verification failed and therefore possible data tampering was detected.
		/// </exception>
		string DecryptToStringAndCheckHash(byte[] encryptedData);

		/// <summary>
		/// Decrypts some encrypted byte data and checks the hash to ensure the data has not been tampered with
		/// and returns the decrypted string.
		/// </summary>
		/// <remarks>
		/// This decrypts the output of <see cref="HashAndEncrypt(byte[])"/>
		/// </remarks>
		/// <param name="encryptedDataBase64String">The encrypted data as a base-64 encoded string</param>
		/// <returns>The decrypted string</returns>
		/// <exception cref="CryptographicException">
		/// If the hash verification failed and therefore possible data tampering was detected.
		/// </exception>
		string DecryptToStringAndCheckHash(string encryptedDataBase64String);
	}
}
using System;
using System.Web.Mvc;
using DigitallyCreated.Utilities.Bcl;
using DigitallyCreated.Utilities.Bcl.Configuration;


namespace DigitallyCreated.Utilities.Mvc
{
	/// <summary>
	/// The CryptoBinder is able to decrypt a string form value using the <see cref="AesCrypto"/> object
	/// and using the password set in the web.config with the <see cref="CryptoConfigurationSection"/>.
	/// </summary>
	public class CryptoBinder : IModelBinder
	{
		/// <summary>
		/// Binds (and decrypts) an encrypted field to a string
		/// </summary>
		/// <param name="controllerContext">The controller context.</param>
		/// <param name="bindingContext">The binding context.</param>
		/// <returns>The decrypted string</returns>
		public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
		{
			if (bindingContext.ModelType != typeof(string))
				throw new InvalidOperationException("You cannot bind an object that is not a string with the CryptoBinder");

			ValueProviderResult valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
			if (valueProviderResult == null)
				return null;

			string encryptedString = valueProviderResult.AttemptedValue;

			using (ICrypto crypto = new AesCrypto())
			{
				return crypto.DecryptToString(encryptedString);
			}
		}
	}
}
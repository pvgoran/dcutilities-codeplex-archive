using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;


namespace DigitallyCreated.Utilities.Mvc
{
	/// <summary>
	/// <see cref="HtmlHelper"/> methods that enable you to easily integrate Gravatars onto a web page
	/// </summary>
	/// <remarks>
	/// The Gravatar website is http://www.gravatar.com
	/// </remarks>
	public static class GravatarHtmlHelpers
	{
		/// <summary>
		/// Renders an HTML img tag that references a Gravatar avatar based off a user's email address.
		/// The Gravatar rating is set to a maximum of <see cref="GravatarRating.G"/>, the size will be
		/// 80x80 pixels and the default Gravatar will be <see cref="GravatarDefaults.Indenticon"/>.
		/// </summary>
		/// <param name="html">The <see cref="HtmlHelper"/></param>
		/// <param name="email">The email address</param>
		/// <param name="alt">The alt text for the img tag</param>
		/// <returns>The img tag string</returns>
		public static MvcHtmlString Gravatar(this HtmlHelper html, string email, string alt)
		{
			return html.Gravatar(email, alt, GravatarDefaults.Indenticon, 80);
		}


		/// <summary>
		/// Renders an HTML img tag that references a Gravatar avatar based off a user's email address.
		/// The Gravatar rating is set to a maximum of <see cref="GravatarRating.G"/>.
		/// </summary>
		/// <param name="html">The <see cref="HtmlHelper"/></param>
		/// <param name="email">The email address</param>
		/// <param name="alt">The alt text for the img tag</param>
		/// <param name="defaultGravatar">
		/// The avatar you want to use if the email address doesn't have a Gravatar associated with it.
		/// You can either provide a URL to the image you want to use, or pass in one of the constants
		/// defined in <see cref="GravatarDefaults"/>.
		/// </param>
		/// <param name="size">The size in pixels of the image return. Must be between 1 and 512.</param>
		/// <returns>The img tag string</returns>
		/// <exception cref="ArgumentException">
		/// If <paramref name="size"/> is less than 1 or greater than 512
		/// </exception>
		public static MvcHtmlString Gravatar(this HtmlHelper html, string email, string alt, string defaultGravatar, int size)
		{
			return html.Gravatar(email, defaultGravatar, size, new Dictionary<string, object> { { "alt", alt } });
		}


		/// <summary>
		/// Renders an HTML img tag that references a Gravatar avatar based off a user's email address.
		/// The Gravatar rating is set to a maximum of <see cref="GravatarRating.G"/>.
		/// </summary>
		/// <param name="html">The <see cref="HtmlHelper"/></param>
		/// <param name="email">The email address</param>
		/// <param name="defaultGravatar">
		/// The avatar you want to use if the email address doesn't have a Gravatar associated with it.
		/// You can either provide a URL to the image you want to use, or pass in one of the constants
		/// defined in <see cref="GravatarDefaults"/>.
		/// </param>
		/// <param name="size">The size in pixels of the image return. Must be between 1 and 512.</param>
		/// <param name="htmlAttributes">Any HTML attributes you want applied to the img tag</param>
		/// <returns>The img tag string</returns>
		/// <exception cref="ArgumentException">
		/// If <paramref name="size"/> is less than 1 or greater than 512
		/// </exception>
		public static MvcHtmlString Gravatar(this HtmlHelper html, string email, string defaultGravatar, int size, IDictionary<string, object> htmlAttributes)
		{
			return html.Gravatar(email, defaultGravatar, size, GravatarRating.G, htmlAttributes);
		}


		/// <summary>
		/// Renders an HTML img tag that references a Gravatar avatar based off a user's email address
		/// </summary>
		/// <param name="html">The <see cref="HtmlHelper"/></param>
		/// <param name="email">The email address</param>
		/// <param name="defaultGravatar">
		/// The avatar you want to use if the email address doesn't have a Gravatar associated with it.
		/// You can either provide a URL to the image you want to use, or pass in one of the constants
		/// defined in <see cref="GravatarDefaults"/>.
		/// </param>
		/// <param name="size">The size in pixels of the image return. Must be between 1 and 512.</param>
		/// <param name="rating">The maximum rating that the Gravatar can be.</param>
		/// <param name="htmlAttributes">Any HTML attributes you want applied to the img tag</param>
		/// <returns>The img tag string</returns>
		/// <exception cref="ArgumentException">
		/// If <paramref name="size"/> is less than 1 or greater than 512
		/// </exception>
		public static MvcHtmlString Gravatar(this HtmlHelper html, string email, string defaultGravatar, int size, GravatarRating rating, IDictionary<string, object> htmlAttributes)
		{
			if (size < 1 || size > 512)
				throw new ArgumentException("size must be between 1 and 512", "size");

			UrlHelper url = new UrlHelper(html.ViewContext.RequestContext);
			defaultGravatar = url.Encode(defaultGravatar);
			if (email == null) email = String.Empty;
			email = email.ToLower();

			StringBuilder hashBuilder = new StringBuilder();
			MD5 md5 = MD5.Create();

			foreach (byte aByte in md5.ComputeHash(Encoding.Default.GetBytes(email)))
				hashBuilder.Append(aByte.ToString("x2"));

			TagBuilder img = new TagBuilder("img");
			img.MergeAttributes(htmlAttributes);
			img.Attributes["src"] = String.Format("http://www.gravatar.com/avatar/{0}?d={1}&s={2}&r={3}", hashBuilder, defaultGravatar, size, rating.ToString().ToLower());
			img.Attributes["width"] = size.ToString();
			img.Attributes["height"] = size.ToString();
			return MvcHtmlString.Create(img.ToString(TagRenderMode.SelfClosing));
		}
	}

	/// <summary>
	/// An enumeration of the the different Gravatar ratings
	/// </summary>
	public enum GravatarRating
	{
		/// <summary>
		/// A G rated gravatar is suitable for display on all websites with any audience type. 
		/// </summary>
		G,

		/// <summary>
		/// PG rated gravatars may contain rude gestures, provocatively dressed individuals, the lesser swear
		/// words, or mild violence. 
		/// </summary>
		PG,

		/// <summary>
		/// R rated gravatars may contain such things as harsh profanity, intense violence, nudity, or hard
		/// drug use. 
		/// </summary>
		R,

		/// <summary>
		/// X rated gravatars may contain hardcore sexual imagery or extremely disturbing violence.
		/// </summary>
		X
	}

	/// <summary>
	/// Constants that allows you to instruct Gravatar to return a particular preset default image when a
	/// user doesn't have a Gravatar.
	/// </summary>
	public static class GravatarDefaults
	{
		/// <summary>
		/// Specifies the Identicon default icon to be used when a Gravatar cannot be found for an email
		/// </summary>
		public const string Indenticon = "identicon";

		/// <summary>
		/// Specifies the MonsterID default icon to be used when a Gravatar cannot be found for an email
		/// </summary>
		public const string MonsterId = "monsterid";

		/// <summary>
		/// Specifies the Wavatar default icon to be used when a Gravatar cannot be found for an email
		/// </summary>
		public const string Wavatar = "wavatar";

		/// <summary>
		/// Specifies that an HTTP 404 error should occur when a Gravatar cannot be found for an email
		/// </summary>
		public const string Error404 = "404";
	}
}
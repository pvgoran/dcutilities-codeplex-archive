using System.Text.RegularExpressions;


namespace DigitallyCreated.Utilities.Bcl
{
	/// <summary>
	/// A set of common useful regular expressions
	/// </summary>
	public static class RegularExpressions
	{
		/// <summary>
		/// A regular expression that identifies HTTP/S and FTP/S URLs
		/// </summary>
		/// <remarks>
		/// Regex taken from http://flanders.co.nz/2009/11/08/a-good-url-regular-expression-repost/ 
		/// (from the wave (Dec 7 2009 version) + IP address &amp; anchor fix by Alexey in the comments)
		/// </remarks>
		public static Regex Url = new Regex(@"(?#Protocol)(?:(?:ht|f)tp(?:s?)\:\/\/|~\/|\/)(?#Username:Password)(?:\w+:\w+@)?((?#Subdomains)(?:(?:[-\w\d{1-3}]+\.)+(?#TopLevel Domains)(?:com|org|net|gov|mil|biz|info|mobi|name|aero|jobs|edu|co\.uk|ac\.uk|it|fr|tv|museum|asia|local|travel|[a-z]{2})?)|(?#IP)((\b25[0-5]\b|\b[2][0-4][0-9]\b|\b[0-1]?[0-9]?[0-9]\b)(\.(\b25[0-5]\b|\b[2][0-4][0-9]\b|\b[0-1]?[0-9]?[0-9]\b)){3}))(?#Port)(?::[\d]{1,5})?(?#Directories)(?:(?:(?:\/(?:[-\w~!$+|.,=]|%[a-f\d]{2})+)+|\/)+|\?|#)?(?#Query)(?:(?:\?(?:[-\w~!$+|.,*:]|%[a-f\d{2}])+=?(?:[-\w~!$+|.,*:=]|%[a-f\d]{2})*)(?:&(?:[-\w~!$+|.,*:]|%[a-f\d{2}])+=?(?:[-\w~!$+|.,*:=]|%[a-f\d]{2})*)*)*(?#Anchor)(?:#(?:[-\w~!$ |/.,*:;=]|%[a-f\d]{2})*)?", RegexOptions.IgnoreCase);
	}
}
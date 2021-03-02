using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;


namespace DigitallyCreated.Utilities.Bcl
{
	/// <summary>
	/// Contains utility methods for working with dates, times and timezones
	/// </summary>
	public static class TimeUtils
	{
		private static readonly TimeSpan _OneMinute = new TimeSpan(0, 1, 0);
		private static readonly TimeSpan _TwoMinutes = new TimeSpan(0, 2, 0);
		private static readonly TimeSpan _OneHour = new TimeSpan(1, 0, 0);
		private static readonly TimeSpan _TwoHours = new TimeSpan(2, 0, 0);
		private static readonly TimeSpan _OneDay = new TimeSpan(1, 0, 0, 0);
		private static readonly TimeSpan _TwoDays = new TimeSpan(2, 0, 0, 0);
		private static readonly TimeSpan _OneWeek = new TimeSpan(7, 0, 0, 0);
		private static readonly TimeSpan _TwoWeeks = new TimeSpan(14, 0, 0, 0);
		private static readonly TimeSpan _OneMonth = new TimeSpan(31, 0, 0, 0);
		private static readonly TimeSpan _TwoMonths = new TimeSpan(62, 0, 0, 0);
		private static readonly TimeSpan _OneYear = new TimeSpan(365, 0, 0, 0);
		private static readonly TimeSpan _TwoYears = new TimeSpan(730, 0, 0, 0);

		private static readonly object _TzMapLock = new object();
		private static IDictionary<string, TimeZoneInfo> _TzIdToWindowsTimeZoneMapping;
		private static IDictionary<TimeZoneInfo, ICollection<string>> _WindowsTimeZoneToTzIdMapping;


		/// <summary>
		/// Converts a <see cref="TimeSpan"/> to an "ago" string format. This means approximate casual time
		/// span strings like "just now", "2 days ago", "1 month ago", etc.
		/// </summary>
		/// <remarks>
		/// <para>
		/// One should note that this gets more and more inaccurate the longer the time span, especially
		/// because once you get over a month, inaccuracies such as the different lengths of months and leap
		/// years are not taken into account.
		/// </para>
		/// <para>
		/// Below is a list of all texts you can get:
		/// </para>
		/// <list type="bullet">
		///		<item><description>in the future</description></item>
		///		<item><description>just now</description></item>
		///		<item><description>1 minute ago</description></item>
		///		<item><description><i>x</i> minutes ago</description></item>
		///		<item><description>1 hour ago</description></item>
		///		<item><description><i>x</i> hours ago</description></item>
		///		<item><description>yesterday</description></item>
		///		<item><description><i>x</i> days ago</description></item>
		///		<item><description>1 week ago</description></item>
		///		<item><description><i>x</i> weeks ago</description></item>
		///		<item><description>1 month ago</description></item>
		/// 	<item><description><i>x</i> months ago</description></item>
		/// 	<item><description>1 year ago</description></item>
		/// 	<item><description><i>x</i> years ago</description></item>
		/// </list>
		/// </remarks>
		/// <param name="timeSpan">The time span</param>
		/// <returns>The "ago" string</returns>
		public static string ToAgoString(this TimeSpan timeSpan)
		{
			return ToAgoString(timeSpan, false);
		}


		/// <summary>
		/// Converts a <see cref="TimeSpan"/> to an "ago" string format. This means approximate casual time
		/// span strings like "just now", "2 days ago", "1 month ago", etc.
		/// </summary>
		/// <remarks>
		/// <para>
		/// One should note that this gets more and more inaccurate the longer the time span, especially
		/// because once you get over a month, inaccuracies such as the different lengths of months and leap
		/// years are not taken into account.
		/// </para>
		/// <para>
		/// Below is a list of all texts you can get if <paramref name="shortForm"/> is false:
		/// </para>
		/// <list type="bullet">
		///		<item><description>in the future</description></item>
		///		<item><description>just now</description></item>
		///		<item><description>1 minute ago</description></item>
		///		<item><description><i>x</i> minutes ago</description></item>
		///		<item><description>1 hour ago</description></item>
		///		<item><description><i>x</i> hours ago</description></item>
		///		<item><description>yesterday</description></item>
		///		<item><description><i>x</i> days ago</description></item>
		///		<item><description>1 week ago</description></item>
		///		<item><description><i>x</i> weeks ago</description></item>
		///		<item><description>1 month ago</description></item>
		/// 	<item><description><i>x</i> months ago</description></item>
		/// 	<item><description>1 year ago</description></item>
		/// 	<item><description><i>x</i> years ago</description></item>
		/// </list>
		/// <para>
		/// Below is a list of all texts you can get if <paramref name="shortForm"/> is true:
		/// </para>
		/// <list type="bullet">
		///		<item><description>future</description></item>
		///		<item><description>now</description></item>
		///		<item><description>1 min ago</description></item>
		///		<item><description><i>x</i> mins ago</description></item>
		///		<item><description>1 hr ago</description></item>
		///		<item><description><i>x</i> hrs ago</description></item>
		///		<item><description>yesterday</description></item>
		///		<item><description><i>x</i> days ago</description></item>
		///		<item><description>1 wk ago</description></item>
		///		<item><description><i>x</i> wks ago</description></item>
		///		<item><description>1 mth ago</description></item>
		/// 	<item><description><i>x</i> mths ago</description></item>
		/// 	<item><description>1 yr ago</description></item>
		/// 	<item><description><i>x</i> yrs ago</description></item>
		/// </list>
		/// </remarks>
		/// <param name="timeSpan">The time span</param>
		/// <param name="shortForm">
		/// If true, the short form of the texts are used instead of the longer ones (see remarks)
		/// </param>
		/// <returns>The "ago" string</returns>
		public static string ToAgoString(this TimeSpan timeSpan, bool shortForm)
		{
			if (timeSpan < TimeSpan.Zero)
				return shortForm ? "future" : "in the future";
			if (timeSpan < _OneMinute)
				return shortForm ? "now" : "just now";
			if (timeSpan < _TwoMinutes)
				return shortForm ? "1 min ago" : "1 minute ago";
			if (timeSpan < _OneHour)
				return String.Format(shortForm ? "{0} mins ago" : "{0} minutes ago", timeSpan.Minutes);
			if (timeSpan < _TwoHours)
				return shortForm ? "1 hr ago" : "1 hour ago";
			if (timeSpan < _OneDay)
				return String.Format(shortForm ? "{0} hrs ago" : "{0} hours ago", timeSpan.Hours);
			if (timeSpan < _TwoDays)
				return "yesterday";
			if (timeSpan < _OneWeek)
				return String.Format("{0} days ago", timeSpan.Days);
			if (timeSpan < _TwoWeeks)
				return shortForm ? "1 wk ago" : "1 week ago";
			if (timeSpan < _OneMonth)
				return String.Format(shortForm ? "{0} wks ago" : "{0} weeks ago", timeSpan.Days / 7);
			if (timeSpan < _TwoMonths)
				return shortForm ? "1 mth ago" : "1 month ago";
			if (timeSpan < _OneYear)
				return String.Format(shortForm ? "{0} mths ago" : "{0} months ago", timeSpan.Days / 31);
			if (timeSpan < _TwoYears)
				return shortForm ? "1 yr ago" : "1 year ago";

			return String.Format(shortForm ? "{0} yrs ago" : "{0} years ago", timeSpan.Days / 365);
		}
		


		/// <summary>
		/// Loads the TzId mapping data from the XML embedded resource
		/// </summary>
		private static void LoadTzIdMappingFromXml()
		{
			if (_TzIdToWindowsTimeZoneMapping != null) 
				return;

			lock (_TzMapLock)
			{
				if (_TzIdToWindowsTimeZoneMapping != null)
					return;

				_TzIdToWindowsTimeZoneMapping = new Dictionary<string, TimeZoneInfo>();
				_WindowsTimeZoneToTzIdMapping = new Dictionary<TimeZoneInfo, ICollection<string>>();

				Stream stream = Assembly.GetCallingAssembly().GetManifestResourceStream("DigitallyCreated.Utilities.Bcl.TzIdToWindowsTzMapping.xml");
				if (stream == null)
					throw new Exception("Could not load the timezone mapping XML. It seems to be missing.");

				using (TextReader reader = new StreamReader(stream))
				{
					XDocument xmlDoc = XDocument.Load(reader);

					var tzMap = from mapZoneElement in xmlDoc.Descendants("mapZone")
					            select new
					                   	{
					                   		TzId = mapZoneElement.Attribute("type").Value,
											WindowsId = mapZoneElement.Attribute("other").Value,
					                   	};

					foreach (var mapping in tzMap)
					{
						TimeZoneInfo tzi;
						try
						{
							tzi = TimeZoneInfo.FindSystemTimeZoneById(mapping.WindowsId);
						}
						catch (TimeZoneNotFoundException)
						{
							continue; //Skip this zone, since we can't find it in Windows
						}
						
						if (_TzIdToWindowsTimeZoneMapping.ContainsKey(mapping.TzId))
							throw new Exception("Duplicate TzId found: " + mapping.TzId);

						_TzIdToWindowsTimeZoneMapping.Add(mapping.TzId, tzi);

						if (_WindowsTimeZoneToTzIdMapping.ContainsKey(tzi) == false)
							_WindowsTimeZoneToTzIdMapping[tzi] = new List<string>();

						_WindowsTimeZoneToTzIdMapping[tzi].Add(mapping.TzId);
					}
				}
			}
		}


		/// <summary>
		/// Gets the <see cref="TimeZoneInfo"/> for the specified TzId (also known as an Olson Time Zone key)
		/// </summary>
		/// <remarks>
		/// TzIds are discussed on Wikipedia (http://en.wikipedia.org/wiki/List_of_tz_zones_by_name) and
		/// the data source for the TzId->TimeZoneInfo mapping can be found at this web address:
		/// http://unicode.org/repos/cldr-tmp/trunk/diff/supplemental/windows_tzid.html
		/// </remarks>
		/// <param name="tzId">The TzId</param>
		/// <returns>
		/// The <see cref="TimeZoneInfo"/> found or null if one could not be found to match the TzId specified
		/// </returns>
		public static TimeZoneInfo GetTimeZoneInfoForTzId(string tzId)
		{
			LoadTzIdMappingFromXml();
			if (_TzIdToWindowsTimeZoneMapping.ContainsKey(tzId))
				return _TzIdToWindowsTimeZoneMapping[tzId];
			return null;
		}


		/// <summary>
		/// Gets the TzId (also known as an Olson Time Zone key) for the specified <see cref="TimeZoneInfo"/>
		/// </summary>
		/// <remarks>
		/// TzIds are discussed on Wikipedia (http://en.wikipedia.org/wiki/List_of_tz_zones_by_name) and the
		/// data source for the TzId->TimeZoneInfo mapping can be found at this web address:
		/// http://unicode.org/repos/cldr-tmp/trunk/diff/supplemental/windows_tzid.html
		/// </remarks>
		/// <param name="timeZoneInfo">
		/// The <see cref="TimeZoneInfo"/> (must be a system time zone gotten from 
		/// <see cref="TimeZoneInfo.GetSystemTimeZones"/>)
		/// </param>
		/// <returns>
		/// The TzId found or null if one could not be found to match the <see cref="TimeZoneInfo"/> specified
		/// </returns>
		public static string ToTzId(this TimeZoneInfo timeZoneInfo)
		{
			LoadTzIdMappingFromXml();
			if (_WindowsTimeZoneToTzIdMapping.ContainsKey(timeZoneInfo))
				return _WindowsTimeZoneToTzIdMapping[timeZoneInfo].First();
			return null;
		}


		/// <summary>
		/// Gets all the TzIds (also known as Olson Time Zone keys) that fall within the specified 
		/// <see cref="TimeZoneInfo"/>
		/// </summary>
		/// <remarks>
		/// TzIds are discussed on Wikipedia (http://en.wikipedia.org/wiki/List_of_tz_zones_by_name) and the
		/// data source for the TzId->TimeZoneInfo mapping can be found at this web address:
		/// http://unicode.org/repos/cldr-tmp/trunk/diff/supplemental/windows_tzid.html
		/// </remarks>
		/// <param name="timeZoneInfo">
		/// The <see cref="TimeZoneInfo"/> (must be a system time zone gotten from 
		/// <see cref="TimeZoneInfo.GetSystemTimeZones"/>)
		/// </param>
		/// <returns>
		/// The TzIds found or null if none could be found to match the <see cref="TimeZoneInfo"/> specified
		/// </returns>
		public static IEnumerable<string> GetAllTzIds(this TimeZoneInfo timeZoneInfo)
		{
			LoadTzIdMappingFromXml();
			if (_WindowsTimeZoneToTzIdMapping.ContainsKey(timeZoneInfo))
				return _WindowsTimeZoneToTzIdMapping[timeZoneInfo];
			return null;
		}
	}
}
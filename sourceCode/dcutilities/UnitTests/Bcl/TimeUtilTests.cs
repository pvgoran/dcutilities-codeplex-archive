using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace DigitallyCreated.Utilities.Bcl
{
	/// <summary>
	/// Tests methods in the <see cref="TimeUtils"/>
	/// </summary>
	[TestClass]
	public class TimeUtilTests
	{
		/// <summary>
		/// Smoke tests the <see cref="TimeUtils.GetTimeZoneInfoForTzId"/> and
		/// <see cref="TimeUtils.ToTzId"/> methods
		/// </summary>
		[TestMethod]
		public void TestTzIdToWindowsTimeZoneMappingMethods()
		{
			TimeZoneInfo timeZoneInfo = TimeUtils.GetTimeZoneInfoForTzId("Australia/Sydney");
			Assert.AreEqual("AUS Eastern Standard Time", timeZoneInfo.Id);

			string tzId = timeZoneInfo.ToTzId();
			Assert.AreEqual("Australia/Sydney", tzId);

			IEnumerable<string> tzIds = timeZoneInfo.GetAllTzIds();
			Assert.AreEqual(2, tzIds.Count());
			Assert.IsTrue(tzIds.Contains("Australia/Sydney"));
			Assert.IsTrue(tzIds.Contains("Australia/Melbourne"));
		}
	}
}
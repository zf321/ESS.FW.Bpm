using System;
using ESS.FW.Bpm.Engine.Impl.Calendar;
using ESS.FW.Bpm.Engine.Impl.Util;
using NUnit.Framework;


namespace Engine.Tests.Standalone.Calendar
{

    
	using PvmTestCase = PvmTestCase;

    /// <summary>
	/// 
	/// </summary>
    [TestFixture]
    public class DurationBusinessCalendarTest : PvmTestCase
	{

        [Test]
        public virtual void TestSimpleDuration()
	  {
		DurationBusinessCalendar businessCalendar = new DurationBusinessCalendar();

		//SimpleDateFormat simpleDateFormat = new SimpleDateFormat("yyyy MM dd - HH:mm");
		DateTime now = DateTime.Parse("2010 06 11 - 17:23");
		ClockUtil.CurrentTime = now;

		DateTime? duedate = businessCalendar.ResolveDuedate("P2DT5H70M");

		DateTime expectedDuedate = DateTime.Parse("2010 06 13 - 23:33");

		Assert.AreEqual(expectedDuedate, duedate);
	  }

	}

}
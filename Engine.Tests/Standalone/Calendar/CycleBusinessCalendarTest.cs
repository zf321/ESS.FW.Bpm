using System;
using ESS.FW.Bpm.Engine.Impl.Calendar;
using ESS.FW.Bpm.Engine.Impl.Util;
using NUnit.Framework;

namespace Engine.Tests.Standalone.Calendar
{
    [TestFixture]
    public class CycleBusinessCalendarTest : PvmTestCase
    {
        [Test]
        public virtual void TestSimpleDuration()
        {
            var businessCalendar = new CycleBusinessCalendar();

            //SimpleDateFormat simpleDateFormat = new SimpleDateFormat("yyyy MM dd - HH:mm");
            var now = DateTime.Parse("2010 06 11 - 17:23");
            ClockUtil.CurrentTime = now;

            var duedate = businessCalendar.ResolveDuedate("R/P2DT5H70M");

            var expectedDuedate = DateTime.Parse("2010 06 13 - 23:33");

            Assert.AreEqual(expectedDuedate, duedate);
        }

        [Test]
        public virtual void TestSimpleCron()
        {
            var businessCalendar = new CycleBusinessCalendar();

            //SimpleDateFormat simpleDateFormat = new SimpleDateFormat("yyyy MM dd - HH:mm");
            var now = DateTime.Parse("2011 03 11 - 17:23");
            ClockUtil.CurrentTime = now;

            var duedate = businessCalendar.ResolveDuedate("0 0 0 1 * ?");

            var expectedDuedate = DateTime.Parse("2011 04 1 - 00:00");

            Assert.AreEqual(expectedDuedate, duedate);
        }
    }
}
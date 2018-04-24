using System;
using ESS.FW.Bpm.Engine.Impl.Calendar;
using ESS.FW.Bpm.Engine.Impl.Util;
using NUnit.Framework;

namespace Engine.Tests.Standalone.Calendar
{
    [TestFixture]
    public class DurationHelperTest
    {
        //[TestFixtureTearDown]
        public static void ResetTime()
        {
            ClockUtil.Reset();
        }
        private DateTime Parse(string str)
        {
            //SimpleDateFormat simpleDateFormat = new SimpleDateFormat("yyyyMMdd-HH:mm:ss");
            DateTime r;
            DateTime.TryParseExact(str, @"yyyyMMdd-HH:mm:ss", null, System.Globalization.DateTimeStyles.None,out r);
            return r;
        }

        [Test]
        public virtual void ShouldNotExceedNumber()
        {
            ClockUtil.CurrentTime = new DateTime(1970, 1, 1);
            var dh = new DurationHelper("R2/PT10S");

            ClockUtil.CurrentTime = new DateTime(1970,1,1).AddSeconds(15);
            var test= dh.DateAfter;
            Assert.AreEqual(20, dh.DateAfter.Value.TimeOfDay.Seconds);


            ClockUtil.CurrentTime = new DateTime(1970, 1, 1).AddSeconds(30);
            Assert.IsNull(dh.DateAfter);
        }

        [Test]
        public virtual void ShouldNotExceedNumberNegative()
        {
            ClockUtil.CurrentTime = Parse("19700101-00:00:00");
            var dh = new DurationHelper("R2/PT10S/1970-01-01T00:00:50");

            ClockUtil.CurrentTime = Parse("19700101-00:00:20");
            Assert.AreEqual(Parse("19700101-00:00:30"), dh.DateAfter);


            ClockUtil.CurrentTime = Parse("19700101-00:00:35");

            Assert.AreEqual(Parse("19700101-00:00:40"), dh.DateAfter);
        }

        [Test]
        public virtual void ShouldNotExceedNumberPeriods()
        {
            ClockUtil.CurrentTime = Parse("19700101-00:00:00");
            var dh = new DurationHelper("R2/1970-01-01T00:00:00/1970-01-01T00:00:10");

            ClockUtil.CurrentTime = Parse("19700101-00:00:15");
            Assert.AreEqual(Parse("19700101-00:00:20"), dh.DateAfter);


            ClockUtil.CurrentTime = Parse("19700101-00:00:30");
            Assert.IsNull(dh.DateAfter);
        }
    }
}
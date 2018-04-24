using System;
using ESS.FW.Bpm.Engine.Impl.Util;

namespace Engine.Tests.Util
{
    public sealed class ClockTestUtil
    {

        /// <summary>
        /// Increments the current time by the given seconds.
        /// </summary>
        /// <param name="seconds"> the seconds to add to the clock </param>
        /// <returns> the new current time </returns>
        public static DateTime IncrementClock(long seconds)
        {
            DateTime time = ClockUtil.CurrentTime;
            ClockUtil.CurrentTime = time.AddSeconds(seconds);
            return ClockUtil.CurrentTime;
        }

        /// <summary>
        /// Sets the clock to a date without milliseconds. Older mysql
        /// versions do not support milliseconds. Test which test timestamp
        /// should avoid timestamps with milliseconds.
        /// </summary>
        /// <returns> the new current time </returns>
        public static DateTime SetClockToDateWithoutMilliseconds()
        {
            ClockUtil.CurrentTime = new DateTime(1363608000000L);
            return ClockUtil.CurrentTime;
        }

    }

}
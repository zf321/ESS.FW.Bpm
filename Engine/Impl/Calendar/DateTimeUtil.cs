using System;

namespace ESS.FW.Bpm.Engine.Impl.Calendar
{
    /// <summary>
    ///     Utility for <seealso cref="DateTime" /> that uses the JVM timezone
    ///     for date / time related operations.
    ///     This is important as the JVM timezone and the system timezone may
    ///     differ which leads to different behavior in
    ///     <seealso cref="java.text.SimpleDateFormat" /> (using JVM default timezone) and
    ///     JODA time (using system default timezone).
    ///     
    /// </summary>
    public class DateTimeUtil
    {
        private static readonly TimeZone JvmDefaultDateTimeZone = TimeZone.CurrentTimeZone;

        public static DateTime Now()
        {
            return DateTime.Now;
        }

        public static DateTime ParseDateTime(string date)
        {
            //if (date.Contains("PT"))
            //{
            DateTime r;
            DateTime.TryParse(date, out r);
            if (r != DateTime.MinValue)
            {
                return r;
            }
            else
            {
                DurationHelper dur = new DurationHelper(date);
                return dur.DateAfter.Value;
            }
                
            //}
            //else
            //{
             //   return DateTime.Parse(date);
           // }
        }

        public static DateTime ParseDate(string date)
        {
            return ParseDateTime(date).Date;
        }
    }
}
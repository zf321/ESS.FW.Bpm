using System;
using System.Collections.Generic;
using System.Globalization;
using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.Impl.Util;

namespace ESS.FW.Bpm.Engine.Impl.Calendar
{
    /// <summary>
    ///      
    /// </summary>
    public class DefaultBusinessCalendar : IBusinessCalendar
    {
        private static readonly IDictionary<string, int> Units = new Dictionary<string, int>();

        static DefaultBusinessCalendar()
        {
            //units["millis"] = DateTime.;
            //units["seconds"] = DateTime.SECOND;
            //units["second"] = DateTime.SECOND;
            //units["minute"] = DateTime.MINUTE;
            //units["minutes"] = DateTime.MINUTE;
            //units["hour"] = DateTime.HOUR;
            //units["hours"] = DateTime.HOUR;
            //units["day"] = DateTime.DAY_OF_YEAR;
            //units["days"] = DateTime.DAY_OF_YEAR;
            //units["week"] = DateTime.WEEK_OF_YEAR;
            //units["weeks"] = DateTime.WEEK_OF_YEAR;
            //units["month"] = DateTime.MONTH;
            //units["months"] = DateTime.MONTH;
            //units["year"] = DateTime.YEAR;
            //units["years"] = DateTime.YEAR;
        }

        public virtual DateTime? ResolveDuedate(string duedate)
        {
            var resolvedDuedate = ClockUtil.CurrentTime;

            var tokens = duedate.Split(" and ", true);
            foreach (var token in tokens)
                resolvedDuedate = AddSingleUnitQuantity(resolvedDuedate, token);

            return resolvedDuedate;
        }

        protected internal virtual DateTime AddSingleUnitQuantity(DateTime startDate, string singleUnitQuantity)
        {
            var spaceIndex = singleUnitQuantity.IndexOf(" ", StringComparison.Ordinal);
            if ((spaceIndex == -1) || (singleUnitQuantity.Length < spaceIndex + 1))
                throw new ProcessEngineException("invalid duedate format: " + singleUnitQuantity);

            var quantityText = singleUnitQuantity.Substring(0, spaceIndex);
            int? quantity = Convert.ToInt32(quantityText);

            var unitText = singleUnitQuantity.Substring(spaceIndex + 1).Trim().ToLower();

            //int unit = units[unitText].Value;

            var calendar = new GregorianCalendar();
            //calendar.Time = startDate;
            //calendar.add(unit, quantity);

            //return calendar.Time;
            return calendar.ToDateTime(startDate.Year, startDate.Month, startDate.Day, startDate.Hour, startDate.Minute,
                startDate.Second, startDate.Millisecond);
        }
    }
}
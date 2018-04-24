using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Impl.Calendar
{
    /// <summary>
    ///      
    /// </summary>
    public class MapBusinessCalendarManager : IBusinessCalendarManager
    {
        private readonly IDictionary<string, IBusinessCalendar> _businessCalendars =
            new Dictionary<string, IBusinessCalendar>();

        public virtual IBusinessCalendar GetBusinessCalendar(string businessCalendarRef)
        {
            return _businessCalendars[businessCalendarRef.ToLower()];
        }

        public virtual IBusinessCalendarManager AddBusinessCalendar(string businessCalendarRef,
            IBusinessCalendar businessCalendar)
        {
            _businessCalendars[businessCalendarRef.ToLower()] = businessCalendar;
            return this;
        }
    }
}
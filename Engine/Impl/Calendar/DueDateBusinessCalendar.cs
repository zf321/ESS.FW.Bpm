using System;
using ESS.FW.Bpm.Engine.Impl.Util;

namespace ESS.FW.Bpm.Engine.Impl.Calendar
{
    public class DueDateBusinessCalendar : IBusinessCalendar
    {
        public const string Name = "dueDate";

        private static readonly EngineUtilLogger Log = ProcessEngineLogger.UtilLogger;

        public virtual DateTime? ResolveDuedate(string duedate)
        {
            try
            {
                return DateTimeUtil.ParseDateTime(duedate);
            }
            catch (System.Exception e)
            {
                throw Log.ExceptionWhileResolvingDuedate(duedate, e);
            }
        }
    }
}
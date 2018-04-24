using System;
using ESS.FW.Bpm.Engine.Impl.Util;
using Quartz;

namespace ESS.FW.Bpm.Engine.Impl.Calendar
{
    public class CycleBusinessCalendar : IBusinessCalendar
    {
        private static readonly EngineUtilLogger Log = ProcessEngineLogger.UtilLogger;

        public const string Name = "cycle";

        //TOOD 时间解析
        public virtual DateTime? ResolveDuedate(string duedateDescription)
        {
            //try
            //{
                if (duedateDescription.StartsWith("R", StringComparison.Ordinal))
                {
                    return (new DurationHelper(duedateDescription)).DateAfter.Value;
                }
                CronExpression ce = new CronExpression(duedateDescription);
                return ce.GetTimeAfter(new DateTimeOffset(ClockUtil.CurrentTime)).Value.DateTime;
            //}
            //catch (System.Exception e)
            //{
            //    throw Log.ExceptionWhileParsingCronExpresison(duedateDescription, e);
            //}
        }
    }
}
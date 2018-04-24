using System;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;

namespace ESS.FW.Bpm.Engine.Impl.Calendar
{
    /// <summary>
    ///      
    /// </summary>
    public class DurationBusinessCalendar : IBusinessCalendar
    {
        private static readonly EngineUtilLogger Log = ProcessEngineLogger.UtilLogger;

        public const string Name = "duration";

        public virtual DateTime? ResolveDuedate(string duedate)
        {
            try
            {
                //throw new NotImplementedException();
                var dh = new DurationHelper(duedate);
                return dh.DateAfter;
            }
            catch (System.Exception e)
            {
                throw Log.ExceptionWhileResolvingDuedate(duedate, e);
            }
        }
    }
}
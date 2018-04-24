using System;

namespace ESS.FW.Bpm.Engine.Impl.Calendar
{
    /// <summary>
    ///      
    /// </summary>
    public interface IBusinessCalendar
    {
        DateTime? ResolveDuedate(string duedateDescription);
    }
}
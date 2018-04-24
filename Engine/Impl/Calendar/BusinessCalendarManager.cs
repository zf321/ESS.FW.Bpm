namespace ESS.FW.Bpm.Engine.Impl.Calendar
{
    /// <summary>
    ///      
    /// </summary>
    public interface IBusinessCalendarManager
    {
        IBusinessCalendar GetBusinessCalendar(string businessCalendarRef);
    }
}
using System;

namespace ESS.FW.Bpm.Engine.Impl.JobExecutor
{
    /// <summary>
    ///      
    ///     
    /// </summary>
    public class GetUnlockedTimersByDuedateCmd //: ICommand<IList<TimerEntity>>
    {
        protected internal DateTime Duedate;
        protected internal Page Page;

        public GetUnlockedTimersByDuedateCmd(DateTime duedate, Page page)
        {
            this.Duedate = duedate;
            this.Page = page;
        }

        //public virtual IList<TimerEntity> execute(CommandContext commandContext)
        //{

        //    return Context.CommandContext.JobManager.findUnlockedTimersByDuedate(duedate, page);
        //}
    }
}
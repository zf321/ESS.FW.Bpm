using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Impl.JobExecutor
{
    /// <summary>
    ///     
    /// </summary>
    public class CallerRunsRejectedJobsHandler : IRejectedJobsHandler
    {
        public virtual void JobsRejected(IList<string> jobIds, ProcessEngineImpl processEngine, JobExecutor jobExecutor)
        {
            jobExecutor.GetExecuteJobsRunnable(jobIds, processEngine).Run(null);
        }
    }
}
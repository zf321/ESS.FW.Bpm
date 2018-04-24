using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Impl.JobExecutor
{
    /// <summary>
    ///     
    /// </summary>
    public class NotifyAcquisitionRejectedJobsHandler : IRejectedJobsHandler
    {
        public virtual void JobsRejected(IList<string> jobIds, ProcessEngineImpl processEngine, JobExecutor jobExecutor)
        {
            var acquireJobsRunnable = jobExecutor.AcquireJobsRunnable;
            if (acquireJobsRunnable is SequentialJobAcquisitionRunnable)
            {
                var context = ((SequentialJobAcquisitionRunnable) acquireJobsRunnable).AcquisitionContext;
                context.SubmitRejectedBatch(processEngine.Name, jobIds);
            }
            else
            {
                jobExecutor.GetExecuteJobsRunnable(jobIds, processEngine).Run(null);
            }
        }
    }
}
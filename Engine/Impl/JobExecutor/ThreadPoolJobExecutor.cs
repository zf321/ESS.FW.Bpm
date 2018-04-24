using ESS.FW.Bpm.Engine.Exception;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Engine.Impl.JobExecutor
{
    public class ThreadPoolJobExecutor : JobExecutor
    {
        
        protected internal override void StartExecutingJobs()
        {
            StartJobAcquisitionThread();
        }

        protected internal override void StopExecutingJobs()
        {
            StopJobAcquisitionThread();
        }

        public override void ExecuteJobs(IList<string> jobIds, ProcessEngineImpl processEngine)
        {
            try
            {
                var runnable = GetExecuteJobsRunnable(jobIds, processEngine);
                ThreadPool.QueueUserWorkItem(runnable.Run);
            }
            catch (RejectedExecutionException)
            {
                LogRejectedExecution(processEngine, jobIds.Count);
                RejectedJobsHandler.JobsRejected(jobIds, processEngine, this);
            }
        }
    }
}
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Repository;
using System.Threading;

namespace ESS.FW.Bpm.Engine.Impl.JobExecutor
{
    /// <summary>
    ///     
    /// </summary>
    public class JobExecutorLogger : ProcessEngineLogger
    {
        public virtual void DebugAcquiredJobNotFound(string jobId)
        {
            LogDebug("001", "Acquired job with id '{0}' not found.", jobId);
        }

        public virtual void ExceptionWhileExecutingJob(JobEntity job, System.Exception exception)
        {
            LogWarn("002", "Exception while executing job {0}: ", job, exception);
        }

        public virtual void DebugFallbackToDefaultRetryStrategy()
        {
            LogDebug("003", "Falling back to default retry stratefy");
        }

        public virtual void DebugDecrementingRetriesForJob(string id)
        {
            LogDebug("004", "Decrementing retries of job {0}", id);
        }

        public virtual void DebugInitiallyAppyingRetryCycleForJob(string id, int times)
        {
            LogDebug("005", "Applying job retry time cycle for the first time for job {0}, retires {1}", id, times);
        }

        public virtual void ExceptionWhileExecutingJob(string nextJobId, System.Exception t)
        {
            LogWarn("006", "Exception while executing job {0}: ", nextJobId, t);
        }

        public virtual void CouldNotDeterminePriority(ExecutionEntity execution, object value, ProcessEngineException e)
        {
            LogWarn("007",
                "Could not determine priority for job created in context of execution {0}. Using default priority {1}",
                execution, value, e);
        }

        public virtual void DebugAddingNewExclusiveJobToJobExecutorCOntext(string jobId)
        {
            LogInfo("008", "Adding new exclusive job to job executor context. Job Id='{0}'", jobId);
        }

        public virtual void TimeoutDuringShutdown()
        {
            LogWarn("009",
                "Timeout during shutdown of job executor. The current running jobs could not end within 60 seconds after shutdown operation");
        }

        public virtual void InterruptedWhileShuttingDownjobExecutor(ThreadInterruptedException e)
        {
            LogWarn("010", "Interrupted while shutting down the job executor", e);
        }

        public virtual void DebugJobAcquisitionThreadSleeping(long millis)
        {
            LogDebug("011", "Job acquisition thread sleeping for {0} millis", millis);
        }

        public virtual void JobExecutorThreadWokeUp()
        {
            LogDebug("012", "Job acquisition thread woke up");
        }

        public virtual void JobExecutionWaitInterrupted()
        {
            LogDebug("013", "Job Execution wait interrupted");
        }

        public virtual void StartingUpJobExecutor(string name)
        {
            LogInfo("014", "Starting up the JobExecutor[{0}].", name);
        }

        public virtual void ShuttingDownTheJobExecutor(string name)
        {
            LogInfo("015", "Shutting down the JobExecutor[{0}]", name);
        }

        public virtual void IgnoringSuspendedJob(IProcessDefinition processDefinition)
        {
            LogDebug("016", "Ignoring job of suspended {0}", processDefinition);
        }

        public virtual void DebugNotifyingJobExecutor(string @string)
        {
            LogDebug("017", "Notifying Job Executor of new job {0}", @string);
        }

        public virtual void StartingToAcquireJobs(string name)
        {
            LogInfo("018", "{0} starting to acquire jobs", name);
        }

        public virtual void ExceptionDuringJobAcquisition(System.Exception e)
        {
            LogError("019", "Exception during job acquisition {0}", e.Message, e);
        }

        public virtual void StoppedJobAcquisition(string name)
        {
            LogInfo("020", "{0} stopped job acquisition", name);
        }

        public virtual void ExceptionWhileUnlockingJob(string jobId, System.Exception t)
        {
            LogWarn("021", "Exception while unaquiring job {0}: ", jobId, t);
        }

        public virtual void AcquiredJobs(string processEngine, AcquiredJobs acquiredJobs)
        {
            LogDebug("022", "Acquired {0} jobs for process engine '{1}': {2}", acquiredJobs.Size(), processEngine, acquiredJobs.JobIdBatches);
        }

        public virtual void ExecuteJobs(string processEngine, ICollection<string> jobs)
        {
            LogDebug("023", "Execute jobs for process engine '{0}': {1}", processEngine, jobs);
        }

        public virtual void DebugFailedJobNotFound(string jobId)
        {
            LogDebug("024", "Failed job with id '{0}' not found.", jobId);
        }

        public virtual ProcessEngineException WrapJobExecutionFailure(JobFailureCollector jobFailureCollector,
            System.Exception cause)
        {
            var job = jobFailureCollector.Job;
            if (job != null)
                return
                    new ProcessEngineException(
                        ExceptionMessage("025", "Exception while executing job {0}: ", jobFailureCollector.JobId), cause);
            return
                new ProcessEngineException(
                    ExceptionMessage("025", "Exception while executing job {0}: ", jobFailureCollector.JobId), cause);
        }

        public virtual ProcessEngineException JobNotFoundException(string jobId)
        {
            return new ProcessEngineException(ExceptionMessage("026", "No job found with id '{0}'", jobId));
        }
    }
}
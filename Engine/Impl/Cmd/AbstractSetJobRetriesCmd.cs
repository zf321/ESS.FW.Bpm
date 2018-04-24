using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///     
    /// </summary>
    public class AbstractSetJobRetriesCmd
    {
        protected internal const string Retries = "retries";

        protected internal virtual string LogEntryOperation
        {
            get { return UserOperationLogEntryFields.OperationTypeSetJobRetries; }
        }

        protected internal virtual void SetJobRetriesByJobId(string jobId, int retries, CommandContext commandContext)
        {
            JobEntity job = commandContext.JobManager.FindJobById(jobId);
            if (job != null)
            {
                foreach (var checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
                {
                    checker.CheckUpdateJob(job);
                }

                if (job.InInconsistentLockState)
                {
                    job.ResetLock();
                }
                int oldRetries = job.Retries;
                job.Retries = retries;

                PropertyChange propertyChange = new PropertyChange(Retries, oldRetries, job.Retries);
                commandContext.OperationLogManager.LogJobOperation(LogEntryOperation, job.Id, job.JobDefinitionId,
                    job.ProcessInstanceId, job.ProcessDefinitionId, job.ProcessDefinitionKey, propertyChange);
            }
            else
            {
                throw new ProcessEngineException("No job found with id '" + jobId + "'.");
            }
        }
    }
}
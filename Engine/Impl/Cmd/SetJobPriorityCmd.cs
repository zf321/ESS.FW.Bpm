using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///     
    /// </summary>
    public class SetJobPriorityCmd : ICommand<object>
    {
        public const string JobPriorityProperty = "priority";

        protected internal string JobId;
        protected internal long Priority;

        public SetJobPriorityCmd(string jobId, long priority)
        {
            this.JobId = jobId;
            this.Priority = priority;
        }

        public virtual object Execute(CommandContext commandContext)
        {
            EnsureUtil.EnsureNotNull("job id must not be null", "jobId", JobId);

            JobEntity job = commandContext.JobManager.FindJobById(JobId);
            EnsureUtil.EnsureNotNull(typeof(NotFoundException), "No job found with id '" + JobId + "'", "job", job);

            foreach (ICommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
            {
                checker.CheckUpdateJob(job);
            }

            long currentPriority = job.Priority;
            job.Priority = Priority;

            CreateOpLogEntry(commandContext, currentPriority, job);
            return null;
        }

        protected internal virtual void CreateOpLogEntry(CommandContext commandContext, long previousPriority,
            JobEntity job)
        {
            PropertyChange propertyChange = new PropertyChange(JobPriorityProperty, previousPriority, job.Priority);
            commandContext.OperationLogManager.LogJobOperation(
                UserOperationLogEntryFields.OperationTypeSetPriority, job.Id, job.JobDefinitionId,
                job.ProcessInstanceId, job.ProcessDefinitionId, job.ProcessDefinitionKey, propertyChange);
        }
    }
}
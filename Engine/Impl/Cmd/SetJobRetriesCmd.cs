using System;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///     
    /// </summary>
    [Serializable]
    public class SetJobRetriesCmd : AbstractSetJobRetriesCmd, ICommand<object>
    {
        protected internal const long SerialVersionUid = 1L;
        protected internal readonly string JobDefinitionId;
        protected internal readonly string JobId;
        protected internal readonly int Retries;


        public SetJobRetriesCmd(string jobId, string jobDefinitionId, int retries)
        {
            if ((ReferenceEquals(jobId, null) || (jobId.Length == 0)) &&
                (ReferenceEquals(jobDefinitionId, null) || (jobDefinitionId.Length == 0)))
                throw new ProcessEngineException("Either job definition id or job id has to be provided as parameter.");

            if (retries < 0)
                throw new ProcessEngineException("The number of job retries must be a non-negative Integer, but '" +
                                                 retries + "' has been provided.");

            this.JobId = jobId;
            this.JobDefinitionId = jobDefinitionId;
            this.Retries = retries;
        }

        protected internal virtual CommandContext JobRetriesByJobDefinitionId
        {
            set
            {
                IJobDefinitionManager jobDefinitionManager = value.JobDefinitionManager;
                JobDefinitionEntity jobDefinition = jobDefinitionManager.FindById(JobDefinitionId);

                if (jobDefinition != null)
                {
                    var processDefinitionId = jobDefinition.ProcessDefinitionId;
                    foreach (var checker in value.ProcessEngineConfiguration.CommandCheckers)
                    {
                        checker.CheckUpdateProcessInstanceByProcessDefinitionId(processDefinitionId);
                    }
                }

                value.JobManager.UpdateFailedJobRetriesByJobDefinitionId(JobDefinitionId, Retries);

                PropertyChange propertyChange = new PropertyChange(AbstractSetJobRetriesCmd.Retries, null, Retries);
                value.OperationLogManager.LogJobOperation(LogEntryOperation, null, JobDefinitionId, null, null, null,
                    propertyChange);
            }
        }

        public virtual object Execute(CommandContext commandContext)
        {
            if (!ReferenceEquals(JobId, null))
                SetJobRetriesByJobId(JobId, Retries, commandContext);
            else
                JobRetriesByJobDefinitionId = commandContext;
            return null;
        }
    }
}
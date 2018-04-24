using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.oplog;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{



    /// <summary>
    ///     
    /// </summary>
    public class SetJobDefinitionPriorityCmd : ICommand<object>
    {
        public const string JobDefinitionOverridingPriority = "overridingPriority";
        protected internal bool Cascade;

        protected internal string JobDefinitionId;
        protected internal long? Priority;

        public SetJobDefinitionPriorityCmd(string jobDefinitionId, long? priority, bool cascade)
        {
            this.JobDefinitionId = jobDefinitionId;
            this.Priority = priority;
            this.Cascade = cascade;
        }

        public virtual object Execute(CommandContext commandContext)
        {
            EnsureUtil.EnsureNotNull(typeof(NotValidException), "jobDefinitionId", JobDefinitionId);

            JobDefinitionEntity jobDefinition = commandContext.JobDefinitionManager.FindById(JobDefinitionId);

            EnsureUtil.EnsureNotNull(typeof(NotFoundException),
                "Job definition with id '" + JobDefinitionId + "' does not exist", "jobDefinition", jobDefinition);

            CheckUpdateProcess(commandContext, jobDefinition);

            long? currentPriority = jobDefinition.OverridingJobPriority;
            jobDefinition.JobPriority = Priority;

            var opLogContext = new UserOperationLogContext();
            CreateJobDefinitionOperationLogEntry(opLogContext, currentPriority, jobDefinition);

            if (Cascade && Priority != null)
            {
                commandContext.JobManager.UpdateJobPriorityByDefinitionId(JobDefinitionId, Priority.Value);
                CreateCascadeJobsOperationLogEntry(opLogContext, jobDefinition);
            }

            commandContext.OperationLogManager.LogUserOperations(opLogContext);

            return null;
        }

        protected internal virtual void CheckUpdateProcess(CommandContext commandContext,
            JobDefinitionEntity jobDefinition)
        {
            var processDefinitionId = jobDefinition.ProcessDefinitionId;

            foreach (var checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
            {
                checker.CheckUpdateProcessDefinitionById(processDefinitionId);

                if (Cascade)
                    checker.CheckUpdateProcessInstanceByProcessDefinitionId(processDefinitionId);
            }
        }

        protected internal virtual void CreateJobDefinitionOperationLogEntry(UserOperationLogContext opLogContext,
            long? previousPriority, JobDefinitionEntity jobDefinition)
        {
            PropertyChange propertyChange = new PropertyChange(JobDefinitionOverridingPriority, previousPriority,
                jobDefinition.OverridingJobPriority);

            UserOperationLogContextEntry entry =
                UserOperationLogContextEntryBuilder.Entry(UserOperationLogEntryFields.OperationTypeSetPriority,
                    EntityTypes.JobDefinition).InContextOf(jobDefinition).PropertyChanges(propertyChange).Create();

            opLogContext.AddEntry(entry);
        }

        protected internal virtual void CreateCascadeJobsOperationLogEntry(UserOperationLogContext opLogContext,
            JobDefinitionEntity jobDefinition)
        {
            // old value is unknown
            PropertyChange propertyChange = new PropertyChange(SetJobPriorityCmd.JobPriorityProperty, null,
                jobDefinition.OverridingJobPriority);

            UserOperationLogContextEntry entry =
                UserOperationLogContextEntryBuilder.Entry(UserOperationLogEntryFields.OperationTypeSetPriority,
                    EntityTypes.Job).InContextOf(jobDefinition).PropertyChanges(propertyChange).Create();

            opLogContext.AddEntry(entry);
        }
    }
}
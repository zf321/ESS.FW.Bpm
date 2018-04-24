using System;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using ESS.FW.Bpm.Engine.Management;
using ESS.FW.Bpm.Engine.Management.Impl;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///     
    ///     
    /// </summary>
    public abstract class AbstractSetJobDefinitionStateCmd : AbstractSetStateCmd
    {
        protected internal new DateTime ExecutionDate;
        protected internal bool IsProcessDefinitionTenantIdSet;

        protected internal string JobDefinitionId;
        protected internal string ProcessDefinitionId;
        protected internal string ProcessDefinitionKey;

        protected internal string ProcessDefinitionTenantId;

        public AbstractSetJobDefinitionStateCmd(IUpdateJobDefinitionSuspensionStateBuilder builder)
            : base(builder.IncludeJobs, builder.GetExecutionDate())
        {
            JobDefinitionId = builder.JobDefinitionId;
            ProcessDefinitionId = builder.ProcessDefinitionId;
            ProcessDefinitionKey = builder.ProcessDefinitionKey;

            IsProcessDefinitionTenantIdSet = builder.ProcessDefinitionTenantIdSet;
            ProcessDefinitionTenantId = builder.ProcessDefinitionTenantId;
        }

        protected internal override IJobHandlerConfiguration JobHandlerConfiguration
        {
            get
            {
                if (!ReferenceEquals(JobDefinitionId, null))
                    return
                        TimerChangeJobDefinitionSuspensionStateJobHandler.JobDefinitionSuspensionStateConfiguration
                            .ByJobDefinitionId(JobDefinitionId, IncludeSubResources);
                if (!ReferenceEquals(ProcessDefinitionId, null))
                    return
                        TimerChangeJobDefinitionSuspensionStateJobHandler.JobDefinitionSuspensionStateConfiguration
                            .ByProcessDefinitionId(ProcessDefinitionId, IncludeSubResources);
                if (!IsProcessDefinitionTenantIdSet)
                    return
                        TimerChangeJobDefinitionSuspensionStateJobHandler.JobDefinitionSuspensionStateConfiguration
                            .ByProcessDefinitionKey(ProcessDefinitionKey, IncludeSubResources);
                return
                    TimerChangeJobDefinitionSuspensionStateJobHandler.JobDefinitionSuspensionStateConfiguration
                        .ByProcessDefinitionKeyAndTenantId(ProcessDefinitionKey, ProcessDefinitionTenantId,
                            IncludeSubResources);
            }
        }

        /// <summary>
        ///     Subclasses should return the type of the <seealso cref="IJobHandler{T}" /> here. it will be used when
        ///     the user provides an execution date on which the actual state change will happen.
        /// </summary>
        protected internal abstract override string DelayedExecutionJobHandlerType { get; }

        protected internal override AbstractSetStateCmd NextCommand
        {
            get
            {
                var jobCommandBuilder = CreateJobCommandBuilder();

                return GetNextCommand(jobCommandBuilder);
            }
        }

        protected internal override void CheckParameters(CommandContext commandContext)
        {
            if (ReferenceEquals(JobDefinitionId, null) && ReferenceEquals(ProcessDefinitionId, null) &&
                ReferenceEquals(ProcessDefinitionKey, null))
                throw new ProcessEngineException(
                    "Job definition id, process definition id nor process definition key cannot be null");
        }

        protected internal override void CheckAuthorization(CommandContext commandContext)
        {
            foreach (var checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
            {
                if (!ReferenceEquals(JobDefinitionId, null))
                {
                    IJobDefinitionManager jobDefinitionManager = commandContext.JobDefinitionManager;
                    JobDefinitionEntity jobDefinition = jobDefinitionManager.FindById(JobDefinitionId);

                    if (jobDefinition != null && !ReferenceEquals(jobDefinition.ProcessDefinitionKey, null))
                    {
                        var processDefinitionKey = jobDefinition.ProcessDefinitionKey;
                        checker.CheckUpdateProcessDefinitionByKey(processDefinitionKey);

                        if (includeSubResources)
                        {
                            checker.CheckUpdateProcessInstanceByProcessDefinitionKey(processDefinitionKey);
                        }
                    }
                }
                else
                {
                    if (!ReferenceEquals(ProcessDefinitionId, null))
                    {
                        checker.CheckUpdateProcessDefinitionById(ProcessDefinitionId);

                        if (includeSubResources)
                        {
                            checker.CheckUpdateProcessInstanceByProcessDefinitionId(ProcessDefinitionId);
                        }
                    }
                    else
                    {
                        if (!ReferenceEquals(ProcessDefinitionKey, null))
                        {
                            checker.CheckUpdateProcessDefinitionByKey(ProcessDefinitionKey);

                            if (includeSubResources)
                            {
                                checker.CheckUpdateProcessInstanceByProcessDefinitionKey(ProcessDefinitionKey);
                            }
                        }
                    }
                }
            }
        }

        protected internal override void UpdateSuspensionState(CommandContext commandContext, ISuspensionState suspensionState)
        {
            IJobDefinitionManager jobDefinitionManager = commandContext.JobDefinitionManager;
            IJobManager jobManager = commandContext.JobManager;

            if (!string.IsNullOrEmpty(JobDefinitionId))
            {
                jobDefinitionManager.UpdateJobDefinitionSuspensionStateById(JobDefinitionId, suspensionState);
            }
            else if (!string.IsNullOrEmpty(ProcessDefinitionId))
            {
                jobDefinitionManager.UpdateJobDefinitionSuspensionStateByProcessDefinitionId(ProcessDefinitionId,suspensionState);
                jobManager.UpdateStartTimerJobSuspensionStateByProcessDefinitionId(ProcessDefinitionId, suspensionState);
            }
            else if (!string.IsNullOrEmpty(ProcessDefinitionKey))
            {
                if (!IsProcessDefinitionTenantIdSet)
                {
                    jobDefinitionManager.UpdateJobDefinitionSuspensionStateByProcessDefinitionKey(ProcessDefinitionKey,suspensionState);
                    jobManager.UpdateStartTimerJobSuspensionStateByProcessDefinitionKey(ProcessDefinitionKey,suspensionState);
                }
                else
                {
                    jobDefinitionManager.UpdateJobDefinitionSuspensionStateByProcessDefinitionKeyAndTenantId(ProcessDefinitionKey, ProcessDefinitionTenantId, suspensionState);
                    jobManager.UpdateStartTimerJobSuspensionStateByProcessDefinitionKeyAndTenantId(ProcessDefinitionKey, ProcessDefinitionTenantId, suspensionState);
                }
            }
        }

        protected internal override void LogUserOperation(CommandContext commandContext)
        {
            PropertyChange propertyChange = new PropertyChange(SuspensionStateProperty, null, NewSuspensionState.Name);
            commandContext.OperationLogManager.LogJobDefinitionOperation(LogEntryOperation, JobDefinitionId,
               ProcessDefinitionId, ProcessDefinitionKey, propertyChange);
        }

        protected internal virtual UpdateJobSuspensionStateBuilderImpl CreateJobCommandBuilder()
        {
            var builder = new UpdateJobSuspensionStateBuilderImpl();

            if (!string.IsNullOrEmpty(JobDefinitionId))
            {
                builder.ByJobDefinitionId(JobDefinitionId);
            }
            else if (!string.IsNullOrEmpty(ProcessDefinitionId))
            {
                builder.ByProcessDefinitionId(ProcessDefinitionId);
            }
            else if (!string.IsNullOrEmpty(ProcessDefinitionKey))
            {
                builder.ByProcessDefinitionKey(ProcessDefinitionKey);

                if (IsProcessDefinitionTenantIdSet && !string.IsNullOrEmpty(ProcessDefinitionTenantId))
                    builder.ProcessDefinitionTenantId(ProcessDefinitionTenantId);
                else if (IsProcessDefinitionTenantIdSet)
                    builder.ProcessDefinitionWithoutTenantId();
            }
            return builder;
        }

        protected internal abstract AbstractSetJobStateCmd GetNextCommand(
            UpdateJobSuspensionStateBuilderImpl jobCommandBuilder);
    }
}
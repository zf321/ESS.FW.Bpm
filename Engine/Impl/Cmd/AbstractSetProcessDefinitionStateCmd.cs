using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using ESS.FW.Bpm.Engine.Impl.runtime;
using ESS.FW.Bpm.Engine.Management;
using ESS.FW.Bpm.Engine.Management.Impl;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Persistence.Entity.Impl;
using ESS.FW.Bpm.Engine.Repository.Impl;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{

    /// <summary>
    ///     
    ///     
    ///     
    /// </summary>
    public abstract class AbstractSetProcessDefinitionStateCmd : AbstractSetStateCmd
    {
        protected internal bool IsTenantIdSet;

        protected internal string ProcessDefinitionId;
        protected internal string ProcessDefinitionKey;

        protected internal string TenantId;

        public AbstractSetProcessDefinitionStateCmd(UpdateProcessDefinitionSuspensionStateBuilderImpl builder)
            : base(builder.IncludeProcessInstances, builder.ExecutionDate)
        {
            ProcessDefinitionId = builder.ProcessDefinitionId;
            ProcessDefinitionKey = builder.ProcessDefinitionKey;

            IsTenantIdSet = builder.TenantIdSet;
            TenantId = builder.ProcessDefinitionTenantId;
        }

        protected internal override IJobHandlerConfiguration JobHandlerConfiguration
        {
            get
            {
                if (!ReferenceEquals(ProcessDefinitionId, null))
                    return
                        TimerChangeProcessDefinitionSuspensionStateJobHandler
                            .ProcessDefinitionSuspensionStateConfiguration.ByProcessDefinitionId(ProcessDefinitionId,
                                IncludeSubResources);
                if (IsTenantIdSet)
                    return
                        TimerChangeProcessDefinitionSuspensionStateJobHandler
                            .ProcessDefinitionSuspensionStateConfiguration.ByProcessDefinitionKeyAndTenantId(
                                ProcessDefinitionKey, TenantId, IncludeSubResources);
                return
                    TimerChangeProcessDefinitionSuspensionStateJobHandler.ProcessDefinitionSuspensionStateConfiguration
                        .ByProcessDefinitionKey(ProcessDefinitionKey, IncludeSubResources);
            }
        }

        // ABSTRACT METHODS ////////////////////////////////////////////////////////////////////

        /// <summary>
        ///     Subclasses should return the type of the <seealso cref="IJobHandler{T}" /> here. it will be used when
        ///     the user provides an execution date on which the actual state change will happen.
        /// </summary>
        protected internal abstract override string DelayedExecutionJobHandlerType { get; }

        protected internal override AbstractSetStateCmd NextCommand
        {
            get
            {
                var processInstanceCommandBuilder = CreateProcessInstanceCommandBuilder();

                return GetNextCommand(processInstanceCommandBuilder);
            }
        }

        protected internal override void CheckParameters(CommandContext commandContext)
        {
            // Validation of input parameters
            if (ReferenceEquals(ProcessDefinitionId, null) && ReferenceEquals(ProcessDefinitionKey, null))
                throw new ProcessEngineException("Process definition id / key cannot be null");
        }

        protected internal override void CheckAuthorization(CommandContext commandContext)
        {
            foreach (var checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
                if (!ReferenceEquals(ProcessDefinitionId, null))
                {
                    checker.CheckUpdateProcessDefinitionById(ProcessDefinitionId);

                    if (includeSubResources)
                        checker.CheckUpdateProcessInstanceByProcessDefinitionId(ProcessDefinitionId);
                }
                else
                {
                    if (!ReferenceEquals(ProcessDefinitionKey, null))
                    {
                        checker.CheckUpdateProcessDefinitionByKey(ProcessDefinitionKey);

                        if (includeSubResources)
                            checker.CheckUpdateProcessInstanceByProcessDefinitionKey(ProcessDefinitionKey);
                    }
                }
        }
        protected internal override void UpdateSuspensionState(CommandContext commandContext,
            ISuspensionState suspensionState)
        {
            IProcessDefinitionManager processDefinitionManager = commandContext.ProcessDefinitionManager;

            if (ProcessDefinitionId!= null)
            {
                processDefinitionManager.UpdateProcessDefinitionSuspensionStateById(ProcessDefinitionId, suspensionState);
            }
            else if (IsTenantIdSet)
            {
                processDefinitionManager.UpdateProcessDefinitionSuspensionStateByKeyAndTenantId(ProcessDefinitionKey, TenantId, suspensionState);
            }
            else
            {
                processDefinitionManager.UpdateProcessDefinitionSuspensionStateByKey(ProcessDefinitionKey, suspensionState);
            }

            commandContext.RunWithoutAuthorization(()=>{
                var jobDefinitionSuspensionStateBuilder = CreateJobDefinitionCommandBuilder();
                var jobDefinitionCmd = GetSetJobDefinitionStateCmd(jobDefinitionSuspensionStateBuilder);
                jobDefinitionCmd.DisableLogUserOperation();
                jobDefinitionCmd.Execute(commandContext);
            });
        }

        protected internal virtual UpdateJobDefinitionSuspensionStateBuilderImpl CreateJobDefinitionCommandBuilder()
        {
            var jobDefinitionBuilder = new UpdateJobDefinitionSuspensionStateBuilderImpl();

            if (!ReferenceEquals(ProcessDefinitionId, null))
            {
                jobDefinitionBuilder.ByProcessDefinitionId(ProcessDefinitionId);
            }
            else if (!ReferenceEquals(ProcessDefinitionKey, null))
            {
                jobDefinitionBuilder.ByProcessDefinitionKey(ProcessDefinitionKey);

                if (IsTenantIdSet && !ReferenceEquals(TenantId, null))
                    jobDefinitionBuilder.SetProcessDefinitionTenantId(TenantId);
                else if (IsTenantIdSet)
                    jobDefinitionBuilder.GetProcessDefinitionWithoutTenantId();
            }
            return jobDefinitionBuilder;
        }

        protected internal virtual UpdateProcessInstanceSuspensionStateBuilderImpl CreateProcessInstanceCommandBuilder()
        {
            var processInstanceBuilder = new UpdateProcessInstanceSuspensionStateBuilderImpl();

            if (!ReferenceEquals(ProcessDefinitionId, null))
            {
                processInstanceBuilder.ByProcessDefinitionId(ProcessDefinitionId);
            }
            else if (!ReferenceEquals(ProcessDefinitionKey, null))
            {
                processInstanceBuilder.ByProcessDefinitionKey(ProcessDefinitionKey);

                if (IsTenantIdSet && !ReferenceEquals(TenantId, null))
                    processInstanceBuilder.ProcessDefinitionTenantId(TenantId);
                else if (IsTenantIdSet)
                    processInstanceBuilder.ProcessDefinitionWithoutTenantId();
            }
            return processInstanceBuilder;
        }

        protected internal override void LogUserOperation(CommandContext commandContext)
        {
            PropertyChange propertyChange = new PropertyChange(SuspensionStateProperty, null, NewSuspensionState.Name);
            commandContext.OperationLogManager.LogProcessDefinitionOperation(LogEntryOperation, ProcessDefinitionId, ProcessDefinitionKey, propertyChange);
        }

        /// <summary>
        ///     Subclasses should return the type of the <seealso cref="AbstractSetJobDefinitionStateCmd" /> here.
        ///     It will be used to suspend or activate the <seealso cref="IJobDefinition" />s.
        /// </summary>
        /// <param name="jobDefinitionSuspensionStateBuilder"> </param>
        protected internal abstract AbstractSetJobDefinitionStateCmd GetSetJobDefinitionStateCmd(
            UpdateJobDefinitionSuspensionStateBuilderImpl jobDefinitionSuspensionStateBuilder);

        protected internal abstract AbstractSetProcessInstanceStateCmd GetNextCommand(
            UpdateProcessInstanceSuspensionStateBuilderImpl processInstanceCommandBuilder);
        
    }
}
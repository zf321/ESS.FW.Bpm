using System;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.Cmd;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;

namespace ESS.FW.Bpm.Engine.Management.Impl
{

    public class UpdateJobDefinitionSuspensionStateBuilderImpl : IUpdateJobDefinitionSuspensionStateBuilder,
        IUpdateJobDefinitionSuspensionStateSelectBuilder, IUpdateJobDefinitionSuspensionStateTenantBuilder
    {
        private static readonly CommandLogger Log = ProcessEngineLogger.CmdLogger;

        protected internal readonly ICommandExecutor CommandExecutor;

        protected DateTime? executionDate;

        public UpdateJobDefinitionSuspensionStateBuilderImpl(ICommandExecutor commandExecutor)
        {
            this.CommandExecutor = commandExecutor;
        }

        /// <summary>
        ///     Creates a builder without CommandExecutor which can not be used to update
        ///     the suspension state via <seealso cref="#activate()" /> or <seealso cref="#suspend()" />. Can only be
        ///     used in combination with your own command.
        /// </summary>
        public UpdateJobDefinitionSuspensionStateBuilderImpl() : this(null)
        {
        }

        public virtual string ProcessDefinitionKey { get; protected internal set; }

        public virtual string ProcessDefinitionId { get; protected internal set; }

        public virtual string ProcessDefinitionTenantId { get; protected internal set; }

        public virtual bool ProcessDefinitionTenantIdSet { get; protected internal set; }

        public virtual string JobDefinitionId { get; protected internal set; }

        public virtual bool IncludeJobs { get; protected internal set; }



        public virtual void Activate()
        {
            ValidateParameters();

            var command = new ActivateJobDefinitionCmd(this);
            CommandExecutor.Execute(command);
        }

        public virtual void Suspend()
        {
            ValidateParameters();

            var command = new SuspendJobDefinitionCmd(this);
            CommandExecutor.Execute(command);
        }

        public IUpdateJobDefinitionSuspensionStateBuilder SetIncludeJobs(bool includeJobs)
        {
            IncludeJobs = includeJobs;
            return this;
        }
        

        public virtual IUpdateJobDefinitionSuspensionStateBuilder ByJobDefinitionId(string jobDefinitionId)
        {
            EnsureUtil.EnsureNotNull("jobDefinitionId", jobDefinitionId);
            this.JobDefinitionId = jobDefinitionId;
            return this;
        }

        public virtual IUpdateJobDefinitionSuspensionStateBuilder ByProcessDefinitionId(string processDefinitionId)
        {
            EnsureUtil.EnsureNotNull("processDefinitionId", processDefinitionId);
            this.ProcessDefinitionId = processDefinitionId;
            return this;
        }

        public virtual IUpdateJobDefinitionSuspensionStateTenantBuilder ByProcessDefinitionKey(string processDefinitionKey)
        {
            EnsureUtil.EnsureNotNull("processDefinitionKey", processDefinitionKey);
            this.ProcessDefinitionKey = processDefinitionKey;
            return this;
        }

        public virtual IUpdateJobDefinitionSuspensionStateBuilder GetProcessDefinitionWithoutTenantId()
        {
            ProcessDefinitionTenantId = null;
            ProcessDefinitionTenantIdSet = true;
            return this;
        }

        public virtual IUpdateJobDefinitionSuspensionStateBuilder SetProcessDefinitionTenantId(string tenantId)
        {
            EnsureUtil.EnsureNotNull("tenantId", tenantId);

            ProcessDefinitionTenantId = tenantId;
            ProcessDefinitionTenantIdSet = true;
            return this;
        }

        protected internal virtual void ValidateParameters()
        {
            EnsureUtil.EnsureOnlyOneNotNull(
                "Need to specify either a job definition id, a process definition id or a process definition key.",
                JobDefinitionId, ProcessDefinitionId, ProcessDefinitionKey);

            if (ProcessDefinitionTenantIdSet &
                (!ReferenceEquals(JobDefinitionId, null) || !ReferenceEquals(ProcessDefinitionId, null)))
                throw Log.ExceptionUpdateSuspensionStateForTenantOnlyByProcessDefinitionKey();

            EnsureUtil.EnsureNotNull("commandExecutor", CommandExecutor);
        }

        public IUpdateJobDefinitionSuspensionStateBuilder ExecutionDate(DateTime? executionDate)
        {
            this.executionDate = executionDate;
            return this;
        }

        public DateTime? GetExecutionDate()
        {
            return this.executionDate;
        }
    }
}
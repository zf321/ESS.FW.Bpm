using System;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.Cmd;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;

namespace ESS.FW.Bpm.Engine.Management.Impl
{

    public class UpdateJobSuspensionStateBuilderImpl : IUpdateJobSuspensionStateBuilder,
        IUpdateJobSuspensionStateSelectBuilder, IUpdateJobSuspensionStateTenantBuilder
    {
        private static readonly CommandLogger Log = ProcessEngineLogger.CmdLogger;

        protected internal readonly ICommandExecutor CommandExecutor;
        

        public UpdateJobSuspensionStateBuilderImpl(ICommandExecutor commandExecutor)
        {
            this.CommandExecutor = commandExecutor;
        }

        /// <summary>
        ///     Creates a builder without CommandExecutor which can not be used to update
        ///     the suspension state via <seealso cref="#activate()" /> or <seealso cref="#suspend()" />. Can be
        ///     used in combination with your own command.
        /// </summary>
        public UpdateJobSuspensionStateBuilderImpl() : this(null)
        {
        }

        public virtual string ProcessDefinitionKey { get; protected internal set; }

        public virtual string ProcessDefinitionId { get; protected internal set; }

        public virtual string processDefinitionTenantId { get; protected internal set; }

        public virtual bool ProcessDefinitionTenantIdSet { get; protected internal set; }

        public virtual string JobId { get; protected internal set; }

        public virtual string JobDefinitionId { get; protected internal set; }

        public virtual string ProcessInstanceId { get; protected internal set; }

        public virtual void Activate()
        {
            ValidateParameters();

            var command = new ActivateJobCmd(this);
            CommandExecutor.Execute(command);
        }

        public virtual void Suspend()
        {
            ValidateParameters();

            var command = new SuspendJobCmd(this);
            CommandExecutor.Execute(command);
        }
        
        

        public virtual IUpdateJobSuspensionStateBuilder ByJobId(string jobId)
        {
            EnsureUtil.EnsureNotNull("jobId", jobId);
            this.JobId = jobId;
            return this;
        }

        public virtual IUpdateJobSuspensionStateBuilder ByJobDefinitionId(string jobDefinitionId)
        {
            EnsureUtil.EnsureNotNull("jobDefinitionId", jobDefinitionId);
            this.JobDefinitionId = jobDefinitionId;
            return this;
        }

        public virtual IUpdateJobSuspensionStateBuilder ByProcessInstanceId(string processInstanceId)
        {
            EnsureUtil.EnsureNotNull("processInstanceId", processInstanceId);
            this.ProcessInstanceId = processInstanceId;
            return this;
        }

        public virtual IUpdateJobSuspensionStateBuilder ByProcessDefinitionId(string processDefinitionId)
        {
            EnsureUtil.EnsureNotNull("processDefinitionId", processDefinitionId);
            this.ProcessDefinitionId = processDefinitionId;
            return this;
        }

        public virtual IUpdateJobSuspensionStateTenantBuilder ByProcessDefinitionKey(string processDefinitionKey)
        {
            EnsureUtil.EnsureNotNull("processDefinitionKey", processDefinitionKey);
            this.ProcessDefinitionKey = processDefinitionKey;
            return this;
        }

        public virtual IUpdateJobSuspensionStateBuilder ProcessDefinitionWithoutTenantId()
        {
            processDefinitionTenantId = null;
            ProcessDefinitionTenantIdSet = true;
            return this;
        }

        public virtual IUpdateJobSuspensionStateBuilder ProcessDefinitionTenantId(string tenantId)
        {
            EnsureUtil.EnsureNotNull("tenantId", tenantId);

            processDefinitionTenantId = tenantId;
            ProcessDefinitionTenantIdSet = true;
            return this;
        }

        protected internal virtual void ValidateParameters()
        {
            //ensureOnlyOneNotNull(
            //    "Need to specify either a job id, a job definition id, a process instance id, a process definition id or a process definition key.",
            //    jobId, jobDefinitionId, processInstanceId, processDefinitionId, processDefinitionKey);

            if (ProcessDefinitionTenantIdSet &
                (!ReferenceEquals(JobId, null) || !ReferenceEquals(JobDefinitionId, null) ||
                 !ReferenceEquals(ProcessInstanceId, null) || !ReferenceEquals(ProcessDefinitionId, null)))
                throw Log.ExceptionUpdateSuspensionStateForTenantOnlyByProcessDefinitionKey();

            EnsureUtil.EnsureNotNull("commandExecutor", CommandExecutor);
        }
    }
}
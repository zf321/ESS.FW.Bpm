using System;
using ESS.FW.Bpm.Engine.Impl.Cmd;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Runtime;

namespace ESS.FW.Bpm.Engine.Impl.runtime
{

    public class UpdateProcessInstanceSuspensionStateBuilderImpl : IUpdateProcessInstanceSuspensionStateBuilder,
        IUpdateProcessInstanceSuspensionStateSelectBuilder, IUpdateProcessInstanceSuspensionStateTenantBuilder
    {
        //private static readonly CommandLogger LOG = ProcessEngineLogger.CMD_LOGGER;

        protected internal readonly ICommandExecutor CommandExecutor;
        

        public UpdateProcessInstanceSuspensionStateBuilderImpl(ICommandExecutor commandExecutor)
        {
            this.CommandExecutor = commandExecutor;
        }

        /// <summary>
        ///     Creates a builder without CommandExecutor which can not be used to update
        ///     the suspension state via <seealso cref="#activate()" /> or <seealso cref="#suspend()" />. Can be
        ///     used in combination with your own command.
        /// </summary>
        public UpdateProcessInstanceSuspensionStateBuilderImpl() : this(null)
        {
        }

        public virtual string ProcessDefinitionKey { get; protected internal set; }

        public virtual string ProcessDefinitionId { get; protected internal set; }

        public virtual string processDefinitionTenantId { get; protected internal set; }

        public virtual bool ProcessDefinitionTenantIdSet { get; protected internal set; }

        public virtual string ProcessInstanceId { get; protected internal set; }

        public virtual void Activate()
        {
            ValidateParameters();

            var command = new ActivateProcessInstanceCmd(this);
            CommandExecutor.Execute(command);
        }

        public virtual void Suspend()
        {
            ValidateParameters();

            var command = new SuspendProcessInstanceCmd(this);
            CommandExecutor.Execute(command);
        }
        

        public virtual IUpdateProcessInstanceSuspensionStateBuilder ByProcessInstanceId(string processInstanceId)
        {
            EnsureUtil.EnsureNotNull("processInstanceId", processInstanceId);
            this.ProcessInstanceId = processInstanceId;
            return this;
        }

        public virtual IUpdateProcessInstanceSuspensionStateBuilder ByProcessDefinitionId(string processDefinitionId)
        {
            EnsureUtil.EnsureNotNull("processDefinitionId", processDefinitionId);
            this.ProcessDefinitionId = processDefinitionId;
            return this;
        }

        public virtual IUpdateProcessInstanceSuspensionStateTenantBuilder ByProcessDefinitionKey(
            string processDefinitionKey)
        {
            EnsureUtil.EnsureNotNull("processDefinitionKey", processDefinitionKey);
            this.ProcessDefinitionKey = processDefinitionKey;
            return this;
        }

        public virtual IUpdateProcessInstanceSuspensionStateTenantBuilder ProcessDefinitionWithoutTenantId()
        {
            processDefinitionTenantId = null;
            ProcessDefinitionTenantIdSet = true;
            return this;
        }

        public virtual IUpdateProcessInstanceSuspensionStateTenantBuilder ProcessDefinitionTenantId(string tenantId)
        {
            EnsureUtil.EnsureNotNull("tenantId", tenantId);

            processDefinitionTenantId = tenantId;
            ProcessDefinitionTenantIdSet = true;
            return this;
        }

        protected internal virtual void ValidateParameters()
        {
            EnsureUtil.EnsureOnlyOneNotNull(
                "Need to specify either a process instance id, a process definition id or a process definition key.",
                ProcessInstanceId, ProcessDefinitionId, ProcessDefinitionKey);

            if (ProcessDefinitionTenantIdSet &&
                (!ReferenceEquals(ProcessInstanceId, null) || !ReferenceEquals(ProcessDefinitionId, null)))
            {
                //throw LOG.exceptionUpdateSuspensionStateForTenantOnlyByProcessDefinitionKey();
            }

            EnsureUtil.EnsureNotNull("commandExecutor", CommandExecutor);
        }
    }
}
using System;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.Cmd;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;

namespace ESS.FW.Bpm.Engine.Repository.Impl
{
    //JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
    //	import static org.camunda.bpm.engine.impl.Util.EnsureUtil.ensureNotNull;
    //JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
    //	import static org.camunda.bpm.engine.impl.Util.EnsureUtil.ensureOnlyOneNotNull;

    public class UpdateProcessDefinitionSuspensionStateBuilderImpl : IUpdateProcessDefinitionSuspensionStateBuilder,
        IUpdateProcessDefinitionSuspensionStateSelectBuilder, IUpdateProcessDefinitionSuspensionStateTenantBuilder
    {
        private static readonly CommandLogger Log = ProcessEngineLogger.CmdLogger;

        protected internal readonly ICommandExecutor CommandExecutor;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
        protected internal DateTime? ExecutionDateRenamed;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
        protected internal bool IncludeProcessInstancesRenamed;
        protected internal bool IsTenantIdSet;
        protected internal string processDefinitionId;

        protected internal string processDefinitionKey;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
        protected internal string ProcessDefinitionTenantIdRenamed;

        public UpdateProcessDefinitionSuspensionStateBuilderImpl(ICommandExecutor commandExecutor)
        {
            this.CommandExecutor = commandExecutor;
        }

        /// <summary>
        ///     Creates a builder without CommandExecutor which can not be used to update
        ///     the suspension state via <seealso cref="#activate()" /> or <seealso cref="#suspend()" />. Can be
        ///     used in combination with your own command.
        /// </summary>
        public UpdateProcessDefinitionSuspensionStateBuilderImpl() : this(null)
        {
        }

        public virtual string ProcessDefinitionKey
        {
            get { return processDefinitionKey; }
        }

        public virtual string ProcessDefinitionId
        {
            get { return processDefinitionId; }
        }

        public virtual bool IncludeProcessInstances
        {
            get { return IncludeProcessInstancesRenamed; }
        }

        public virtual DateTime? ExecutionDate
        {
            get { return ExecutionDateRenamed; }
        }

        public virtual string ProcessDefinitionTenantId
        {
            get { return ProcessDefinitionTenantIdRenamed; }
        }

        public virtual bool TenantIdSet
        {
            get { return IsTenantIdSet; }
        }

        public virtual void Activate()
        {
            ValidateParameters();

            var command = new ActivateProcessDefinitionCmd(this);
            CommandExecutor.Execute(command);
        }

        public virtual void Suspend()
        {
            ValidateParameters();

            var command = new SuspendProcessDefinitionCmd(this);
            CommandExecutor.Execute(command);
        }

        IUpdateProcessDefinitionSuspensionStateBuilder IUpdateProcessDefinitionSuspensionStateBuilder.
            IncludeProcessInstances(bool includeProcessInstances)
        {
            throw new NotImplementedException();
        }

        IUpdateProcessDefinitionSuspensionStateBuilder IUpdateProcessDefinitionSuspensionStateBuilder.ExecutionDate(
            DateTime executionDate)
        {
            throw new NotImplementedException();
        }

        //IUpdateProcessDefinitionSuspensionStateBuilder IUpdateProcessDefinitionSuspensionStateSelectBuilder.
        //    ByProcessDefinitionId(string processDefinitionId)
        //{
        //    throw new NotImplementedException();
        //}

        IUpdateProcessDefinitionSuspensionStateTenantBuilder IUpdateProcessDefinitionSuspensionStateSelectBuilder.
            ByProcessDefinitionKey(string processDefinitionKey)
        {
            throw new NotImplementedException();
        }

        IUpdateProcessDefinitionSuspensionStateBuilder IUpdateProcessDefinitionSuspensionStateTenantBuilder.
            ProcessDefinitionWithoutTenantId()
        {
            throw new NotImplementedException();
        }

        IUpdateProcessDefinitionSuspensionStateBuilder IUpdateProcessDefinitionSuspensionStateTenantBuilder.
            ProcessDefinitionTenantId(string tenantId)
        {
            throw new NotImplementedException();
        }

        public virtual IUpdateProcessDefinitionSuspensionStateBuilder/*UpdateProcessDefinitionSuspensionStateBuilderImpl*/ ByProcessDefinitionId(
            string processDefinitionId)
        {
            EnsureUtil.EnsureNotNull("processDefinitionId", processDefinitionId);
            this.processDefinitionId = processDefinitionId;
            return this;
        }

        public virtual UpdateProcessDefinitionSuspensionStateBuilderImpl byProcessDefinitionKey(
            string processDefinitionKey)
        {
            EnsureUtil.EnsureNotNull("processDefinitionKey", processDefinitionKey);
            this.processDefinitionKey = processDefinitionKey;
            return this;
        }

        public virtual UpdateProcessDefinitionSuspensionStateBuilderImpl includeProcessInstances(
            bool includeProcessInstance)
        {
            IncludeProcessInstancesRenamed = includeProcessInstance;
            return this;
        }

        public virtual UpdateProcessDefinitionSuspensionStateBuilderImpl executionDate(DateTime? date)
        {
            ExecutionDateRenamed = date;
            return this;
        }

        public virtual UpdateProcessDefinitionSuspensionStateBuilderImpl processDefinitionWithoutTenantId()
        {
            ProcessDefinitionTenantIdRenamed = null;
            IsTenantIdSet = true;
            return this;
        }

        public virtual UpdateProcessDefinitionSuspensionStateBuilderImpl processDefinitionTenantId(string tenantId)
        {
            EnsureUtil.EnsureNotNull("tenantId", tenantId);

            ProcessDefinitionTenantIdRenamed = tenantId;
            IsTenantIdSet = true;
            return this;
        }

        protected internal virtual void ValidateParameters()
        {
            //ensureOnlyOneNotNull("Need to specify either a process instance id or a process definition key.",
            //    processDefinitionId, processDefinitionKey);

            if (!ReferenceEquals(processDefinitionId, null) && IsTenantIdSet)
                throw Log.ExceptionUpdateSuspensionStateForTenantOnlyByProcessDefinitionKey();

            EnsureUtil.EnsureNotNull("commandExecutor", CommandExecutor);
        }
    }
}
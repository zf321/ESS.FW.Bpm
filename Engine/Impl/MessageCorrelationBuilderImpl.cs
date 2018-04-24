using System.Collections.Generic;


using ESS.FW.Bpm.Engine.Impl.Cmd;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Variable;


namespace ESS.FW.Bpm.Engine.Impl
{

    /// <summary>
    ///     
    ///     
    /// </summary>
    public class MessageCorrelationBuilderImpl : IMessageCorrelationBuilder
    {
        private static readonly CommandLogger Log = ProcessEngineLogger.CmdLogger;
        protected internal string businessKey;
        protected internal CommandContext commandContext;

        protected internal ICommandExecutor commandExecutor;
        protected internal IVariableMap correlationLocalVariables;

        protected internal IVariableMap correlationProcessInstanceVariables;

        protected internal bool IsExclusiveCorrelation;
        protected internal bool IsTenantIdSet;

        protected internal string messageName;
        protected internal IVariableMap payloadProcessInstanceVariables;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
        protected internal string processDefinitionId;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
        protected internal string processInstanceId;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
        protected internal string tenantId;

        public MessageCorrelationBuilderImpl(ICommandExecutor commandExecutor, string messageName) : this(messageName)
        {
            EnsureUtil.EnsureNotNull("commandExecutor", commandExecutor);
            this.commandExecutor = commandExecutor;
        }

        public MessageCorrelationBuilderImpl(CommandContext commandContext, string messageName) : this(messageName)
        {
            EnsureUtil.EnsureNotNull("commandContext", commandContext);
            this.commandContext = commandContext;
        }

        private MessageCorrelationBuilderImpl(string messageName)
        {
            this.messageName = messageName;
        }

        // getters //////////////////////////////////

        public virtual ICommandExecutor CommandExecutor
        {
            get { return commandExecutor; }
        }

        public virtual CommandContext CommandContext
        {
            get { return commandContext; }
        }

        public virtual string MessageName
        {
            get { return messageName; }
        }

        public virtual string BusinessKey
        {
            get { return businessKey; }
        }

        public virtual IDictionary<string, object> CorrelationProcessInstanceVariables
        {
            get { return correlationProcessInstanceVariables; }
        }

        public virtual IDictionary<string, object> CorrelationLocalVariables
        {
            get { return correlationLocalVariables; }
        }

        public virtual IDictionary<string, object> PayloadProcessInstanceVariables
        {
            get { return payloadProcessInstanceVariables; }
        }

        public virtual bool ExclusiveCorrelation
        {
            get { return IsExclusiveCorrelation; }
        }

        public virtual string TenantId
        {
            get { return tenantId; }
        }

        public virtual bool TenantIdSet
        {
            get { return IsTenantIdSet; }
        }

        public virtual IMessageCorrelationBuilder ProcessInstanceBusinessKey(string businessKey)
        {
            EnsureUtil.EnsureNotNull("businessKey", businessKey);
            this.businessKey = businessKey;
            return this;
        }

        public virtual IMessageCorrelationBuilder ProcessInstanceVariableEquals(string variableName, object variableValue)
        {
            EnsureUtil.EnsureNotNull("variableName", variableName);
            EnsureCorrelationProcessInstanceVariablesInitialized();

            correlationProcessInstanceVariables.PutValue(variableName, variableValue);
            return this;
        }

        public virtual IMessageCorrelationBuilder ProcessInstanceVariablesEqual(IDictionary<string, object> variables)
        {
            EnsureUtil.EnsureNotNull("variables", variables);
            EnsureCorrelationProcessInstanceVariablesInitialized();

            foreach (var it in variables)
                correlationProcessInstanceVariables.PutValue(it.Key, it.Value);
            return this;
        }

        public virtual IMessageCorrelationBuilder LocalVariableEquals(string variableName, object variableValue)
        {
            EnsureUtil.EnsureNotNull("variableName", variableName);
            EnsureCorrelationLocalVariablesInitialized();

            correlationLocalVariables.PutValue(variableName, variableValue);
            return this;
        }

        public virtual IMessageCorrelationBuilder LocalVariablesEqual(IDictionary<string, object> variables)
        {
            EnsureUtil.EnsureNotNull("variables", variables);
            EnsureCorrelationLocalVariablesInitialized();
            foreach (var it in variables)
                correlationLocalVariables.PutValue(it.Key, it.Value);
            return this;
        }

        public virtual IMessageCorrelationBuilder ProcessInstanceId(string id)
        {
            EnsureUtil.EnsureNotNull("processInstanceId", id);
            processInstanceId = id;
            return this;
        }

        public virtual IMessageCorrelationBuilder ProcessDefinitionId(string processDefinitionId)
        {
            EnsureUtil.EnsureNotNull("processDefinitionId", processDefinitionId);
            processDefinitionId = processDefinitionId;
            return this;
        }

        public virtual IMessageCorrelationBuilder SetVariable(string variableName, object variableValue)
        {
            EnsureUtil.EnsureNotNull("variableName", variableName);
            EnsurePayloadProcessInstanceVariablesInitialized();
            payloadProcessInstanceVariables.PutValue(variableName, variableValue);
            return this;
        }

        public virtual IMessageCorrelationBuilder SetVariables(IDictionary<string, object> variables)
        {
            if (variables != null)
            {
                EnsurePayloadProcessInstanceVariablesInitialized();
                foreach (var it in variables)
                    payloadProcessInstanceVariables.PutValue(it.Key, it.Value);
            }
            return this;
        }

        public virtual IMessageCorrelationBuilder SetTenantId(string tenantId)
        {
            EnsureUtil.EnsureNotNull(
                "The tenant-id cannot be null. Use 'withoutTenantId()' if you want to correlate the message to a process definition or an execution which has no tenant-id.",
                "tenantId", tenantId);

            IsTenantIdSet = true;
            this.tenantId = tenantId;
            return this;
        }

        public virtual IMessageCorrelationBuilder WithoutTenantId()
        {
            IsTenantIdSet = true;
            tenantId = null;
            return this;
        }

        public virtual void Correlate()
        {
            CorrelateWithResult();
        }

        public virtual IMessageCorrelationResult CorrelateWithResult()
        {
            EnsureProcessDefinitionIdNotSet();
            EnsureProcessInstanceAndTenantIdNotSet();

            return Execute(new CorrelateMessageCmd(this));
        }

        public virtual void CorrelateExclusively()
        {
            IsExclusiveCorrelation = true;

            Correlate();
        }

        public virtual void CorrelateAll()
        {
            CorrelateAllWithResult();
        }

        public virtual IList<IMessageCorrelationResult> CorrelateAllWithResult()
        {
            EnsureProcessDefinitionIdNotSet();
            EnsureProcessInstanceAndTenantIdNotSet();

            return Execute(new CorrelateAllMessageCmd(this));
        }

        public virtual IProcessInstance CorrelateStartMessage()
        {
            EnsureCorrelationVariablesNotSet();
            EnsureProcessDefinitionAndTenantIdNotSet();

            return Execute(new CorrelateStartMessageCmd(this));
        }

        protected internal virtual void EnsureCorrelationProcessInstanceVariablesInitialized()
        {
            if (correlationProcessInstanceVariables == null)
                correlationProcessInstanceVariables = new VariableMapImpl();
        }

        protected internal virtual void EnsureCorrelationLocalVariablesInitialized()
        {
            if (correlationLocalVariables == null)
                correlationLocalVariables = new VariableMapImpl();
        }

        protected internal virtual void EnsurePayloadProcessInstanceVariablesInitialized()
        {
            if (payloadProcessInstanceVariables == null)
                payloadProcessInstanceVariables = new VariableMapImpl();
        }

        protected internal virtual void EnsureProcessDefinitionIdNotSet()
        {
            if (!ReferenceEquals(processDefinitionId, null))
                throw Log.ExceptionCorrelateMessageWithProcessDefinitionId();
        }

        protected internal virtual void EnsureProcessInstanceAndTenantIdNotSet()
        {
            if (!ReferenceEquals(processInstanceId, null) && IsTenantIdSet)
                throw Log.ExceptionCorrelateMessageWithProcessInstanceAndTenantId();
        }

        protected internal virtual void EnsureCorrelationVariablesNotSet()
        {
            if ((correlationProcessInstanceVariables != null) || (correlationLocalVariables != null))
                throw Log.ExceptionCorrelateStartMessageWithCorrelationVariables();
        }

        protected internal virtual void EnsureProcessDefinitionAndTenantIdNotSet()
        {
            if (!ReferenceEquals(processDefinitionId, null) && IsTenantIdSet)
                throw Log.ExceptionCorrelateMessageWithProcessDefinitionAndTenantId();
        }

        protected internal virtual T Execute<T>(ICommand<T> command)
        {
            if (commandExecutor != null)
                return commandExecutor.Execute(command);
            return command.Execute(commandContext);
        }
    }
}
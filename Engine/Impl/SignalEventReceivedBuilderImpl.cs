using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Cmd;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Variable;


namespace ESS.FW.Bpm.Engine.Impl
{

    public class SignalEventReceivedBuilderImpl : ISignalEventReceivedBuilder
    {
        private static readonly CommandLogger Log = ProcessEngineLogger.CmdLogger;

        protected internal readonly ICommandExecutor CommandExecutor;

        protected internal IVariableMap Variables;

        public SignalEventReceivedBuilderImpl(ICommandExecutor commandExecutor, string signalName)
        {
            this.CommandExecutor = commandExecutor;
            this.SignalName = signalName;
        }

        public virtual string SignalName { get; }

        public virtual string ExecutionId { get; protected internal set; }

        public virtual string TenantId { get; protected internal set; }

        public virtual bool IsTenantIdSet { get; protected internal set; }

        public virtual ISignalEventReceivedBuilder SetVariables(IDictionary<string, object> variables)
        {
            if (variables != null)
            {
                if (this.Variables == null)
                    this.Variables = (IVariableMap)new VariableMapImpl();
                foreach (var it in variables)
                    this.Variables.PutValue(it.Key, it.Value);
                //this.variables variables);
            }
            return this;
        }

        public virtual ISignalEventReceivedBuilder SetExecutionId(string executionId)
        {
            EnsureUtil.EnsureNotNull("executionId", executionId);
            ExecutionId = executionId;
            return this;
        }

        public virtual ISignalEventReceivedBuilder SetTenantId(string tenantId)
        {
            EnsureUtil.EnsureNotNull(
                "The tenant-id cannot be null. Use 'withoutTenantId()' if you want to send the signal to a process definition or an execution which has no tenant-id.",
                "tenantId", tenantId);

            TenantId = tenantId;
            IsTenantIdSet = true;

            return this;
        }

        public virtual ISignalEventReceivedBuilder WithoutTenantId()
        {
            // tenant-id is null
            IsTenantIdSet = true;
            return this;
        }

        public virtual void Send()
        {
            if (ExecutionId != null && IsTenantIdSet)
                throw Log.ExceptionDeliverSignalToSingleExecutionWithTenantId();

            var command = new SignalEventReceivedCmd(this);
            CommandExecutor.Execute(command);
        }

        public virtual IVariableMap GetVariables()
        {
            return Variables;
        }
    }
}
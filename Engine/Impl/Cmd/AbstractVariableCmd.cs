using System;
using ESS.FW.Bpm.Engine.Impl.Core.Variable.Scope;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Pvm.Runtime;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{

    /// <summary>
    ///     .
    /// </summary>
    [Serializable]
    public abstract class AbstractVariableCmd : ICommand<object>
    {
        private const long SerialVersionUid = 1L;

        protected internal CommandContext CommandContext;
        protected internal string EntityId;
        protected internal bool IsLocal;
        protected internal bool PreventLogUserOperation;

        public AbstractVariableCmd(string entityId, bool isLocal)
        {
            this.EntityId = entityId;
            this.IsLocal = isLocal;
        }

        protected internal abstract AbstractVariableScope Entity { get; }

        protected internal abstract ExecutionEntity ContextExecution { get; }

        protected internal abstract string LogEntryOperation { get; }

        public virtual object Execute(CommandContext commandContext)
        {
            this.CommandContext = commandContext;

            var scope = Entity;

            ExecuteOperation(scope);

            var contextExecution = ContextExecution;
            if (contextExecution != null)
            {
                contextExecution.DispatchDelayedEventsAndPerformOperation((e)=> {});
            }

            if (!PreventLogUserOperation)
                LogVariableOperation(scope);

            return null;
        }

        public virtual AbstractVariableCmd DisableLogUserOperation()
        {
            PreventLogUserOperation = true;
            return this;
        }

        protected internal abstract void LogVariableOperation(AbstractVariableScope scope);

        protected internal abstract void ExecuteOperation(AbstractVariableScope scope);
    }
}
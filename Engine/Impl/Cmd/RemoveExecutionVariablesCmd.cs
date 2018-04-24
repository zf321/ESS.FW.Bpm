using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Core.Variable.Scope;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    

    /// <summary>
    ///     
    ///     
    /// </summary>
    [Serializable]
    public class RemoveExecutionVariablesCmd : AbstractRemoveVariableCmd
    {
        private const long SerialVersionUid = 1L;

        public RemoveExecutionVariablesCmd(string executionId, ICollection<string> variableNames, bool isLocal)
            : base(executionId, variableNames, isLocal)
        {
        }

        protected internal override ExecutionEntity ContextExecution
        {
            get
            {
                return (ExecutionEntity)Entity;
            }
        }

        protected internal override AbstractVariableScope Entity
        {
            get
            {
                EnsureUtil.EnsureNotNull("executionId", EntityId);

                ExecutionEntity execution = CommandContext.ExecutionManager.FindExecutionById(EntityId);

                EnsureUtil.EnsureNotNull("execution " + EntityId + " doesn't exist", "execution", execution);

                CheckRemoveExecutionVariables(execution);

                return (AbstractVariableScope)execution;
            }
        }

        protected internal override void LogVariableOperation(AbstractVariableScope scope)
        {
            ExecutionEntity execution = (ExecutionEntity)scope;
            CommandContext.OperationLogManager.LogVariableOperation(LogEntryOperation, execution.Id, null, PropertyChange.EmptyChange);
        }

        protected internal virtual void CheckRemoveExecutionVariables(ExecutionEntity execution)
        {
            foreach (var checker in CommandContext.ProcessEngineConfiguration.CommandCheckers)
                checker.CheckUpdateProcessInstance(execution);
        }
    }
}
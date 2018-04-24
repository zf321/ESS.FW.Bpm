using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Core.Variable.Scope;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Impl.Util;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///      
    ///     
    /// </summary>
    [Serializable]
    public class SetExecutionVariablesCmd : AbstractSetVariableCmd
    {
        private const long SerialVersionUid = 1L;

        public SetExecutionVariablesCmd(string executionId, IDictionary<string, object> variables, bool isLocal)
            : base(executionId, variables, isLocal)
        {
        }

        protected ExecutionEntity GetEntity()
        {
            EnsureUtil.EnsureNotNull("executionId", EntityId);

            ExecutionEntity execution = CommandContext.ExecutionManager.FindExecutionById(EntityId);

            EnsureUtil.EnsureNotNull("execution " + EntityId + " doesn't exist", "execution", execution);

            CheckSetExecutionVariables(execution);

            return execution;
        }
        protected internal override AbstractVariableScope Entity
        {
            get
            {
                EnsureUtil.EnsureNotNull("executionId", EntityId);

                ExecutionEntity execution = CommandContext.ExecutionManager.FindExecutionById(EntityId);

                EnsureUtil.EnsureNotNull("execution " + EntityId + " doesn't exist", "execution", execution);

                CheckSetExecutionVariables(execution);

                return execution;
            }
        }

        protected internal override ExecutionEntity ContextExecution
        {
            get
            {
                return GetEntity();
            }
        }

        protected internal override void LogVariableOperation(AbstractVariableScope scope)
        {
            ExecutionEntity execution = (ExecutionEntity)scope;
            CommandContext.OperationLogManager.LogVariableOperation(LogEntryOperation, execution.Id, null,
                PropertyChange.EmptyChange);
        }

        protected internal virtual void CheckSetExecutionVariables(ExecutionEntity execution)
        {
            foreach (var checker in CommandContext.ProcessEngineConfiguration.CommandCheckers)
                checker.CheckUpdateProcessInstance(execution);
        }
    }
}
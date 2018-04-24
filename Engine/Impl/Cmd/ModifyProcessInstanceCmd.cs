using System.Collections.Generic;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Persistence.Entity.Impl;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    /// </summary>
    public class ModifyProcessInstanceCmd : ICommand<object>
    {
        private static readonly CommandLogger Log = ProcessEngineLogger.CmdLogger;

        protected internal ProcessInstanceModificationBuilderImpl Builder;

        public ModifyProcessInstanceCmd(ProcessInstanceModificationBuilderImpl processInstanceModificationBuilder)
        {
            Builder = processInstanceModificationBuilder;
        }

        protected internal virtual string LogEntryOperation
        {
            get { return UserOperationLogEntryFields.OperationTypeModifyProcessInstance; }
        }

        public virtual object Execute(CommandContext commandContext)
        {
            var processInstanceId = Builder.ProcessInstanceId;

            IExecutionManager executionManager = commandContext.ExecutionManager;
            ExecutionEntity processInstance = executionManager.FindExecutionById(processInstanceId);


            CheckUpdateProcessInstance(processInstance, commandContext);

            processInstance.PreserveScope = true;

            var instructions = Builder.ModificationOperations;

            for (var i = 0; i < instructions.Count; i++)
            {
                var instruction = instructions[i];
                Log.DebugModificationInstruction(processInstanceId, i + 1, instruction.Describe());

                instruction.SkipCustomListeners = Builder.SkipCustomListeners;
                instruction.SkipIoMappings = Builder.SkipIoMappings;
                instruction.Execute(commandContext);
            }

            processInstance = executionManager.FindExecutionById(processInstanceId);

            if (!processInstance.HasChildren())
            {
                if (processInstance.Activity == null)
                {
                    // process instance was cancelled
                    CheckDeleteProcessInstance(processInstance, commandContext);
                    DeletePropagate(processInstance,"Cancellation due to process instance modification",
                        Builder.SkipCustomListeners, Builder.SkipIoMappings);
                }
                else if (processInstance.IsEnded)
                {
                    // process instance has ended regularly
                    processInstance.PropagateEnd();
                }
            }

            commandContext.OperationLogManager.LogProcessInstanceOperation(LogEntryOperation, processInstanceId, null,
                null,new List<PropertyChange>() { PropertyChange.EmptyChange});
            return null;
        }

        protected internal virtual void CheckUpdateProcessInstance(ExecutionEntity execution,
            CommandContext commandContext)
        {
            foreach (var checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
                checker.CheckUpdateProcessInstance(execution);
        }

        protected internal virtual void CheckDeleteProcessInstance(ExecutionEntity execution,
            CommandContext commandContext)
        {
            foreach (var checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
                checker.CheckDeleteProcessInstance(execution);
        }
        protected internal virtual void DeletePropagate(ExecutionEntity processInstance, string deleteReason, bool skipCustomListeners, bool skipIoMappings)
        {
            ExecutionEntity topmostDeletableExecution = processInstance;
            ExecutionEntity parentScopeExecution = (ExecutionEntity)topmostDeletableExecution.GetParentScopeExecution(true);

            while (parentScopeExecution != null && (parentScopeExecution.NonEventScopeExecutions.Count <= 1))
            {
                topmostDeletableExecution = parentScopeExecution;
                parentScopeExecution = (ExecutionEntity)topmostDeletableExecution.GetParentScopeExecution(true);
            }

            topmostDeletableExecution.DeleteCascade(deleteReason, skipCustomListeners, skipIoMappings, false);
        }

    }
}
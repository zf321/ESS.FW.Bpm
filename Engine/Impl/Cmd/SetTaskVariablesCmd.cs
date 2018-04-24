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
    public class SetTaskVariablesCmd : AbstractSetVariableCmd
    {
        private const long SerialVersionUid = 1L;

        public SetTaskVariablesCmd(string taskId, IDictionary<string, object> variables, bool isLocal)
            : base(taskId, variables, isLocal)
        {
        }

        protected TaskEntity GetEntity()
        {
            EnsureUtil.EnsureNotNull("taskId", EntityId);

            TaskEntity task = CommandContext.TaskManager.FindTaskById(EntityId);

            EnsureUtil.EnsureNotNull("ITask " + EntityId + " doesn't exist", "ITask", task);

            CheckSetTaskVariables(task);

            return task;
        }
        protected internal override AbstractVariableScope Entity
        {
            get
            {
                EnsureUtil.EnsureNotNull("taskId", EntityId);

                TaskEntity task = CommandContext.TaskManager.FindTaskById(EntityId);

                EnsureUtil.EnsureNotNull("ITask " + EntityId + " doesn't exist", "ITask", task);

                CheckSetTaskVariables(task);

                return task;
            }
        }

        protected internal override ExecutionEntity ContextExecution
        {
            get
            {
                return GetEntity().GetExecution();
            }
        }

        protected internal override void LogVariableOperation(AbstractVariableScope scope)
        {
            TaskEntity task = (TaskEntity)scope;
            CommandContext.OperationLogManager.LogVariableOperation(LogEntryOperation, null, task.Id,
                PropertyChange.EmptyChange);
        }

        protected internal virtual void CheckSetTaskVariables(TaskEntity task)
        {
            foreach (var checker in CommandContext.ProcessEngineConfiguration.CommandCheckers)
                checker.CheckUpdateTask(task);
        }
    }
}
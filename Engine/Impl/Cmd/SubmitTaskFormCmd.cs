using System;
using System.Collections.Generic;

using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.task;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Task;
using ESS.FW.Bpm.Engine.Variable;
using ESS.FW.Bpm.Engine.Variable.Value;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{

    /// <summary>
    ///      
    ///     
    /// </summary>
    [Serializable]
    public class SubmitTaskFormCmd : ICommand<object>
    {
        private const long SerialVersionUid = 1L;
        protected internal IVariableMap Properties;

        protected internal string TaskId;

        public SubmitTaskFormCmd(string taskId, IDictionary<string, ITypedValue> properties)
        {
            this.TaskId = taskId;
            this.Properties = Variables.FromMap(properties);
        }

        public virtual object Execute(CommandContext commandContext)
        {
            EnsureUtil.EnsureNotNull("taskId", TaskId);
            ITaskManager taskManager = commandContext.TaskManager;
            TaskEntity task = taskManager.FindTaskById(TaskId);
            EnsureUtil.EnsureNotNull("Cannot find ITask with id " + TaskId, "ITask", task);

            foreach (var checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
            {
                checker.CheckTaskWork(task);
            }

            TaskDefinition taskDefinition = task.TaskDefinition;
            if (taskDefinition != null)
            {
                var taskFormHandler = taskDefinition.TaskFormHandler;
                taskFormHandler.SubmitFormVariables(Properties, task);
            }
            else
            {
                // set variables on standalone ITask
                task.Variables = Properties;
            }

            // complete or resolve the ITask
            if (DelegationState.Pending.Equals(task.DelegationState))
            {
                task.Resolve();
                task.CreateHistoricTaskDetails(UserOperationLogEntryFields.OperationTypeResolve);
            }
            else
            {
                task.Complete();
                task.CreateHistoricTaskDetails(UserOperationLogEntryFields.OperationTypeComplete);
            }

            return null;
        }
    }
}
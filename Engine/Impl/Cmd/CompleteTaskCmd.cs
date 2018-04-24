using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{

    /// <summary>
    ///     
    /// </summary>
    [Serializable]
    public class CompleteTaskCmd : ICommand<object>
    {
        protected internal string TaskId;
        protected internal IDictionary<string, object> Variables;

        public CompleteTaskCmd(string taskId, IDictionary<string, object> variables)
        {
            this.TaskId = taskId;
            this.Variables = variables;
        }

        public virtual object Execute(CommandContext commandContext)
        {
            EnsureUtil.EnsureNotNull("taskId", TaskId);

            ITaskManager taskManager = commandContext.TaskManager;
            TaskEntity task = taskManager.FindTaskById(TaskId);
            EnsureUtil.EnsureNotNull("Cannot find ITask with id " + TaskId, "ITask", task);
            //TODO auth
            CheckCompleteTask(task, commandContext);

            if (Variables != null)
            {
                task.ExecutionVariables = Variables;
            }
           
            CompleteTask(task);
            return null;
        }

        protected internal virtual void CompleteTask(TaskEntity task)
        {
            task.Complete();
        }

        protected internal virtual void CheckCompleteTask(TaskEntity task, CommandContext commandContext)
        {
            foreach (var checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
                checker.CheckTaskWork(task);
        }
    }
}
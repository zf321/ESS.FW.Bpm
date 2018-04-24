using System;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    

    /// <summary>
    ///     
    /// </summary>
    [Serializable]
    public class SetTaskPriorityCmd : ICommand<object>
    {
        private const long SerialVersionUid = 1L;

        protected internal int Priority;
        protected internal string TaskId;

        public SetTaskPriorityCmd(string taskId, int priority)
        {
            this.TaskId = taskId;
            this.Priority = priority;
        }

        public virtual object Execute(CommandContext commandContext)
        {
            EnsureUtil.EnsureNotNull("taskId", TaskId);

            ITaskManager taskManager = commandContext.TaskManager;
            TaskEntity task = taskManager.FindTaskById(TaskId);
            EnsureUtil.EnsureNotNull("Cannot find ITask with id " + TaskId, "ITask", task);

            CheckTaskPriority(task, commandContext);

            task.Priority = Priority;

            task.CreateHistoricTaskDetails(UserOperationLogEntryFields.OperationTypeSetPriority);
            return null;
        }

        protected internal virtual void CheckTaskPriority(TaskEntity task, CommandContext commandContext)
        {
            foreach (var checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
                checker.CheckTaskAssign(task);
        }
    }
}
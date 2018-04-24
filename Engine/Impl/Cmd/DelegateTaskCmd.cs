using System;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    /// </summary>
    [Serializable]
    public class DelegateTaskCmd : ICommand<object>
    {
        private const long SerialVersionUid = 1L;

        protected internal string TaskId;
        protected internal string UserId;

        public DelegateTaskCmd(string taskId, string userId)
        {
            this.TaskId = taskId;
            this.UserId = userId;
        }

        public virtual object Execute(CommandContext commandContext)
        {
            EnsureUtil.EnsureNotNull("taskId", TaskId);

            ITaskManager taskManager = commandContext.TaskManager;
            TaskEntity task = taskManager.FindTaskById(TaskId);
            EnsureUtil.EnsureNotNull("Cannot find ITask with id " + TaskId, "ITask", task);

            CheckDelegateTask(task, commandContext);

            task.Delegate(UserId);//.@delegate(userId);

            task.CreateHistoricTaskDetails(UserOperationLogEntryFields.OperationTypeDelegate);

            return null;
        }

        protected internal virtual void CheckDelegateTask(TaskEntity task, CommandContext commandContext)
        {
            foreach (var checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
                checker.CheckTaskAssign(task);
        }
    }
}
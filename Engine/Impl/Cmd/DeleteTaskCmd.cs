using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///     
    /// </summary>
    [Serializable]
    public class DeleteTaskCmd : ICommand<object>
    {
        private const long SerialVersionUid = 1L;
        protected internal bool Cascade;
        protected internal string DeleteReason;
        protected internal string TaskId;
        protected internal ICollection<string> TaskIds;

        public DeleteTaskCmd(string taskId, string deleteReason, bool cascade)
        {
            this.TaskId = taskId;
            this.Cascade = cascade;
            this.DeleteReason = deleteReason;
        }

        public DeleteTaskCmd(ICollection<string> taskIds, string deleteReason, bool cascade)
        {
            this.TaskIds = taskIds;
            this.Cascade = cascade;
            this.DeleteReason = deleteReason;
        }

        public virtual object Execute(CommandContext commandContext)
        {
            if (!ReferenceEquals(TaskId, null))
                DeleteTask(TaskId, commandContext);
            else if (TaskIds != null)
                foreach (var id in TaskIds)
                    DeleteTask(id, commandContext);
            else
                throw new ProcessEngineException("taskId and taskIds are null");


            return null;
        }

        protected internal virtual void DeleteTask(string taskId, CommandContext commandContext)
        {
            ITaskManager taskManager = commandContext.TaskManager;
            TaskEntity task = taskManager.FindTaskById(taskId);

            if (task != null)
            {
                if (!ReferenceEquals(task.ExecutionId, null))
                {
                    throw new ProcessEngineException("The ITask cannot be deleted because is part of a running process");
                }
                if (!ReferenceEquals(task.CaseExecutionId, null))
                {
                    throw new ProcessEngineException(
                        "The ITask cannot be deleted because is part of a running case instance");
                }

                CheckDeleteTask(task, commandContext);

                string reason = (ReferenceEquals(DeleteReason, null) || DeleteReason.Length == 0)
                    ? TaskEntity.DeleteReasonDeleted
                    : DeleteReason;
                task.Delete(reason, Cascade);
            }
            else if (Cascade)
            {
                context.Impl.Context.CommandContext.HistoricTaskInstanceManager.DeleteHistoricTaskInstanceById(taskId);
            }
        }

        protected internal virtual void CheckDeleteTask(TaskEntity task, CommandContext commandContext)
        {
            foreach (var checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
                checker.CheckDeleteTask(task);
        }
    }
}
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
    public class ClaimTaskCmd : ICommand<object>
    {
        private const long SerialVersionUid = 1L;

        protected internal string TaskId;
        protected internal string UserId;

        public ClaimTaskCmd(string taskId, string userId)
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

            CheckClaimTask(task, commandContext);

            if (!ReferenceEquals(UserId, null))
            {
                if (!ReferenceEquals(task.Assignee, null))
                {
                    if (!task.Assignee.Equals(UserId))
                    {
                        // When the ITask is already claimed by another user, throw exception. Otherwise, ignore
                        // this, post-conditions of method already met.
                        throw new TaskAlreadyClaimedException(task.Id, task.Assignee);
                    }
                }
                else
                {
                    task.Assignee = UserId;
                }
            }
            else
            {
                // ITask should be assigned to no one
                task.Assignee = null;
            }

            task.CreateHistoricTaskDetails(UserOperationLogEntryFields.OperationTypeClaim);

            return null;
        }

        protected internal virtual void CheckClaimTask(TaskEntity task, CommandContext commandContext)
        {
            foreach (var checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
                checker.CheckTaskWork(task);
        }
    }
}
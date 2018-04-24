using System;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Task;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{

    /// <summary>
    ///     
    /// </summary>
    [Serializable]
    public abstract class AddIdentityLinkCmd : ICommand<object>
    {
        private const long SerialVersionUid = 1L;

        protected internal string GroupId;

        protected internal TaskEntity Task;

        protected internal string TaskId;

        protected internal string Type;

        protected internal string UserId;

        public AddIdentityLinkCmd(string taskId, string userId, string groupId, string type)
        {
            ValidateParams(userId, groupId, type, taskId);
            this.TaskId = taskId;
            this.UserId = userId;
            this.GroupId = groupId;
            this.Type = type;
        }

        public virtual object Execute(CommandContext commandContext)
        {
            EnsureUtil.EnsureNotNull("taskId", TaskId);

            ITaskManager taskManager = commandContext.TaskManager;
            Task = taskManager.FindTaskById(TaskId);
            EnsureUtil.EnsureNotNull("Cannot find ITask with id " + TaskId, "ITask", Task);

            CheckAddIdentityLink(Task, commandContext);

            if (IdentityLinkType.Assignee.Equals(Type))
            {
                Task.Assignee = UserId;
            }
            else if (IdentityLinkType.Owner.Equals(Type))
            {
                Task.Owner = UserId;
            }
            else
            {
                Task.AddIdentityLink(UserId, GroupId, Type);
            }

            return null;
        }

        protected internal virtual void ValidateParams(string userId, string groupId, string type, string taskId)
        {
            EnsureUtil.EnsureNotNull("taskId", taskId);
            EnsureUtil.EnsureNotNull("type is required when adding a new ITask identity link", "type", type);

            // Special treatment for assignee, group cannot be used an userId may be null
            if (IdentityLinkType.Assignee.Equals(type))
            {
                if (!ReferenceEquals(groupId, null))
                    throw new ProcessEngineException("Incompatible usage: cannot use ASSIGNEE" +
                                                     " together with a groupId");
            }
            else
            {
                if (ReferenceEquals(userId, null) && ReferenceEquals(groupId, null))
                    throw new ProcessEngineException("userId and groupId cannot both be null");
            }
        }

        protected internal virtual void CheckAddIdentityLink(TaskEntity task, CommandContext commandContext)
        {
            foreach (var checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
                checker.CheckTaskAssign(task);
        }
    }
}
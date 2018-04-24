using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Persistence.Entity.Impl;
using ESS.FW.Bpm.Engine.Task;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    


    /// <summary>
    ///     
    ///     
    /// </summary>
    [Serializable]
    public class GetIdentityLinksForTaskCmd : ICommand<IList<IIdentityLink>>
    {
        private const long SerialVersionUid = 1L;
        protected internal string TaskId;

        public GetIdentityLinksForTaskCmd(string taskId)
        {
            this.TaskId = taskId;
        }
        
        public virtual IList<IIdentityLink> Execute(CommandContext commandContext)
        {
            EnsureUtil.EnsureNotNull("taskId", TaskId);

            ITaskManager taskManager = commandContext.TaskManager;
            TaskEntity task = taskManager.FindTaskById(TaskId);
            EnsureUtil.EnsureNotNull("Cannot find ITask with id " + TaskId, "ITask", task);

            CheckGetIdentityLink(task, commandContext);

            var identityLinks = (IList<IIdentityLink>)task.IdentityLinks;

            //assignee is not part of identity links in the db.
            // so if there is one, we add it here.
            // @Tom: we discussed this long on skype and you agreed; -)
            // an assigneeis*an identityLink, and so must it be reflected in the API


             //Note: we cant move this code to the TaskEntity(which would be cleaner),
             //since the ITask.delete cascased to all associated identityLinks
             //and of course this leads to exception while trying to delete a non-existing identityLink
            if (!ReferenceEquals(task.Assignee, null))
            {
                var identityLink = new IdentityLinkEntity();
                identityLink.UserId = task.Assignee;
                identityLink.Task = task;
                identityLink.Type = IdentityLinkType.Assignee;
                identityLinks.Add(identityLink);
            }
            if (!ReferenceEquals(task.Owner, null))
            {
                var identityLink = new IdentityLinkEntity();
                identityLink.UserId = task.Owner;
                identityLink.Task = task;
                identityLink.Type = IdentityLinkType.Owner;
                identityLinks.Add(identityLink);
            }
            return (IList<IIdentityLink>)task.IdentityLinks;
        }

        protected internal virtual void CheckGetIdentityLink(TaskEntity task, CommandContext commandContext)
        {
            foreach (var checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
                checker.CheckReadTask(task);
        }
    }
}
using System;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{


    /// <summary>
    ///     
    /// </summary>
    [Serializable]
    public class AddIdentityLinkForProcessDefinitionCmd : ICommand<object>
    {
        private const long SerialVersionUid = 1L;

        protected internal string GroupId;

        protected internal string ProcessDefinitionId;

        protected internal string UserId;

        public AddIdentityLinkForProcessDefinitionCmd(string processDefinitionId, string userId, string groupId)
        {
            ValidateParams(userId, groupId, processDefinitionId);
            this.ProcessDefinitionId = processDefinitionId;
            this.UserId = userId;
            this.GroupId = groupId;
        }

        public virtual object Execute(CommandContext commandContext)
        {
            ProcessDefinitionEntity processDefinition = context.Impl.Context.CommandContext.ProcessDefinitionManager.FindLatestProcessDefinitionById(ProcessDefinitionId);

            EnsureUtil.EnsureNotNull("Cannot find process definition with id " + ProcessDefinitionId, "processDefinition", processDefinition);

            processDefinition.AddIdentityLink(UserId, GroupId);
            return null;
        }

        protected internal virtual void ValidateParams(string userId, string groupId, string processDefinitionId)
        {
            EnsureUtil.EnsureNotNull("processDefinitionId", processDefinitionId);

            if (ReferenceEquals(userId, null) && ReferenceEquals(groupId, null))
                throw new ProcessEngineException("userId and groupId cannot both be null");
        }
    }
}
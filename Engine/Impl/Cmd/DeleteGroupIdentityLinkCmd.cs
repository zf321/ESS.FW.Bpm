using System;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///     
    /// </summary>
    [Serializable]
    public class DeleteGroupIdentityLinkCmd : DeleteIdentityLinkCmd
    {
        private const long SerialVersionUid = 1L;

        public DeleteGroupIdentityLinkCmd(string taskId, string groupId, string type)
            : base(taskId, null, groupId, type)
        {
        }

        public override object Execute(CommandContext commandContext)
        {
            base.Execute(commandContext);

            PropertyChange propertyChange = new PropertyChange(Type, null, GroupId);

            commandContext.OperationLogManager.LogLinkOperation(UserOperationLogEntryFields.OperationTypeDeleteGroupLink, Task, propertyChange);

            return null;
        }
    }
}
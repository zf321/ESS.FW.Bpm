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
    public class AddUserIdentityLinkCmd : AddIdentityLinkCmd
    {
        private const long SerialVersionUid = 1L;

        public AddUserIdentityLinkCmd(string taskId, string userId, string type) : base(taskId, userId, null, type)
        {
        }

        public override object Execute(CommandContext commandContext)
        {
            base.Execute(commandContext);

            PropertyChange propertyChange = new PropertyChange(Type, null, UserId);

            commandContext.OperationLogManager.LogLinkOperation(UserOperationLogEntryFields.OperationTypeAddUserLink, Task, propertyChange);

            return null;
        }
    }
}
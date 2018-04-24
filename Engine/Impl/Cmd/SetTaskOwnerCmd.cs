using System;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Task;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///     
    /// </summary>
    [Serializable]
    public class SetTaskOwnerCmd : AddIdentityLinkCmd
    {
        private const long SerialVersionUid = 1L;

        public SetTaskOwnerCmd(string taskId, string userId) : base(taskId, userId, null, IdentityLinkType.Owner)
        {
        }

        public override object Execute(CommandContext commandContext)
        {
            base.Execute(commandContext);
            Task.CreateHistoricTaskDetails(UserOperationLogEntryFields.OperationTypeSetOwner);
            return null;
        }
    }
}
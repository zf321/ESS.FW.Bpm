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
    public class AssignTaskCmd : AddIdentityLinkCmd
    {
        private const long SerialVersionUid = 1L;

        public AssignTaskCmd(string taskId, string userId) : base(taskId, userId, null, IdentityLinkType.Assignee)
        {
        }

        public override object Execute(CommandContext commandContext)
        {
            base.Execute(commandContext);
            Task.CreateHistoricTaskDetails(UserOperationLogEntryFields.OperationTypeAssign);
            return null;
        }
    }
}
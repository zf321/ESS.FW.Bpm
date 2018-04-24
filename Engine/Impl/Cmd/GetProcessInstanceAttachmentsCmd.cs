using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Task;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///      
    /// </summary>
    [Serializable]
    public class GetProcessInstanceAttachmentsCmd : ICommand<IList<IAttachment>>
    {
        private const long SerialVersionUid = 1L;
        protected internal string ProcessInstanceId;

        public GetProcessInstanceAttachmentsCmd(string taskId)
        {
            ProcessInstanceId = taskId;
        }

        public virtual IList<IAttachment> Execute(CommandContext commandContext)
        {
            return commandContext.AttachmentManager.FindAttachmentsByProcessInstanceId(ProcessInstanceId);
        }
    }
}
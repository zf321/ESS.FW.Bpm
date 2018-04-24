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
    public class GetTaskAttachmentsCmd : ICommand<IList<IAttachment>>
    {
        private const long SerialVersionUid = 1L;
        protected internal string TaskId;

        public GetTaskAttachmentsCmd(string taskId)
        {
            this.TaskId = taskId;
        }

        public virtual IList<IAttachment> Execute(CommandContext commandContext)
        {
            return commandContext.AttachmentManager.FindAttachmentsByTaskId(TaskId);
        }
    }
}
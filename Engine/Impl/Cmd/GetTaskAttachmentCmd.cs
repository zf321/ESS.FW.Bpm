using System;
using System.Net.Mail;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Task;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///     
    /// </summary>
    [Serializable]
    public class GetTaskAttachmentCmd : ICommand<IAttachment>
    {
        private const long SerialVersionUid = 1L;

        protected internal string AttachmentId;
        protected internal string TaskId;

        public GetTaskAttachmentCmd(string taskId, string attachmentId)
        {
            this.AttachmentId = attachmentId;
            this.TaskId = taskId;
        }

        public virtual IAttachment Execute(CommandContext commandContext)
        {
            return commandContext.AttachmentManager.FindAttachmentByTaskIdAndAttachmentId(TaskId, AttachmentId);
        }
    }
}
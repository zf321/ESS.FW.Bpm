using System;
using System.Net.Mail;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Task;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    /// </summary>
    [Serializable]
    public class GetAttachmentCmd : ICommand<IAttachment>
    {
        private const long SerialVersionUid = 1L;
        protected internal string AttachmentId;

        public GetAttachmentCmd(string attachmentId)
        {
            this.AttachmentId = attachmentId;
        }

        public virtual IAttachment Execute(CommandContext commandContext)
        {
            return commandContext.AttachmentManager.Get(AttachmentId);
        }
    }
}
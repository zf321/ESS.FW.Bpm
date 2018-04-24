using System;
using System.Net.Mail;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Task;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///      
    /// </summary>
    [Serializable]
    public class SaveAttachmentCmd : ICommand<object>
    {
        private const long SerialVersionUid = 1L;
        protected internal IAttachment Attachment;

        public SaveAttachmentCmd(IAttachment attachment)
        {
            this.Attachment = attachment;
        }

        public virtual object Execute(CommandContext commandContext)
        {
            AttachmentEntity updateAttachment = commandContext.AttachmentManager.Get(Attachment.Id);

            updateAttachment.Name = Attachment.Name;
            updateAttachment.Description = Attachment.Description;

            return null;
        }
    }
}
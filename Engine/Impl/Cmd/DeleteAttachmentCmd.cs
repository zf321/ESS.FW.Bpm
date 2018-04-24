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
    public class DeleteAttachmentCmd : ICommand<object>
    {
        private const long SerialVersionUid = 1L;
        protected internal string AttachmentId;

        public DeleteAttachmentCmd(string attachmentId)
        {
            this.AttachmentId = attachmentId;
        }

        public virtual object Execute(CommandContext commandContext)
        {
            AttachmentEntity attachment = commandContext.AttachmentManager.Get(AttachmentId);

            commandContext.AttachmentManager.Delete(attachment);

            if (!ReferenceEquals(attachment.ContentId, null))
            {
                commandContext.ByteArrayManager.DeleteByteArrayById(attachment.ContentId);
            }

            if (!ReferenceEquals(attachment.TaskId, null))
            {
                TaskEntity task = commandContext.TaskManager.FindTaskById(attachment.TaskId);

                PropertyChange propertyChange = new PropertyChange("name", null, attachment.Name);

                commandContext.OperationLogManager.LogAttachmentOperation(
                    UserOperationLogEntryFields.OperationTypeDeleteAttachment, task, propertyChange);
            }
            return null;
        }
    }
}
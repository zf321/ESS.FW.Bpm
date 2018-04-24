using System;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{

    /// <summary>
    ///     
    /// </summary>
    [Serializable]
    public class DeleteTaskAttachmentCmd : ICommand<object>
    {
        private const long SerialVersionUid = 1L;

        protected internal string AttachmentId;
        protected internal string TaskId;

        public DeleteTaskAttachmentCmd(string taskId, string attachmentId)
        {
            this.AttachmentId = attachmentId;
            this.TaskId = taskId;
        }

        public virtual object Execute(CommandContext commandContext)
        {
            var attachment =
                (AttachmentEntity)
                    commandContext.AttachmentManager.FindAttachmentByTaskIdAndAttachmentId(TaskId, AttachmentId);

            EnsureUtil.EnsureNotNull(
                "No attachment exist for ITask id '" + TaskId + " and attachmentId '" + AttachmentId + "'.", "attachment",
                attachment);

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
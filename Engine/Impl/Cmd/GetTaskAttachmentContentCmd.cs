using System;
using System.IO;
using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///     
    /// </summary>
    [Serializable]
    public class GetTaskAttachmentContentCmd : ICommand<Stream>
    {
        private const long SerialVersionUid = 1L;

        protected internal string AttachmentId;
        protected internal string TaskId;

        public GetTaskAttachmentContentCmd(string taskId, string attachmentId)
        {
            this.AttachmentId = attachmentId;
            this.TaskId = taskId;
        }

        public virtual Stream Execute(CommandContext commandContext)
        {
            AttachmentEntity attachment = (AttachmentEntity)commandContext.AttachmentManager.FindAttachmentByTaskIdAndAttachmentId(TaskId, AttachmentId);

            if (attachment == null)
            {
                return null;
            }

            string contentId = attachment.ContentId;
            if (string.ReferenceEquals(contentId, null))
            {
                return null;
            }

            ResourceEntity byteArray = commandContext.ByteArrayManager.Get(contentId);

            byte[] bytes = byteArray.Bytes;

            return new System.IO.MemoryStream(bytes );
        }
    }
}
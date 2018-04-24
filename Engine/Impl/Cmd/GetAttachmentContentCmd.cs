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
    public class GetAttachmentContentCmd : ICommand<Stream>
    {
        private const long SerialVersionUid = 1L;
        protected internal string AttachmentId;

        public GetAttachmentContentCmd(string attachmentId)
        {
            this.AttachmentId = attachmentId;
        }

        public virtual Stream Execute(CommandContext commandContext)
        {
            AttachmentEntity attachment = commandContext.AttachmentManager.Get( AttachmentId);

            string contentId = attachment.ContentId;
            if (contentId == null)
            {
                return null;
            }

            ResourceEntity byteArray = commandContext.ByteArrayManager.Get(contentId);
            byte[] bytes = byteArray.Bytes;

            return new System.IO.MemoryStream(bytes);
        }
    }
}
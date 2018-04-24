using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Task;
using ESS.FW.DataAccess;


namespace ESS.FW.Bpm.Engine.Persistence.Entity
{
    public interface IAttachmentManager : IRepository<AttachmentEntity,string>
    {
        void DeleteAttachmentsByTaskId(string taskId);
        IAttachment FindAttachmentByTaskIdAndAttachmentId(string taskId, string attachmentId);
        IList<IAttachment> FindAttachmentsByProcessInstanceId(string processInstanceId);
        IList<IAttachment> FindAttachmentsByTaskId(string taskId);
    }
}
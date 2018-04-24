using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Autofac.Features.AttributeFilters;

using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Task;
using Microsoft.Extensions.Logging;
using ESS.FW.DataAccess;
using ESS.FW.DataAccess.EF;
using ESS.FW.Common.Components;
using System.Linq;

namespace ESS.FW.Bpm.Engine.Persistence.Entity.Impl
{
    /// <summary>
	///  
	/// </summary>
    [Component]
    public class AttachmentManager : AbstractHistoricManagerNet<AttachmentEntity>, IAttachmentManager
    {
        private IByteArrayManager _byteArrayManager;
        public AttachmentManager(DbContext dbContex, ILoggerFactory loggerFactory, IByteArrayManager byteArrayManager, IDGenerator idGenerator) : base(dbContex, loggerFactory, idGenerator)
        {
            _byteArrayManager = byteArrayManager;
        }

        public virtual IList<IAttachment> FindAttachmentsByProcessInstanceId(string processInstanceId)
        {
            CheckHistoryEnabled();
            //return ListExt.ConvertToListT<IAttachment>(DbEntityManager.SelectList("selectAttachmentsByProcessInstanceId", processInstanceId));
            return Find(m => m.ProcessInstanceId == processInstanceId).ToList().Cast<IAttachment>().ToList();
        }
        
        public virtual IList<IAttachment> FindAttachmentsByTaskId(string taskId)
        {
            CheckHistoryEnabled();
            //return ListExt.ConvertToListT<IAttachment>(DbEntityManager.SelectList("selectAttachmentsByTaskId", taskId));
            return Find(m => m.TaskId == taskId).ToList().Cast<IAttachment>().ToList();
        }
        
        public virtual void DeleteAttachmentsByTaskId(string taskId)
        {
            CheckHistoryEnabled();
            //IList<AttachmentEntity> attachments = ListExt.ConvertToListT<AttachmentEntity>(DbEntityManager.SelectList("selectAttachmentsByTaskId", taskId));
            IEnumerable<AttachmentEntity> attachments = Find(m => m.TaskId == taskId);
            foreach (AttachmentEntity attachment in attachments)
            {
                string contentId = attachment.ContentId;
                if (contentId != null)
                {
                    _byteArrayManager.DeleteByteArrayById(contentId);
                }
                Delete(attachment);
            }
        }

        public virtual IAttachment FindAttachmentByTaskIdAndAttachmentId(string taskId, string attachmentId)
        {
            CheckHistoryEnabled();

            //IDictionary<string, string> parameters = new Dictionary<string, string>();
            //parameters["taskId"] = taskId;
            //parameters["id"] = attachmentId;

            //return (AttachmentEntity)DbEntityManager.SelectOne("selectAttachmentByTaskIdAndAttachmentId", parameters);
            return First(m => m.TaskId == taskId && m.Id == attachmentId);
        }

    }


}
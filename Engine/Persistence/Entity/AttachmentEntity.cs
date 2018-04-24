using ESS.FW.Bpm.Engine.Impl.DB;
using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Task;
using ESS.Shared.Entities.Bpm;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{
    /// <summary>
	///  
	/// </summary>
	[Serializable]
    public partial class AttachmentEntity : IAttachment, IDbEntity, IHasDbRevision
    {
        public virtual object GetPersistentState()
        {
            IDictionary<string, object> persistentState = new Dictionary<string, object>();
            persistentState["name"] = Name;
            persistentState["description"] = Description;
            return persistentState;
        }

        public virtual int RevisionNext
        {
            get
            {
                return Revision + 1;
            }
        }

        public virtual string Id { get; set; }

        public int Revision { get; set; }

        public virtual string Name { get; set; }

        public virtual string Description { get; set; }

        public virtual string Type { get; set; }

        public virtual string TaskId { get; set; }

        public virtual string ProcessInstanceId { get; set; }

        public virtual string Url { get; set; }

        public virtual string ContentId { get; set; }

        public virtual ResourceEntity Content { get; set; }

        public virtual string TenantId { get; set; }


        public override string ToString()
        {
            return this.GetType().Name + "[id=" + Id + ", revision=" + Revision + ", name=" + Name + ", description=" + Description + ", type=" + Type + ", taskId=" + TaskId + ", processInstanceId=" + ProcessInstanceId + ", url=" + Url + ", contentId=" + ContentId + ", content=" + Content + ", tenantId=" + TenantId + "]";
        }
    }

}
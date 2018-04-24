using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Identity;
using ESS.FW.Bpm.Engine.Impl.DB;
using ESS.Shared.Entities.Bpm;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{



    /// <summary>
    ///  
    /// </summary>
    [Serializable]
    public class GroupEntity : IGroup, IDbEntity, IHasDbRevision
    {
        public GroupEntity()
        {
        }

        public GroupEntity(string id)
        {
            this.Id = id;
        }

        public virtual object GetPersistentState()
        {
            IDictionary<string, object> persistentState = new Dictionary<string, object>();
            persistentState["name"] = Name;
            persistentState["type"] = Type;
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
        public virtual string Name { get; set; }
        public virtual string Type { get; set; }
        public virtual int Revision { get; set; }
        public virtual ICollection<TenantMembershipEntity> TenantMembers { get; set; }

        public virtual ICollection<UserEntity> Users { get; set; }

        public override string ToString()
        {
            return this.GetType().Name + "[id=" + Id + ", revision=" + Revision + ", name=" + Name + ", type=" + Type + "]";
        }
    }

}
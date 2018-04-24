using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Identity;
using ESS.FW.Bpm.Engine.Impl.DB;
using ESS.Shared.Entities.Bpm;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{


    [Serializable]
    public class TenantEntity : ITenant, IDbEntity, IHasDbRevision
    {
        public TenantEntity()
        {
        }

        public TenantEntity(string id)
        {
            this.Id = id;
        }

        public virtual object GetPersistentState()
        {
            IDictionary<string, object> persistentState = new Dictionary<string, object>();
            persistentState["name"] = Name;
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


        public virtual int Revision { get; set; }

        public virtual ICollection<TenantMembershipEntity> TenantMembers { get; set; }
        public override string ToString()
        {
            return "TenantEntity [id=" + Id + ", name=" + Name + ", revision=" + Revision + "]";
        }

    }

}
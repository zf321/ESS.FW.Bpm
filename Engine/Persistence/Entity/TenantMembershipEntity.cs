using System;
using ESS.Shared.Entities.Bpm;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{


    /// <summary>
    /// A relationship between a tenant and an user or a group.
    /// </summary>
    [Serializable]
    public class TenantMembershipEntity : IDbEntity
    {

        private const long SerialVersionUid = 1L;

        public TenantEntity Tenant { get; set; }
        public UserEntity User { get; set; }

        public GroupEntity Group { get; set; }
        public virtual string Id { get; set; }

        public virtual object GetPersistentState()
        {
            // entity is not updatable
            return typeof(TenantMembershipEntity);
        }




        public virtual UserEntity GetUser()
        {
            return User;
        }

        public virtual void SetUser(UserEntity user)
        {
            this.User = user;
        }

        public virtual GroupEntity GetGroup()
        {
            return Group;
        }

        public virtual void SetGroup(GroupEntity group)
        {
            this.Group = group;
        }

        public virtual string TenantId
        {
            get
            {
                return Tenant.Id;
            }
            set
            {
                Tenant.Id = value;
            }
        }

        public virtual string UserId
        {
            get
            {
                if (User != null)
                {
                    return User.Id;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (User != null)
                {
                    User.Id = value;
                }
            }
        }

        public virtual string GroupId
        {
            get
            {
                if (Group != null)
                {
                    return Group.Id;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (Group != null)
                {
                    Group.Id = value;
                }
            }
        }

        public virtual TenantEntity GetTenant()
        {
            return Tenant;
        }

        public virtual void SetTenant(TenantEntity tenant)
        {
            this.Tenant = tenant;
        }

        public override string ToString()
        {
            return "TenantMembershipEntity [id=" + Id + ", tenant=" + Tenant + ", user=" + User + ", group=" + Group + "]";
        }

    }

}
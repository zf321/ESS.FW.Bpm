


using ESS.FW.Bpm.Engine.Authorization;

namespace ESS.FW.Bpm.Engine.Impl.DB
{
    /// <summary>
    ///     
    /// </summary>
    public class PermissionCheck
    {
        /// <summary>
        ///     the permission to check for
        /// </summary>
        protected internal Permissions permission;

        protected internal int perms;

        /// <summary>
        ///     the type of the resource to check permissions for
        /// </summary>
        protected internal Resources resource;

        protected internal int resourceType;

        public virtual Permissions Permission
        {
            get { return permission; }
            set
            {
                permission = value;
                if (value != null)
                {
                    perms = (int)value;
                }
            }
        }


        public virtual int Perms
        {
            get { return perms; }
        }

        public virtual Resources Resource
        {
            get { return resource; }
            set
            {
                resource = value;

                if (value != null)
                {
                    resourceType = (int)value;
                }
            }
        }


        public virtual int ResourceType
        {
            get { return resourceType; }
        }

        public virtual string ResourceId { get; set; }


        public virtual string ResourceIdQueryParam { get; set; }


        public virtual long? AuthorizationNotFoundReturnValue { get; set; }
    }
}
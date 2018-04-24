using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.DB;
using System.ComponentModel.DataAnnotations.Schema;
using ESS.Shared.Entities.Bpm;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{

    /// <summary>
    /// 
    /// 
    /// </summary>
    [Serializable]
    public  class AuthorizationEntity : IAuthorization, IDbEntity, IHasDbRevision
    {

        public  int AuthTypeGlobal = 0;

        ///
         /// A Grant Authorization ranges over a users or a group and grants a set of permissions.
         /// Grant authorizations are commonly used for adding permissions to a user or group that
         /// the global authorization revokes.
         ///
        public  int AuthTypeGrant = 1;

        /**
         * A Revoke Authorization ranges over a user or a group and revokes a set of permissions.
         * Revoke authorizations are commonly used for revoking permissions to a user or group the
         * the global authorization grants.
         */
        public  int AuthTypeRevoke = 2;

        /** The identifier used for relating to all users or all resourceIds.
         *  Cannot be used for groups.*/
        public static string Any = "*";
        protected internal static readonly EnginePersistenceLogger Log = ProcessEngineLogger.PersistenceLogger;

        protected internal string userId;
        protected internal string groupId;

        public AuthorizationEntity()
        {
        }

        public AuthorizationEntity(int type)
        {
            this.AuthorizationType = type;

            if (AuthorizationType == AuthTypeGlobal)
            {
                this.userId = Any;
            }

            ResetPermissions();
        }

        protected internal virtual void ResetPermissions()
        {
            if (AuthorizationType == AuthTypeGlobal)
            {
                this.Permissions = Authorization.Permissions.None;

            }
            else if (AuthorizationType == AuthTypeGrant)
            {
                this.Permissions = Authorization.Permissions.None;

            }
            else if (AuthorizationType == AuthTypeRevoke)
            {
                this.Permissions = Authorization.Permissions.All;

            }
            else
            {
                throw Log.EngineAuthorizationTypeException(AuthorizationType, AuthTypeGlobal, AuthTypeGrant, AuthTypeRevoke);
            }
        }

        // grant / revoke methods ////////////////////////////

        public virtual void AddPermission(Permissions p)
        {
            Permissions |= p;
        }
        

        public virtual void RemovePermission(Permissions p)
        {
            Permissions &= ~p;
        }

        public virtual bool IsPermissionGranted(Permissions p)
        {
            if (AuthTypeRevoke == AuthorizationType)
            {
                throw Log.PermissionStateException("isPermissionGranted", "REVOKE");
            }
            return (Permissions & p) == p;
        }

        public virtual bool IsPermissionRevoked(Permissions p)
        {
            if (AuthTypeGrant == AuthorizationType)
            {
                throw Log.PermissionStateException("isPermissionRevoked", "GRANT");
            }
            return (Permissions & p) != p;
        }

        public virtual bool EveryPermissionGranted
        {
            get
            {
                if (AuthTypeRevoke == AuthorizationType)
                {
                    throw Log.PermissionStateException("isEveryPermissionGranted", "REVOKE");
                }
                return Permissions ==Engine.Authorization.Permissions.All;
            }
        }

        public virtual bool EveryPermissionRevoked
        {
            get
            {
                if (AuthorizationType == AuthTypeGrant)
                {
                    throw Log.PermissionStateException("isEveryPermissionRevoked", "GRANT");
                }
                return Permissions == 0;
            }
        }

        public virtual Permissions[] GetPermissions(Permissions[] permissions)
        {

            List<Permissions> result = new List<Permissions>();

            foreach (Permissions permission in permissions)
            {
                if ((AuthTypeGlobal == AuthorizationType || AuthTypeGrant == AuthorizationType) && IsPermissionGranted(permission))
                {

                    result.Add(permission);

                }
                else if (AuthTypeRevoke == AuthorizationType && IsPermissionRevoked(permission))
                {

                    result.Add(permission);

                }
            }
            return result.ToArray();
        }

        public virtual void SetPermissions(Permissions[] permissions)
        {
            ResetPermissions();
            foreach (Permissions permission in permissions)
            {
                if (AuthTypeRevoke == AuthorizationType)
                {
                    RemovePermission(permission);

                }
                else
                {
                    AddPermission(permission);

                }
            }
        }

        // getters setters ///////////////////////////////

        public virtual int AuthorizationType { get; set; }
        public virtual Permissions Permissions { get; set; }

        public virtual string GroupId
        {
            get
            {
                return groupId;
            }
            set
            {
                if (value != null && AuthorizationType == AuthTypeGlobal)
                {
                    throw Log.NotUsableGroupIdForGlobalAuthorizationException();
                }
                this.groupId = value;
            }
        }


        public virtual string UserId
        {
            get
            {
                return userId;
            }
            set
            {
                if (value != null && AuthorizationType == AuthTypeGlobal && !Any.Equals(value))
                {
                    throw Log.IllegalValueForUserIdException(value, Any);
                }
                this.userId = value;
            }
        }


        public virtual Resources ResourceType { get; set; }

        public virtual Resources GetResource()
        {
            return ResourceType;
        }

        public virtual void SetResource(Resources resource)
        {
            this.ResourceType = resource;//.GetResourceType();
        }

        public virtual string ResourceId { get; set; }


        public virtual string Id { get; set; }

        public virtual int Revision { get; set; }

        public virtual void SetPermissions(Permissions permissions)
        {
            this.Permissions = permissions;
        }

        public virtual Permissions GetPermissions()
        {
            return Permissions;
        }

        public virtual int RevisionNext
        {
            get
            {
                return Revision + 1;
            }
        }

        public virtual object GetPersistentState()
        {

                Dictionary<string, object> state = new Dictionary<string, object>();
                state["userId"] = userId;
                state["groupId"] = groupId;
                state["resourceType"] = ResourceType;
                state["resourceId"] = ResourceId;
                state["permissions"] = Permissions;

                return state;
        }


        [NotMapped]
        public Resources Resource
        {
            get
            {
                return (Resources)(int)this.Permissions;
            }
            set
            {
                this.Permissions = (Engine.Authorization.Permissions)(int)value;
            }
        }

        public override string ToString()
        {
            return this.GetType().Name + "[id=" + Id + ", revision=" + Revision + ", authorizationType=" + AuthorizationType + ", permissions=" + Permissions + ", userId=" + userId + ", groupId=" + groupId + ", resourceType=" + ResourceType + ", resourceId=" + ResourceId + "]";
        }

       
    }

}
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Persistence.Entity.Impl;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///     <para>Command allowing to perform an authorization check</para>
    ///     
    /// </summary>
    public class AuthorizationCheckCmd : ICommand<bool>
    {
        protected internal IList<string> GroupIds;
        protected internal Permissions Permission;
        protected internal Resources Resource;
        protected internal string ResourceId;

        protected internal string UserId;

        public AuthorizationCheckCmd(string userId, IList<string> groupIds, Permissions permission, Resources resource,
            string resourceId)
        {
            this.UserId = userId;
            this.GroupIds = groupIds;
            this.Permission = permission;
            this.Resource = resource;
            this.ResourceId = resourceId;
        }

        public virtual bool Execute(CommandContext commandContext)
        {
            IAuthorizationManager authorizationManager = commandContext.AuthorizationManager;
            return authorizationManager.IsAuthorized(UserId,GroupIds, Permission, Resource, ResourceId);
        }
    }
}
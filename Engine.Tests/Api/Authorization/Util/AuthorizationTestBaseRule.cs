using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Identity;
using ESS.FW.Bpm.Model.Dmn.instance;
using NUnit.Framework;

namespace Engine.Tests.Api.Authorization.Util
{
    /// <summary>
    /// </summary>
    public class AuthorizationTestBaseRule //: TestWatcher
    {
        protected internal IList<IAuthorization> Authorizations = new List<IAuthorization>();

        protected internal ProcessEngineRule EngineRule;
        protected internal IList<IGroup> Groups = new List<IGroup>();

        protected internal IList<IUser> Users = new List<IUser>();

        public AuthorizationTestBaseRule(ProcessEngineRule engineRule)
        {
            EngineRule = engineRule;
        }

        public virtual void EnableAuthorization(string userId)
        {
            EngineRule.ProcessEngine.ProcessEngineConfiguration.SetAuthorizationEnabled(true);
            if (!ReferenceEquals(userId, null))
                EngineRule.IdentityService.AuthenticatedUserId = userId;
        }

        public virtual void DisableAuthorization()
        {
            EngineRule.ProcessEngine.ProcessEngineConfiguration.SetAuthorizationEnabled(false);
            EngineRule.IdentityService.ClearAuthentication();
        }

        protected internal virtual void Finished(IDescription description)
        {
            EngineRule.IdentityService.ClearAuthentication();

            DeleteManagedAuthorizations();

            //base.Finished(description);            

            Assert.True( Users.Count == 0,"Users have been created but not deleted");
            Assert.True( Groups.Count == 0,"Groups have been created but not deleted");
        }

        public virtual void ManageAuthorization(IAuthorization authorization)
        {
            Authorizations.Add(authorization);
        }

        protected internal virtual IAuthorization CreateAuthorization(int type, Resources resource, string resourceId)
        {
            var authorization = EngineRule.IAuthorizationService.CreateNewAuthorization(type);

            authorization.Resource = resource;
            if (!ReferenceEquals(resourceId, null))
                authorization.ResourceId = resourceId;

            return authorization;
        }

        public virtual void CreateGrantAuthorization(Resources resource, string resourceId, string userId,
            params Permissions[] permissions)
        {
            var authorization = CreateAuthorization(AuthorizationFields.AuthTypeGrant, resource, resourceId);
            authorization.UserId = userId;
            foreach (var permission in permissions)
                authorization.AddPermission(permission);

            EngineRule.IAuthorizationService.SaveAuthorization(authorization);
            ManageAuthorization(authorization);
        }

        protected internal virtual void DeleteManagedAuthorizations()
        {
            foreach (var authorization in Authorizations)
                EngineRule.IAuthorizationService.DeleteAuthorization(authorization.Id);
        }

        public virtual void CreateUserAndGroup(string userId, string groupId)
        {
            var user = EngineRule.IdentityService.NewUser(userId);
            EngineRule.IdentityService.SaveUser(user);
            Users.Add(user);

            var group = EngineRule.IdentityService.NewGroup(groupId);
            EngineRule.IdentityService.SaveGroup(group);
            Groups.Add(group);
        }

        public virtual void DeleteUsersAndGroups()
        {
            foreach (var user in Users)
                EngineRule.IdentityService.DeleteUser(user.Id);
            Users.Clear();

            foreach (var group in Groups)
                EngineRule.IdentityService.DeleteGroup(group.Id);
            Groups.Clear();
        }
    }
}
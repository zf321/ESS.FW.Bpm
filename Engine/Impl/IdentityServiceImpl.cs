using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Threading;
using ESS.FW.Bpm.Engine.Identity;
using ESS.FW.Bpm.Engine.Impl.Cmd;
using ESS.FW.Bpm.Engine.Impl.Identity;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl
{
    /// <summary>
    ///      
    /// </summary>
    public class IdentityServiceImpl : ServiceImpl, IIdentityService
    {
        /// <summary>
        ///     thread local holding the current authentication
        /// </summary>
        private readonly ThreadLocal<Authentication> _currentAuthentication = new ThreadLocal<Authentication>();

        public virtual bool ReadOnly
        {
            get { return CommandExecutor.Execute(new IsIdentityServiceReadOnlyCmd()); }
        }

        public virtual IGroup NewGroup(string groupId)
        {
            return CommandExecutor.Execute(new CreateGroupCmd(groupId));
        }

        public virtual IUser NewUser(string userId)
        {
            return CommandExecutor.Execute(new CreateUserCmd(userId));
        }

        public virtual ITenant NewTenant(string tenantId)
        {
            return CommandExecutor.Execute(new CreateTenantCmd(tenantId));
        }

        public virtual void SaveGroup(IGroup group)
        {
            CommandExecutor.Execute(new SaveGroupCmd((GroupEntity) group));
        }

        public virtual void SaveUser(IUser user)
        {
            CommandExecutor.Execute(new SaveUserCmd(user));
        }

        public virtual void SaveTenant(ITenant tenant)
        {
            CommandExecutor.Execute(new SaveTenantCmd(tenant));
        }

        public virtual IQueryable<IUser> CreateUserQuery(Expression<Func<UserEntity, bool>> expression )
        {
            return CommandExecutor.Execute(new CreateQueryCmd<UserEntity>(expression));
        }

        public virtual IQueryable<IGroup> CreateGroupQuery()
        {
            return CommandExecutor.Execute(new CreateGroupQueryCmd());
        }

        public virtual IQueryable<ITenant> CreateTenantQuery()
        {
            return CommandExecutor.Execute(new CreateTenantQueryCmd());
        }

        public virtual void CreateMembership(string userId, string groupId)
        {
            CommandExecutor.Execute(new CreateMembershipCmd(userId, groupId));
        }

        public virtual void DeleteGroup(string groupId)
        {
            CommandExecutor.Execute(new DeleteGroupCmd(groupId));
        }

        public virtual void DeleteMembership(string userId, string groupId)
        {
            CommandExecutor.Execute(new DeleteMembershipCmd(userId, groupId));
        }

        public virtual bool CheckPassword(string userId, string password)
        {
            return CommandExecutor.Execute(new CheckPassword(userId, password));
        }

        public virtual void DeleteUser(string userId)
        {
            CommandExecutor.Execute(new DeleteUserCmd(userId));
        }

        public virtual void DeleteTenant(string tenantId)
        {
            CommandExecutor.Execute(new DeleteTenantCmd(tenantId));
        }

        public virtual void SetUserPicture(string userId, Picture picture)
        {
            CommandExecutor.Execute(new SetUserPictureCmd(userId, picture));
        }

        public virtual Picture GetUserPicture(string userId)
        {
            return CommandExecutor.Execute(new GetUserPictureCmd(userId));
        }

        public virtual void DeleteUserPicture(string userId)
        {
            CommandExecutor.Execute(new DeleteUserPictureCmd(userId));
        }

        public virtual string AuthenticatedUserId
        {
            set { Authentication = new Authentication(value, null); }
        }

        public virtual Authentication Authentication
        {
            set
            {
                if (value == null)
                {
                    ClearAuthentication();
                }
                else
                {
                    if (!ReferenceEquals(value.UserId, null))
                        EnsureUtil.EnsureValidIndividualResourceId("Invalid user id provided", value.UserId);
                    if (value.GroupIds != null)
                        EnsureUtil.EnsureValidIndividualResourceIds("At least one invalid group id provided",
                            value.GroupIds);
                    if (value.TenantIds != null)
                        EnsureUtil.EnsureValidIndividualResourceIds("At least one invalid tenant id provided",
                            value.TenantIds);

                    _currentAuthentication.Value = value;
                }
            }
        }

        public virtual void SetAuthentication(string userId, IList<string> groups)
        {
            Authentication = new Authentication(userId, groups);
        }

        public virtual void SetAuthentication(string userId, IList<string> groups, IList<string> tenantIds)
        {
            Authentication = new Authentication(userId, groups, tenantIds);
        }

        public virtual void ClearAuthentication()
        {
            //CurrentAuthentication.Remove();
        }

        public virtual Authentication CurrentAuthentication
        {
            get { return _currentAuthentication.Value; }
        }

        public virtual string GetUserInfo(string userId, string key)
        {
            return CommandExecutor.Execute(new GetUserInfoCmd(userId, key));
        }

        public virtual IList<string> GetUserInfoKeys(string userId)
        {
            return CommandExecutor.Execute(new GetUserInfoKeysCmd(userId, IdentityInfoEntity.typeUserinfo));
        }

        public virtual IList<string> GetUserAccountNames(string userId)
        {
            return CommandExecutor.Execute(new GetUserInfoKeysCmd(userId, IdentityInfoEntity.typeUseraccount));
        }

        public virtual void SetUserInfo(string userId, string key, string value)
        {
            CommandExecutor.Execute(new SetUserInfoCmd(userId, key, value));
        }

        public virtual void DeleteUserInfo(string userId, string key)
        {
            CommandExecutor.Execute(new DeleteUserInfoCmd(userId, key));
        }

        public virtual void DeleteUserAccount(string userId, string accountName)
        {
            CommandExecutor.Execute(new DeleteUserInfoCmd(userId, accountName));
        }

        public virtual IAccount GetUserAccount(string userId, string userPassword, string accountName)
        {
            return CommandExecutor.Execute(new GetUserAccountCmd(userId, userPassword, accountName));
        }

        public virtual void SetUserAccount(string userId, string userPassword, string accountName,
            string accountUsername, string accountPassword, IDictionary<string, string> accountDetails)
        {
            CommandExecutor.Execute(new SetUserInfoCmd(userId, userPassword, accountName, accountUsername,
                accountPassword, accountDetails));
        }

        public virtual void CreateTenantUserMembership(string tenantId, string userId)
        {
            CommandExecutor.Execute(new CreateTenantUserMembershipCmd(tenantId, userId));
        }

        public virtual void CreateTenantGroupMembership(string tenantId, string groupId)
        {
            CommandExecutor.Execute(new CreateTenantGroupMembershipCmd(tenantId, groupId));
        }

        public virtual void DeleteTenantUserMembership(string tenantId, string userId)
        {
            CommandExecutor.Execute(new DeleteTenantUserMembershipCmd(tenantId, userId));
        }

        public virtual void DeleteTenantGroupMembership(string tenantId, string groupId)
        {
            CommandExecutor.Execute(new DeleteTenantGroupMembershipCmd(tenantId, groupId));
        }

        public IQueryable<ITenant> CreateTenantQuery(Expression<Func<TenantEntity, bool>> expression = null)
        {
            throw new NotImplementedException();
        }
    }
}
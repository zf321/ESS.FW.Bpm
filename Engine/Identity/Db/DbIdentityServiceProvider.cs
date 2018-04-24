using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Impl.Identity;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac.Features.AttributeFilters;

using ESS.FW.Bpm.Engine.Impl.Cfg;
using Microsoft.Extensions.Logging;
using ESS.FW.DataAccess;

namespace ESS.FW.Bpm.Engine.Identity.Db
{
    /// <summary>
	/// <para><seealso cref="WritableIdentityProvider"/> implementation backed by a
	/// database. This implementation is used for the built-in user management.</para>
	/// 
	/// 
	/// 
	/// </summary>
    public class DbIdentityServiceProvider : DbReadOnlyIdentityServiceProvider, IWritableIdentityProvider

    {
        public DbIdentityServiceProvider(DbContext dbContex, ILoggerFactory loggerFactory, IDGenerator idGenerator, IRepository<GroupEntity, string> groupRepository) : base(dbContex, loggerFactory, idGenerator, groupRepository)
        {
        }

        // users ////////////////////////////////////////////////////////

        public virtual IUser CreateNewUser(string userId)
        {
            CheckAuthorization(Permissions.Create, Resources.User, null);
            return new UserEntity(userId);
        }

        public virtual IUser SaveUser(IUser user)
        {
            UserEntity userEntity = (UserEntity)user;

            // encrypt password
            userEntity.EncryptPassword();

            if (userEntity.Revision == 0)
            {
                CheckAuthorization(Permissions.Create, Resources.User, null);
                Add(userEntity);
                CreateDefaultAuthorizations(userEntity);
            }
            else
            {
                CheckAuthorization(Permissions.Update, Resources.User, user.Id);
                 Merge(userEntity);
            }

            return userEntity;
        }

        public virtual void DeleteUser(string userId)
        {
            CheckAuthorization(Permissions.Delete, Resources.User, userId);
            IUser user = FindUserById(userId);
            if (user != null)
            {
                DeleteMembershipsByUserId(userId);
                DeleteTenantMembershipsOfUser(userId);

                DeleteAuthorizations(Resources.User, userId);
                Delete((UserEntity)user);
            }
        }

        // groups ////////////////////////////////////////////////////////

        public virtual IGroup CreateNewGroup(string groupId)
        {
            CheckAuthorization(Permissions.Create, Resources.Group, null);
            return new GroupEntity(groupId);
        }

        public virtual IGroup SaveGroup(IGroup group)
        {
            GroupEntity groupEntity = (GroupEntity)group;
            if (groupEntity.Revision == 0)
            {
                CheckAuthorization(Permissions.Create, Resources.Group, null);
                
                CommandContext.GetDbEntityManager<GroupEntity>().Add(groupEntity);
                CreateDefaultAuthorizations(group);
            }
            else
            {
                CheckAuthorization(Permissions.Update, Resources.Group, group.Id);
                CommandContext.GetDbEntityManager<GroupEntity>().Update(groupEntity);
            }
            return groupEntity;
        }

        public virtual void DeleteGroup(string groupId)
        {
            CheckAuthorization(Permissions.Delete, Resources.Group, groupId);
            IGroup group = FindGroupById(groupId);
            if (group != null)
            {
                DeleteMembershipsByGroupId(groupId);
                DeleteTenantMembershipsOfGroup(groupId);

                DeleteAuthorizations(Resources.Group, groupId);
                CommandContext.GetDbEntityManager<GroupEntity>().Delete((GroupEntity)group);
            }
        }

        // tenants //////////////////////////////////////////////////////

        public virtual ITenant CreateNewTenant(string tenantId)
        {
            CheckAuthorization(Permissions.Create, Resources.Tenant, null);
            return new TenantEntity(tenantId);
        }

        public virtual ITenant SaveTenant(ITenant tenant)
        {
            TenantEntity tenantEntity = (TenantEntity)tenant;
            if (tenantEntity.Revision == 0)
            {
                CheckAuthorization(Permissions.Create, Resources.Tenant, null);
                CommandContext.GetDbEntityManager<TenantEntity>().Add(tenantEntity);
                CreateDefaultAuthorizations(tenant);
            }
            else
            {
                CheckAuthorization(Permissions.Update, Resources.Tenant, tenant.Id);
                CommandContext.GetDbEntityManager<TenantEntity>().Update(tenantEntity);
            }
            return tenantEntity;
        }

        public virtual void DeleteTenant(string tenantId)
        {
            CheckAuthorization(Permissions.Delete, Resources.Tenant, tenantId);
            ITenant tenant = FindTenantById(tenantId);
            if (tenant != null)
            {
                DeleteTenantMembershipsOfTenant(tenantId);

                DeleteAuthorizations(Resources.Tenant, tenantId);
                CommandContext.GetDbEntityManager<TenantEntity>().Delete((TenantEntity)tenant);
            }
        }

        // membership //////////////////////////////////////////////////////

        public virtual void CreateMembership(string userId, string groupId)
        {
            CheckAuthorization(Permissions.Create, Resources.GroupMembership, groupId);
            IUser user = FindUserById(userId);
            IGroup group = FindGroupById(groupId);
            MembershipEntity membership = new MembershipEntity();
            membership.UserId=user.Id;
            membership.GroupId=group.Id;
            CommandContext.GetDbEntityManager<MembershipEntity>().Add(membership);
            CreateDefaultMembershipAuthorizations(userId, groupId);
        }

        public virtual void DeleteMembership(string userId, string groupId)
        {
            CheckAuthorization(Permissions.Delete, Resources.GroupMembership, groupId);
            DeleteAuthorizations(Resources.GroupMembership, groupId);

            CommandContext.GetDbEntityManager<MembershipEntity>().Delete(c => c.UserId == userId && c.GroupId ==groupId);
        }

        protected internal virtual void DeleteMembershipsByUserId(string userId)
        {
            CommandContext.GetDbEntityManager<MembershipEntity>().Delete(c => c.UserId == userId);
        }

        protected internal virtual void DeleteMembershipsByGroupId(string groupId)
        {
            CommandContext.GetDbEntityManager<MembershipEntity>().Delete(c=>c.GroupId == groupId);
        }

        public virtual void CreateTenantUserMembership(string tenantId, string userId)
        {
            CheckAuthorization(Permissions.Create, Resources.TenantMembership, tenantId);

            ITenant tenant = FindTenantById(tenantId);
            IUser user = FindUserById(userId);

            EnsureUtil.EnsureNotNull("No tenant found with id '" + tenantId + "'.", "tenant", tenant);
            EnsureUtil.EnsureNotNull("No user found with id '" + userId + "'.", "user", user);

            TenantMembershipEntity membership = new TenantMembershipEntity();
            membership.SetTenant((TenantEntity)tenant);
            membership.SetUser((UserEntity)user);

            CommandContext.GetDbEntityManager<TenantMembershipEntity>().Add(membership);

            CreateDefaultTenantMembershipAuthorizations(tenant, user);
        }

        public virtual void CreateTenantGroupMembership(string tenantId, string groupId)
        {
            CheckAuthorization(Permissions.Create, Resources.TenantMembership, tenantId);

            ITenant tenant = FindTenantById(tenantId);
            IGroup group = FindGroupById(groupId);

            EnsureUtil.EnsureNotNull("No tenant found with id '" + tenantId + "'.", "tenant", tenant);
            EnsureUtil.EnsureNotNull("No group found with id '" + groupId + "'.", "group", group);

            TenantMembershipEntity membership = new TenantMembershipEntity();
            membership.SetTenant((TenantEntity)tenant);
            membership.SetGroup((GroupEntity)group);

            CommandContext.GetDbEntityManager<TenantMembershipEntity>().Add(membership);

            CreateDefaultTenantMembershipAuthorizations(tenant, group);
        }

        public virtual void DeleteTenantUserMembership(string tenantId, string userId)
        {
            CheckAuthorization(Permissions.Delete, Resources.TenantMembership, tenantId);
            DeleteAuthorizations(Resources.TenantMembership, userId);

            DeleteAuthorizationsForUser(Resources.Tenant, tenantId, userId);

            CommandContext.GetDbEntityManager<TenantMembershipEntity>()
                .Delete(c => c.TenantId == tenantId && c.UserId == userId);
        }

        public virtual void DeleteTenantGroupMembership(string tenantId, string groupId)
        {
            CheckAuthorization(Permissions.Delete, Resources.TenantMembership, tenantId);
            DeleteAuthorizations(Resources.TenantMembership, groupId);

            DeleteAuthorizationsForGroup(Resources.Tenant, tenantId, groupId);

            CommandContext.GetDbEntityManager<TenantMembershipEntity>()
                .Delete(c => c.TenantId == tenantId && c.GroupId == groupId);
        }

        protected internal virtual void DeleteTenantMembershipsOfUser(string userId)
        {
            CommandContext.GetDbEntityManager<TenantMembershipEntity>()
                .Delete(c => c.UserId == userId);
        }

        protected internal virtual void DeleteTenantMembershipsOfGroup(string groupId)
        {
            CommandContext.GetDbEntityManager<TenantMembershipEntity>()
                .Delete(c => c.GroupId == groupId);
        }

        protected internal virtual void DeleteTenantMembershipsOfTenant(string tenant)
        {
            CommandContext.GetDbEntityManager<TenantMembershipEntity>()
                .Delete(c => c.TenantId == tenant);
        }

        // authorizations ////////////////////////////////////////////////////////////

        protected internal virtual void CreateDefaultAuthorizations(UserEntity userEntity)
        {
            if (context.Impl.Context.ProcessEngineConfiguration.AuthorizationEnabled)
            {
                SaveDefaultAuthorizations(ResourceAuthorizationProvider.NewUser(userEntity));
            }
        }

        protected internal virtual void CreateDefaultAuthorizations(IGroup group)
        {
            if (AuthorizationEnabled)
            {
                SaveDefaultAuthorizations(ResourceAuthorizationProvider.NewGroup(group));
            }
        }

        protected internal virtual void CreateDefaultAuthorizations(ITenant tenant)
        {
            if (AuthorizationEnabled)
            {
                SaveDefaultAuthorizations(ResourceAuthorizationProvider.NewTenant(tenant));
            }
        }

        protected internal virtual void CreateDefaultMembershipAuthorizations(string userId, string groupId)
        {
            if (AuthorizationEnabled)
            {
                SaveDefaultAuthorizations(ResourceAuthorizationProvider.GroupMembershipCreated(groupId, userId));
            }
        }

        protected internal virtual void CreateDefaultTenantMembershipAuthorizations(ITenant tenant, IUser user)
        {
            if (AuthorizationEnabled)
            {
                SaveDefaultAuthorizations(ResourceAuthorizationProvider.TenantMembershipCreated(tenant, user));
            }
        }

        protected internal virtual void CreateDefaultTenantMembershipAuthorizations(ITenant tenant, IGroup group)
        {
            if (AuthorizationEnabled)
            {
                SaveDefaultAuthorizations(ResourceAuthorizationProvider.TenantMembershipCreated(tenant, group));
            }
        }


    }
}

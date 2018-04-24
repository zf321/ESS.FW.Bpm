using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Autofac.Features.AttributeFilters;

using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Identity;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Persistence;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using Microsoft.Extensions.Logging;
using ESS.FW.DataAccess;
using Context = ESS.FW.Bpm.Engine.context.Impl.Context;
using ESS.FW.Common.Components;

namespace ESS.FW.Bpm.Engine.Identity.Db
{
    /// <summary>
    ///     <para>Read only implementation of DB-backed identity service</para>
    /// </summary>
    [Component]
    public class DbReadOnlyIdentityServiceProvider : AbstractManagerNet<UserEntity>, IReadOnlyIdentityProvider
    {
        private readonly IRepository<GroupEntity, string> _groupRepository;

        public DbReadOnlyIdentityServiceProvider( DbContext dbContex,
            ILoggerFactory loggerFactory, IDGenerator idGenerator,
            IRepository<GroupEntity, string> groupRepository) : base(dbContex, loggerFactory, idGenerator)
        {
            _groupRepository = groupRepository;
        }

        // users /////////////////////////////////////////

        public virtual IUser FindUserById(string userId)
        {
            CheckAuthorization(Permissions.Read, Resources.User, userId);
            return Get(userId);
        }

        public IList<string> FindAuditUserId(string workflowid, string state, string branchId, string originator)
        {
            IList<string> userIds = new List<string>();
            
            return userIds;
        }
        //public virtual IQueryable<IUser> CreateUserQuery()
        //{
        //    return new DbUserQueryImpl(Context.GetProcessEngineConfiguration().GetCommandExecutorTxRequired());
        //}

        //public virtual IQueryable<IUser> CreateUserQuery(CommandContext commandContext)
        //{
        //    return new DbUserQueryImpl();
        //}


        //public virtual long FindUserCountByQueryCriteria(DbUserQueryImpl query)
        //{
        //    ConfigureQuery(query, Resources.User);
        //    return (long)DbEntityManager.SelectOne("selectUserCountByQueryCriteria", query);
        //}

        //public virtual IList<IUser> FindUserByQueryCriteria(DbUserQueryImpl query)
        //{
        //    ConfigureQuery(query, Resources.User);
        //    return ListExt.ConvertToListT<IUser>( DbEntityManager.SelectList("selectUserByQueryCriteria", query));
        //}


        public virtual bool CheckPassword(string userId, string password)
        {
            var user = FindUserById(userId);
            if (user != null && password != null && MatchPassword(password, (UserEntity)user))
                return true;
            return false;
        }

        // groups //////////////////////////////////////////

        public virtual IGroup FindGroupById(string groupId)
        {
            CheckAuthorization(Permissions.Read, Resources.Group, groupId);
            return _groupRepository.Get(groupId);
        }

        //public virtual IQueryable<IGroup> CreateGroupQuery()
        //{
        //    return new DbGroupQueryImpl(Context.GetProcessEngineConfiguration().GetCommandExecutorTxRequired());
        //}

        //public virtual IQueryable<IGroup> CreateGroupQuery(CommandContext commandContext)
        //{
        //    return new DbGroupQueryImpl();
        //}

        //public virtual long FindGroupCountByQueryCriteria(DbGroupQueryImpl query)
        //{
        //    ConfigureQuery(query, Resources.Group);
        //    return (long)DbEntityManager.SelectOne("selectGroupCountByQueryCriteria", query);
        //}

        //public virtual IList<IGroup> FindGroupByQueryCriteria(DbGroupQueryImpl query)
        //{
        //    ConfigureQuery(query, Resources.Group);
        //    return ListExt.ConvertToListT<IGroup>(DbEntityManager.SelectList("selectGroupByQueryCriteria", query));
        //}

        //tenants //////////////////////////////////////////

        public virtual ITenant FindTenantById(string tenantId)
        {
            throw new NotImplementedException();
            CheckAuthorization(Permissions.Read, Resources.Tenant, tenantId);
            //return DbEntityManager.SelectById<TenantEntity>(typeof(TenantEntity), tenantId);
        }

        public virtual IQueryable<ITenant> CreateTenantQuery()
        {
            throw new NotImplementedException();
            //return new DbTenantQueryImpl(Context.GetProcessEngineConfiguration().GetCommandExecutorTxRequired());
        }

        public virtual IQueryable<ITenant> CreateTenantQuery(CommandContext commandContext)
        {
            throw new NotImplementedException();
            //return new DbTenantQueryImpl();
        }

        public IQueryable<IUser> CreateUserQuery()
        {
            throw new NotImplementedException();
        }

        public IQueryable<IGroup> CreateGroupQuery()
        {
            throw new NotImplementedException();
        }

        public IQueryable<IGroup> CreateGroupQuery(CommandContext commandContext)
        {
            throw new NotImplementedException();
        }

        protected internal virtual bool MatchPassword(string password, UserEntity user)
        {
            throw new NotImplementedException();
            //string saltedPassword = SaltedPassword(password, user.GetSalt());
            //return Context.GetProcessEngineConfiguration().GetPasswordManager().check(saltedPassword, user.GetPassword());
        }

        //public virtual long FindTenantCountByQueryCriteria(DbTenantQueryImpl query)
        //{
        //    throw new NotImplementedException();
        //    //ConfigureQuery(query, Resources.Tenant);
        //    //return (long)DbEntityManager.SelectOne("selectTenantCountByQueryCriteria", query);
        //}

        //public virtual IList<ITenant> FindTenantByQueryCriteria(DbTenantQueryImpl query)
        //{
        //    throw new NotImplementedException();
        //    //ConfigureQuery(query, Resources.Tenant);
        //    //return DbEntityManager.SelectList("selectTenantByQueryCriteria", query);
        //}

        //authorizations ////////////////////////////////////////////////////

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Override protected void configureQuery(@SuppressWarnings("rawtypes") org.camunda.bpm.engine.impl.AbstractQuery query, org.camunda.bpm.engine.authorization.Resource resource)
        //protected internal override void ConfigureQuery(ListQueryParameterObject query, Resources resource)
        //{
        //    throw new NotImplementedException();
        //    //context.Impl.Context.CommandContext.AuthorizationManager.ConfigureQuery(query, resource);
        //}

        public override void CheckAuthorization(Permissions permission, Resources resource, string resourceId)
        {
            Context.CommandContext.AuthorizationManager.CheckAuthorization(permission, resource, resourceId);
        }

    }
}
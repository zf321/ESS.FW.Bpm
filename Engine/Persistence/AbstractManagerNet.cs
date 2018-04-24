using System;
using System.Linq;
using System.Linq.Expressions;
using ESS.FW.Bpm.Engine.Impl.DB;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.Impl.Cfg.Auth;
using ESS.FW.Bpm.Engine.Impl.Identity;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Persistence.Entity.Impl;
using ESS.FW.DataAccess;
using Microsoft.Extensions.Logging;
using ESS.FW.DataAccess.EF;
using ESS.FW.Bpm.Engine.Impl.Util;
using Microsoft.EntityFrameworkCore;
using Autofac.Features.AttributeFilters;

using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.Shared.Entities.Bpm;

namespace ESS.FW.Bpm.Engine.Persistence
{
    /// <summary>
    /// 基础Manager
	/// </summary>
	public class AbstractManagerNet<TEntity> : EfRepository<TEntity, string>/*, ISession*/ where TEntity : class, IDbEntity, new()
    {
        protected internal static readonly EnginePersistenceLogger Log = ProcessEngineLogger.PersistenceLogger;
        protected readonly IDGenerator _idGenerator;
        protected ILoggerFactory _loggerFactory;

        public AbstractManagerNet(DbContext dbContext, ILoggerFactory loggerFactory, IDGenerator idGenerator) : base(dbContext, loggerFactory)
        {
            _idGenerator = idGenerator;
            _loggerFactory = loggerFactory;
        }

        /// <summary>
        /// Id为Null时调用Id生成器
        /// </summary>
        /// <param name="dbEntity"></param>
        public override TEntity Add(TEntity dbEntity)
        {
            
            EnsureHasId(dbEntity);
            ValidateId(dbEntity);
            Log.LogDebug("EF插入缓存:", $"{dbEntity.GetType().Name} {dbEntity.Id}");
            return base.Add(dbEntity);
        }
        public void Merge(TEntity dbEntity)
        {
            throw new NotImplementedException();
        }
        protected void ValidateId(TEntity dbEntity)
        {
            EnsureUtil.EnsureValidIndividualResourceId("Entity " + dbEntity + " has an invalid id", dbEntity.Id);
        }

        protected void EnsureHasId(TEntity dbEntity)
        {
            if (dbEntity.Id == null)
            {
                String nextId = _idGenerator.NewGuid();
                dbEntity.Id = nextId;
            }
        }

        /// <summary>
        /// 初始化DbEntityManager 参数在ProcessEngineConfigurationImpl中初始化，CommandContext中提取
        /// </summary>

        public virtual void Close()
        {
        }

        public virtual void Flush()
        {
        }

        // authorizations ///////////////////////////////////////

        protected internal virtual CommandContext CommandContext
        {
            get
            {
                return context.Impl.Context.CommandContext;
            }
        }
        /// <summary>
        /// 获取任意Entity的Repository
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected IRepository<T, string> GetDbEntityManager<T>() where T : class, IDbEntity, new()
        {
            return CommandContext.GetDbEntityManager<T>();
        }

        //protected internal virtual void ConfigureQuery(ListQueryParameterObject query, Authorization.Resources resource)
        //{
        //    //AuthorizationManager.ConfigureQuery(query, resource);
        //}

        public virtual void CheckAuthorization(Permissions permission, Authorization.Resources resource, string resourceId)
        {
            AuthorizationManager.CheckAuthorization(permission, resource, resourceId);
        }
        protected IAuthorizationManager AuthorizationManager
        {
            get { return CommandContext.Scope.Resolve<IAuthorizationManager>(); }
        }
        public virtual bool AuthorizationEnabled
        {
            get
            {
                return context.Impl.Context.ProcessEngineConfiguration.AuthorizationEnabled;
            }
        }

        protected internal virtual Authentication CurrentAuthentication
        {
            get
            {
                return context.Impl.Context.CommandContext.Authentication;
            }
        }

        protected internal virtual IResourceAuthorizationProvider ResourceAuthorizationProvider
        {
            get
            {
                return context.Impl.Context.ProcessEngineConfiguration.ResourceAuthorizationProvider;
            }
        }

        protected internal virtual void DeleteAuthorizations(Resources resource, string resourceId)
        {
            AuthorizationManager.DeleteAuthorizationsByResourceId(resource, resourceId);
        }

        protected internal virtual void DeleteAuthorizationsForUser(Resources resource, string resourceId, string userId)
        {
            throw new NotImplementedException();
            //AuthorizationManager.DeleteAuthorizationsByResourceIdAndUserId(resource, resourceId, userId);
        }

        protected internal virtual void DeleteAuthorizationsForGroup(Resources resource, string resourceId, string groupId)
        {
            throw new NotImplementedException();
            //AuthorizationManager.DeleteAuthorizationsByResourceIdAndGroupId(resource, resourceId, groupId);
        }
        
        public virtual void SaveDefaultAuthorizations(AuthorizationEntity[] authorizations)
        {
            if (authorizations != null && authorizations.Length > 0)
            {
                context.Impl.Context.CommandContext.RunWithoutAuthorization(() =>
                {
                    foreach (AuthorizationEntity authorization in authorizations)
                    {

                        if (authorization.Id == null)
                        {
                            AuthorizationManager.Add(authorization);
                        }
                        else
                        {
                            AuthorizationManager.Update(authorization);
                        }

                    }
                });
            }
        }

        public virtual void DeleteDefaultAuthorizations(AuthorizationEntity[] authorizations)
        {
            if (authorizations != null && authorizations.Length > 0)
            {
                context.Impl.Context.CommandContext.RunWithoutAuthorization<object>(() =>
                {
                    foreach (AuthorizationEntity authorization in authorizations)
                    {
                        AuthorizationManager.Delete(authorization);
                    }
                    return null;
                });
            }
        }
    }

}

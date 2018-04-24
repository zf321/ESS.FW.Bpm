using System;
using ESS.FW.Bpm.Engine.Impl.DB;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.Impl.Cfg.Auth;
using ESS.FW.Bpm.Engine.Impl.Identity;
using ESS.FW.Bpm.Engine.Impl.Interceptor;

namespace ESS.FW.Bpm.Engine.Persistence
{
    /// <summary>
    /// 基础Manager
	/// </summary>
        
    [Obsolete("弃用")]
	public abstract class AbstractManager/* : ISession*/
    {

        //public virtual void Insert(IDbEntity dbEntity)
        //{
        //    DbEntityManager.Insert(dbEntity);
        //}

        //public virtual void Delete(IDbEntity dbEntity)
        //{
        //    DbEntityManager.Delete(dbEntity);
        //}
        //protected internal virtual DbEntityManager DbEntityManager
        //{
        //    get
        //    {
        //        return GetSession<DbEntityManager>();
        //    }
        //}
        //[Obsolete("弃用",true)]
        //protected internal virtual DbSqlSession DbSqlSession
        //{
        //    get
        //    {
        //        return GetSession<DbSqlSession>();
        //    }
        //}

        //protected internal virtual T GetSession<T>(Type sessionClass)
        //{
        //    return context.Impl.Context.CommandContext.GetSession<T>(sessionClass);
        //}
        /// <summary>
        /// 移除多余的type参数 由T获取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        //protected virtual T GetSession<T>()
        //{
        //    return context.Impl.Context.CommandContext.Scope.Resolve<T>();
        //}
        //protected internal virtual DeploymentManager DeploymentManager
        //{
        //    get
        //    {
        //        return GetSession<DeploymentManager>();
        //    }
        //}

        //protected internal virtual ResourceManager ResourceManager
        //{
        //    get
        //    {
        //        return GetSession<ResourceManager>();
        //    }
        //}

        //protected internal virtual ByteArrayManager ByteArrayManager
        //{
        //    get
        //    {
        //        return GetSession<ByteArrayManager>();
        //    }
        //}

        //protected internal virtual ProcessDefinitionManager ProcessDefinitionManager
        //{
        //    get
        //    {
        //        return GetSession<ProcessDefinitionManager>();
        //    }
        //}

        //protected internal virtual CaseDefinitionManager CaseDefinitionManager
        //{
        //    get
        //    {
        //        return GetSession<CaseDefinitionManager>();
        //    }
        //}

        //protected internal virtual DecisionDefinitionManager DecisionDefinitionManager
        //{
        //    get
        //    {
        //        return GetSession<DecisionDefinitionManager>();
        //    }
        //}

        //protected internal virtual DecisionRequirementsDefinitionManager DecisionRequirementsDefinitionManager
        //{
        //    get
        //    {
        //        return GetSession<DecisionRequirementsDefinitionManager>();
        //    }
        //}

        //protected internal virtual HistoricDecisionInstanceManager HistoricDecisionInstanceManager
        //{
        //    get
        //    {
        //        return getSession(typeof(HistoricDecisionInstanceManager));
        //    }
        //}

        //protected internal virtual CaseExecutionManager CaseInstanceManager
        //{
        //    get
        //    {
        //        return GetSession<CaseExecutionManager>();
        //    }
        //}

        //protected internal virtual CaseExecutionManager CaseExecutionManager
        //{
        //    get
        //    {
        //        return GetSession<CaseExecutionManager>();
        //    }
        //}

        //protected internal virtual ExecutionManager ProcessInstanceManager
        //{
        //    get
        //    {
        //        return GetSession<ExecutionManager>();// (typeof(ExecutionManager));
        //    }
        //}

        //protected internal virtual TaskManager TaskManager
        //{
        //    get
        //    {
        //        return GetSession<TaskManager>();
        //    }
        //}

        //protected internal virtual TaskReportManager TaskReportManager
        //{
        //    get
        //    {
        //        return GetSession<TaskReportManager>();
        //    }
        //}

        //protected internal virtual IdentityLinkManager IdentityLinkManager
        //{
        //    get
        //    {
        //        return GetSession<IdentityLinkManager>();
        //    }
        //}

        //protected internal virtual VariableInstanceManager VariableInstanceManager
        //{
        //    get
        //    {
        //        return GetSession<VariableInstanceManager>(typeof(VariableInstanceManager));
        //    }
        //}

        //protected internal virtual HistoricProcessInstanceManager HistoricProcessInstanceManager
        //{
        //    get
        //    {
        //        return GetSession<HistoricProcessInstanceManager>();
        //    }
        //}

        //protected internal virtual HistoricCaseInstanceManager HistoricCaseInstanceManager
        //{
        //    get
        //    {
        //        return GetSession<HistoricCaseInstanceManager>();
        //    }
        //}

        //protected internal virtual HistoricDetailManager HistoricDetailManager
        //{
        //    get
        //    {
        //        return GetSession<HistoricDetailManager>();
        //    }
        //}

        //protected internal virtual HistoricVariableInstanceManager HistoricVariableInstanceManager
        //{
        //    get
        //    {
        //        return GetSession<HistoricVariableInstanceManager>();
        //    }
        //}

        //protected internal virtual HistoricActivityInstanceManager HistoricActivityInstanceManager
        //{
        //    get
        //    {
        //        return GetSession<HistoricActivityInstanceManager>();
        //    }
        //}

        //protected internal virtual HistoricCaseActivityInstanceManager HistoricCaseActivityInstanceManager
        //{
        //    get
        //    {
        //        return GetSession<HistoricCaseActivityInstanceManager>();
        //    }
        //}

        //protected internal virtual HistoricTaskInstanceManager HistoricTaskInstanceManager
        //{
        //    get
        //    {
        //        return GetSession<HistoricTaskInstanceManager>();
        //    }
        //}

        //protected internal virtual HistoricIncidentManager HistoricIncidentManager
        //{
        //    get
        //    {
        //        return GetSession<HistoricIncidentManager>();
        //    }
        //}

        //protected internal virtual HistoricIdentityLinkLogManager HistoricIdentityLinkManager
        //{
        //    get
        //    {
        //        return GetSession<HistoricIdentityLinkLogManager>();
        //    }
        //}

        //protected internal virtual HistoricJobLogManager HistoricJobLogManager
        //{
        //    get
        //    {
        //        return GetSession<HistoricJobLogManager>();
        //    }
        //}

        //protected internal virtual HistoricExternalTaskLogManager HistoricExternalTaskLogManager
        //{
        //    get
        //    {
        //        return GetSession<HistoricExternalTaskLogManager>();
        //    }
        //}

        //protected internal virtual JobManager JobManager
        //{
        //    get
        //    {
        //        return GetSession<JobManager>();
        //    }
        //}

        //protected internal virtual JobDefinitionManager JobDefinitionManager
        //{
        //    get
        //    {
        //        return GetSession<JobDefinitionManager>();
        //    }
        //}

        //protected internal virtual UserOperationLogManager UserOperationLogManager
        //{
        //    get
        //    {
        //        return GetSession<UserOperationLogManager>();
        //    }
        //}

        //protected internal virtual EventSubscriptionManager EventSubscriptionManager
        //{
        //    get
        //    {
        //        return GetSession<EventSubscriptionManager>();
        //    }
        //}

        //protected internal virtual IdentityInfoManager IdentityInfoManager
        //{
        //    get
        //    {
        //        return GetSession<IdentityInfoManager>();
        //    }
        //}

        //protected internal virtual AttachmentManager AttachmentManager
        //{
        //    get
        //    {
        //        return GetSession<AttachmentManager>();
        //    }
        //}

        //protected internal virtual ReportManager HistoricReportManager
        //{
        //    get
        //    {
        //        return GetSession<ReportManager>();
        //    }
        //}

        //protected internal virtual BatchManager BatchManager
        //{
        //    get
        //    {
        //        return GetSession<BatchManager>();
        //    }
        //}

        //protected internal virtual HistoricBatchManager HistoricBatchManager
        //{
        //    get
        //    {
        //        return GetSession<HistoricBatchManager>();
        //    }
        //}

        //protected internal virtual TenantManager TenantManager
        //{
        //    get
        //    {
        //        return GetSession<TenantManager>();
        //    }
        //}

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

        protected internal virtual IAuthorizationManager AuthorizationManager
        {
            get
            {
                return CommandContext.Scope.Resolve<IAuthorizationManager>();
            }
        }

        //protected internal virtual void ConfigureQuery(ListQueryParameterObject query, Authorization.Resources resource)
        //{
        //    AuthorizationManager.ConfigureQuery(query, resource,"");
        //}

        protected internal virtual void CheckAuthorization(Permissions permission, Authorization.Resources resource, string resourceId)
        {
            AuthorizationManager.CheckAuthorization(permission, resource, resourceId);
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
            AuthorizationManager.DeleteAuthorizationsByResourceIdAndUserId(resource, resourceId, userId);
        }

        protected internal virtual void DeleteAuthorizationsForGroup(Resources resource, string resourceId, string groupId)
        {
            AuthorizationManager.DeleteAuthorizationsByResourceIdAndGroupId(resource, resourceId, groupId);
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
                context.Impl.Context.CommandContext.RunWithoutAuthorization(() =>
                {
                    foreach (AuthorizationEntity authorization in authorizations)
                    {
                        AuthorizationManager.Delete(authorization);
                    }
                });
            }
        }
        
    }

}

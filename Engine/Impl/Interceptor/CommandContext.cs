using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Autofac.Features.Metadata;

using ESS.FW.Bpm.Engine.Application;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Dmn.Impl.Entity.Repository;
using ESS.FW.Bpm.Engine.EntityFramework;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.DB;
using ESS.FW.Bpm.Engine.Impl.DB.EntityManager.Cache;
using ESS.FW.Bpm.Engine.Impl.Identity;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using ESS.FW.Bpm.Engine.Persistence;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Common.Components;
using Microsoft.Extensions.Logging;
using ESS.FW.DataAccess;
using Context = ESS.FW.Bpm.Engine.context.Impl.Context;
using ESS.FW.Bpm.Engine.Common;
using System.Transactions;
using ESS.FW.Bpm.Engine.Exception;
using org.camunda.bpm.engine.impl.history.@event;
using ESS.FW.Bpm.Engine.Impl.DB.EntityManager.Operation;
using ESS.FW.Bpm.Engine.Impl.DB.EntityManager;
using ESS.Shared.Entities.Bpm;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ESS.FW.Bpm.Engine.Impl.Interceptor
{
    /// <summary>
    ///     主要类型ProcessEngineConfigurationImpl
    /// </summary>
    public class CommandContext
    {
        private static readonly ContextLogger Log = ProcessEngineLogger.ContextLogger;
        private static readonly EnginePersistenceLogger pLog = ProcessEngineLogger.PersistenceLogger;

        protected internal ICollection<ICommandContextListener> CommandContextListeners =
            new LinkedList<ICommandContextListener>();

        protected internal IFailedJobCommandFactory failedJobCommandFactory;
        protected internal ProcessEngineConfigurationImpl processEngineConfiguration;
        public IScope Scope;

        protected internal ITransactionContext transactionContext;
        protected Queue<CommittableTransaction> transactionQueues;
        protected Queue<CommittableTransaction> trancactionReadyToRollBack;

        public CommandContext()
        {
        }

        public CommandContext(ProcessEngineConfigurationImpl processEngineConfiguration, IScope scope)
            : this(processEngineConfiguration, processEngineConfiguration.TransactionContextFactory, scope)
        {
        }

        public CommandContext(ProcessEngineConfigurationImpl processEngineConfiguration,
            ITransactionContextFactory transactionContextFactory, IScope scope)
        {
            Scope = scope;
            this.processEngineConfiguration = processEngineConfiguration;
            failedJobCommandFactory = processEngineConfiguration.FailedJobCommandFactory;
            //sessionFactories = processEngineConfiguration.SessionFactories;
            transactionContext = transactionContextFactory.OpenTransactionContext(this);
            RestrictUserOperationLogToAuthenticatedUsers =
                processEngineConfiguration.RestrictUserOperationLogToAuthenticatedUsers;
        }
        /// <summary>
        /// 当前外部代码事务
        /// </summary>
        public System.Transactions.Transaction CurrentTransaction
        {
            get
            {
                if (transactionQueues.Count > 0)
                {
                    return transactionQueues.Peek();
                }
                return null;
            }
        }
        /// <summary>
        /// 进入SubProcess时创建新事务
        /// </summary>
        public void AddTransaction()
        {
            //TODO 外部代码事务隔离级别配置
            TransactionOptions options = new TransactionOptions();
            CommittableTransaction tr = new CommittableTransaction(options);
            if (transactionQueues == null)
            {
                transactionQueues = new Queue<CommittableTransaction>();
            }
            transactionQueues.Enqueue(tr);
        }
        public void MoveToRollBack()
        {
            var tr = transactionQueues.Dequeue();
            if (trancactionReadyToRollBack == null)
            {
                trancactionReadyToRollBack = new Queue<CommittableTransaction>();
            }
            trancactionReadyToRollBack.Enqueue(tr);
        }
        public virtual ProcessEngineConfigurationImpl ProcessEngineConfiguration
        {
            get { return processEngineConfiguration; }
        }


        public virtual IDeploymentManager DeploymentManager
        {
            get { return Scope.Resolve<IDeploymentManager>(); }
        }

        public virtual IResourceManager ResourceManager
        {
            get
            {
                return Scope.Resolve<IResourceManager>();
            }
        }

        public virtual IByteArrayManager ByteArrayManager
        {
            get { return Scope.Resolve<IByteArrayManager>(); }
        }

        public virtual IProcessDefinitionManager ProcessDefinitionManager
        {
            get { return Scope.Resolve<IProcessDefinitionManager>(); }
        }

        public virtual IAbstractResourceDefinitionManager<ProcessDefinitionEntity> ProcessDefinitionManager_2
        {
            get { return Scope.Resolve<IAbstractResourceDefinitionManager<ProcessDefinitionEntity>>(); }
        }

        public virtual IExecutionManager ExecutionManager
        {
            get { return Scope.Resolve<IExecutionManager>(); }
        }

        public virtual ITaskManager TaskManager
        {
            get { return Scope.Resolve<ITaskManager>(); }
        }

        public virtual ITaskReportManager TaskReportManager
        {
            get { return Scope.Resolve<ITaskReportManager>(); }
        }

        public virtual IMeterLogManager MeterLogManager
        {
            get { return Scope.Resolve<IMeterLogManager>(); }
        }

        public virtual IIdentityLinkManager IdentityLinkManager
        {
            get { return Scope.Resolve<IIdentityLinkManager>(); }
        }

        public virtual IVariableInstanceManager VariableInstanceManager
        {
            get { return Scope.Resolve<IVariableInstanceManager>(); }
        }

        public virtual IHistoricProcessInstanceManager HistoricProcessInstanceManager
        {
            get { return Scope.Resolve<IHistoricProcessInstanceManager>(); }
        }

        public virtual IHistoricCaseInstanceManager HistoricCaseInstanceManager
        {
            get { return Scope.Resolve<IHistoricCaseInstanceManager>(); }
        }

        public virtual IHistoricDecisionInstanceManager HistoricDecisionInstanceManager
        {
            get { return Scope.Resolve<IHistoricDecisionInstanceManager>(); }
        }

        public virtual IHistoricDetailManager HistoricDetailManager
        {
            get { return Scope.Resolve<IHistoricDetailManager>(); }
        }

        public virtual IUserOperationLogManager OperationLogManager
        {
            get { return Scope.Resolve<IUserOperationLogManager>(); }
        }

        public virtual IHistoricVariableInstanceManager HistoricVariableInstanceManager
        {
            get { return Scope.Resolve<IHistoricVariableInstanceManager>(); }
        }

        public virtual IHistoricActivityInstanceManager HistoricActivityInstanceManager
        {
            get { return Scope.Resolve<IHistoricActivityInstanceManager>(); }
        }

        public virtual IHistoricCaseActivityInstanceManager HistoricCaseActivityInstanceManager
        {
            get { return Scope.Resolve<IHistoricCaseActivityInstanceManager>(); }
        }

        public virtual IHistoricTaskInstanceManager HistoricTaskInstanceManager
        {
            get { return Scope.Resolve<IHistoricTaskInstanceManager>(); }
        }

        public virtual IHistoricIncidentManager HistoricIncidentManager
        {
            get { return Scope.Resolve<IHistoricIncidentManager>(); }
        }

        public virtual IHistoricIdentityLinkLogManager HistoricIdentityLinkManager
        {
            get { return Scope.Resolve<IHistoricIdentityLinkLogManager>(); }
        }

        public virtual IJobManager JobManager
        {
            get { return Scope.Resolve<IJobManager>(); }
        }

        public virtual IBatchManager BatchManager
        {
            get { return Scope.Resolve<IBatchManager>(); }
        }

        public virtual IHistoricBatchManager HistoricBatchManager
        {
            get { return Scope.Resolve<IHistoricBatchManager>(); }
        }

        public virtual IJobDefinitionManager JobDefinitionManager
        {
            get { return Scope.Resolve<IJobDefinitionManager>(); }
        }

        public virtual IIncidentManager IncidentManager
        {
            get { return Scope.Resolve<IIncidentManager>(); }
        }

        public virtual IIdentityInfoManager IdentityInfoManager
        {
            get { return Scope.Resolve<IIdentityInfoManager>(); }
        }

        public virtual IAttachmentManager AttachmentManager
        {
            get { return Scope.Resolve<IAttachmentManager>(); }
        }

        //public virtual TableDataManager TableDataManager
        //{
        //    get
        //    {
        //        return GetSession<TableDataManager>(typeof(TableDataManager));
        //    }
        //}

        public virtual ICommentManager CommentManager
        {
            get { return Scope.Resolve<ICommentManager>(); }
        }

        public virtual IEventSubscriptionManager EventSubscriptionManager
        {
            get { return Scope.Resolve<IEventSubscriptionManager>(); }
        }

        //public virtual IDictionary<Type, ISessionFactory> SessionFactories
        //{
        //    get { return sessionFactories; }
        //}

        public virtual IPropertyManager PropertyManager
        {
            get { return Scope.Resolve<IPropertyManager>(); }
        }

        //public virtual StatisticsManager StatisticsManager
        //{
        //    get
        //    {
        //        return GetSession<StatisticsManager>(typeof(StatisticsManager));
        //    }
        //}

        //public virtual IHistoricStatisticsManager HistoricStatisticsManager
        //{
        //    get
        //    {
        //        return Scope.Resolve<IPropertyManager>();
        //        return GetSession<HistoricStatisticsManager>(typeof(HistoricStatisticsManager));
        //    }
        //}

        public virtual IHistoricJobLogManager HistoricJobLogManager
        {
            get
            {
                return Scope.Resolve<IHistoricJobLogManager>();
            }
        }

        public virtual IHistoricExternalTaskLogManager HistoricExternalTaskLogManager
        {
            get { return Scope.Resolve<IHistoricExternalTaskLogManager>(); }
        }

        public virtual IReportManager HistoricReportManager
        {
            get { return Scope.Resolve<IReportManager>(); }
        }

        public virtual IAuthorizationManager AuthorizationManager
        {
            get { return Scope.Resolve<IAuthorizationManager>(); }
        }

        public virtual IReadOnlyIdentityProvider ReadOnlyIdentityProvider
        {
            get { return Scope.Resolve<IReadOnlyIdentityProvider>(); }
        }

        public virtual IWritableIdentityProvider WritableIdentityProvider
        {
            get { return Scope.Resolve<IWritableIdentityProvider>(); }
        }

        public virtual ITenantManager TenantManager
        {
            get { return Scope.Resolve<ITenantManager>(); }
        }

        // CMMN /////////////////////////////////////////////////////////////////////


        // DMN //////////////////////////////////////////////////////////////////////

        public virtual IDecisionDefinitionManager DecisionDefinitionManager
        {
            get { return Scope.Resolve<IDecisionDefinitionManager>(); }
        }

        public virtual IAbstractResourceDefinitionManager<DecisionDefinitionEntity> DecisionDefinitionManager_2
        {
            get { return Scope.Resolve<IAbstractResourceDefinitionManager<DecisionDefinitionEntity>>(); }
        }

        public virtual IDecisionRequirementsDefinitionManager DecisionRequirementsDefinitionManager
        {
            get { return Scope.Resolve<IDecisionRequirementsDefinitionManager>(); }
        }

        public virtual IAbstractResourceDefinitionManager<DecisionRequirementsDefinitionEntity>
            DecisionRequirementsDefinitionManager_2
        {
            get { return Scope.Resolve<IAbstractResourceDefinitionManager<DecisionRequirementsDefinitionEntity>>(); }
        }

        //public virtual HistoricDecisionInstanceManager HistoricDecisionInstanceManager
        //{
        //    get { return GetSession<HistoricDecisionInstanceManager>(typeof (HistoricDecisionInstanceManager)); }
        //}

        public virtual ITransactionContext TransactionContext
        {
            get { return transactionContext; }
        }

        //public virtual IDictionary<Type, ISession> Sessions
        //{
        //    get { return sessions; }
        //}

        public virtual IFailedJobCommandFactory FailedJobCommandFactory
        {
            get { return failedJobCommandFactory; }
        }

        public virtual Authentication Authentication
        {
            get
            {
                var identityService = processEngineConfiguration.IdentityService;
                return identityService.CurrentAuthentication;
            }
        }

        public virtual string AuthenticatedUserId
        {
            get
            {
                var identityService = processEngineConfiguration.IdentityService;
                var currentAuthentication = identityService.CurrentAuthentication;
                if (currentAuthentication == null)
                    return null;
                return currentAuthentication.UserId;
            }
        }

        public virtual IList<string> AuthenticatedGroupIds
        {
            get
            {
                var identityService = processEngineConfiguration.IdentityService;
                var currentAuthentication = identityService.CurrentAuthentication;
                if (currentAuthentication == null)
                    return null;
                return currentAuthentication.GroupIds;
            }
        }

        public virtual bool AuthorizationCheckEnabled { get; set; } = true;

        public virtual bool UserOperationLogEnabled { get; protected internal set; } = true;

        public virtual bool LogUserOperationEnabled
        {
            set { UserOperationLogEnabled = value; }
        }

        public virtual bool TenantCheckEnabled { set; get; } = true;


        public virtual JobEntity CurrentJob { get; set; }


        public virtual bool RestrictUserOperationLogToAuthenticatedUsers { get; set; }

        public DbContext DbContext
        {
            get
            {
                var dbcontexts = Scope.Resolve<IEnumerable<DbContext>>();
                return dbcontexts.FirstOrDefault(c => c.GetType() == typeof(BpmDbContext));
            }
        }

        // Filter ////////////////////////////////////////////////////////////////////

        public virtual IFilterManager FilterManager
        {
            get
            {
                return Scope.Resolve<IFilterManager>();
            }
        }

        // External Tasks ////////////////////////////////////////////////////////////

        public virtual IExternalTaskManager ExternalTaskManager
        {
            get
            {
                return Scope.Resolve<IExternalTaskManager>();
            }
        }
        //TODO 缓存  CacheAwareHistoryEventProducer.FindInCache
        public DbEntityCache DbEntityCache
        {
            get
            {
                DbEntityCache dbEntityCache = null;
                var jobExecutorContext = Context.JobExecutorContext;
                var processEngineConfiguration = Context.ProcessEngineConfiguration;

                if (processEngineConfiguration != null && processEngineConfiguration.DbEntityCacheReuseEnabled &&
                    jobExecutorContext != null)
                {
                    dbEntityCache = jobExecutorContext.EntityCache;
                    if (dbEntityCache == null)
                    {
                        dbEntityCache = new DbEntityCache(processEngineConfiguration.DbEntityCacheKeyMapping);
                        jobExecutorContext.EntityCache = dbEntityCache;
                    }
                }
                else
                {
                    if (processEngineConfiguration != null)
                        dbEntityCache = new DbEntityCache(processEngineConfiguration.DbEntityCacheKeyMapping);
                    else
                        dbEntityCache = new DbEntityCache();
                }
                return dbEntityCache;
            }
        }

        public virtual ProcessEngineConfiguration GetProcessEngineConfiguration()
        {
            return processEngineConfiguration;
        }

        public virtual void RunWithoutAuthorization(Action runnable)
        {
            var commandContext = Context.CommandContext;
            var authorizationEnabled = commandContext.AuthorizationCheckEnabled;
            try
            {
                commandContext.DisableAuthorizationCheck();
                runnable();
            }
            catch (ProcessEngineException e)
            {
                throw e;
            }
            catch (RuntimeException e)
            {
                throw e;
            }
            catch (System.Exception e)
            {
                throw new ProcessEngineException(e.Message, e);
            }
            finally
            {
                if (authorizationEnabled)
                    commandContext.EnableAuthorizationCheck();
            }
        }

        public virtual T RunWithoutAuthorization<T>(Func<T> runnable)
        {
            var commandContext = Context.CommandContext;
            var authorizationEnabled = commandContext.AuthorizationCheckEnabled;
            try
            {
                commandContext.DisableAuthorizationCheck();
                return runnable();
            }
            catch (ProcessEngineException e)
            {
                throw e;
            }
            catch (RuntimeException e)
            {
                throw e;
            }            
            catch (System.Exception e)
            {
                throw new ProcessEngineException(e);
            }
            finally
            {
                if (authorizationEnabled)
                    commandContext.EnableAuthorizationCheck();
            }
        }

        //protected internal virtual IProcessApplicationReference GetTargetProcessApplication(CaseExecutionEntity execution)
        //{
        //    return ProcessApplicationContextUtil.getTargetProcessApplication(execution);
        //}

        protected internal virtual bool RequiresContextSwitch(IProcessApplicationReference processApplicationReference)
        {
            return ProcessApplicationContextUtil.RequiresContextSwitch(processApplicationReference);
        }

        public virtual void Close(CommandInvocationContext commandInvocationContext)
        {
            // the intention of this method is that all resources are closed properly,
            // even
            // if exceptions occur in close or flush methods of the sessions or the
            // transaction context.

            //try
            //{
            //    try
            //    {
            //        try
            //        {
            if (commandInvocationContext.Throwable == null)
            {
                FireCommandContextClose();
                FlushSessions();
                //清理缓存
                Context.CommandContext.DbEntityCache.Clear();
            }
            //}
            //catch (System.Exception exception)
            //{
            //    commandInvocationContext.TrySetThrowable(exception);
            //}
            //finally
            //{
            //try
            //{
            if (commandInvocationContext.Throwable == null)
            {
                FlushTransactions();//外部代码事务
                try
                {
                    transactionContext.Commit(); //系统数据库事务
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    EntityEntry entry = ex.Entries.ElementAt(0);
                    var dbOp = new DbEntityOperation();
                    dbOp.SetFailed(false);
                    dbOp.EntityType = entry.Entity.GetType();
                    dbOp.Entity = entry.Entity as IDbEntity;
                    HandleOptimisticLockingException(dbOp);
                }
            }

            //}
            //catch (System.Exception exception)
            //{
            //    commandInvocationContext.TrySetThrowable(exception);
            //}

            //if (commandInvocationContext.Throwable != null)
            //{
            //    // fire command failed (must not fail itself)
            //    FireCommandFailed(commandInvocationContext.Throwable);

            //    if (ShouldLogInfo(commandInvocationContext.Throwable))
            //        Log.InfoException(commandInvocationContext.Throwable);
            //    else if (ShouldLogFine(commandInvocationContext.Throwable))
            //        Log.DebugException(commandInvocationContext.Throwable);
            //    else
            //        Log.ErrorException(commandInvocationContext.Throwable);
            //    transactionContext.Rollback();
            //}
            //}
            //}
            //catch (System.Exception exception)
            //{
            //    commandInvocationContext.TrySetThrowable(exception);
            //}
            //finally
            //{
            CloseSessions(commandInvocationContext);
            //}
            //}
            //catch (System.Exception exception)
            //{
            //    commandInvocationContext.TrySetThrowable(exception);
            //}

            // rethrow the original exception if there was one
            //commandInvocationContext.Rethrow();
        }
        private List<IOptimisticLockingListener> optimisticLockingListeners;
        protected void HandleOptimisticLockingException(DbOperation dbOperation)
        {
            bool isHandled = false;

            if (optimisticLockingListeners != null)
            {
                foreach (IOptimisticLockingListener optimisticLockingListener in optimisticLockingListeners)
                {
                    //if (optimisticLockingListener.EntityType == null || optimisticLockingListener.EntityType.IsSubclassOf(dbOperation.EntityType))
                    if (optimisticLockingListener.EntityType == null || dbOperation.EntityType.IsSubclassOf(optimisticLockingListener.EntityType))
                    {
                        optimisticLockingListener.FailedOperation(dbOperation);
                        isHandled = true;
                    }
                }
            }

            if (!isHandled)
            {
                throw pLog.ConcurrentUpdateDbEntityException(dbOperation);
            }
        }

        public void RegisterOptimisticLockingListener(IOptimisticLockingListener optimisticLockingListener)
        {
            if (optimisticLockingListeners == null)
            {
                optimisticLockingListeners = new List<IOptimisticLockingListener>();
            }
            optimisticLockingListeners.Add(optimisticLockingListener);
        }

        protected internal virtual bool ShouldLogInfo(System.Exception exception)
        {
            return exception is TaskAlreadyClaimedException;
        }

        protected internal virtual bool ShouldLogFine(System.Exception exception)
        {
            return exception is OptimisticLockingException || exception is BadUserRequestException;
        }

        protected internal virtual void FireCommandContextClose()
        {
            foreach (var listener in CommandContextListeners)
                listener.OnCommandContextClose(this);
        }

        protected internal virtual void FireCommandFailed(System.Exception t)
        {
            foreach (var listener in CommandContextListeners)
                try
                {
                    listener.OnCommandFailed(this, t);
                }
                catch (System.Exception)
                {
                    Log.ExceptionWhileInvokingOnCommandFailed(t);
                }
        }

        protected internal virtual void FlushSessions()
        {
            //foreach (var item in SessionList)
            //{
            //    item.Flush();
            //}
        }
        /// <summary>
        /// 提交待提交事务，并回滚待回滚事务
        /// </summary>
        protected void FlushTransactions()
        {
            //Log.LogDebug("待Commit外部事务Count:" + transactionQueues==null?"0":transactionQueues.Count.ToString(), "待RollBack外部事务Count:" + trancactionReadyToRollBack==null?"0": trancactionReadyToRollBack.Count.ToString());
            while (transactionQueues != null && transactionQueues.Count > 0)
            {
                Log.LogDebug("外部事务Commit", "");
                var tr = transactionQueues.Dequeue();
                try
                {
                    tr.Commit();
                }
                catch (System.Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    tr.Dispose();
                }
            }
            while (trancactionReadyToRollBack != null && trancactionReadyToRollBack.Count > 0)
            {
                Log.LogDebug("外部事务回滚", "");
                var tr = trancactionReadyToRollBack.Dequeue();
                try
                {
                    tr.Rollback();
                }
                catch (System.Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    tr.Dispose();
                }
            }
        }

        protected internal virtual void CloseSessions(CommandInvocationContext commandInvocationContext)
        {
            //Scope.Dispose();
            //Log.LogInfo("销毁DI对象","------------------Dispose CommandContext Scope------------------");
            //foreach (var session in SessionList)
            //    try
            //    {
            //        session.Close();
            //    }
            //    catch (System.Exception exception)
            //    {
            //        commandInvocationContext.TrySetThrowable(exception);
            //    }
        }

        public IRepository<TEntity, string> GetDbEntityManager<TEntity>() where TEntity : class, IDbEntity, new()
        {
            var reps = Scope.Resolve<IRepository<TEntity, string>>();
            return reps;
        }

        // getters and setters //////////////////////////////////////////////////////

        public virtual void RegisterCommandContextListener(ICommandContextListener commandContextListener)
        {
            if (!CommandContextListeners.Contains(commandContextListener))
                CommandContextListeners.Add(commandContextListener);
        }

        public virtual void EnableAuthorizationCheck()
        {
            AuthorizationCheckEnabled = true;
        }

        public virtual void DisableAuthorizationCheck()
        {
            AuthorizationCheckEnabled = false;
        }


        public virtual void EnableUserOperationLog()
        {
            UserOperationLogEnabled = true;
        }

        public virtual void DisableUserOperationLog()
        {
            UserOperationLogEnabled = false;
        }

        public virtual void EnableTenantCheck()
        {
            TenantCheckEnabled = true;
        }

        public virtual void DisableTenantCheck()
        {
            TenantCheckEnabled = false;
        }


        /// <summary>
        ///     添加LoggerFactory
        /// </summary>
        /// <returns></returns>
        public ILoggerFactory GetLoggerFactory()
        {
            return Scope.Resolve<ILoggerFactory>();
        }

        public IDGenerator GetIdGenerator()
        {
            return processEngineConfiguration.IdGenerator;
        }
        /// <summary>
        /// 移除已被删除的entity
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entities"></param>
        /// <returns></returns>
        public IList<TEntity> PruneDeletedEntities<TEntity>(IList<TEntity> entities) where TEntity : class, IDbEntity
        {
            for (int i = 0; i < entities.Count; i++)
            {
                if (Context.CommandContext.DbContext.Entry<TEntity>(entities[i]).State == EntityState.Deleted)
                {
                    entities.RemoveAt(i);
                }
            }
            return entities;
        }
        /// <summary>
        /// 获取EF实体当前状态
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public EntityState GetEntityStateInEF<TEntity>(TEntity entity) where TEntity : class, IDbEntity
        {
            return Context.CommandContext.DbContext.Entry<TEntity>(entity).State;
        }
        public void Merge<TEntity>(TEntity entity) where TEntity : class, IDbEntity, new()
        {
            var cache = GetDbEntityManager<TEntity>().Get(entity.Id);
            if (cache == null)
            {
                throw new System.Exception(string.Format("EF缓存中找不到 {0} id:{1}", typeof(TEntity).Name, entity.Id));
            }
            PropertyUpdateHelper.UpDate<TEntity>(cache, entity);
        }
    }
}
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Autofac.Features.AttributeFilters;

using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.DB;
using ESS.FW.Bpm.Engine.Impl.Event;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Common.Components;
using Microsoft.Extensions.Logging;
using ESS.FW.DataAccess;
using Context = ESS.FW.Bpm.Engine.context.Impl.Context;

namespace ESS.FW.Bpm.Engine.Persistence.Entity.Impl
{
    /// <summary>
    ///     
    /// </summary>
    [Component]
    public class ProcessDefinitionManager : AbstractManagerNet<ProcessDefinitionEntity>,
        IAbstractResourceDefinitionManager<ProcessDefinitionEntity>, IProcessDefinitionManager
    {
        protected internal static readonly EnginePersistenceLogger Log = ProcessEngineLogger.PersistenceLogger;
        private readonly IHistoricIdentityLinkLogManager _historicIdentityLinkLogManager;

        // insert ///////////////////////////////////////////////////////////
        private readonly IHistoricIncidentManager _historicIncidentManager;
        private readonly IHistoricJobLogManager _historicJobLogManager;

        private readonly IJobManager _jobManager;
        private readonly IJobDefinitionManager _jobDefinitionManager;
        private readonly IIdentityLinkManager _identityLinkManager;
        private readonly IExecutionManager _execuutionManager;
        //private readonly IEventSubscriptionManager _eventSubscriptionManager;

        public ProcessDefinitionManager(DbContext dbContex,
            ILoggerFactory loggerFactory,
            IDGenerator idGenerator,
            IHistoricIncidentManager historicIncidentManager,
            IHistoricIdentityLinkLogManager historicIdentityLinkLogManager,
            IHistoricJobLogManager historicJobLogManager,
            IJobManager jobManager,
            IJobDefinitionManager jobDefinitionManager,
            IIdentityLinkManager identityLinkManager,
            IExecutionManager executionManager) : base(dbContex, loggerFactory, idGenerator)
        {
            _historicIncidentManager = historicIncidentManager;
            _historicIdentityLinkLogManager = historicIdentityLinkLogManager;
            _historicJobLogManager = historicJobLogManager;
            _jobManager = jobManager;
            _jobDefinitionManager = jobDefinitionManager;
            _identityLinkManager = identityLinkManager;
            _execuutionManager = executionManager;
            //_eventSubscriptionManager = eventSubscriptionManager;
        }

        public ProcessDefinitionEntity FindLatestDefinitionByKey(string key)
        {
            return FindLatestProcessDefinitionByKey(key);
        }

        public ProcessDefinitionEntity FindLatestDefinitionById(string id)
        {
            return FindLatestProcessDefinitionById(id);
        }

        public ProcessDefinitionEntity GetCachedResourceDefinitionEntity(string definitionId)
        {
            return CommandContext.ProcessEngineConfiguration.deploymentCache.ProcessDefinitionCache.Get(definitionId);
            //return DbEntityManager.GetCachedEntity(definitionId);
        }

        public ProcessDefinitionEntity FindLatestDefinitionByKeyAndTenantId(string definitionKey, string tenantId)
        {
            return FindLatestProcessDefinitionByKeyAndTenantId(definitionKey, tenantId);
        }

        public ProcessDefinitionEntity FindDefinitionByKeyVersionAndTenantId(string definitionKey,
            int? definitionVersion, string tenantId)
        {
            return FindProcessDefinitionByKeyVersionAndTenantId(definitionKey, definitionVersion, tenantId);
        }

        public ProcessDefinitionEntity FindDefinitionByDeploymentAndKey(string deploymentId, string definitionKey)
        {
            return FindProcessDefinitionByDeploymentAndKey(deploymentId, definitionKey);
        }

        public virtual void InsertProcessDefinition(ProcessDefinitionEntity processDefinition)
        {
            //TODO 数据库插入流程定义实现

            //DbEntityManager.Insert(processDefinition);
            Add(processDefinition);
            CreateDefaultAuthorizations(processDefinition);
        }

        // select ///////////////////////////////////////////////////////////

        /// <returns>
        ///     the latest version of the process definition with the given key (from any tenant)
        /// </returns>
        /// <exception cref="ProcessEngineException">
        ///     if more than one tenant has a process definition with the given key
        /// </exception>
        /// <seealso cref= FindLatestProcessDefinitionByKeyAndTenantId( String, String
        /// )
        /// </seealso>
        public virtual ProcessDefinitionEntity FindLatestProcessDefinitionByKey(string processDefinitionKey)
        {
            //IList<ProcessDefinitionEntity> processDefinitions =ListExt.ConvertToListT< ProcessDefinitionEntity>(DbEntityManager.SelectList("selectLatestProcessDefinitionByKey", ConfigureParameterizedQuery(processDefinitionKey)));
            var lastProcessDefinitionVersion = Find(m => m.Key == processDefinitionKey)
                .OrderByDescending(m => m.Version)
                .FirstOrDefault();
            if (lastProcessDefinitionVersion == null) return null;
            IList<ProcessDefinitionEntity> processDefinitions =
                Find(m => m.Key == processDefinitionKey && m.Version == lastProcessDefinitionVersion.Version)
                    .ToList();
            if (processDefinitions.Count == 0)
                return null;
            if (processDefinitions.Count >= 1)
                return processDefinitions[0];
            throw Log.MultipleTenantsForProcessDefinitionKeyException(processDefinitionKey);
        }

        /// <returns>
        ///     the latest version of the process definition with the given key and tenant id
        /// </returns>
        /// <seealso cref= # findLatestProcessDefinitionByKeyAndTenantId( String, String
        /// )
        /// </seealso>
        public virtual ProcessDefinitionEntity FindLatestProcessDefinitionByKeyAndTenantId(string processDefinitionKey,
            string tenantId)
        {
            //IDictionary<string, string> parameters = new Dictionary<string, string>();
            //parameters["processDefinitionKey"] = processDefinitionKey;
            //parameters["tenantId"] = tenantId;

            //paramenters=[{key:"processDefinitionKey",value:"Process_1"}],[{key:"tenandId",value:null}]
            //return (ProcessDefinitionEntity)DbEntityManager.SelectOne("selectLatestProcessDefinitionByKeyWithoutTenantId", parameters);

            return Find(m => m.Key == processDefinitionKey && m.TenantId == null)
                .OrderByDescending(m => m.Version)
                .FirstOrDefault();
        }

        public virtual ProcessDefinitionEntity FindLatestProcessDefinitionById(string processDefinitionId)
        {
            //return Get<ProcessDefinitionEntity>(typeof(ProcessDefinitionEntity), processDefinitionId);
            return Get(processDefinitionId);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings({ "unchecked" }) public java.Util.List<org.camunda.bpm.engine.repository.ProcessDefinition> findProcessDefinitionsByQueryCriteria(org.camunda.bpm.engine.impl.ProcessDefinitionQueryImpl processDefinitionQuery, org.camunda.bpm.engine.impl.Page page)
        //public virtual IList<IProcessDefinition> FindProcessDefinitionsByQueryCriteria(ProcessDefinitionQueryImpl processDefinitionQuery, Page page)
        //{
        //    throw new NotImplementedException();

        //    ConfigureProcessDefinitionQuery(processDefinitionQuery);
        //    //return ListExt.ConvertToListT<IProcessDefinition>(DbEntityManager.SelectList("selectProcessDefinitionsByQueryCriteria", processDefinitionQuery, page));
        //    //return DbEntityManager.SelectPaged(page.FirstResult,page.MaxResults,)
        //}

        //public virtual long FindProcessDefinitionCountByQueryCriteria(ProcessDefinitionQueryImpl processDefinitionQuery)
        //{
        //    throw new NotImplementedException();
        //    ConfigureProcessDefinitionQuery(processDefinitionQuery);
        //    //return (long) DbEntityManager.SelectOne("selectProcessDefinitionCountByQueryCriteria", processDefinitionQuery);
        //}

        public virtual ProcessDefinitionEntity FindProcessDefinitionByDeploymentAndKey(string deploymentId,
            string processDefinitionKey)
        {
            //IDictionary<string, object> parameters = new Dictionary<string, object>();
            //parameters["deploymentId"] = deploymentId;
            //parameters["processDefinitionKey"] = processDefinitionKey;
            //return (ProcessDefinitionEntity) DbEntityManager.SelectOne("selectProcessDefinitionByDeploymentAndKey", parameters);
            return Single(m => m.DeploymentId == deploymentId && m.Key == processDefinitionKey);
        }

        public virtual ProcessDefinitionEntity FindProcessDefinitionByKeyVersionAndTenantId(string processDefinitionKey,
            int? processDefinitionVersion, string tenantId)
        {
            //IDictionary<string, object> parameters = new Dictionary<string, object>();
            //parameters["processDefinitionVersion"] = processDefinitionVersion;
            //parameters["processDefinitionKey"] = processDefinitionKey;
            //parameters["tenantId"] = tenantId;
            //IList<ProcessDefinitionEntity> results = ListExt.ConvertToListT<ProcessDefinitionEntity>(DbEntityManager.SelectList("selectProcessDefinitionByKeyVersionAndTenantId", parameters));
            IList<ProcessDefinitionEntity> results =
                Find(
                        m =>
                            m.Key == processDefinitionKey && m.Version == (int)processDefinitionVersion &&
                            m.TenantId == tenantId)
                    .ToList();

            if (results.Count == 1)
                return results[0];
            if (results.Count > 1)
                throw Log.ToManyProcessDefinitionsException(results.Count, processDefinitionKey,
                    processDefinitionVersion, tenantId);
            return null;
        }

        //public virtual IList<IProcessDefinition> FindProcessDefinitionsByKey(string processDefinitionKey)
        //{
        //    ProcessDefinitionQueryImpl processDefinitionQuery = (new ProcessDefinitionQueryImpl()).processDefinitionKey(processDefinitionKey);
        //    return FindProcessDefinitionsByQueryCriteria(processDefinitionQuery, null);
        //}

        //public virtual IList<IProcessDefinition> FindProcessDefinitionsStartableByUser(string user)
        //{
        //    return (IList<IProcessDefinition>)(new ProcessDefinitionQueryImpl()).startableByUser(user).List();
        //}

        public virtual string FindPreviousProcessDefinitionId(string processDefinitionKey, int? version, string tenantId)
        {
            //IDictionary<string, object> @params = new Dictionary<string, object>();
            //@params["key"] = processDefinitionKey;
            //@params["version"] = version;
            //@params["tenantId"] = tenantId;
            //return (string) DbEntityManager.SelectOne("selectPreviousProcessDefinitionId", @params);
            return Single(m => m.Key == processDefinitionKey && m.Version == version && m.TenantId == tenantId)
                .Id;
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") public java.Util.List<org.camunda.bpm.engine.repository.ProcessDefinition> findProcessDefinitionsByDeploymentId(String deploymentId)
        public virtual IList<ProcessDefinitionEntity> FindProcessDefinitionsByDeploymentId(string deploymentId)
        {
            //return ListExt.ConvertToListT<IProcessDefinition>(DbEntityManager.SelectList("selectProcessDefinitionByDeploymentId", deploymentId));
            return Find(m => m.DeploymentId == deploymentId)
                .ToList();
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") public java.Util.List<org.camunda.bpm.engine.repository.ProcessDefinition> findProcessDefinitionsByKeyIn(String... keys)
        public virtual IList<IProcessDefinition> FindProcessDefinitionsByKeyIn(params string[] keys)
        {
            //return ListExt.ConvertToListT<IProcessDefinition>(DbEntityManager.SelectList("selectProcessDefinitionByKeyIn", keys));
            return Find(m => keys.Contains(m.Key))
                .ToList() as IList<IProcessDefinition>;
        }

        // update ///////////////////////////////////////////////////////////

        public virtual void UpdateProcessDefinitionSuspensionStateById(string processDefinitionId,
            ISuspensionState suspensionState)
        {
            #region mybatis-sql-mapping

              

                #endregion


            //throw new NotImplementedException();
            //IDictionary<string, object> parameters = new Dictionary<string, object>();
            //parameters["processDefinitionId"] = processDefinitionId;
            //parameters["suspensionState"] = suspensionState.StateCode;
            //DbEntityManager.Update(typeof(ProcessDefinitionEntity), "updateProcessDefinitionSuspensionStateByParameters", ConfigureParameterizedQuery(parameters));
            ProcessDefinitionEntity data = Get(processDefinitionId);
            data.SuspensionState = suspensionState.StateCode;
            data.Revision++;
            Update(data);
            DbContext.SaveChanges();            
        }

        public virtual void UpdateProcessDefinitionSuspensionStateByKey(string processDefinitionKey,
            ISuspensionState suspensionState)
        {
            throw new NotImplementedException();
            IDictionary<string, object> parameters = new Dictionary<string, object>();
            parameters["processDefinitionKey"] = processDefinitionKey;
            parameters["isTenantIdSet"] = false;
            parameters["suspensionState"] = suspensionState.StateCode;
            //DbEntityManager.Update(typeof(ProcessDefinitionEntity), "updateProcessDefinitionSuspensionStateByParameters", ConfigureParameterizedQuery(parameters));
        }

        public virtual void UpdateProcessDefinitionSuspensionStateByKeyAndTenantId(string processDefinitionKey,
            string tenantId, ISuspensionState suspensionState)
        {
            throw new NotImplementedException();
            IDictionary<string, object> parameters = new Dictionary<string, object>();
            parameters["processDefinitionKey"] = processDefinitionKey;
            parameters["isTenantIdSet"] = true;
            parameters["tenantId"] = tenantId;
            parameters["suspensionState"] = suspensionState.StateCode;
            //DbEntityManager.Update(typeof(ProcessDefinitionEntity), "updateProcessDefinitionSuspensionStateByParameters", ConfigureParameterizedQuery(parameters));
        }

        /// <summary>
        ///     Deletes the subscriptions for the process definition, which is
        ///     identified by the given process definition id.
        /// </summary>
        /// <param name="processDefinitionId"> the id of the process definition </param>
        public virtual void DeleteSubscriptionsForProcessDefinition(string processDefinitionId)
        {
            //throw new NotImplementedException();
            IList<EventSubscriptionEntity> eventSubscriptionsToRemove = new List<EventSubscriptionEntity>();
            // remove message event subscriptions:
            IList<EventSubscriptionEntity> messageEventSubscriptions = EventSubscriptionManager.FindEventSubscriptionsByConfiguration(EventType.Message.Name, processDefinitionId);
            foreach (var item in messageEventSubscriptions)
            {
                eventSubscriptionsToRemove.Add(item);
            }

            //// remove signal event subscriptions:
            IList<EventSubscriptionEntity> signalEventSubscriptions = EventSubscriptionManager.FindEventSubscriptionsByConfiguration(EventType.Signal.Name, processDefinitionId);
            //eventSubscriptionsToRemove.AddRange(signalEventSubscriptions);
            foreach (var item in signalEventSubscriptions)
            {
                eventSubscriptionsToRemove.Add(item);
            }

            foreach (EventSubscriptionEntity eventSubscriptionEntity in eventSubscriptionsToRemove)
            {
                eventSubscriptionEntity.Delete();
            }
        }

        /// <summary>
        /// Deletes the timer start events for the given process definition.
        /// </summary>
        /// <param name="processDefinition"> the process definition </param>
        protected internal virtual void DeleteTimerStartEventsForProcessDefinition(IProcessDefinition processDefinition)
        {
            //throw new NotImplementedException();
            IList<JobEntity> timerStartJobs = _jobManager.FindJobsByConfiguration(TimerStartEventJobHandler.TYPE, processDefinition.Key, processDefinition.TenantId);

            ProcessDefinitionEntity latestVersion = this.FindLatestProcessDefinitionByKeyAndTenantId(processDefinition.Key, processDefinition.TenantId);

            //// delete timer start event jobs only if this is the latest version of the process definition.
            if (latestVersion != null && latestVersion.Id.Equals(processDefinition.Id))
            {
                foreach (IJob job in timerStartJobs)
                {
                    ((JobEntity)job).Delete();
                }
            }
        }

        protected internal virtual IEventSubscriptionManager EventSubscriptionManager
        {
            get
            {
                return CommandContext.EventSubscriptionManager;
            }
        }

        /// <summary>
        ///     Deletes the given process definition from the database and cache.
        ///     If cascadeToHistory and cascadeToInstances is set to true it deletes
        ///     the history and the process instances.
        ///     *Note*: If more than one process definition, from one deployment, is deleted in
        ///     a single transaction and the cascadeToHistory and cascadeToInstances flag was set to true it
        ///     can cause a dirty deployment cache. The process instances of ALL process definitions must be deleted,
        ///     before every process definition can be deleted! In such cases the cascadeToInstances flag
        ///     have to set to false!
        ///     On deletion of all process instances, the task listeners will be deleted as well.
        ///     Deletion of tasks and listeners needs the redeployment of deployments.
        ///     It can cause to problems if is done sequential with the deletion of process definition
        ///     in a single transaction.
        ///     *For example*:
        ///     Deployment contains two process definition. First process definition
        ///     and instances will be removed, also cleared from the cache.
        ///     Second process definition will be removed and his instances.
        ///     Deletion of instances will cause redeployment this deploys again
        ///     first into the cache. Only the second will be removed from cache and
        ///     first remains in the cache after the deletion process.
        /// </summary>
        /// <param name="processDefinition"> the process definition which should be deleted </param>
        /// <param name="processDefinitionId"> the id of the process definition </param>
        /// <param name="cascadeToHistory"> if true the history will deleted as well </param>
        /// <param name="cascadeToInstances"> if true the process instances are deleted as well </param>
        /// <param name="skipCustomListeners"> if true skips the custom listeners on deletion of instances </param>
        public virtual void DeleteProcessDefinition(IProcessDefinition processDefinition, string processDefinitionId,
            bool cascadeToHistory, bool cascadeToInstances, bool skipCustomListeners)
        {
            if (cascadeToHistory)
            {
                CascadeDeleteHistoryForProcessDefinition(processDefinitionId);
                if (cascadeToInstances)
                    CascadeDeleteProcessInstancesForProcessDefinition(processDefinitionId, skipCustomListeners);
            }
            else
            {
                var processInstanceCount =
                    _execuutionManager.FindProcessInstanceCountByProcessDefinitionId(processDefinitionId);
                if (processInstanceCount != 0)
                    throw Log.DeleteProcessDefinitionWithProcessInstancesException(processDefinitionId,
                        processInstanceCount);
            }

            // remove related authorization parameters in IdentityLink table
            _identityLinkManager.DeleteIdentityLinksByProcDef(processDefinitionId);

            // remove timer start events:
            DeleteTimerStartEventsForProcessDefinition(processDefinition);

            //delete process definition from database
            Delete(processDefinitionId);
            // remove process definition from cache:
            Context.ProcessEngineConfiguration.DeploymentCache.RemoveProcessDefinition(processDefinitionId);

            DeleteSubscriptionsForProcessDefinition(processDefinitionId);

            // delete job definitions
            _jobDefinitionManager.DeleteJobDefinitionsByProcessDefinitionId(processDefinition.Id);
        }

        // delete  ///////////////////////////////////////////////////////////

        /// <summary>
        ///     Cascades the deletion of the process definition to the process instances.
        ///     Skips the custom listeners if the flag was set to true.
        /// </summary>
        /// <param name="processDefinitionId"> the process definition id </param>
        /// <param name="skipCustomListeners"> true if the custom listeners should be skipped at process instance deletion </param>
        protected internal virtual void CascadeDeleteProcessInstancesForProcessDefinition(string processDefinitionId,
            bool skipCustomListeners)
        {
            throw new NotImplementedException();
            //ProcessInstanceManager.DeleteProcessInstancesByProcessDefinition(processDefinitionId, "deleted process definition", true, skipCustomListeners);
        }

        /// <summary>
        ///     Cascades the deletion of a process definition to the history, deletes the history.
        /// </summary>
        /// <param name="processDefinitionId"> the process definition id </param>
        protected internal virtual void CascadeDeleteHistoryForProcessDefinition(string processDefinitionId)
        {
            // remove historic incidents which are not referenced to a process instance
            _historicIncidentManager.DeleteHistoricIncidentsByProcessDefinitionId(processDefinitionId);

            //// remove historic identity links which are not reference to a process instance
            _historicIdentityLinkLogManager.DeleteHistoricIdentityLinksLogByProcessDefinitionId(processDefinitionId);

            //// remove historic job log entries not related to a process instance
            _historicJobLogManager.DeleteHistoricJobLogsByProcessDefinitionId(processDefinitionId);
        }



        // helper ///////////////////////////////////////////////////////////

        protected internal virtual void CreateDefaultAuthorizations(IProcessDefinition processDefinition)
        {
            if (AuthorizationEnabled)
            {
                var provider = ResourceAuthorizationProvider;
                var authorizations = provider.NewProcessDefinition(processDefinition);
                SaveDefaultAuthorizations(authorizations);
            }
        }

        //protected internal virtual void ConfigureProcessDefinitionQuery(ProcessDefinitionQueryImpl query)
        //{
        //    AuthorizationManager.ConfigureProcessDefinitionQuery(query);
        //    TenantManager.ConfigureQuery(query);
        //}

        //protected internal virtual ListQueryParameterObject ConfigureParameterizedQuery(object parameter)
        //{
        //    throw new NotImplementedException();
        //    //return TenantManager.ConfigureQuery(parameter);
        //}
    }
}
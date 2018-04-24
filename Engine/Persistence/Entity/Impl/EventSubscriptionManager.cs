using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.DB;
using ESS.FW.Bpm.Engine.Impl.Event;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using ESS.FW.Bpm.Engine.Impl.Util;
using Microsoft.Extensions.Logging;
using ESS.FW.DataAccess;
using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Autofac.Features.AttributeFilters;

using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Common.Components;

namespace ESS.FW.Bpm.Engine.Persistence.Entity.Impl
{

    /// <summary>
    /// 
    /// </summary>
    [Component]
    public class EventSubscriptionManager : AbstractManagerNet<EventSubscriptionEntity>, IEventSubscriptionManager
    {

        protected internal static readonly EnginePersistenceLogger Log = ProcessEngineLogger.PersistenceLogger;

        /// <summary>
        /// keep track of subscriptions created in the current command </summary>
        protected internal IList<EventSubscriptionEntity> CreatedSignalSubscriptions = new List<EventSubscriptionEntity>();
        private readonly IJobManager _jobManager;

        private readonly IExecutionManager _executionManager;

        private readonly IRepository<ExecutionEntity, string> _repositoryExecutionEntity;

        public EventSubscriptionManager(DbContext dbContex,
            ILoggerFactory loggerFactory,
            IJobManager jobManager,
            IDGenerator idGenerator,
            IExecutionManager executionManager,
            IRepository<ExecutionEntity, string> repositoryExecutionEntity) : base(dbContex, loggerFactory, idGenerator)
        {
            _jobManager = jobManager;
            _executionManager = executionManager;

            _repositoryExecutionEntity = repositoryExecutionEntity;
        }

        public void Insert(EventSubscriptionEntity persistentObject)
        {
            Add(persistentObject);
            if (persistentObject.IsSubscriptionForEventType(EventType.Signal))
            {
                CreatedSignalSubscriptions.Add(persistentObject);
            }
        }

        public virtual void DeleteEventSubscription(EventSubscriptionEntity persistentObject)
        {
            Log.LogDebug("EFÉ¾³ý»º´æEventSubscriptionEntity,", "id:" + persistentObject.Id);
            Delete(persistentObject);
            if (persistentObject.IsSubscriptionForEventType(EventType.Signal))
            {
                CreatedSignalSubscriptions.Remove(persistentObject);
            }

            // if the event subscription has been triggered asynchronously but not yet executed
            IList<JobEntity> asyncJobs = _jobManager.FindJobsByConfiguration(ProcessEventJobHandler.TYPE, persistentObject.Id, persistentObject.TenantId);

            foreach (JobEntity asyncJob in asyncJobs)
            {
                asyncJob.Delete();
            }
        }

        public virtual void DeleteAndFlushEventSubscription(EventSubscriptionEntity persistentObject)
        {
            DeleteEventSubscription(persistentObject);
            throw new System.NotImplementedException();
            //DbEntityManager.FlushEntity(persistentObject);
        }

        public virtual EventSubscriptionEntity FindEventSubscriptionById(string id)
        {
            //return (EventSubscriptionEntity)DbEntityManager.SelectOne("selectEventSubscription", id);
            return First(c => c.Id == id);
        }

        //public virtual long FindEventSubscriptionCountByQueryCriteria(EventSubscriptionQueryImpl eventSubscriptionQueryImpl)
        //{
        //    ConfigureQuery(eventSubscriptionQueryImpl);
        //    throw new System.NotImplementedException();
        //    //return (long) DbEntityManager.SelectOne("selectEventSubscriptionCountByQueryCriteria", eventSubscriptionQueryImpl);
        //}

        ////JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        ////ORIGINAL LINE: @SuppressWarnings("unchecked") public java.Util.List<org.camunda.bpm.engine.runtime.EventSubscription> findEventSubscriptionsByQueryCriteria(org.camunda.bpm.engine.impl.EventSubscriptionQueryImpl eventSubscriptionQueryImpl, org.camunda.bpm.engine.impl.Page page)
        //public virtual IList<IEventSubscription> FindEventSubscriptionsByQueryCriteria(EventSubscriptionQueryImpl eventSubscriptionQueryImpl, Page page)
        //{
        //    ConfigureQuery(eventSubscriptionQueryImpl);
        //    throw new System.NotImplementedException();
        //    //return ListExt.ConvertToListT<IEventSubscription>(DbEntityManager.SelectList("selectEventSubscriptionByQueryCriteria", eventSubscriptionQueryImpl, page));
        //}

        /// <summary>
        /// Find all signal event subscriptions with the given event name for any tenant.
        /// </summary>
        /// <seealso cref= #findSignalEventSubscriptionsByEventNameAndTenantId(String, String) </seealso>
        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") public java.Util.List<EventSubscriptionEntity> findSignalEventSubscriptionsByEventName(String eventName)
        public virtual IList<EventSubscriptionEntity> FindSignalEventSubscriptionsByEventName(string eventName)
        {
            //const string query = "selectSignalEventSubscriptionsByEventName";

            //ISet<EventSubscriptionEntity> eventSubscriptions = new HashSet<EventSubscriptionEntity>(ListExt.ConvertToListT<EventSubscriptionEntity>(DbEntityManager.SelectList(query, ConfigureParameterizedQuery(eventName))));

            //// add events created in this command (not visible yet in query)
            //foreach (EventSubscriptionEntity entity in CreatedSignalSubscriptions)
            //{
            //    if (eventName.Equals(entity.EventName))
            //    {
            //        eventSubscriptions.Add(entity);
            //    }
            //}
            //return new List<EventSubscriptionEntity>(eventSubscriptions);

            //IList< EventSubscriptionEntity> eventSubscriptions =( from a in DbContext.Set<EventSubscriptionEntity>()
            //            join b in DbContext.Set<ExecutionEntity>()
            //            on a.ExecutionId equals b.Id
            //            where (a.ExecutionId == null || b.SuspensionState == 1)&&
            //                    a.EventType== "signal" &&a.EventName==eventName
            //            select a).ToList();
            var query_0 = from a in DbContext.Set<EventSubscriptionEntity>()
                          where a.ExecutionId == null && a.EventType == "signal" && a.EventName == eventName
                          select a;
            var query_1 = from a in DbContext.Set<EventSubscriptionEntity>()
                          join b in DbContext.Set<ExecutionEntity>() on a.ExecutionId equals b.Id
                          where b.SuspensionState == 1
                          select a;

            List<EventSubscriptionEntity> eventSubscriptions = query_0.ToList();
            eventSubscriptions.AddRange(query_1.ToList());
            foreach (EventSubscriptionEntity entity in CreatedSignalSubscriptions)
            {
                if (eventName == entity.EventName)
                {
                    eventSubscriptions.Add(entity);
                }
            }
            return eventSubscriptions;
        }

        /// <summary>
        /// Find all signal event subscriptions with the given event name and tenant.
        /// </summary>
        public virtual IList<EventSubscriptionEntity> FindSignalEventSubscriptionsByEventNameAndTenantId(string eventName, string tenantId)
        {
            #region java source

            //final String query = "selectSignalEventSubscriptionsByEventNameAndTenantId";

            //Map<String, Object> parameter = new HashMap<String, Object>();
            //parameter.put("eventName", eventName);
            //parameter.put("tenantId", tenantId);
            //Set<EventSubscriptionEntity> eventSubscriptions = new HashSet<EventSubscriptionEntity>(getDbEntityManager().selectList(query, parameter));

            //// add events created in this command (not visible yet in query)
            //for (EventSubscriptionEntity entity : createdSignalSubscriptions)
            //{
            //    if (eventName.equals(entity.getEventName()) && hasTenantId(entity, tenantId))
            //    {
            //        eventSubscriptions.add(entity);
            //    }
            //}
            //return new ArrayList<EventSubscriptionEntity>(eventSubscriptions);


            //<select id = "selectSignalEventSubscriptionsByEventNameAndTenantId" resultMap = "eventSubscriptionResultMap" parameterType = "org.camunda.bpm.engine.impl.db.ListQueryParameterObject" >
            //     select EVT.*
            //     from ${ prefix}ACT_RU_EVENT_SUBSCR EVT
            //     left join ${ prefix}ACT_RU_EXECUTION EXC on EVT.EXECUTION_ID_ = EXC.ID_
            //     where(EVENT_TYPE_ = 'signal')
            //     and(EVENT_NAME_ = #{parameter.eventName})
            //     and(EVT.EXECUTION_ID_ is null or EXC.SUSPENSION_STATE_ = 1)
            //     <if test = "parameter.tenantId != null" >
            //        and EVT.TENANT_ID_ = #{parameter.tenantId}
            //     </if>
            //     <if test = "parameter.tenantId == null" >
            //        and EVT.TENANT_ID_ is null
            //     </if>
            //</select >

            //< resultMap id = "eventSubscriptionResultMap" type = "org.camunda.bpm.engine.impl.persistence.entity.EventSubscriptionEntity" >   
            //    < id property = "id" column = "ID_" jdbcType = "VARCHAR" />
            //    < result property = "revision" column = "REV_" jdbcType = "INTEGER" />
            //    < result property = "eventType" column = "EVENT_TYPE_" jdbcType = "VARCHAR" />
            //    < result property = "eventName" column = "EVENT_NAME_" jdbcType = "VARCHAR" />
            //    < result property = "executionId" column = "EXECUTION_ID_" jdbcType = "VARCHAR" />
            //    < result property = "processInstanceId" column = "PROC_INST_ID_" jdbcType = "VARCHAR" />
            //    < result property = "activityId" column = "ACTIVITY_ID_" jdbcType = "VARCHAR" />
            //    < result property = "configuration" column = "CONFIGURATION_" jdbcType = "VARCHAR" />
            //    < result property = "created" column = "CREATED_" jdbcType = "TIMESTAMP" />
            //    < result property = "tenantId" column = "TENANT_ID_" jdbcType = "VARCHAR" />
            //</ resultMap >

            #endregion

            var datas = (from evt in Find(c => c.EventType.Equals("signal"))
                         join exc in _repositoryExecutionEntity.GetAll()
                         on evt.ExecutionId equals exc.Id
                         into tmp
                         from result in tmp.DefaultIfEmpty()
                         where evt.EventName == eventName && string.IsNullOrEmpty(tenantId) ? evt.TenantId == null : evt.TenantId == tenantId
                         select new
                         {
                             Id = evt.Id,
                             Revision = evt.Revision,
                             EventType = evt.EventType,
                             EventName = evt.EventName,
                             ExecutionId = evt.ExecutionId,
                             ProcessInstanceId = evt.ProcessInstanceId,
                             ActivityId = evt.ActivityId,
                             Configuration = evt.Configuration,
                             Created = evt.Created,
                             TenantId = evt.TenantId
                         })
                         .ToList()
                         .Select(d => new EventSubscriptionEntity
                         {
                             Id = d.Id,
                             Revision = d.Revision,
                             EventType = d.EventType,
                             EventName = d.EventName,
                             ExecutionId = d.ExecutionId,
                             ProcessInstanceId = d.ProcessInstanceId,
                             ActivityId = d.ActivityId,
                             Configuration = d.Configuration,
                             Created = d.Created,
                             TenantId = d.TenantId
                         }).ToList();

            foreach (EventSubscriptionEntity entity in CreatedSignalSubscriptions)
            {
                if (eventName.Equals(entity.EventName) && HasTenantId(entity, tenantId))
                {
                    datas.Add(entity);
                }
            }

            return new List<EventSubscriptionEntity>(datas);
        }

        /// <summary>
        /// Find all signal event subscriptions with the given event name which belongs to the given tenant or no tenant.
        /// </summary>
        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") public java.Util.List<EventSubscriptionEntity> findSignalEventSubscriptionsByEventNameAndTenantIdIncludeWithoutTenantId(String eventName, String tenantId)
        public virtual IList<EventSubscriptionEntity> FindSignalEventSubscriptionsByEventNameAndTenantIdIncludeWithoutTenantId(string eventName, string tenantId)
        {
            //const string query = "selectSignalEventSubscriptionsByEventNameAndTenantIdIncludeWithoutTenantId";

            //IDictionary<string, object> parameter = new Dictionary<string, object>();
            //parameter["eventName"] = eventName;
            //parameter["tenantId"] = tenantId;
            //throw new System.NotImplementedException();
            //IList<EventSubscriptionEntity> eventSubscriptions = new HashSet<EventSubscriptionEntity>(ListExt.ConvertToListT<EventSubscriptionEntity>(DbEntityManager.SelectList(query, parameter)));

            throw new NotImplementedException();

            //IList<EventSubscriptionEntity> eventSubscriptions = DbEntityManager.SelectList(m => m.EventName == eventName && m.TenantId == tenantId);
            //// add events created in this command (not visible yet in query)
            //foreach (EventSubscriptionEntity entity in CreatedSignalSubscriptions)
            //{
            //    if (eventName.Equals(entity.EventName) && (entity.TenantId == null || HasTenantId(entity, tenantId)))
            //    {
            //        eventSubscriptions.Add(entity);
            //    }
            //}
            //return eventSubscriptions;
        }

        protected internal virtual bool HasTenantId(EventSubscriptionEntity entity, string tenantId)
        {
            if (tenantId == null)
            {
                return entity.TenantId == null;
            }
            else
            {
                return tenantId.Equals(entity.TenantId);
            }
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") public java.Util.List<EventSubscriptionEntity> findSignalEventSubscriptionsByExecution(String executionId)
        public virtual IList<EventSubscriptionEntity> FindSignalEventSubscriptionsByExecution(string executionId)
        {
            //const string query = "selectSignalEventSubscriptionsByExecution";
            //throw new System.NotImplementedException();
            //ISet<EventSubscriptionEntity> selectList = new HashSet<EventSubscriptionEntity>(ListExt.ConvertToListT<EventSubscriptionEntity>(DbEntityManager.SelectList(query, executionId)));

            return Find(c => c.EventType.Equals("signal") && c.Id.Equals(executionId)).ToList();
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") public java.Util.List<EventSubscriptionEntity> findSignalEventSubscriptionsByNameAndExecution(String name, String executionId)
        public virtual IList<EventSubscriptionEntity> FindSignalEventSubscriptionsByNameAndExecution(string name, string executionId)
        {
            //const string query = "selectSignalEventSubscriptionsByNameAndExecution";
            //IDictionary<string, string> @params = new Dictionary<string, string>();
            //@params["executionId"] = executionId;
            //@params["eventName"] = name;
            //          throw new System.NotImplementedException();
            //ISet<EventSubscriptionEntity> selectList = new HashSet<EventSubscriptionEntity>(ListExt.ConvertToListT<EventSubscriptionEntity>(DbEntityManager.SelectList(query, @params)));   

            return Find(c => c.EventType.Equals("signal") && c.ExecutionId.Equals(executionId) && c.EventName.Equals(name)).ToList();
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") public java.Util.List<EventSubscriptionEntity> findEventSubscriptionsByExecutionAndType(String executionId, String type, boolean lockResult)
        /// <summary>
        /// Î´ÊµÏÖ£ºlockResult
        /// </summary>
        /// <param name="executionId"></param>
        /// <param name="type"></param>
        /// <param name="lockResult"></param>
        /// <returns></returns>
        public virtual IList<EventSubscriptionEntity> FindEventSubscriptionsByExecutionAndType(string executionId, string type, bool lockResult)
        {
            //const string query = "selectEventSubscriptionsByExecutionAndType";
            //IDictionary<string, object> @params = new Dictionary<string, object>();
            //@params["executionId"] = executionId;
            //@params["eventType"] = type;
            //@params["lockResult"] = lockResult;
            //throw new System.NotImplementedException();
            //return ListExt.ConvertToListT<EventSubscriptionEntity>(DbEntityManager.SelectList(query, @params)) ;


            //select*
            //from ${ prefix}ACT_RU_EVENT_SUBSCR
            //where(EVENT_TYPE_ = #{parameter.eventType})
            //and(EXECUTION_ID_ = #{parameter.executionId})
            //<if test = "parameter.lockResult" >
            //  ${ constant_for_update}
            //</if>

            //Todo: ${constant_for_update}
            return Find(c => c.EventType.Equals(type) && c.ExecutionId.Equals(executionId)).ToList();
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") public java.Util.List<EventSubscriptionEntity> findEventSubscriptionsByExecution(String executionId)
        public virtual IList<EventSubscriptionEntity> FindEventSubscriptionsByExecution(string executionId)
        {
            //const string query = "selectEventSubscriptionsByExecution";
            //return ListExt.ConvertToListT<EventSubscriptionEntity>(DbEntityManager.SelectList(query, executionId));
            return Find(m => m.ExecutionId == executionId).ToList();
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") public java.Util.List<EventSubscriptionEntity> findEventSubscriptions(String executionId, String type, String activityId)
        public virtual IList<EventSubscriptionEntity> FindEventSubscriptions(string executionId, string type, string activityId)
        {
            //const string query = "selectEventSubscriptionsByExecutionTypeAndActivity";
            //IDictionary<string, string> @params = new Dictionary<string, string>();
            //@params["executionId"] = executionId;
            //@params["eventType"] = type;
            //@params["activityId"] = activityId;
            //throw new System.NotImplementedException();
            //return ListExt.ConvertToListT<EventSubscriptionEntity>(DbEntityManager.SelectList(query, @params));
            return Find(m => m.ExecutionId == executionId && m.EventType == type && m.ActivityId == activityId).ToList();
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") public java.Util.List<EventSubscriptionEntity> findEventSubscriptionsByConfiguration(String type, String configuration)
        public virtual IList<EventSubscriptionEntity> FindEventSubscriptionsByConfiguration(string type, string configuration)
        {
            //const string query = "selectEventSubscriptionsByConfiguration";
            //IDictionary<string, string> @params = new Dictionary<string, string>();
            //@params["eventType"] = type;
            //@params["configuration"] = configuration;
            //throw new System.NotImplementedException();
            //return ListExt.ConvertToListT<EventSubscriptionEntity>(DbEntityManager.SelectList(query, @params));
            return Find(m => m.EventType == type && m.Configuration == configuration).ToList();
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") public java.Util.List<EventSubscriptionEntity> findEventSubscriptionsByNameAndTenantId(String type, String eventName, String tenantId)
        public virtual IList<EventSubscriptionEntity> FindEventSubscriptionsByNameAndTenantId(string type, string eventName, string tenantId)
        {
            //const string query = "selectEventSubscriptionsByNameAndTenantId";
            //IDictionary<string, string> @params = new Dictionary<string, string>();
            //@params["eventType"] = type;
            //@params["eventName"] = eventName;
            //@params["tenantId"] = tenantId;
            //throw new System.NotImplementedException();
            //return ListExt.ConvertToListT<EventSubscriptionEntity>(DbEntityManager.SelectList(query, @params));
            return Find(m => m.EventType == type && m.EventName == eventName && m.TenantId == tenantId).ToList();
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") public java.Util.List<EventSubscriptionEntity> findEventSubscriptionsByNameAndExecution(String type, String eventName, String executionId, boolean lockResult)
        public virtual IList<EventSubscriptionEntity> FindEventSubscriptionsByNameAndExecution(string type, string eventName, string executionId, bool lockResult)
        {

            // first check cache in case entity is already loaded
            //ExecutionEntity cachedExecution = DbEntityManager.GetCachedEntity<ExecutionEntity>(typeof(ExecutionEntity), executionId);
            ExecutionEntity cachedExecution = _executionManager.FindExecutionById(executionId);
            if (cachedExecution != null && !lockResult)
            {
                IList<EventSubscriptionEntity> eventSubscriptions = cachedExecution.EventSubscriptions;
                IList<EventSubscriptionEntity> result = new List<EventSubscriptionEntity>();
                foreach (EventSubscriptionEntity subscription in eventSubscriptions)
                {
                    if (MatchesSubscription(subscription, type, eventName))
                    {
                        result.Add(subscription);
                    }
                }
                return result;
            }
            else
            {
                //const string query = "selectEventSubscriptionsByNameAndExecution";
                //IDictionary<string, object> @params = new Dictionary<string, object>();
                //@params["eventType"] = type;
                //@params["eventName"] = eventName;
                //@params["executionId"] = executionId;
                //@params["lockResult"] = lockResult;
                //return ListExt.ConvertToListT<EventSubscriptionEntity>(DbEntityManager.SelectList(query, @params));

                //select*
                //from ${ prefix}ACT_RU_EVENT_SUBSCR
                //where(EVENT_TYPE_ = #{parameter.eventType})
                //and(EVENT_NAME_ = #{parameter.eventName})
                //and(EXECUTION_ID_ = #{parameter.executionId})
                //<if test = "parameter.lockResult" >
                //${ constant_for_update}
                //</if>

                //Todo: ${constant_for_update}
                return Find(c => c.EventType.Equals(type) && c.EventName.Equals(eventName) && c.ExecutionId.Equals(executionId)).ToList();
            }
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") public java.Util.List<EventSubscriptionEntity> findEventSubscriptionsByProcessInstanceId(String processInstanceId)
        public virtual IList<EventSubscriptionEntity> FindEventSubscriptionsByProcessInstanceId(string processInstanceId)
        {
            //throw new System.NotImplementedException();
            //return ListExt.ConvertToListT<EventSubscriptionEntity>(DbEntityManager.SelectList("selectEventSubscriptionsByProcessInstanceId", processInstanceId));
            return Find(m => m.ProcessInstanceId == processInstanceId).ToList();
        }

        /// <returns> the message start event subscriptions with the given message name (from any tenant)
        /// </returns>
        /// <seealso cref= #findMessageStartEventSubscriptionByNameAndTenantId(String, String) </seealso>
        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") public java.Util.List<EventSubscriptionEntity> findMessageStartEventSubscriptionByName(String messageName)
        public virtual IList<EventSubscriptionEntity> FindMessageStartEventSubscriptionByName(string messageName)
        {
            //return ListExt.ConvertToListT<EventSubscriptionEntity>(DbEntityManager.SelectList("selectMessageStartEventSubscriptionByName", ConfigureParameterizedQuery(messageName)));
            //queryTenantCheck
            return Find(m => m.EventType == "message" && m.EventName == messageName && m.ExecutionId == null).ToList();
        }

        /// <returns> the message start event subscription with the given message name and tenant id
        /// </returns>
        /// <seealso cref= #findMessageStartEventSubscriptionByName(String) </seealso>
        public virtual EventSubscriptionEntity FindMessageStartEventSubscriptionByNameAndTenantId(string messageName, string tenantId)
        {
            //IDictionary<string, string> parameters = new Dictionary<string, string>();
            //parameters["messageName"] = messageName;
            //parameters["tenantId"] = tenantId;
            //return (EventSubscriptionEntity) DbEntityManager.SelectOne("selectMessageStartEventSubscriptionByNameAndTenantId", parameters);

            return
                First(
                    c =>
                        c.EventType.Equals("message") && c.EventName.Equals(messageName) &&
                        string.IsNullOrEmpty(c.ExecutionId) &&
                        (string.IsNullOrEmpty(tenantId) ? c.TenantId == null : c.TenantId.Equals(tenantId)));
        }

        //protected internal virtual void ConfigureQuery(EventSubscriptionQueryImpl query)
        //{
        //    AuthorizationManager.ConfigureEventSubscriptionQuery(query);
        //    TenantManager.ConfigureQuery(query);
        //}

        //protected internal virtual ListQueryParameterObject ConfigureParameterizedQuery(object parameter)
        //{
        //    return TenantManager.ConfigureQuery(parameter);
        //}

        protected internal virtual bool MatchesSubscription(EventSubscriptionEntity subscription, string type, string eventName)
        {
            EnsureUtil.EnsureNotNull("event type", type);
            string subscriptionEventName = subscription.EventName;

            return type.Equals(subscription.EventType) && ((eventName == null && subscriptionEventName == null) || (eventName != null && eventName.Equals(subscriptionEventName)));
        }

    }

}
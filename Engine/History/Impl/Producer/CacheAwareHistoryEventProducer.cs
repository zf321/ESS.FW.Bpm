using System;
using System.Linq;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.Bpm.Engine.History.Impl.Handler;
using ESS.FW.Bpm.Engine.Impl.DB.EntityManager;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Runtime;

namespace ESS.FW.Bpm.Engine.History.Impl.Producer
{

    /// <summary>
    ///     <para>
    ///         This HistoryEventProducer is aware of the <seealso cref="DbEntityManager" /> cache
    ///         and works in combination with the <seealso cref="DbHistoryEventHandler" />.
    ///     </para>
    ///     
    /// </summary>
    public class CacheAwareHistoryEventProducer: DefaultHistoryEventProducer, IHistoryEventProducer //
    {
        protected internal override HistoricActivityInstanceEventEntity LoadActivityInstanceEventEntity(
            ExecutionEntity execution)
        {
            var activityInstanceId = execution.ActivityInstanceId;

            HistoricActivityInstanceEventEntity cachedEntity = FindInCache< HistoricActivityInstanceEventEntity>(typeof(HistoricActivityInstanceEventEntity), activityInstanceId);

            if (cachedEntity != null)
            {
                return cachedEntity;

            }
            else
            {
                return NewActivityInstanceEventEntity(execution);

            }
        }

        protected internal override HistoricProcessInstanceEventEntity LoadProcessInstanceEventEntity(
            ExecutionEntity execution)
        {
            var processInstanceId = execution.ProcessInstanceId;

            HistoricProcessInstanceEventEntity cachedEntity = FindInCache< HistoricProcessInstanceEventEntity>(typeof(HistoricProcessInstanceEventEntity), processInstanceId);

            if (cachedEntity != null)
            {
                return cachedEntity;
            }
            else
            {
                return NewProcessInstanceEventEntity(execution);
            }
        }

        protected internal override HistoricTaskInstanceEventEntity LoadTaskInstanceEvent(IDelegateTask task)
        {
            var taskId = task.Id;

            HistoricTaskInstanceEventEntity cachedEntity = FindInCache< HistoricTaskInstanceEventEntity>(typeof(HistoricTaskInstanceEventEntity), taskId);

            if (cachedEntity != null)
            {
                return cachedEntity;

            }
            else
            {
                return NewTaskInstanceEventEntity(task);

            }
        }

        protected internal override HistoricIncidentEntity LoadIncidentEvent(IIncident incident)
        {
            var incidentId = incident.Id;

            var cachedEntity =
                FindInCache<HistoricIncidentEntity>(typeof(HistoricIncidentEntity), incidentId);

            if (cachedEntity != null)
                return cachedEntity;

            return NewIncidentEventEntity(incident);
        }


        /// <summary>
        ///     find a cached entity by primary key
        /// </summary>
        protected internal virtual T FindInCache<T>(Type type, string id) where T : HistoryEvent,new()
        {            
            return Context.CommandContext.DbContext.Set<T>().SingleOrDefault(t => t.Id == id);

            //TODO 只取缓存 存在bug？
            //return Context.CommandContext.GetDbEntityManager<T>().Get(id);

            //return Context.CommandContext.DbEntityCache.GetCachedEntity<T>(id);//.GetCachedEntity(type, id) as T;

            //return Context.CommandContext.DbEntityCache.GetCachedEntity<T>(id);
            //return Context.CommandContext.DbEntityManager.GetCachedEntity<T>(type, id);
        }
    }
}
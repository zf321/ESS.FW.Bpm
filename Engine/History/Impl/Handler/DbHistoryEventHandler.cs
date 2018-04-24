using System;
using System.Collections.Generic;

using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.Bpm.Engine.Impl.DB.EntityManager;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.DataAccess;
using ESS.FW.Bpm.Engine.Common.Utils;
using ESS.FW.Bpm.Engine.Impl.DB;
using ESS.FW.Bpm.Engine.Batch.Impl.History;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Common;
using Autofac.Features.Metadata;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ESS.FW.Bpm.Engine.History.Impl.Handler
{
    /// <summary>
    ///     <para>
    ///         History event handler that writes history events to the process engine
    ///         database using the DbEntityManager.
    ///     </para>
    /// </summary>
    public class DbHistoryEventHandler : IHistoryEventHandler
    {
        protected UtilsLogger log = UtilsLogger.IoUtilLogger;
        //[Obsolete("删除",true)]
        //protected internal virtual DbEntityManager DbEntityManager
        //{
        //    get { return Context.CommandContext.DbEntityManager; }
        //}
        protected IRepository<HistoricDetailEventEntity, string> historyEventRepository
        {
            get
            {

                var reps = Context.CommandContext.Scope.Resolve<IRepository<HistoricDetailEventEntity, string>>();
                return reps;
            }
        }

        public virtual void HandleEvent(HistoryEvent historyEvent)
        {
            if (historyEvent is HistoricVariableUpdateEventEntity)
                InsertHistoricVariableUpdateEntity((HistoricVariableUpdateEventEntity)historyEvent);
            else if (historyEvent is HistoricDecisionEvaluationEvent)
                InsertHistoricDecisionEvaluationEvent((HistoricDecisionEvaluationEvent)historyEvent);
            else
                InsertOrUpdate(historyEvent);
        }

        public virtual void HandleEvents(IList<HistoryEvent> historyEvents)
        {
            foreach (var historyEvent in historyEvents)
                HandleEvent(historyEvent);
        }

        /// <summary>
        ///     general history event insert behavior
        /// </summary>
        protected internal virtual void InsertOrUpdate(HistoryEvent historyEvent)
        {
            log.LogDebug("InsertOrUpdateEF缓存：", historyEvent.GetType().Name + " id:" + historyEvent.Id, historyEvent);
            if (IsInitialEvent(historyEvent))
            {
                Insert(historyEvent);
            }
            else
            {
                //throw new NotImplementedException();
                //TODO CachedEntity改了原始逻辑
                //if (dbEntityManager.getCachedEntity(historyEvent.getClass(), historyEvent.getId()) == null)
                //if (repository.Get(historyEvent.Id) == null)
                //{
                if (historyEvent is HistoricScopeInstanceEvent)
                {
                    // if this is a scope, get start time from existing event in DB
                    HistoricScopeInstanceEvent existingEvent = Get(historyEvent);
                    if (existingEvent != null)
                    {
                        HistoricScopeInstanceEvent historicScopeInstanceEvent = (HistoricScopeInstanceEvent)historyEvent;
                        historicScopeInstanceEvent.StartTime = existingEvent.StartTime;
                    }
                }
                else
                {
                }

                if (historyEvent.Id == null)
                {
                    //源码如此
                    //dbSqlSession.insert(historyEvent);
                }
                else
                {
                    Update(historyEvent);
                    //throw new NotImplementedException();
                    //dbEntityManager.merge(historyEvent);

                }
            }
        }
        private void Insert(HistoryEvent historyEvent)
        {
            if (historyEvent == null)
            {
                throw new NotImplementedException("historyEvent is null");
            }
            log.LogDebug("(dbHistoryEventHander)InsertEF缓存：", historyEvent.GetType().Name + " id:" + historyEvent.Id, historyEvent);
            Context.CommandContext.DbEntityCache.PutPersistent(historyEvent);
            if (historyEvent is HistoricActivityInstanceEventEntity)
            {
                //Context.CommandContext.GetDbEntityManager<HistoricActivityInstanceEventEntity>().Add(historyEvent as HistoricActivityInstanceEventEntity);
                Context.CommandContext.HistoricActivityInstanceManager.Add((HistoricActivityInstanceEventEntity)historyEvent);
            }
            else if (historyEvent is HistoricBatchEntity)
            {
                //Context.CommandContext.GetDbEntityManager<HistoricBatchEntity>().Add(historyEvent as HistoricBatchEntity);
                Context.CommandContext.HistoricBatchManager.Add((HistoricBatchEntity)historyEvent);
            }
            else if (historyEvent is HistoricCaseActivityInstanceEventEntity)
            {
                //Context.CommandContext.GetDbEntityManager<HistoricCaseActivityInstanceEventEntity>().Add(historyEvent as HistoricCaseActivityInstanceEventEntity);
                Context.CommandContext.HistoricCaseActivityInstanceManager.Add((HistoricCaseActivityInstanceEventEntity)historyEvent);
            }
            else if (historyEvent is HistoricCaseInstanceEventEntity)
            {
                //Context.CommandContext.GetDbEntityManager<HistoricCaseInstanceEventEntity>().Add(historyEvent as HistoricCaseInstanceEventEntity);
                Context.CommandContext.HistoricCaseInstanceManager.Add((HistoricCaseInstanceEventEntity)historyEvent);
            }
            else if (historyEvent is HistoricDecisionInputInstanceEntity)
            {
                //Context.CommandContext.GetDbEntityManager<HistoricDecisionInputInstanceEntity>().Add(historyEvent as HistoricDecisionInputInstanceEntity);
                Context.CommandContext.DbContext.Set<HistoricDecisionInputInstanceEntity>().Add((HistoricDecisionInputInstanceEntity)historyEvent);                
            }
            else if (historyEvent is HistoricDecisionInstanceEntity)
            {
                //Context.CommandContext.GetDbEntityManager<HistoricDecisionInstanceEntity>().Add(historyEvent as HistoricDecisionInstanceEntity);
                Context.CommandContext.HistoricDecisionInstanceManager.Add((HistoricDecisionInstanceEntity)historyEvent);
            }
            else if (historyEvent is HistoricDecisionOutputInstanceEntity)
            {
                //Context.CommandContext.GetDbEntityManager<HistoricDecisionOutputInstanceEntity>().Add(historyEvent as HistoricDecisionOutputInstanceEntity);
                Context.CommandContext.DbContext.Set<HistoricDecisionOutputInstanceEntity>().Add((HistoricDecisionOutputInstanceEntity)historyEvent);                
            }
            else if (historyEvent is HistoricDetailEventEntity)
            {
                //Context.CommandContext.GetDbEntityManager<HistoricDetailEventEntity>().Add(historyEvent as HistoricDetailEventEntity);
                Context.CommandContext.HistoricDetailManager.Add((HistoricDetailEventEntity)historyEvent);
            }
            else if (historyEvent is HistoricExternalTaskLogEntity)
            {
                //Context.CommandContext.GetDbEntityManager<HistoricExternalTaskLogEntity>().Add(historyEvent as HistoricExternalTaskLogEntity);
                Context.CommandContext.HistoricExternalTaskLogManager.Add((HistoricExternalTaskLogEntity)historyEvent);
            }
            else if (historyEvent is HistoricIdentityLinkLogEventEntity)
            {
                //Context.CommandContext.GetDbEntityManager<HistoricIdentityLinkLogEventEntity>().Add(historyEvent as HistoricIdentityLinkLogEventEntity);
                Context.CommandContext.HistoricIdentityLinkManager.Add((HistoricIdentityLinkLogEventEntity)historyEvent);
            }
            else if (historyEvent is HistoricIncidentEntity)
            {
                //Context.CommandContext.GetDbEntityManager<HistoricIncidentEntity>().Add(historyEvent as HistoricIncidentEntity);
                Context.CommandContext.HistoricIncidentManager.Add((HistoricIncidentEntity)historyEvent);
            }
            else if (historyEvent is HistoricJobLogEventEntity)
            {
                //Context.CommandContext.GetDbEntityManager<HistoricJobLogEventEntity>().Add(historyEvent as HistoricJobLogEventEntity);
                Context.CommandContext.HistoricJobLogManager.Add((HistoricJobLogEventEntity)historyEvent);
            }
            else if (historyEvent is HistoricProcessInstanceEventEntity)
            {
                //Context.CommandContext.GetDbEntityManager<HistoricProcessInstanceEventEntity>().Add(historyEvent as HistoricProcessInstanceEventEntity);
                Context.CommandContext.HistoricProcessInstanceManager.Add((HistoricProcessInstanceEventEntity)historyEvent);
            }
            else if (historyEvent is Event.HistoricTaskInstanceEventEntity)
            {
                //Context.CommandContext.GetDbEntityManager<Event.HistoricTaskInstanceEventEntity>().Add((HistoricTaskInstanceEventEntity)historyEvent);
                Context.CommandContext.HistoricTaskInstanceManager.Add((HistoricTaskInstanceEventEntity)historyEvent);
            }
            //else if (historyEvent is HistoricVariableInstanceEntity)
            //{
            //    Context.CommandContext.GetDbEntityManager<HistoricVariableInstanceEntity>().Add(historyEvent as HistoricVariableInstanceEntity);
            //}
            else
            {
                throw new NotImplementedException("historyEvent类型错误：" + historyEvent.GetType().Name);
            }
        }
        private HistoricScopeInstanceEvent Get(HistoryEvent historyEvent)
        {
            if (historyEvent == null)
            {
                throw new NotImplementedException("historyEvent is null");
            }
            string id = historyEvent.Id;
            if (historyEvent is HistoricActivityInstanceEventEntity)
            {
                return Context.CommandContext.HistoricActivityInstanceManager.Get(id);
                // Todo: Context.ComandContext.GetDbEntityManager<TEntity>()方法有问题
                //return Context.CommandContext.GetDbEntityManager<HistoricActivityInstanceEventEntity>().Get(id);
            }
            //else if (historyEvent is HistoricBatchEntity)
            //{
            //    return Context.CommandContext.HistoricBatchManager.Get(id);
            //    //return Context.CommandContext.GetDbEntityManager<HistoricBatchEntity>().Get(id);
            //}
            else if (historyEvent is HistoricCaseActivityInstanceEventEntity)
            {
                return Context.CommandContext.HistoricCaseActivityInstanceManager.Get(id);
                //return Context.CommandContext.GetDbEntityManager<HistoricCaseActivityInstanceEventEntity>().Get(id);
            }
            else if (historyEvent is HistoricCaseInstanceEventEntity)
            {
                return Context.CommandContext.HistoricCaseInstanceManager.Get(id);
                //return Context.CommandContext.GetDbEntityManager<HistoricCaseInstanceEventEntity>().Get(id);
            }
            //else if (historyEvent is HistoricDecisionInputInstanceEntity)
            //{
            //    return Context.CommandContext.GetDbEntityManager<HistoricDecisionInputInstanceEntity>().Get(id);
            //}
            //else if (historyEvent is HistoricDecisionInstanceEntity)
            //{
            //    return Context.CommandContext.GetDbEntityManager<HistoricDecisionInstanceEntity>().Get(id);
            //}
            //else if (historyEvent is HistoricDecisionOutputInstanceEntity)
            //{
            //    return Context.CommandContext.GetDbEntityManager<HistoricDecisionOutputInstanceEntity>().Get(id);
            //}
            //else if (historyEvent is HistoricDetailEventEntity)
            //{
            //    Context.CommandContext.GetDbEntityManager<HistoricDetailEventEntity>().Add(historyEvent as HistoricDetailEventEntity);
            //}
            //else if (historyEvent is HistoricExternalTaskLogEntity)
            //{
            //    Context.CommandContext.GetDbEntityManager<HistoricExternalTaskLogEntity>().Add(historyEvent as HistoricExternalTaskLogEntity);
            //}
            //else if (historyEvent is HistoricIdentityLinkLogEntity)
            //{
            //    Context.CommandContext.GetDbEntityManager<HistoricIdentityLinkLogEntity>().Add(historyEvent as HistoricIdentityLinkLogEntity);
            //}
            //else if (historyEvent is HistoricIncidentEntity)
            //{
            //    Context.CommandContext.GetDbEntityManager<HistoricIncidentEntity>().Add(historyEvent as HistoricIncidentEntity);
            //}
            //else if (historyEvent is HistoricJobLogEventEntity)
            //{
            //    return Context.CommandContext.GetDbEntityManager<HistoricJobLogEventEntity>().Get(id);
            //}
            else if (historyEvent is HistoricProcessInstanceEventEntity)
            {
                return Context.CommandContext.HistoricProcessInstanceManager.Get(id);
                //return Context.CommandContext.GetDbEntityManager<HistoricProcessInstanceEventEntity>().Get(id);
            }
            else if (historyEvent is HistoricTaskInstanceEventEntity)
            {
                return Context.CommandContext.HistoricTaskInstanceManager.Get(id);
                //return Context.CommandContext.GetDbEntityManager<Event.HistoricTaskInstanceEventEntity>().Get(id);
            }
            //}else if(historyEvent is HistoricVariableInstanceEntity)
            //{
            //    Context.CommandContext.GetDbEntityManager<HistoricVariableInstanceEntity>().Add(historyEvent as HistoricVariableInstanceEntity);
            //}
            else
            {
                throw new NotImplementedException("historyEvent类型错误：" + historyEvent.GetType().Name);
            }
        }
        private void Update(HistoryEvent historyEvent)
        {
            if (historyEvent is HistoricProcessInstanceEventEntity)
            {
                HistoricProcessInstanceEventEntity data = historyEvent as HistoricProcessInstanceEventEntity;
                //TODO 不在EF上下文，再次更新会异常
                log.LogDebug("Entity当前状态", Context.CommandContext.GetEntityStateInEF<HistoricProcessInstanceEventEntity>(data).ToString());
                if (Context.CommandContext.GetEntityStateInEF<HistoricProcessInstanceEventEntity>(data) != EntityState.Detached)
                {
                    //TODO dbEntityManager.merge(historyEvent)
                    //Context.CommandContext.GetDbEntityManager<HistoricProcessInstanceEventEntity>().Update(data);
                    Context.CommandContext.HistoricProcessInstanceManager.Update(data);
                }
                else//不在当前上下文,已经被设置过
                {
                    PropertyUpdateHelper.UpDate(Context.CommandContext.HistoricProcessInstanceManager.Get(data.Id), data);
                    //PropertyUpdateHelper.UpDate<HistoricProcessInstanceEventEntity>(Context.CommandContext.GetDbEntityManager<HistoricProcessInstanceEventEntity>().Get(data.Id), data);
                }
            }
            else if
                (historyEvent is HistoricActivityInstanceEventEntity)
            {
                HistoricActivityInstanceEventEntity data = historyEvent as HistoricActivityInstanceEventEntity;
                log.LogDebug("Entity当前状态", Context.CommandContext.GetEntityStateInEF<HistoricActivityInstanceEventEntity>(data).ToString() + " " + data.Id + " " + data.ActivityInstanceState);
                if (Context.CommandContext.GetEntityStateInEF<HistoricActivityInstanceEventEntity>(data) != EntityState.Detached)
                {
                    //Context.CommandContext.GetDbEntityManager<HistoricActivityInstanceEventEntity>().Update(data);
                    Context.CommandContext.HistoricActivityInstanceManager.Update(data);
                }
                else
                {
                    //log.LogDebug("EF更新缓存", typeof(HistoricActivityInstanceEventEntity) + " " + data.Id+" state:"+data.ActivityInstanceState);
                    // Todo: Context.CommandContext.GetDbEntityManager<TEntity>()方法有问题
                    //var cache = Context.CommandContext.HistoricActivityInstanceManager.Get(data.Id);
                    PropertyUpdateHelper.UpDate(Context.CommandContext.HistoricActivityInstanceManager.Get(data.Id), data);
                    //log.LogDebug("测试001", Context.CommandContext.GetDbEntityManager<HistoricActivityInstanceEventEntity>().Get(data.Id).ActivityInstanceState.ToString());
                }
            }
            else if (historyEvent is Event.HistoricTaskInstanceEventEntity)
            {
                Event.HistoricTaskInstanceEventEntity data = historyEvent as Event.HistoricTaskInstanceEventEntity;
                if (Context.CommandContext.GetEntityStateInEF<Event.HistoricTaskInstanceEventEntity>(data) != EntityState.Detached)
                {
                    //Context.CommandContext.GetDbEntityManager<Event.HistoricTaskInstanceEventEntity>().Update(data);
                    Context.CommandContext.HistoricTaskInstanceManager.Update(data);
                }
                else
                {
                    //PropertyUpdateHelper.UpDate<Event.HistoricTaskInstanceEventEntity>(Context.CommandContext.GetDbEntityManager<Event.HistoricTaskInstanceEventEntity>().Get(data.Id), data);
                    PropertyUpdateHelper.UpDate(Context.CommandContext.HistoricTaskInstanceManager.Get(data.Id), data);
                }

            }
            else if (historyEvent is HistoricIncidentEntity)
            {
                HistoricIncidentEntity data = (HistoricIncidentEntity)historyEvent;
                if (Context.CommandContext.GetEntityStateInEF<HistoricIncidentEntity>(data) != EntityState.Detached)
                {
                    //Context.CommandContext.GetDbEntityManager<HistoricIncidentEntity>().Update(data);
                    Context.CommandContext.HistoricIncidentManager.Update(data);
                }
                else
                {
                    //PropertyUpdateHelper.UpDate<HistoricIncidentEntity>(Context.CommandContext.GetDbEntityManager<HistoricIncidentEntity>().Get(data.Id), data);
                    PropertyUpdateHelper.UpDate(Context.CommandContext.HistoricIncidentManager.Get(data.Id), data);
                }
            }
            else
            {
                throw new NotImplementedException(string.Format("historyEvent类型:{0} 更新未实现", historyEvent.GetType().ToString()));
            }

        }
        /// <summary>
        ///     customized insert behavior for HistoricVariableUpdateEventEntity
        /// </summary>
        protected internal virtual void InsertHistoricVariableUpdateEntity(
            HistoricVariableUpdateEventEntity historyEvent)
        {
            log.LogDebug("InsertHistoricVariableUpdateEntityEF缓存：", historyEvent.GetType().Name + " id:" + historyEvent.Id, historyEvent);
            //throw new NotImplementedException();
            //var dbEntityManager = DbEntityManager;

            // insert update only if history level = FULL
            if (ShouldWriteHistoricDetail(historyEvent))
            {
                // insert byte array entity (if applicable)
                var byteValue = historyEvent.ByteValue;
                if (byteValue != null)
                {
                    var byteArrayEntity = new ResourceEntity(historyEvent.VariableName, byteValue);
                    //Context.CommandContext.GetDbEntityManager<ResourceEntity>().Add(byteArrayEntity);//.DbEntityManager.insert(byteArrayEntity);
                    Context.CommandContext.ResourceManager.InsertResource(byteArrayEntity);
                    historyEvent.ByteArrayId = byteArrayEntity.Id;
                }
                //dbEntityManager.Insert(historyEvent);
                historyEventRepository.Add(historyEvent as HistoricDetailEventEntity);
            }

            // always insert/update HistoricProcessVariableInstance
            if (historyEvent.IsEventOfType(HistoryEventTypes.VariableInstanceCreate))
            {
                var persistentObject = new HistoricVariableInstanceEntity(historyEvent);
                //dbEntityManager.insert(persistentObject);
                //Context.CommandContext.GetDbEntityManager<HistoricVariableInstanceEntity>().Add(persistentObject);
                Context.CommandContext.HistoricVariableInstanceManager.Add(persistentObject);
            }
            else if (historyEvent.IsEventOfType(HistoryEventTypes.VariableInstanceUpdate) ||
                     historyEvent.IsEventOfType(HistoryEventTypes.VariableInstanceMigrate))
            {
                HistoricVariableInstanceEntity historicVariableInstanceEntity =
                    //dbEntityManager.selectById<HistoricVariableInstanceEntity>(typeof(HistoricVariableInstanceEntity),historyEvent.VariableInstanceId);
                    //Context.CommandContext.GetDbEntityManager<HistoricVariableInstanceEntity>().Get(historyEvent.VariableInstanceId);
                    Context.CommandContext.HistoricVariableInstanceManager.Get(historyEvent.VariableInstanceId);
                if (historicVariableInstanceEntity != null)
                {
                    historicVariableInstanceEntity.UpdateFromEvent(historyEvent);
                    historicVariableInstanceEntity.State = HistoricVariableInstanceEntity.StateCreated;
                }
                else
                {
                    // #CAM-1344 / #SUPPORT-688
                    // this is a FIX for process instances which were started in camunda fox 6.1 and migrated to camunda BPM 7.0.
                    // in fox 6.1 the HistoricVariable instances were flushed to the DB when the process instance completed.
                    // Since fox 6.2 we populate the HistoricVariable table as we go.
                    var persistentObject = new HistoricVariableInstanceEntity(historyEvent);
                    //Context.CommandContext.GetDbEntityManager<HistoricVariableInstanceEntity>().Add(persistentObject);
                    Context.CommandContext.HistoricVariableInstanceManager.Add(persistentObject);
                }
            }
            else if (historyEvent.IsEventOfType(HistoryEventTypes.VariableInstanceDelete))
            {
                HistoricVariableInstanceEntity historicVariableInstanceEntity =
                //Context.CommandContext.GetDbEntityManager<HistoricVariableInstanceEntity>().Single(m => m.Id ==
                //    historyEvent.VariableInstanceId);
                Context.CommandContext.HistoricVariableInstanceManager.Single(
                    m => m.Id == historyEvent.VariableInstanceId);
                if (historicVariableInstanceEntity != null)
                {
                    //版本更新
                    //historicVariableInstanceEntity.Delete();
                    historicVariableInstanceEntity.State = HistoricVariableInstanceEntity.StateDeleted;
                }
            }
        }

        protected internal virtual bool ShouldWriteHistoricDetail(HistoricVariableUpdateEventEntity historyEvent)
        {
            return
                Context.ProcessEngineConfiguration.HistoryLevel.IsHistoryEventProduced(
                    HistoryEventTypes.VariableInstanceUpdateDetail, historyEvent) &&
                !historyEvent.IsEventOfType(HistoryEventTypes.VariableInstanceMigrate);
        }


        protected internal virtual void InsertHistoricDecisionEvaluationEvent(HistoricDecisionEvaluationEvent @event)
        {
            throw new NotImplementedException();
            //Context.CommandContext.HistoricDecisionInstanceManager.insertHistoricDecisionInstances(@event);
        }


        protected internal virtual bool IsInitialEvent(HistoryEvent historyEvent)
        {
            return historyEvent.EventType == null ||
                   historyEvent.IsEventOfType(HistoryEventTypes.ActivityInstanceStart) ||
                   historyEvent.IsEventOfType(HistoryEventTypes.ProcessInstanceStart) ||
                   historyEvent.IsEventOfType(HistoryEventTypes.TaskInstanceCreate) ||
                   historyEvent.IsEventOfType(HistoryEventTypes.FormPropertyUpdate) ||
                   historyEvent.IsEventOfType(HistoryEventTypes.IncidentCreate) ||
                   historyEvent.IsEventOfType(HistoryEventTypes.CaseInstanceCreate) ||
                   historyEvent.IsEventOfType(HistoryEventTypes.DmnDecisionEvaluate) ||
                   historyEvent.IsEventOfType(HistoryEventTypes.BatchStart) ||
                   historyEvent.IsEventOfType(HistoryEventTypes.IdentityLinkAdd) ||
                   historyEvent.IsEventOfType(HistoryEventTypes.IdentityLinkDelete);
        }
    }
}
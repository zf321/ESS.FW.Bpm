using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Autofac.Features.AttributeFilters;

using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.Externaltask;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.History.Impl;
using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.Bpm.Engine.History.Impl.Producer;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Util;
using Microsoft.Extensions.Logging;
using ESS.FW.DataAccess;
using ESS.FW.Common.Components;
using ESS.FW.Bpm.Model.Common;
using System.Linq;

namespace ESS.FW.Bpm.Engine.Persistence.Entity.Impl
{

    [Component]
    public class HistoricExternalTaskLogManager : AbstractManagerNet<HistoricExternalTaskLogEntity>, IHistoricExternalTaskLogManager
    {
        public HistoricExternalTaskLogManager(DbContext dbContex, ILoggerFactory loggerFactory, IDGenerator idGenerator) : base(dbContex, loggerFactory, idGenerator)
        {
        }

        // select /////////////////////////////////////////////////////////////////

        public virtual HistoricExternalTaskLogEntity FindHistoricExternalTaskLogById(string historicExternalTaskLogId)
        {
            //return (HistoricExternalTaskLogEntity)DbEntityManager.SelectOne("selectHistoricExternalTaskLog", historicExternalTaskLogId);
            return Single(m => m.Id == historicExternalTaskLogId);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") public java.Util.List<org.camunda.bpm.engine.history.HistoricExternalTaskLog> findHistoricExternalTaskLogsByQueryCriteria(org.camunda.bpm.engine.impl.HistoricExternalTaskLogQueryImpl query, org.camunda.bpm.engine.impl.Page page)
        // public virtual IList<IHistoricExternalTaskLog> FindHistoricExternalTaskLogsByQueryCriteria(HistoricExternalTaskLogQueryImpl query, Page page)
        // {
        //ConfigureQuery(query);
        //return ListExt.ConvertToListT<IHistoricExternalTaskLog>(DbEntityManager.SelectList("selectHistoricExternalTaskLogByQueryCriteria", query, page)) ;
        // }

        // public virtual long FindHistoricExternalTaskLogsCountByQueryCriteria(HistoricExternalTaskLogQueryImpl query)
        // {
        //ConfigureQuery(query);
        //return (long) DbEntityManager.SelectOne("selectHistoricExternalTaskLogCountByQueryCriteria", query);
        // }

        // delete ///////////////////////////////////////////////////////////////////

        public virtual void DeleteHistoricExternalTaskLogsByProcessInstanceId(string processInstanceId)
        {
            DeleteExceptionByteArrayByParameterMap("ProcessInstanceId", processInstanceId);
            //DbEntityManager.Delete(typeof(HistoricExternalTaskLogEntity), "deleteHistoricExternalTaskLogByProcessInstanceId", processInstanceId);
            Delete(m => m.ProcessInstanceId == processInstanceId);
        }

        // byte array delete ////////////////////////////////////////////////////////

        protected internal virtual void DeleteExceptionByteArrayByParameterMap(string key, string value)
        {
            EnsureUtil.EnsureNotNull(key, value);
            //IDictionary<string, string> parameterMap = new Dictionary<string, string>();
            //parameterMap[key] = value;
            //DbEntityManager.Delete(typeof(ResourceEntity), "deleteErrorDetailsByteArraysByIds", parameterMap);
            //var ids = GetDbEntityManager<HistoricExternalTaskLogEntity>().Find(m => m.ErrorDetailsByteArrayId != null).ParameterSelect(key,value).Select(m => m.ErrorDetailsByteArrayId).ToList();
            var ids = Find(m => m.ErrorDetailsByteArrayId != null).ParameterSelect(key, value).Select(m => m.ErrorDetailsByteArrayId).ToList();
            if (ids != null && ids.Count > 0)
            {                
                GetDbEntityManager<ResourceEntity>().Delete(m => ids.Contains(m.Id));
            }
        }

        // fire history events ///////////////////////////////////////////////////////

        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
        //ORIGINAL LINE: public void fireExternalTaskCreatedEvent(final org.camunda.bpm.engine.externaltask.ExternalTask externalTask)
        public virtual void FireExternalTaskCreatedEvent(IExternalTask externalTask)
        {
            if (IsHistoryEventProduced(HistoryEventTypes.ExternalTaskCreate, externalTask))
            {
                HistoryEventProcessor.ProcessHistoryEvents(new HistoryEventCreatorAnonymousInnerClassHelper(this, externalTask));
            }
        }

        private class HistoryEventCreatorAnonymousInnerClassHelper : HistoryEventProcessor.HistoryEventCreator
        {
            private readonly HistoricExternalTaskLogManager _outerInstance;

            private IExternalTask _externalTask;

            public HistoryEventCreatorAnonymousInnerClassHelper(HistoricExternalTaskLogManager outerInstance, IExternalTask externalTask)
            {
                this._outerInstance = outerInstance;
                this._externalTask = externalTask;
            }

            public override HistoryEvent CreateHistoryEvent(IHistoryEventProducer producer)
            {
                return producer.CreateHistoricExternalTaskLogCreatedEvt(_externalTask);
            }
        }

        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
        //ORIGINAL LINE: public void fireExternalTaskFailedEvent(final org.camunda.bpm.engine.externaltask.ExternalTask externalTask)
        public virtual void FireExternalTaskFailedEvent(IExternalTask externalTask)
        {
            if (IsHistoryEventProduced(HistoryEventTypes.ExternalTaskFail, externalTask))
            {
                HistoryEventProcessor.ProcessHistoryEvents(new HistoryEventCreatorAnonymousInnerClassHelper2(this, externalTask));
            }
        }

        private class HistoryEventCreatorAnonymousInnerClassHelper2 : HistoryEventProcessor.HistoryEventCreator
        {
            private readonly HistoricExternalTaskLogManager _outerInstance;

            private IExternalTask _externalTask;

            public HistoryEventCreatorAnonymousInnerClassHelper2(HistoricExternalTaskLogManager outerInstance, IExternalTask externalTask)
            {
                this._outerInstance = outerInstance;
                this._externalTask = externalTask;
            }

            public override HistoryEvent CreateHistoryEvent(IHistoryEventProducer producer)
            {
                return producer.CreateHistoricExternalTaskLogFailedEvt(_externalTask);
            }
        }

        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
        //ORIGINAL LINE: public void fireExternalTaskSuccessfulEvent(final org.camunda.bpm.engine.externaltask.ExternalTask externalTask)
        public virtual void FireExternalTaskSuccessfulEvent(IExternalTask externalTask)
        {
            if (IsHistoryEventProduced(HistoryEventTypes.ExternalTaskSuccess, externalTask))
            {
                HistoryEventProcessor.ProcessHistoryEvents(new HistoryEventCreatorAnonymousInnerClassHelper3(this, externalTask));
            }
        }

        private class HistoryEventCreatorAnonymousInnerClassHelper3 : HistoryEventProcessor.HistoryEventCreator
        {
            private readonly HistoricExternalTaskLogManager _outerInstance;

            private IExternalTask _externalTask;

            public HistoryEventCreatorAnonymousInnerClassHelper3(HistoricExternalTaskLogManager outerInstance, IExternalTask externalTask)
            {
                this._outerInstance = outerInstance;
                this._externalTask = externalTask;
            }

            public override HistoryEvent CreateHistoryEvent(IHistoryEventProducer producer)
            {
                return producer.CreateHistoricExternalTaskLogSuccessfulEvt(_externalTask);
            }
        }

        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
        //ORIGINAL LINE: public void fireExternalTaskDeletedEvent(final org.camunda.bpm.engine.externaltask.ExternalTask externalTask)
        public virtual void FireExternalTaskDeletedEvent(IExternalTask externalTask)
        {
            if (IsHistoryEventProduced(HistoryEventTypes.ExternalTaskDelete, externalTask))
            {
                HistoryEventProcessor.ProcessHistoryEvents(new HistoryEventCreatorAnonymousInnerClassHelper4(this, externalTask));
            }
        }

        private class HistoryEventCreatorAnonymousInnerClassHelper4 : HistoryEventProcessor.HistoryEventCreator
        {
            private readonly HistoricExternalTaskLogManager _outerInstance;

            private IExternalTask _externalTask;

            public HistoryEventCreatorAnonymousInnerClassHelper4(HistoricExternalTaskLogManager outerInstance, IExternalTask externalTask)
            {
                this._outerInstance = outerInstance;
                this._externalTask = externalTask;
            }

            public override HistoryEvent CreateHistoryEvent(IHistoryEventProducer producer)
            {
                return producer.CreateHistoricExternalTaskLogDeletedEvt(_externalTask);
            }
        }

        // helper /////////////////////////////////////////////////////////

        protected internal virtual bool IsHistoryEventProduced(HistoryEventTypes eventType, IExternalTask externalTask)
        {
            ProcessEngineConfigurationImpl configuration = context.Impl.Context.ProcessEngineConfiguration;
            IHistoryLevel historyLevel = configuration.HistoryLevel;
            return historyLevel.IsHistoryEventProduced(eventType, externalTask);
        }

        // protected internal virtual void ConfigureQuery(HistoricExternalTaskLogQueryImpl query)
        // {
        //AuthorizationManager.ConfigureHistoricExternalTaskLogQuery(query);
        //TenantManager.ConfigureQuery(query);
        // }
    }

}
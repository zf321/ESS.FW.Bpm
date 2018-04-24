using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.Bpm.Engine.History.Impl.Producer;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Runtime;
using Microsoft.Extensions.Logging;
using ESS.FW.DataAccess;
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
    public class HistoricJobLogManager : AbstractHistoricManagerNet<HistoricJobLogEventEntity>, IHistoricJobLogManager
    {
        public HistoricJobLogManager(DbContext dbContex, ILoggerFactory loggerFactory, IDGenerator idGenerator) : base(dbContex, loggerFactory, idGenerator)
        {
        }

        // select /////////////////////////////////////////////////////////////////

        public virtual HistoricJobLogEventEntity FindHistoricJobLogById(string historicJobLogId)
        {
            //return (HistoricJobLogEventEntity) DbEntityManager.SelectOne("selectHistoricJobLog", historicJobLogId);
            return Single(m => m.Id == historicJobLogId);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") public java.Util.List<org.camunda.bpm.engine.history.HistoricJobLog> findHistoricJobLogsByDeploymentId(String deploymentId)
        public virtual IList<IHistoricJobLog> FindHistoricJobLogsByDeploymentId(string deploymentId)
        {
            //return ListExt.ConvertToListT<IHistoricJobLog>(DbEntityManager.SelectList("selectHistoricJobLogByDeploymentId", deploymentId));
            return Find(m => m.DeploymentId == deploymentId).ToList() as IList<IHistoricJobLog>;
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") public java.Util.List<org.camunda.bpm.engine.history.HistoricJobLog> findHistoricJobLogsByQueryCriteria(org.camunda.bpm.engine.impl.HistoricJobLogQueryImpl query, org.camunda.bpm.engine.impl.Page page)
        //public virtual IList<IHistoricJobLog> FindHistoricJobLogsByQueryCriteria(HistoricJobLogQueryImpl query,
        //    Page page)
        //{
        //    ConfigureQuery(query);
        //    return
        //        ListExt.ConvertToListT<IHistoricJobLog>(DbEntityManager.SelectList(
        //            "selectHistoricJobLogByQueryCriteria", query, page));
        //}

        //public virtual long FindHistoricJobLogsCountByQueryCriteria(HistoricJobLogQueryImpl query)
        //{
        //    ConfigureQuery(query);
        //    return (long) DbEntityManager.SelectOne("selectHistoricJobLogCountByQueryCriteria", query);
        //}

        // delete ///////////////////////////////////////////////////////////////////

        public virtual void DeleteHistoricJobLogById(string id)
        {
            if (HistoryEnabled)
            {
                DeleteExceptionByteArrayByParameterMap("id", id);
                //DbEntityManager.Delete(typeof(HistoricJobLogEventEntity), "deleteHistoricJobLogById", id);
                Delete(m => m.Id == id);
            }
        }

        public virtual void DeleteHistoricJobLogByJobId(string jobId)
        {
            if (HistoryEnabled)
            {
                DeleteExceptionByteArrayByParameterMap("jobId", jobId);
                //DbEntityManager.Delete(typeof(HistoricJobLogEventEntity), "deleteHistoricJobLogByJobId", jobId);
                Delete(m => m.JobId == jobId);
            }
        }

        public virtual void DeleteHistoricJobLogsByProcessInstanceId(string processInstanceId)
        {
            if (HistoryEnabled)
            {
                DeleteExceptionByteArrayByParameterMap("processInstanceId", processInstanceId);
                //DbEntityManager.Delete(typeof(HistoricJobLogEventEntity), "deleteHistoricJobLogByProcessInstanceId",processInstanceId);
                Delete(m => m.ProcessInstanceId == processInstanceId);
            }
        }

        public virtual void DeleteHistoricJobLogsByProcessDefinitionId(string processDefinitionId)
        {
            if (HistoryEnabled)
            {
                DeleteExceptionByteArrayByParameterMap("processDefinitionId", processDefinitionId);
                //DbEntityManager.Delete(typeof(HistoricJobLogEventEntity), "deleteHistoricJobLogByProcessDefinitionId",
                //processDefinitionId);
                Delete(m => m.ProcessDefinitionId == processDefinitionId);
            }
        }

        public virtual void DeleteHistoricJobLogsByDeploymentId(string deploymentId)
        {
            if (HistoryEnabled)
            {
                DeleteExceptionByteArrayByParameterMap("deploymentId", deploymentId);
                //DbEntityManager.Delete(typeof(HistoricJobLogEventEntity), "deleteHistoricJobLogByDeploymentId",
                //deploymentId);
                Delete(m => m.DeploymentId == deploymentId);
            }
        }

        public virtual void deleteHistoricJobLogsByHandlerType(string handlerType)
        {
            if (HistoryEnabled)
            {
                DeleteExceptionByteArrayByParameterMap("handlerType", handlerType);
                //DbEntityManager.Delete(typeof(HistoricJobLogEventEntity), "deleteHistoricJobLogByHandlerType",
                //handlerType);
                Delete(m => m.JobDefinitionType == handlerType);
            }
        }

        public virtual void DeleteHistoricJobLogsByJobDefinitionId(string jobDefinitionId)
        {
            if (HistoryEnabled)
            {
                DeleteExceptionByteArrayByParameterMap("jobDefinitionId", jobDefinitionId);
                //DbEntityManager.Delete(typeof(HistoricJobLogEventEntity), "deleteHistoricJobLogByJobDefinitionId",
                //jobDefinitionId);
                Delete(m => m.JobDefinitionId == jobDefinitionId);
            }
        }

        // byte array delete ////////////////////////////////////////////////////////

        protected internal virtual void DeleteExceptionByteArrayByParameterMap(string key, string value)
        {
            EnsureUtil.EnsureNotNull(key, value);
            IDictionary<string, string> parameterMap = new Dictionary<string, string>();
            parameterMap[key] = value;
            //throw new NotImplementedException();
            //TODO deleteExceptionByteArraysByIds
            //DbEntityManager.Delete(typeof(ResourceEntity), "deleteExceptionByteArraysByIds", parameterMap);

        }

        // fire history events ///////////////////////////////////////////////////////
        
        public virtual void FireJobCreatedEvent(IJob job)
        {
            if (IsHistoryEventProduced(HistoryEventTypes.JobCreate, job))
            {
                HistoryEventProcessor.ProcessHistoryEvents(new HistoryEventCreatorAnonymousInnerClassHelper(this, job));
            }
        }

        public virtual void FireJobFailedEvent(IJob job, System.Exception exception)
        {
            if (IsHistoryEventProduced(HistoryEventTypes.JobFail, job))
            {
                HistoryEventProcessor.ProcessHistoryEvents(new HistoryEventCreatorAnonymousInnerClassHelper2(this, job, exception));
            }
        }

        public virtual void FireJobSuccessfulEvent(IJob job)
        {
            if (IsHistoryEventProduced(HistoryEventTypes.JobSuccess, job))
            {
                HistoryEventProcessor.ProcessHistoryEvents(new HistoryEventCreatorAnonymousInnerClassHelper3(this, job));
            }
        }

        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
        //ORIGINAL LINE: public void fireJobDeletedEvent(final org.camunda.bpm.engine.runtime.Job job)
        public virtual void FireJobDeletedEvent(IJob job)
        {
            // throw new NotImplementedException();
            if (IsHistoryEventProduced(HistoryEventTypes.JobDelete, job))
            {
                HistoryEventProcessor.ProcessHistoryEvents(new HistoryEventCreatorAnonymousInnerClassHelper4(this, job));
            }
        }

        public void DeleteHistoricJobLogsByHandlerType(string handlerType)
        {
            if (IsHistoryEnabled)
            {
                DeleteExceptionByteArrayByParameterMap("handlerType", handlerType);
                throw new NotImplementedException();
                //GetDbEntityManager().delete(HistoricJobLogEventEntity.class, "deleteHistoricJobLogByHandlerType", handlerType);
            }
}


        // helper /////////////////////////////////////////////////////////

        protected internal virtual bool IsHistoryEventProduced(HistoryEventTypes eventType, IJob job)
        {
            var configuration =context.Impl.Context.ProcessEngineConfiguration;
            var historyLevel = configuration.HistoryLevel;
            return historyLevel.IsHistoryEventProduced(eventType, job);
        }

        //protected internal virtual void ConfigureQuery(HistoricJobLogQueryImpl query)
        //{
        //    AuthorizationManager.ConfigureHistoricJobLogQuery(query);
        //    TenantManager.ConfigureQuery(query);
        //}

        private class HistoryEventCreatorAnonymousInnerClassHelper : HistoryEventProcessor.HistoryEventCreator
        {
            private readonly HistoricJobLogManager _outerInstance;

            private readonly IJob _job;

            public HistoryEventCreatorAnonymousInnerClassHelper(HistoricJobLogManager outerInstance, IJob job)
            {
                _outerInstance = outerInstance;
                _job = job;
            }

            public override HistoryEvent CreateHistoryEvent(IHistoryEventProducer producer)
            {
                return producer.CreateHistoricJobLogCreateEvt(_job);
            }
        }

        private class HistoryEventCreatorAnonymousInnerClassHelper2 : HistoryEventProcessor.HistoryEventCreator
        {
            private readonly HistoricJobLogManager _outerInstance;
            private readonly System.Exception _exception;

            private readonly IJob _job;

            public HistoryEventCreatorAnonymousInnerClassHelper2(HistoricJobLogManager outerInstance, IJob job,
                System.Exception exception)
            {
                _outerInstance = outerInstance;
                _job = job;
                _exception = exception;
            }

            public override HistoryEvent CreateHistoryEvent(IHistoryEventProducer producer)
            {
                return producer.CreateHistoricJobLogFailedEvt(_job, _exception);
            }
        }

        private class HistoryEventCreatorAnonymousInnerClassHelper3 : HistoryEventProcessor.HistoryEventCreator
        {
            private readonly HistoricJobLogManager _outerInstance;

            private readonly IJob _job;

            public HistoryEventCreatorAnonymousInnerClassHelper3(HistoricJobLogManager outerInstance, IJob job)
            {
                _outerInstance = outerInstance;
                _job = job;
            }

            public override HistoryEvent CreateHistoryEvent(IHistoryEventProducer producer)
            {
                return producer.CreateHistoricJobLogSuccessfulEvt(_job);
            }
        }

        private class HistoryEventCreatorAnonymousInnerClassHelper4 : HistoryEventProcessor.HistoryEventCreator
        {
            private readonly HistoricJobLogManager _outerInstance;

            private readonly IJob _job;

            public HistoryEventCreatorAnonymousInnerClassHelper4(HistoricJobLogManager outerInstance, IJob job)
            {
                _outerInstance = outerInstance;
                _job = job;
            }

            public override HistoryEvent CreateHistoryEvent(IHistoryEventProducer producer)
            {
                return producer.CreateHistoricJobLogDeleteEvt(_job);
            }
        }
    }
}
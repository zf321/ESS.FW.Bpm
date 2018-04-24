using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Batch.History;
using ESS.FW.Bpm.Engine.Batch.Impl;
using ESS.FW.Bpm.Engine.Batch.Impl.History;
using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.History.Impl;
using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.Bpm.Engine.History.Impl.Producer;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using System;
using Microsoft.EntityFrameworkCore;
using Autofac.Features.AttributeFilters;

using Microsoft.Extensions.Logging;
using ESS.FW.DataAccess;
using ESS.FW.Common.Components;

namespace ESS.FW.Bpm.Engine.Persistence.Entity.Impl
{
    [Component]
    public class HistoricBatchManager : AbstractManagerNet<HistoricBatchEntity>, IHistoricBatchManager
    {
        public HistoricBatchManager(DbContext dbContex, ILoggerFactory loggerFactory, IDGenerator idGenerator) : base(dbContex, loggerFactory, idGenerator)
        {
        }

        //	  public virtual long FindBatchCountByQueryCriteria(HistoricBatchQueryImpl historicBatchQuery)
        //	  {
        //		ConfigureQuery(historicBatchQuery);
        //		return (long) DbEntityManager.SelectOne("selectHistoricBatchCountByQueryCriteria", historicBatchQuery);
        //	  }

        ////JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        ////ORIGINAL LINE: @SuppressWarnings("unchecked") public java.Util.List<org.camunda.bpm.engine.batch.history.HistoricBatch> findBatchesByQueryCriteria(org.camunda.bpm.engine.impl.batch.history.HistoricBatchQueryImpl historicBatchQuery, org.camunda.bpm.engine.impl.Page page)
        //	  public virtual IList<IHistoricBatch> FindBatchesByQueryCriteria(HistoricBatchQueryImpl historicBatchQuery, Page page)
        //	  {
        //		ConfigureQuery(historicBatchQuery);
        //		return ListExt.ConvertToListT<IHistoricBatch>(DbEntityManager.SelectList("selectHistoricBatchesByQueryCriteria", historicBatchQuery, page)) ;
        //	  }

        public virtual HistoricBatchEntity FindHistoricBatchById(string batchId)
        {
            //return DbEntityManager.SelectById< HistoricBatchEntity>(typeof(HistoricBatchEntity), batchId);
            return Get(batchId);
        }

        public virtual void DeleteHistoricBatchById(string id)
        {
            //DbEntityManager.Delete(typeof(HistoricBatchEntity), "deleteHistoricBatchById", id);
            Delete(m => m.Id == id);
        }

        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
        //ORIGINAL LINE: public void createHistoricBatch(final org.camunda.bpm.engine.impl.batch.BatchEntity batch)
        public virtual void CreateHistoricBatch(BatchEntity batch)
        {
            ProcessEngineConfigurationImpl configuration = context.Impl.Context.ProcessEngineConfiguration;

            IHistoryLevel historyLevel = configuration.HistoryLevel;
            if (historyLevel.IsHistoryEventProduced(HistoryEventTypes.BatchStart, batch))
            {

                HistoryEventProcessor.ProcessHistoryEvents(new HistoryEventCreatorAnonymousInnerClassHelper(this, batch));
            }
        }

        private class HistoryEventCreatorAnonymousInnerClassHelper : HistoryEventProcessor.HistoryEventCreator
        {
            private readonly HistoricBatchManager _outerInstance;

            private BatchEntity _batch;

            public HistoryEventCreatorAnonymousInnerClassHelper(HistoricBatchManager outerInstance, BatchEntity batch)
            {
                this._outerInstance = outerInstance;
                this._batch = batch;
            }

            public override HistoryEvent CreateHistoryEvent(IHistoryEventProducer producer)
            {
                return producer.CreateBatchStartEvent(_batch);
            }
        }

        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
        //ORIGINAL LINE: public void completeHistoricBatch(final org.camunda.bpm.engine.impl.batch.BatchEntity batch)
        public virtual void CompleteHistoricBatch(BatchEntity batch)
        {
            ProcessEngineConfigurationImpl configuration = context.Impl.Context.ProcessEngineConfiguration;

            IHistoryLevel historyLevel = configuration.HistoryLevel;
            if (historyLevel.IsHistoryEventProduced(HistoryEventTypes.BatchEnd, batch))
            {

                HistoryEventProcessor.ProcessHistoryEvents(new HistoryEventCreatorAnonymousInnerClassHelper2(this, batch));
            }
        }

        private class HistoryEventCreatorAnonymousInnerClassHelper2 : HistoryEventProcessor.HistoryEventCreator
        {
            private readonly HistoricBatchManager _outerInstance;

            private BatchEntity _batch;

            public HistoryEventCreatorAnonymousInnerClassHelper2(HistoricBatchManager outerInstance, BatchEntity batch)
            {
                this._outerInstance = outerInstance;
                this._batch = batch;
            }

            public override HistoryEvent CreateHistoryEvent(IHistoryEventProducer producer)
            {
                return producer.CreateBatchEndEvent(_batch);
            }
        }

        // protected internal virtual void ConfigureQuery(HistoricBatchQueryImpl query)
        // {
        //AuthorizationManager.ConfigureHistoricBatchQuery(query);
        //TenantManager.ConfigureQuery(query);
        // }

    }

}
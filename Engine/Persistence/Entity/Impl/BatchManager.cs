using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Autofac.Features.AttributeFilters;

using ESS.FW.Bpm.Engine.Batch.Impl;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.DB;
using Microsoft.Extensions.Logging;
using ESS.FW.DataAccess;
using ESS.FW.Common.Components;

namespace ESS.FW.Bpm.Engine.Persistence.Entity.Impl
{
    [Component]
    public class BatchManager : AbstractManagerNet<BatchEntity>, IBatchManager
    {
        public BatchManager(DbContext dbContex, ILoggerFactory loggerFactory, IDGenerator idGenerator) : base(dbContex, loggerFactory, idGenerator)
        {
        }

        public virtual void InsertBatch(BatchEntity batch)
        {
            Add(batch);
        }

        public virtual BatchEntity FindBatchById(string id)
        {
            return Get(id);
        }

        //public virtual long? FindBatchCountByQueryCriteria(BatchQueryImpl batchQuery)
        //{
        //    ConfigureQuery(batchQuery);
        //    //return (long?) DbEntityManager.SelectOne("selectBatchCountByQueryCriteria", batchQuery);
        //    throw new System.NotImplementedException();
        //}

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") public java.Util.List<org.camunda.bpm.engine.batch.IBatch> findBatchesByQueryCriteria(org.camunda.bpm.engine.impl.batch.BatchQueryImpl batchQuery, org.camunda.bpm.engine.impl.Page page)
        //public virtual IList<IBatch> FindBatchesByQueryCriteria(BatchQueryImpl batchQuery, Page page)
        //{
        //    ConfigureQuery(batchQuery);
        //    //return ListExt.ConvertToListT<IBatch>(DbEntityManager.SelectList("selectBatchesByQueryCriteria", batchQuery, page));
        //    throw new System.NotImplementedException();
        //}

        public virtual void UpdateBatchSuspensionStateById(string batchId, ISuspensionState suspensionState)
        {
            //IDictionary<string, object> parameters = new Dictionary<string, object>();
            //parameters["batchId"] = batchId;
            //parameters["suspensionState"] = suspensionState.StateCode;

            //var queryParameter = new ListQueryParameterObject();
            //queryParameter.Parameter = parameters;

            //DbEntityManager.Update(typeof(BatchEntity), "updateBatchSuspensionStateByParameters", queryParameter);

            var entity = First(c => c.Id == batchId);
            if (entity != null)
            {
                entity.SuspensionState = suspensionState.StateCode;
                Update(entity);
            }
        }

        //TenantManager.ConfigureQuery(batchQuery);
        //AuthorizationManager.ConfigureBatchQuery(batchQuery);
        // {

        // protected internal virtual void ConfigureQuery(BatchQueryImpl batchQuery)
        // }
    }
}
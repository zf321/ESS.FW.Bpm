using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.Util;
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
    public class HistoricDetailManager : AbstractHistoricManagerNet<HistoricDetailEventEntity>, IHistoricDetailManager
    {
        public HistoricDetailManager(DbContext dbContex, ILoggerFactory loggerFactory, IDGenerator idGenerator) : base(dbContex, loggerFactory, idGenerator)
        {
        }

        public virtual void DeleteHistoricDetailsByProcessInstanceId(string historicProcessInstanceId)
        {
            DeleteHistoricDetailsByProcessCaseInstanceId(historicProcessInstanceId, null);
        }

        public virtual void DeleteHistoricDetailsByCaseInstanceId(string historicCaseInstanceId)
        {
            DeleteHistoricDetailsByProcessCaseInstanceId(null, historicCaseInstanceId);
        }

        public virtual void DeleteHistoricDetailsByProcessCaseInstanceId(string historicProcessInstanceId, string historicCaseInstanceId)
        {
            EnsureUtil.EnsureOnlyOneNotNull("Only the process instance or case instance id should be set", historicProcessInstanceId, historicCaseInstanceId);
            if (HistoryEnabled)
            {
                // delete entries in DB
                IList<IHistoricDetail> historicDetails;
                if (historicProcessInstanceId != null)
                {
                    historicDetails = FindHistoricDetailsByProcessInstanceId(historicProcessInstanceId);
                }
                else
                {
                    historicDetails = FindHistoricDetailsByCaseInstanceId(historicCaseInstanceId);
                }
                if (historicDetails != null)
                {
                    foreach (IHistoricDetail historicDetail in historicDetails)
                    {
                        if (historicDetail is HistoricDetailEventEntity)
                        {
                            ((HistoricDetailEventEntity)historicDetail).Delete();
                        }
                        else
                        {
                            throw new System.NotImplementedException("类型转换异常HistoricDetailEventEntity");
                        }

                    }
                }
                
                //throw new System.NotImplementedException("EF缓存列表");
                //TODO 不需要再次清理缓存？delete entries in Cache
                //IList<HistoricDetailEventEntity> cachedHistoricDetails = Get();// DbEntityManager.GetCachedEntitiesByType< HistoricDetailEventEntity>(typeof(HistoricDetailEventEntity));
                //foreach (HistoricDetailEventEntity historicDetail in cachedHistoricDetails)
                //{
                //    // make sure we only delete the right ones (as we cannot make a proper query in the cache)
                //    if ((historicProcessInstanceId != null && historicProcessInstanceId.Equals(historicDetail.ProcessInstanceId)) || (historicCaseInstanceId != null && historicCaseInstanceId.Equals(historicDetail.CaseInstanceId)))
                //    {
                //        historicDetail.Delete();
                //    }
                //}
            }
        }

        public virtual IList<IHistoricDetail> FindHistoricDetailsByProcessInstanceId(string processInstanceId)
        {
            //return ListExt.ConvertToListT<IHistoricDetail>(DbEntityManager.SelectList("selectHistoricDetailsByProcessInstanceId", processInstanceId)) ;
            return Find(m => m.ProcessInstanceId == processInstanceId).ToList() as IList<IHistoricDetail>;
        }

        public virtual IList<IHistoricDetail> FindHistoricDetailsByCaseInstanceId(string caseInstanceId)
        {
            //return ListExt.ConvertToListT<IHistoricDetail>(DbEntityManager.SelectList("selectHistoricDetailsByCaseInstanceId", caseInstanceId));
            return Find(m => m.CaseInstanceId == caseInstanceId).ToList() as IList<IHistoricDetail>;
        }

        //        public virtual long FindHistoricDetailCountByQueryCriteria(HistoricDetailQueryImpl historicVariableUpdateQuery)
        //        {
        //            ConfigureQuery(historicVariableUpdateQuery);
        //            return (long) DbEntityManager.SelectOne("selectHistoricDetailCountByQueryCriteria", historicVariableUpdateQuery);
        //        }

        ////JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        ////ORIGINAL LINE: @SuppressWarnings("unchecked") public java.Util.List<org.camunda.bpm.engine.history.HistoricDetail> findHistoricDetailsByQueryCriteria(org.camunda.bpm.engine.impl.HistoricDetailQueryImpl historicVariableUpdateQuery, org.camunda.bpm.engine.impl.Page page)
        //        public virtual IList<IHistoricDetail> FindHistoricDetailsByQueryCriteria(HistoricDetailQueryImpl historicVariableUpdateQuery, Page page)
        //        {
        //            ConfigureQuery(historicVariableUpdateQuery);
        //            return ListExt.ConvertToListT<IHistoricDetail>(DbEntityManager.SelectList("selectHistoricDetailsByQueryCriteria", historicVariableUpdateQuery, page));
        //        }

        public virtual void DeleteHistoricDetailsByTaskId(string taskId)
        {
            if (HistoryEnabled)
            {
                // delete entries in DB
                IList<IHistoricDetail> historicDetails = FindHistoricDetailsByTaskId(taskId);

                foreach (IHistoricDetail historicDetail in historicDetails)
                {
                    ((HistoricDetailEventEntity)historicDetail).Delete();
                }
                //throw new System.NotImplementedException("查询缓存列表");
                //TODO 不需要再次清理缓存？delete entries in Cache
                //IList<HistoricDetailEventEntity> cachedHistoricDetails = DbEntityManager.GetCachedEntitiesByType<HistoricDetailEventEntity>(typeof(HistoricDetailEventEntity));
                //foreach (HistoricDetailEventEntity historicDetail in cachedHistoricDetails)
                //{
                //    // make sure we only delete the right ones (as we cannot make a proper query in the cache)
                //    if (taskId.Equals(historicDetail.TaskId))
                //    {
                //        historicDetail.Delete();
                //    }
                //}
            }
        }

        public virtual IList<IHistoricDetail> FindHistoricDetailsByTaskId(string taskId)
        {
            //return ListExt.ConvertToListT<IHistoricDetail>(DbEntityManager.SelectList("selectHistoricDetailsByTaskId", taskId));
            return Find(m => m.TaskId == taskId).ToList().Cast<IHistoricDetail>().ToList();
        }

        //protected internal virtual void ConfigureQuery(HistoricDetailQueryImpl query)
        //{
        //    AuthorizationManager.ConfigureHistoricDetailQuery(query);
        //    TenantManager.ConfigureQuery(query);
        //}

    }

}
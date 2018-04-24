using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Autofac.Features.AttributeFilters;

using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using Microsoft.Extensions.Logging;
using ESS.FW.DataAccess;
using ESS.FW.Common.Components;
using ESS.FW.Bpm.Engine.History.Impl.Event;

namespace ESS.FW.Bpm.Engine.Persistence.Entity.Impl
{
    /// <summary>
	/// 
	/// </summary>
    [Component]
    public class HistoricCaseActivityInstanceManager : AbstractHistoricManagerNet<HistoricCaseActivityInstanceEventEntity>, IHistoricCaseActivityInstanceManager
    {
        public HistoricCaseActivityInstanceManager(DbContext dbContex, ILoggerFactory loggerFactory, IDGenerator idGenerator) : base(dbContex, loggerFactory, idGenerator)
        {
        }

        public virtual void DeleteHistoricCaseActivityInstancesByCaseInstanceId(string historicCaseInstanceId)
        {
            if (HistoryEnabled)
            {
                //DbEntityManager.Delete(typeof(HistoricCaseActivityInstanceEntity), "deleteHistoricCaseActivityInstancesByCaseInstanceId", historicCaseInstanceId);
                Delete(m => m.CaseInstanceId == historicCaseInstanceId);
            }
        }

        public virtual void InsertHistoricCaseActivityInstance(HistoricCaseActivityInstanceEventEntity historicCaseActivityInstance)
        {
            Add(historicCaseActivityInstance);
        }

        public virtual HistoricCaseActivityInstanceEventEntity FindHistoricCaseActivityInstance(string caseActivityId, string caseInstanceId)
        {
            //IDictionary<string, string> parameters = new Dictionary<string, string>();
            //parameters["caseActivityId"] = caseActivityId;
            //parameters["caseInstanceId"] = caseInstanceId;

            //return (HistoricCaseActivityInstanceEntity) DbEntityManager.SelectOne("selectHistoricCaseActivityInstance", parameters);
            return Single(m => m.CaseActivityId == caseActivityId && m.CaseInstanceId == caseInstanceId);
        }

        // public virtual long FindHistoricCaseActivityInstanceCountByQueryCriteria(HistoricCaseActivityInstanceQueryImpl historicCaseActivityInstanceQuery)
        // {
        //ConfigureHistoricCaseActivityInstanceQuery(historicCaseActivityInstanceQuery);
        //return (long) DbEntityManager.SelectOne("selectHistoricCaseActivityInstanceCountByQueryCriteria", historicCaseActivityInstanceQuery);
        // }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") public java.Util.List<org.camunda.bpm.engine.history.HistoricCaseActivityInstance> findHistoricCaseActivityInstancesByQueryCriteria(org.camunda.bpm.engine.impl.HistoricCaseActivityInstanceQueryImpl historicCaseActivityInstanceQuery, org.camunda.bpm.engine.impl.Page page)
        // public virtual IList<IHistoricCaseActivityInstance> FindHistoricCaseActivityInstancesByQueryCriteria(HistoricCaseActivityInstanceQueryImpl historicCaseActivityInstanceQuery, Page page)
        // {
        //ConfigureHistoricCaseActivityInstanceQuery(historicCaseActivityInstanceQuery);
        //return DbEntityManager.SelectList("selectHistoricCaseActivityInstancesByQueryCriteria", historicCaseActivityInstanceQuery, page);
        // }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") public java.Util.List<org.camunda.bpm.engine.history.HistoricCaseActivityInstance> findHistoricCaseActivityInstancesByNativeQuery(java.Util.Map<String, Object> parameterMap, int firstResult, int maxResults)
        public virtual IList<IHistoricCaseActivityInstance> FindHistoricCaseActivityInstancesByNativeQuery(IDictionary<string, object> parameterMap, int firstResult, int maxResults)
        {
            //return DbEntityManager.SelectListWithRawParameter("selectHistoricCaseActivityInstanceByNativeQuery", parameterMap, firstResult, maxResults);
            throw new NotImplementedException();
            //return DbEntityManager.SelectList(m=>m.)
        }

        public virtual long FindHistoricCaseActivityInstanceCountByNativeQuery(IDictionary<string, object> parameterMap)
        {
            throw new NotImplementedException();
            //return (long)DbEntityManager.SelectOne("selectHistoricCaseActivityInstanceCountByNativeQuery", parameterMap);
        }

        // protected internal virtual void ConfigureHistoricCaseActivityInstanceQuery(HistoricCaseActivityInstanceQueryImpl query)
        // {
        //TenantManager.ConfigureQuery(query);
        // }
    }

}
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
	/// 
	/// </summary>
    [Component]
	public class HistoricIncidentManager : AbstractHistoricManagerNet<HistoricIncidentEntity>, IHistoricIncidentManager
    {
        public HistoricIncidentManager(DbContext dbContex, ILoggerFactory loggerFactory, IDGenerator idGenerator) : base(dbContex, loggerFactory, idGenerator)
        {
        }

        // public virtual long FindHistoricIncidentCountByQueryCriteria(HistoricIncidentQueryImpl query)
        // {
        //ConfigureQuery(query);
        //return (long) DbEntityManager.SelectOne("selectHistoricIncidentCountByQueryCriteria", query);
        // }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") public java.Util.List<org.camunda.bpm.engine.history.HistoricIncident> findHistoricIncidentByQueryCriteria(org.camunda.bpm.engine.impl.HistoricIncidentQueryImpl query, org.camunda.bpm.engine.impl.Page page)
        // public virtual IList<IHistoricIncident> FindHistoricIncidentByQueryCriteria(HistoricIncidentQueryImpl query, Page page)
        // {
        //ConfigureQuery(query);
        //return DbEntityManager.SelectList("selectHistoricIncidentByQueryCriteria", query, page);
        // }

        public virtual void DeleteHistoricIncidentsByProcessInstanceId(string processInstanceId)
        {
            if (HistoryLevelFullEnabled)
            {
                //DbEntityManager.Delete(typeof(HistoricIncidentEntity), "deleteHistoricIncidentsByProcessInstanceId", processInstanceId);
                Delete(m => m.ProcessInstanceId == processInstanceId);
            }
        }

        public virtual void DeleteHistoricIncidentsByProcessDefinitionId(string processDefinitionId)
        {
            if (HistoryLevelFullEnabled)
            {
                //DbEntityManager.Delete(typeof(HistoricIncidentEntity), "deleteHistoricIncidentsByProcessDefinitionId", processDefinitionId);
                Delete(m => m.ProcessDefinitionId == processDefinitionId);
            }
        }

        public virtual void DeleteHistoricIncidentsByJobDefinitionId(string jobDefinitionId)
        {
            if (HistoryLevelFullEnabled)
            {
                //DbEntityManager.Delete(typeof(HistoricIncidentEntity), "deleteHistoricIncidentsByJobDefinitionId", jobDefinitionId);
                Delete(m => m.JobDefinitionId == jobDefinitionId);
            }
        }

        // protected internal virtual void ConfigureQuery(HistoricIncidentQueryImpl query)
        // {
        //AuthorizationManager.ConfigureHistoricIncidentQuery(query);
        //TenantManager.ConfigureQuery(query);
        // }

    }

}
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
    public class HistoricIdentityLinkLogManager : AbstractHistoricManagerNet<HistoricIdentityLinkLogEventEntity>, IHistoricIdentityLinkLogManager
    {
        public HistoricIdentityLinkLogManager(DbContext dbContex, ILoggerFactory loggerFactory, IDGenerator idGenerator) : base(dbContex, loggerFactory, idGenerator)
        {
        }

        // public virtual long FindHistoricIdentityLinkLogCountByQueryCriteria(HistoricIdentityLinkLogQueryImpl query)
        // {
        //ConfigureQuery(query);
        ////return (long) DbEntityManager.SelectOne("selectHistoricIdentityLinkCountByQueryCriteria", query);
        //          throw new System.NotImplementedException();
        // }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") public java.Util.List<org.camunda.bpm.engine.history.HistoricIdentityLinkLog> findHistoricIdentityLinkLogByQueryCriteria(org.camunda.bpm.engine.impl.HistoricIdentityLinkLogQueryImpl query, org.camunda.bpm.engine.impl.Page page)
        // public virtual IList<IHistoricIdentityLinkLog> FindHistoricIdentityLinkLogByQueryCriteria(HistoricIdentityLinkLogQueryImpl query, Page page)
        // {
        //ConfigureQuery(query);
        //return DbEntityManager.SelectList("selectHistoricIdentityLinkByQueryCriteria", query, page);
        // }

        public virtual void DeleteHistoricIdentityLinksLogByProcessDefinitionId(string processDefId)
        {
            if (HistoryLevelFullEnabled)
            {
                //DbEntityManager.Delete(typeof(HistoricIdentityLinkLogEntity), "deleteHistoricIdentityLinksByProcessDefinitionId", processDefId);
                Delete(m => m.ProcessDefinitionId == processDefId);
            }
        }

        public virtual void DeleteHistoricIdentityLinksLogByTaskId(string taskId)
        {
            if (HistoryLevelFullEnabled)
            {
                //DbEntityManager.Delete(typeof(HistoricIdentityLinkLogEntity), "deleteHistoricIdentityLinksByTaskId", taskId);
                Delete(m => m.TaskId == taskId);
            }
        }

        // protected internal virtual void ConfigureQuery(HistoricIdentityLinkLogQueryImpl query)
        // {
        //AuthorizationManager.ConfigureHistoricIdentityLinkQuery(query);
        //TenantManager.ConfigureQuery(query);
        // }

    }

}
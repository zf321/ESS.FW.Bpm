using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.Interceptor;

namespace ESS.FW.Bpm.Engine.Persistence.Entity.Impl
{ 
    
    /// 
    /// <summary>
    /// 
    /// 
    /// </summary>
    [System.Obsolete("")]
    public class HistoricStatisticsManager : AbstractManager
    {

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.Util.List<org.camunda.bpm.engine.history.HistoricActivityStatistics> getHistoricStatisticsGroupedByActivity(org.camunda.bpm.engine.impl.HistoricActivityStatisticsQueryImpl query, org.camunda.bpm.engine.impl.Page page)
        //public virtual IList<IHistoricActivityStatistics> GetHistoricStatisticsGroupedByActivity(HistoricActivityStatisticsQueryImpl query, Page page)
        //{
        //    if (EnsureHistoryReadOnProcessDefinition(query))
        //    {
        //        return ListExt.ConvertToListT<IHistoricActivityStatistics>(DbEntityManager.SelectList("selectHistoricActivityStatistics", query, page)) ;
        //    }
        //    else
        //    {
        //        return new List<IHistoricActivityStatistics>();
        //    }
        //}

        //public virtual long GetHistoricStatisticsCountGroupedByActivity(HistoricActivityStatisticsQueryImpl query)
        //{
        //    if (EnsureHistoryReadOnProcessDefinition(query))
        //    {
        //        return (long) DbEntityManager.SelectOne("selectHistoricActivityStatisticsCount", query);
        //    }
        //    else
        //    {
        //        return 0;
        //    }
        //}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.Util.List<org.camunda.bpm.engine.history.HistoricCaseActivityStatistics> getHistoricStatisticsGroupedByCaseActivity(org.camunda.bpm.engine.impl.HistoricCaseActivityStatisticsQueryImpl query, org.camunda.bpm.engine.impl.Page page)
        //public virtual IList<IHistoricCaseActivityStatistics> GetHistoricStatisticsGroupedByCaseActivity(HistoricCaseActivityStatisticsQueryImpl query, Page page)
        //{
        //    return ListExt.ConvertToListT<IHistoricCaseActivityStatistics>(DbEntityManager.SelectList("selectHistoricCaseActivityStatistics", query, page)) ;
        //}

        //public virtual long GetHistoricStatisticsCountGroupedByCaseActivity(HistoricCaseActivityStatisticsQueryImpl query)
        //{
        //    return (long) DbEntityManager.SelectOne("selectHistoricCaseActivityStatisticsCount", query);
        //}

        //protected internal virtual bool EnsureHistoryReadOnProcessDefinition(HistoricActivityStatisticsQueryImpl query)
        //{
        //    CommandContext commandContext = CommandContext;

        //    if (AuthorizationEnabled && CurrentAuthentication != null && commandContext.AuthorizationCheckEnabled)
        //    {
        //        string processDefinitionId = query.ProcessDefinitionId;
        //        ProcessDefinitionEntity definition = ProcessDefinitionManager.FindLatestProcessDefinitionById(processDefinitionId);

        //        if (definition == null)
        //        {
        //            return false;
        //        }

        //        return AuthorizationManager.IsAuthorized(Permissions.ReadHistory, Resources.ProcessDefinition, definition.Key);
        //    }

        //    return true;
        //}

    }

}
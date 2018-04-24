using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Autofac.Features.AttributeFilters;

using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Impl;
using Microsoft.Extensions.Logging;
using ESS.FW.DataAccess;
using ESS.FW.Common.Components;
using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.Bpm.Engine.Impl.Cfg;

namespace ESS.FW.Bpm.Engine.Persistence.Entity.Impl
{
    /// <summary>
	///  
	/// </summary>
    [Component]
    public class HistoricActivityInstanceManager : AbstractHistoricManagerNet<HistoricActivityInstanceEventEntity>, IHistoricActivityInstanceManager
    {
        public HistoricActivityInstanceManager(DbContext dbContex, ILoggerFactory loggerFactory, IDGenerator idGenerator) : base(dbContex, loggerFactory, idGenerator)
        {
        }

        public virtual void DeleteHistoricActivityInstancesByProcessInstanceId(string historicProcessInstanceId)
        {
            if (HistoryEnabled)
            {
                //DbEntityManager.Delete(typeof(HistoricActivityInstanceEntity), "deleteHistoricActivityInstancesByProcessInstanceId", historicProcessInstanceId);
                Delete(m => m.ProcessInstanceId == historicProcessInstanceId);
            }
        }

        public virtual void InsertHistoricActivityInstance(HistoricActivityInstanceEventEntity historicActivityInstance)
        {
            Add(historicActivityInstance);
        }

        public virtual HistoricActivityInstanceEventEntity FindHistoricActivityInstance(string activityId, string processInstanceId)
        {
            //IDictionary<string, string> parameters = new Dictionary<string, string>();
            //parameters["activityId"] = activityId;
            //parameters["processInstanceId"] = processInstanceId;

            //return (HistoricActivityInstanceEntity) DbEntityManager.SelectOne("selectHistoricActivityInstance", parameters);
            return Single(m => m.ActivityId == activityId && m.ProcessInstanceId == processInstanceId);
        }

        //	  public virtual long FindHistoricActivityInstanceCountByQueryCriteria(HistoricActivityInstanceQueryImpl historicActivityInstanceQuery)
        //	  {
        //		ConfigureQuery(historicActivityInstanceQuery);
        //            //return (long) DbEntityManager.SelectOne("selectHistoricActivityInstanceCountByQueryCriteria", historicActivityInstanceQuery);
        //            //          return DbEntityManager.SelectOne(m=>m.)
        //            throw new System.NotImplementedException();
        //	  }

        ////JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        ////ORIGINAL LINE: @SuppressWarnings("unchecked") public java.Util.List<org.camunda.bpm.engine.history.HistoricActivityInstance> findHistoricActivityInstancesByQueryCriteria(org.camunda.bpm.engine.impl.HistoricActivityInstanceQueryImpl historicActivityInstanceQuery, org.camunda.bpm.engine.impl.Page page)
        //	  public virtual IList<IHistoricActivityInstance> FindHistoricActivityInstancesByQueryCriteria(HistoricActivityInstanceQueryImpl historicActivityInstanceQuery, Page page)
        //	  {
        //		ConfigureQuery(historicActivityInstanceQuery);
        //		//return ListExt.ConvertToListT<IHistoricActivityInstance>(DbEntityManager.SelectList("selectHistoricActivityInstancesByQueryCriteria", historicActivityInstanceQuery, page)) ;
        //            throw new System.NotImplementedException();
        //	  }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") public java.Util.List<org.camunda.bpm.engine.history.HistoricActivityInstance> findHistoricActivityInstancesByNativeQuery(java.Util.Map<String, Object> parameterMap, int firstResult, int maxResults)
        public virtual IList<IHistoricActivityInstance> FindHistoricActivityInstancesByNativeQuery(IDictionary<string, object> parameterMap, int firstResult, int maxResults)
        {
            //return ListExt.ConvertToListT<IHistoricActivityInstance>(DbEntityManager.SelectListWithRawParameter("selectHistoricActivityInstanceByNativeQuery", parameterMap, firstResult, maxResults));
            throw new System.NotImplementedException();
        }

        public virtual long FindHistoricActivityInstanceCountByNativeQuery(IDictionary<string, object> parameterMap)
        {
            //return (long) DbEntityManager.SelectOne("selectHistoricActivityInstanceCountByNativeQuery", parameterMap);
            throw new System.NotImplementedException();
        }

        // protected internal virtual void ConfigureQuery(HistoricActivityInstanceQueryImpl query)
        // {
        //AuthorizationManager.ConfigureHistoricActivityInstanceQuery(query);
        //TenantManager.ConfigureQuery(query);
        // }

    }

}
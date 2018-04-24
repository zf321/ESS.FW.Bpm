using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using System.Linq;
using Autofac.Features.AttributeFilters;

using ESS.FW.Bpm.Engine.Impl.Cfg;
using Microsoft.Extensions.Logging;
using ESS.FW.DataAccess;
using ESS.FW.Common.Components;

namespace ESS.FW.Bpm.Engine.Persistence.Entity.Impl
{


    /// <summary>
    /// 
    /// </summary>
    [Component]
    public class HistoricCaseInstanceManager : AbstractHistoricManagerNet<HistoricCaseInstanceEventEntity>, IHistoricCaseInstanceManager
    {
        public HistoricCaseInstanceManager(DbContext dbContex, ILoggerFactory loggerFactory, IDGenerator idGenerator) : base(dbContex, loggerFactory, idGenerator)
        {
        }

        public virtual HistoricCaseInstanceEventEntity FindHistoricCaseInstance(string caseInstanceId)
        {
            if (HistoryEnabled)
            {
                //return DbEntityManager.SelectById< HistoricCaseInstanceEntity>(typeof(HistoricCaseInstanceEntity), caseInstanceId);
                return  Get(caseInstanceId);
            }
            return null;
        }

        public virtual HistoricCaseInstanceEventEntity FindHistoricCaseInstanceEvent(string eventId)
        {
            if (HistoryEnabled)
            {
                //return DbEntityManager.SelectById< HistoricCaseInstanceEventEntity>(typeof(HistoricCaseInstanceEventEntity), eventId);
                return Get(eventId);
            }
            return null;
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") public void deleteHistoricCaseInstanceByCaseDefinitionId(String caseDefinitionId)
        public virtual void DeleteHistoricCaseInstanceByCaseDefinitionId(string caseDefinitionId)
        {
            if (HistoryEnabled)
            {
                //IList<string> historicCaseInstanceIds =ListExt.ConvertToListT<string>(DbEntityManager.SelectList("selectHistoricCaseInstanceIdsByCaseDefinitionId", caseDefinitionId));
                IList<string> historicCaseInstanceIds = Find(m => m.CaseDefinitionId == caseDefinitionId).Select(m => m.CaseInstanceId).ToList();

                foreach (string historicCaseInstanceId in historicCaseInstanceIds)
                {
                    DeleteHistoricCaseInstanceById(historicCaseInstanceId);
                }
            }
        }

        public virtual void DeleteHistoricCaseInstanceById(string historicCaseInstanceId)
        {
            if (HistoryEnabled)
            {
                CommandContext commandContext = context.Impl.Context.CommandContext;

                commandContext.HistoricDetailManager.DeleteHistoricDetailsByCaseInstanceId(historicCaseInstanceId);

                commandContext.HistoricVariableInstanceManager.DeleteHistoricVariableInstanceByCaseInstanceId(historicCaseInstanceId);

                commandContext.HistoricCaseActivityInstanceManager.DeleteHistoricCaseActivityInstancesByCaseInstanceId(historicCaseInstanceId);

                commandContext.HistoricTaskInstanceManager.DeleteHistoricTaskInstancesByCaseInstanceId(historicCaseInstanceId);

                //commandContext.HistoricCaseInstanceManager.Delete(typeof(HistoricCaseInstanceEntity), "deleteHistoricCaseInstance", historicCaseInstanceId);

            }
        }

        //public virtual long FindHistoricCaseInstanceCountByQueryCriteria(HistoricCaseInstanceQueryImpl historicCaseInstanceQuery)
        //{
        //    if (HistoryEnabled)
        //    {
        //        ConfigureHistoricCaseInstanceQuery(historicCaseInstanceQuery);
        //        //return (long)DbEntityManager.SelectOne("selectHistoricCaseInstanceCountByQueryCriteria", historicCaseInstanceQuery);
        //        throw new System.NotImplementedException();
        //    }
        //    return 0;
        //}

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") public java.Util.List<org.camunda.bpm.engine.history.HistoricCaseInstance> findHistoricCaseInstancesByQueryCriteria(org.camunda.bpm.engine.impl.HistoricCaseInstanceQueryImpl historicCaseInstanceQuery, org.camunda.bpm.engine.impl.Page page)
        //public virtual IList<IHistoricCaseInstance> FindHistoricCaseInstancesByQueryCriteria(HistoricCaseInstanceQueryImpl historicCaseInstanceQuery, Page page)
        //{
        //    if (HistoryEnabled)
        //    {
        //        ConfigureHistoricCaseInstanceQuery(historicCaseInstanceQuery);
        //        //return ListExt.ConvertToListT<IHistoricCaseInstance>(DbEntityManager.SelectList("selectHistoricCaseInstancesByQueryCriteria", historicCaseInstanceQuery, page));
        //        throw new System.NotImplementedException();
        //    }
        //    return new List<IHistoricCaseInstance>();
        //}

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") public java.Util.List<org.camunda.bpm.engine.history.HistoricCaseInstance> findHistoricCaseInstancesByNativeQuery(java.Util.Map<String, Object> parameterMap, int firstResult, int maxResults)
        public virtual IList<IHistoricCaseInstance> FindHistoricCaseInstancesByNativeQuery(IDictionary<string, object> parameterMap, int firstResult, int maxResults)
        {
            //return ListExt.ConvertToListT<IHistoricCaseInstance>(DbEntityManager.SelectListWithRawParameter("selectHistoricCaseInstanceByNativeQuery", parameterMap, firstResult, maxResults));
            throw new System.NotImplementedException();
        }

        public virtual long FindHistoricCaseInstanceCountByNativeQuery(IDictionary<string, object> parameterMap)
        {
            //return (long)DbEntityManager.SelectOne("selectHistoricCaseInstanceCountByNativeQuery", parameterMap);
            throw new System.NotImplementedException();
        }

        //protected internal virtual void ConfigureHistoricCaseInstanceQuery(HistoricCaseInstanceQueryImpl query)
        //{
        //    TenantManager.ConfigureQuery(query);
        //}

    }

}
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.History;
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

    //JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to .NET:
    //import

    //static org.camunda.bpm.engine.impl.Util.EnsureUtil.ensureOnlyOneNotNull;

    //using HistoricVariableInstance = org.camunda.bpm.engine.history.HistoricVariableInstance;
    //using HistoricVariableInstanceQuery = org.camunda.bpm.engine.history.HistoricVariableInstanceQuery;


    /// <summary>
    /// 
    /// </summary>
    [Component]
    public class HistoricVariableInstanceManager : AbstractHistoricManagerNet<HistoricVariableInstanceEntity>, IHistoricVariableInstanceManager
    {
        protected IVariableInstanceManager variableInstanceManager;
        public HistoricVariableInstanceManager(DbContext dbContex, ILoggerFactory loggerFactory, IVariableInstanceManager _variableInstanceManager, IDGenerator idGenerator) : base(dbContex, loggerFactory, idGenerator)
        {
            variableInstanceManager = _variableInstanceManager;
        }

        public virtual void DeleteHistoricVariableInstanceByProcessInstanceId(string historicProcessInstanceId)
        {
            DeleteHistoricVariableInstancesByProcessCaseInstanceId(historicProcessInstanceId, null);
        }

        public virtual void DeleteHistoricVariableInstanceByCaseInstanceId(string historicCaseInstanceId)
        {
            DeleteHistoricVariableInstancesByProcessCaseInstanceId(null, historicCaseInstanceId);
        }

        protected internal virtual void DeleteHistoricVariableInstancesByProcessCaseInstanceId(string historicProcessInstanceId, string historicCaseInstanceId)
        {
            EnsureUtil.EnsureOnlyOneNotNull("Only the process instance or case instance id should be set", historicProcessInstanceId, historicCaseInstanceId);
            if (HistoryEnabled)
            {
                //throw new NotImplementedException();
                // delete entries in DB
                IList<IHistoricVariableInstance> historicVariableInstances;
                if (historicProcessInstanceId != null)
                {
                    historicVariableInstances = FindHistoricVariableInstancesByProcessInstanceId(historicProcessInstanceId);
                }
                else
                {
                    historicVariableInstances = FindHistoricVariableInstancesByCaseInstanceId(historicCaseInstanceId);
                }
                if (historicVariableInstances != null)
                {
                    foreach (IHistoricVariableInstance historicVariableInstance in historicVariableInstances)
                    {
                        ((HistoricVariableInstanceEntity)historicVariableInstance).Delete();
                    }
                }
                

                //// delete entries in Cache
                //IList<HistoricVariableInstanceEntity> cachedHistoricVariableInstances = DbEntityManager.GetCachedEntitiesByType(typeof(HistoricVariableInstanceEntity));
                //foreach (HistoricVariableInstanceEntity historicVariableInstance in cachedHistoricVariableInstances)
                //{
                //    // make sure we only delete the right ones (as we cannot make a proper query in the cache)
                //    if ((historicProcessInstanceId != null && historicProcessInstanceId.Equals(historicVariableInstance.ProcessInstanceId)) || (historicCaseInstanceId != null && historicCaseInstanceId.Equals(historicVariableInstance.CaseInstanceId)))
                //    {
                //        historicVariableInstance.Delete();
                //    }
                //}
            }
        }
        
        public virtual IList<IHistoricVariableInstance> FindHistoricVariableInstancesByProcessInstanceId(string processInstanceId)
        {
            //return ListExt.ConvertToListT<IHistoricVariableInstance>(DbEntityManager.SelectList("selectHistoricVariablesByProcessInstanceId", processInstanceId)) ;
            return Find(m => m.ProcessInstanceId == processInstanceId).ToList().Cast<IHistoricVariableInstance>().ToList();
        }
        
        public virtual IList<IHistoricVariableInstance> FindHistoricVariableInstancesByCaseInstanceId(string caseInstanceId)
        {
            //return ListExt.ConvertToListT<IHistoricVariableInstance>(DbEntityManager.SelectList("selectHistoricVariablesByCaseInstanceId", caseInstanceId));
            return Find(m => m.CaseInstanceId == caseInstanceId).ToList().Cast<IHistoricVariableInstance>().ToList();
        }

        //public virtual long FindHistoricVariableInstanceCountByQueryCriteria(HistoricVariableInstanceQueryImpl historicProcessVariableQuery)
        //{
        //    ConfigureQuery(historicProcessVariableQuery);
        //    //return (long) DbEntityManager.SelectOne("selectHistoricVariableInstanceCountByQueryCriteria", historicProcessVariableQuery);
        //    throw new NotImplementedException();
        //}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.Util.List<org.camunda.bpm.engine.history.HistoricVariableInstance> findHistoricVariableInstancesByQueryCriteria(org.camunda.bpm.engine.impl.HistoricVariableInstanceQueryImpl historicProcessVariableQuery, org.camunda.bpm.engine.impl.Page page)
        //public virtual IList<IHistoricVariableInstance> FindHistoricVariableInstancesByQueryCriteria(HistoricVariableInstanceQueryImpl historicProcessVariableQuery, Page page)
        //{
        //    ConfigureQuery(historicProcessVariableQuery);
        //    //return ListExt.ConvertToListT<IHistoricVariableInstance>(DbEntityManager.SelectList("selectHistoricVariableInstanceByQueryCriteria", historicProcessVariableQuery, page)) ;
        //    throw new NotImplementedException();
        //}

        public virtual HistoricVariableInstanceEntity FindHistoricVariableInstanceByVariableInstanceId(string variableInstanceId)
        {
            //return (HistoricVariableInstanceEntity) DbEntityManager.SelectOne("selectHistoricVariableInstanceByVariableInstanceId", variableInstanceId);
            var varIns =variableInstanceManager.FindVariableInstancesById(variableInstanceId);

            return Single(m => m.VariableName == varIns.Name && m.VariableTypeName == varIns.TypeName);
        }

        public virtual void DeleteHistoricVariableInstancesByTaskId(string taskId)
        {
            
            if (HistoryEnabled)
            {
                //IQueryable<IHistoricVariableInstance> historicProcessVariableQuery = (new HistoricVariableInstanceQueryImpl()).TaskIdIn(taskId);
                //IList<IHistoricVariableInstance> historicProcessVariables = historicProcessVariableQuery.list();
                //foreach (IHistoricVariableInstance historicProcessVariable in historicProcessVariables)
                //{
                //    ((HistoricVariableInstanceEntity) historicProcessVariable).delete();
                //}
                Delete(m => m.TaskId == taskId);
            }
        }

        //protected internal virtual void ConfigureQuery(HistoricVariableInstanceQueryImpl query)
        //{
        //    AuthorizationManager.ConfigureHistoricVariableInstanceQuery(query);
        //    TenantManager.ConfigureQuery(query);
        //}

    }
}
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Autofac.Features.AttributeFilters;

using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.DB;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Common.Components;
using Microsoft.Extensions.Logging;
using ESS.FW.Common.Utilities;
using ESS.FW.DataAccess;
using ESS.FW.Common.Extensions;

namespace ESS.FW.Bpm.Engine.Persistence.Entity.Impl
{
    /// <summary>
    ///     
    /// </summary>
    [Component]
    public class ExternalTaskManager : AbstractManagerNet<ExternalTaskEntity>, IExternalTaskManager
    {

        public ExternalTaskManager(DbContext dbContex, ILoggerFactory loggerFactory, IDGenerator idGenerator)
            : base(dbContex, loggerFactory, idGenerator)
        {
        }

        public virtual ExternalTaskEntity FindExternalTaskById(string id)
        {
            //return DbEntityManager.SelectById< ExternalTaskEntity>(typeof(ExternalTaskEntity), id);
            return Get(id);
        }

        public override void Delete(ExternalTaskEntity externalTask)
        {
            Delete(externalTask);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") public java.Util.List<ExternalTaskEntity> findExternalTasksByExecutionId(String id)
        public virtual IList<ExternalTaskEntity> FindExternalTasksByExecutionId(string id)
        {
            //return ListExt.ConvertToListT<ExternalTaskEntity>(DbEntityManager.SelectList("selectExternalTasksByExecutionId", id)) ;
            return Find(m => m.ExecutionId == id)
                .ToList();
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") public java.Util.List<ExternalTaskEntity> findExternalTasksByProcessInstanceId(String processInstanceId)
        public virtual IList<ExternalTaskEntity> FindExternalTasksByProcessInstanceId(string processInstanceId)
        {
            //return ListExt.ConvertToListT<ExternalTaskEntity>(DbEntityManager.SelectList("selectExternalTasksByExecutionId", processInstanceId));
            return Find(m => m.ProcessInstanceId == processInstanceId)
                .ToList();
        }

        public virtual IList<ExternalTaskEntity> SelectExternalTasksForTopics(ICollection<string> topics, int maxResults, bool usePriority)
        {
            //if (topics.Count == 0)
            //    return new List<ExternalTaskEntity>();

            //IDictionary<string, object> parameters = new Dictionary<string, object>();
            //parameters["topics"] = topics;
            //parameters["now"] = ClockUtil.CurrentTime;
            //parameters["applyOrdering"] = usePriority;
            //IList<QueryOrderingProperty> orderingProperties = new List<QueryOrderingProperty>();
            //orderingProperties.Add(ExtTaskPriorityOrderingProperty);
            //parameters["orderingProperties"] = orderingProperties;

            //var parameter = new ListQueryParameterObject(parameters, 0, maxResults);
            //ConfigureQuery(parameter);
            //DbEntityManager manager = DbEntityManager;
            //return ListExt.ConvertToListT<ExternalTaskEntity>(manager.SelectList("selectExternalTasksForTopics", parameter));

            throw new NotImplementedException();
        }

        public virtual void UpdateExternalTaskSuspensionStateByProcessInstanceId(string processInstanceId,
            ISuspensionState suspensionState)
        {
            UpdateExternalTaskSuspensionState(processInstanceId, null, null, suspensionState);
        }

        public virtual void UpdateExternalTaskSuspensionStateByProcessDefinitionId(string processDefinitionId,
            ISuspensionState suspensionState)
        {
            UpdateExternalTaskSuspensionState(null, processDefinitionId, null, suspensionState);
        }

        public virtual void UpdateExternalTaskSuspensionStateByProcessDefinitionKey(string processDefinitionKey,
            ISuspensionState suspensionState)
        {
            UpdateExternalTaskSuspensionState(null, null, processDefinitionKey, suspensionState);
        }

        public virtual void UpdateExternalTaskSuspensionStateByProcessDefinitionKeyAndTenantId(
            string processDefinitionKey, string processDefinitionTenantId, ISuspensionState suspensionState)
        {
            //IDictionary<string, object> parameters = new Dictionary<string, object>();
            //parameters["processDefinitionKey"] = processDefinitionKey;
            //parameters["isProcessDefinitionTenantIdSet"] = true;
            //parameters["processDefinitionTenantId"] = processDefinitionTenantId;
            //parameters["suspensionState"] = suspensionState.StateCode;
            //DbEntityManager.Update(typeof(ExternalTaskEity), "updateExternalTaskSuspensionStateByParameters", ConfigureParameterizedQuery(parameters));

            var predicateProc = ParameterBuilder.True<ProcessDefinitionEntity>();
            var predicateTask = ParameterBuilder.True<ExternalTaskEntity>();
            if (!string.IsNullOrEmpty(processDefinitionKey))
            {
                predicateProc = predicateProc.AndParam(c => c.Key == processDefinitionKey);
                if (!string.IsNullOrEmpty(processDefinitionTenantId))
                    predicateProc = predicateProc.AndParam(c => c.TenantId == processDefinitionTenantId);
                else
                    predicateProc = predicateProc.AndParam(c => c.TenantId == null);
                predicateTask = predicateTask.AndParam(c => DbContext.Set<ProcessDefinitionEntity>().Where(predicateProc).Select(p => p.Id).ToList().Contains(c.ProcessDefinitionId));
            }

            Find(predicateTask).ForEach(d =>
            {
                d.SuspensionState = suspensionState.StateCode;
            });
        }

        //public virtual IList<IExternalTask> FindExternalTasksByQueryCriteria(ExternalTaskQueryImpl externalTaskQuery)
        //{
        //    ConfigureQuery(externalTaskQuery);
        //    throw new System.NotImplementedException();
        //    return ListExt.ConvertToListT<IExternalTask>(DbEntityManager.SelectList("selectExternalTaskByQueryCriteria", externalTaskQuery));
        //}

        //public virtual long FindExternalTaskCountByQueryCriteria(ExternalTaskQueryImpl externalTaskQuery)
        //{
        //    ConfigureQuery(externalTaskQuery);
        //    throw new System.NotImplementedException();
        //    return (long)DbEntityManager.SelectOne("selectExternalTaskCountByQueryCriteria", externalTaskQuery);
        //}

        protected internal virtual void UpdateExternalTaskSuspensionState(string processInstanceId, string processDefinitionId, string processDefinitionKey, ISuspensionState suspensionState)
        {
            //IDictionary<string, object> parameters = new Dictionary<string, object>();
            //parameters["processInstanceId"] = processInstanceId;
            //parameters["processDefinitionId"] = processDefinitionId;
            //parameters["processDefinitionKey"] = processDefinitionKey;
            //parameters["isProcessDefinitionTenantIdSet"] = false;
            //parameters["suspensionState"] = suspensionState.StateCode;
            //DbEntityManager.Update(typeof(ExternalTaskEntity), "updateExternalTaskSuspensionStateByParameters", ConfigureParameterizedQuery(parameters));            

            var predicate = ParameterBuilder.True<ExternalTaskEntity>();
            if (!string.IsNullOrEmpty(processInstanceId))
                predicate = predicate.AndParam(c => c.ProcessInstanceId == processInstanceId);
            if (!string.IsNullOrEmpty(processDefinitionId))
                predicate = predicate.AndParam(c => c.ProcessDefinitionId == processDefinitionId);
            if (!string.IsNullOrEmpty(processDefinitionKey))
                predicate = predicate.AndParam(c => c.ProcessDefinitionKey == processDefinitionKey);

            Find(predicate).ForEach(d =>
            {
                d.SuspensionState = suspensionState.StateCode;
            });
        }

        // {

        // protected internal virtual ListQueryParameterObject ConfigureParameterizedQuery(object parameter)
        // }
        //TenantManager.ConfigureQuery(parameter);
        //AuthorizationManager.ConfigureExternalTaskFetch(parameter);
        // {

        // protected internal virtual void ConfigureQuery(ListQueryParameterObject parameter)
        // }
        //TenantManager.ConfigureQuery(query);
        //AuthorizationManager.ConfigureExternalTaskQuery(query);
        // {

        // protected internal virtual void ConfigureQuery(ExternalTaskQueryImpl query)
        //return TenantManager.ConfigureQuery(parameter);
        // }
    }
}
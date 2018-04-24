using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Autofac.Features.AttributeFilters;

using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.DB;
using ESS.FW.Bpm.Engine.Impl.Javax.EL;
using ESS.FW.Bpm.Engine.Runtime;
using Microsoft.Extensions.Logging;
using ESS.FW.DataAccess;
using ESS.FW.Common.Components;
using ESS.FW.Common.Utilities;
using System.Linq.Expressions;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Common.Extensions;

namespace ESS.FW.Bpm.Engine.Persistence.Entity.Impl
{
    /// <summary>
    ///     
    /// </summary>
    [Component]
    public class ExecutionManager : AbstractManagerNet<ExecutionEntity>, IExecutionManager
    {
        protected internal static readonly EnginePersistenceLogger Log = ProcessEngineLogger.PersistenceLogger;
        private readonly IHistoricProcessInstanceManager _historicProcessInstanceManager;
        private readonly ITaskManager _taskManager;
        

        public ExecutionManager(DbContext dbContex,
            ILoggerFactory loggerFactory,
            IHistoricProcessInstanceManager historicProcessInstanceManager,
            ITaskManager taskManager
            , IDGenerator idGenerator) : base(dbContex, loggerFactory, idGenerator)
        {
            this._historicProcessInstanceManager = historicProcessInstanceManager;
            this._taskManager = taskManager;
        }

        public virtual void InsertExecution(ExecutionEntity execution)
        {
            Add(execution);
            CreateDefaultAuthorizations(execution);
        }

        public virtual void DeleteExecution(ExecutionEntity execution)
        {
            Log.LogDebug("EF删除数据DeleteExecution:", execution.Id);
            Delete(execution);
            if (execution.IsProcessInstanceExecution)
                DeleteAuthorizations(Resources.ProcessInstance, execution.ProcessInstanceId);
        }

        public virtual void DeleteProcessInstancesByProcessDefinition(string processDefinitionId, string deleteReason,
            bool cascade, bool skipCustomListeners)
        {
            Log.LogDebug("EF删除数据DeleteProcessInstancesByProcessDefinition:", processDefinitionId);
            //IList<string> processInstanceIds = DbEntityManager.SelectList("selectProcessInstanceIdsByProcessDefinitionId", processDefinitionId);
            IList<string> processInstanceIds =
                Find(m => m.ProcessDefinitionId == processDefinitionId && string.IsNullOrEmpty(m.ParentId))
                    .Select(m => m.Id)
                    .ToList();
            foreach (var processInstanceId in processInstanceIds)
                DeleteProcessInstance(processInstanceId, deleteReason, cascade, skipCustomListeners);

            if (cascade)
                _historicProcessInstanceManager.DeleteHistoricProcessInstanceByProcessDefinitionId(processDefinitionId);
        }

        public virtual void DeleteProcessInstance(string processInstanceId, string deleteReason)
        {
            Log.LogDebug("EF删除数据DeleteProcessInstance:", processInstanceId);
            //IList<string> processInstanceIds = DbEntityManager.SelectList("selectProcessInstanceIdsByPr
            DeleteProcessInstance(processInstanceId, deleteReason, false, false);
        }

        public virtual void DeleteProcessInstance(string processInstanceId, string deleteReason, bool cascade,
            bool skipCustomListeners)
        {

            DeleteProcessInstance(processInstanceId, deleteReason, cascade, skipCustomListeners, false);
        }

        public virtual void DeleteProcessInstance(string processInstanceId, string deleteReason, bool cascade,
            bool skipCustomListeners, bool externallyTerminated)
        {
            var execution = FindExecutionById(processInstanceId);
            Log.LogDebug("EF删除数据 DeleteProcessInstance:", processInstanceId);
            if (execution == null)
                throw Log.RequestedProcessInstanceNotFoundException(processInstanceId);

            _taskManager.DeleteTasksByProcessInstanceId(processInstanceId, deleteReason, cascade, skipCustomListeners);

            //// delete the execution BEFORE we delete the history, otherwise we will produce orphan HistoricVariableInstance instances
            execution.DeleteCascade(deleteReason, skipCustomListeners, false, externallyTerminated);

            if (cascade)
                _historicProcessInstanceManager.DeleteHistoricProcessInstanceById(processInstanceId);
        }        

        public virtual ExecutionEntity FindSubProcessInstanceBySuperExecutionId(string superExecutionId)
        {
            //return (ExecutionEntity) DbEntityManager.SelectOne("selectSubProcessInstanceBySuperExecutionId", superExecutionId);
            return Single(m => m.SuperExecutionId == superExecutionId);
        }

        public virtual ExecutionEntity FindSubProcessInstanceBySuperCaseExecutionId(string superCaseExecutionId)
        {
            //return (ExecutionEntity) DbEntityManager.SelectOne("selectSubProcessInstanceBySuperCaseExecutionId", superCaseExecutionId);
            return Single(m => m.SuperExecutionId == superCaseExecutionId);
        }

        public virtual IList<ExecutionEntity> FindChildExecutionsByParentExecutionId(string parentExecutionId)
        {
            //return ListExt.ConvertToListT<ExecutionEntity>(DbEntityManager.SelectList("selectExecutionsByParentExecutionId", parentExecutionId));
            return Find(m => m.ParentId == parentExecutionId).ToList();
        }

        public virtual IList<ExecutionEntity> FindExecutionsByProcessInstanceId(string processInstanceId)
        {
            //return ListExt.ConvertToListT<ExecutionEntity>(DbEntityManager.SelectList("selectExecutionsByProcessInstanceId", processInstanceId));
            return Find(m => m.ProcessInstanceId == processInstanceId).ToList();
        }

        public virtual ExecutionEntity FindExecutionById(string executionId)
        {
            //return Get< ExecutionEntity>(typeof(ExecutionEntity), executionId);
            return Get(executionId);
        }

        //public virtual long FindExecutionCountByQueryCriteria(ExecutionQueryImpl executionQuery)
        //{
        //    ConfigureQuery(executionQuery);
        //    return (long)DbEntityManager.SelectOne("selectExecutionCountByQueryCriteria", executionQuery);
        //}

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") public java.Util.List<ExecutionEntity> findExecutionsByQueryCriteria(org.camunda.bpm.engine.impl.ExecutionQueryImpl executionQuery, org.camunda.bpm.engine.impl.Page page)
        //public virtual IList<ExecutionEntity> FindExecutionsByQueryCriteria(ExecutionQueryImpl executionQuery, Page page)
        //{
        //    ConfigureQuery(executionQuery);
        //    return ListExt.ConvertToListT<ExecutionEntity>(DbEntityManager.SelectList("selectExecutionsByQueryCriteria", executionQuery, page));
        //}

        public virtual long FindProcessInstanceCountByProcessDefinitionId(string processDefinitionId)
        {
            var datas = (from res in DbContext.Set<ExecutionEntity>()
                join p in DbContext.Set<ProcessDefinitionEntity>()
                on res.ProcessDefinitionId equals p.Id
                where res.ParentId == null && p.Id == processDefinitionId
                orderby res.Id ascending
                select res).Count();
            return datas;
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") public java.Util.List<org.camunda.bpm.engine.runtime.ProcessInstance> findProcessInstancesByQueryCriteria(org.camunda.bpm.engine.impl.ProcessInstanceQueryImpl processInstanceQuery, org.camunda.bpm.engine.impl.Page page)
        //public virtual IList<IProcessInstance> FindProcessInstancesByQueryCriteria(ProcessInstanceQueryImpl processInstanceQuery, Page page)
        //{
        //    ConfigureQuery(processInstanceQuery);
        //    throw new NotImplementedException();
        //    //return ListExt.ConvertToListT<IProcessInstance>(DbEntityManager.SelectList("selectProcessInstanceByQueryCriteria", processInstanceQuery, page));
        //}

        public IList<IProcessInstance> FindProcessInstancesByProcessInstanceId(string processInstanceId, Page page)
        {
            var datas = (from res in DbContext.Set<ExecutionEntity>()
                join p in DbContext.Set<ProcessDefinitionEntity>()
                on res.ProcessDefinitionId equals p.Id
                where res.ParentId == null && res.ProcessInstanceId == processInstanceId
                orderby res.Id ascending
                select res).Distinct().ToList<IProcessInstance>();
            return datas;
        }

        public IList<IProcessInstance> FindProcessInstancesByProcessDefinitionId(string processDefinitionId, Page page)
        {
            var datas = (from res in DbContext.Set<ExecutionEntity>()
                join p in DbContext.Set<ProcessDefinitionEntity>()
                on res.ProcessDefinitionId equals p.Id
                where res.ParentId == null && p.Id == processDefinitionId
                orderby res.Id ascending
                select res).Distinct().ToList<IProcessInstance>();
            return datas;
        }

        public IList<IProcessInstance> FindProcessInstancesByTenantIds(string[] tenantIds, string processDefinitionKey, Page page)
        {
            var datas = (from res in DbContext.Set<ExecutionEntity>()
                join p in DbContext.Set<ProcessDefinitionEntity>()
                on res.ProcessDefinitionId equals p.Id
                where res.ParentId == null && p.Key == processDefinitionKey && (tenantIds != null && tenantIds.Length>0)?tenantIds.Contains(res.TenantId): string.IsNullOrEmpty(res.TenantId)
                orderby res.Id ascending
                select res).Distinct().ToList<IProcessInstance>();
            return datas;
        }

        public IList<IProcessInstance> FindProcessInstancesByProcessDefinitionKey(string processDefinitionKey, Page page)
        {
            var datas = (from res in DbContext.Set<ExecutionEntity>()
                join p in DbContext.Set<ProcessDefinitionEntity>()
                on res.ProcessDefinitionId equals p.Id
                where res.ParentId == null  && p.Key == processDefinitionKey
                         orderby res.Id ascending
                select res).Distinct().ToList<IProcessInstance>();
            return datas;
        }

        public IList<IProcessInstance> FindProcessInstancesBySuperProcessInstanceId(string processInstanceId, Page page)
        {
            var ids = Find(c => c.ProcessInstanceId == processInstanceId).Select(c => c.Id).ToList();
            var datas = (from res in DbContext.Set<ExecutionEntity>()
                join p in DbContext.Set<ProcessDefinitionEntity>()
                on res.ProcessDefinitionId equals p.Id
                where res.ParentId == null && ids.Contains(res.SuperExecutionId)
                orderby res.Id ascending 
                         select res).ToList<IProcessInstance>();
            return datas;
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") public java.Util.List<String> findProcessInstancesIdsByQueryCriteria(org.camunda.bpm.engine.impl.ProcessInstanceQueryImpl processInstanceQuery)
        //public virtual IList<string> FindProcessInstancesIdsByQueryCriteria(
        //    ProcessInstanceQueryImpl processInstanceQuery)
        //{
        //    ConfigureQuery(processInstanceQuery);
        //    throw new NotImplementedException();
        //    //return ListExt.ConvertToListT<string>(DbEntityManager.SelectList("selectProcessInstanceIdsByQueryCriteria", processInstanceQuery));
        //}

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") public java.Util.List<ExecutionEntity> findEventScopeExecutionsByActivityId(String activityRef, String parentExecutionId)
        public virtual IList<ExecutionEntity> FindEventScopeExecutionsByActivityId(string activityRef,
            string parentExecutionId)
        {
            //IDictionary<string, string> parameters = new Dictionary<string, string>();
            //parameters["activityId"] = activityRef;
            //parameters["parentExecutionId"] = parentExecutionId;
            //return ListExt.ConvertToListT<ExecutionEntity>(DbEntityManager.SelectList("selectExecutionsByParentExecutionId", parameters));
            return Find(m => m.ActivityId == activityRef && m.SuperExecutionId == parentExecutionId).ToList();
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") public java.Util.List<org.camunda.bpm.engine.runtime.Execution> findExecutionsByNativeQuery(java.Util.Map<String, Object> parameterMap, int firstResult, int maxResults)
        public virtual IList<IExecution> FindExecutionsByNativeQuery(IDictionary<string, object> parameterMap,
            int firstResult, int maxResults)
        {
            throw new NotImplementedException();
            //return ListExt.ConvertToListT<IExecution>(DbEntityManager.SelectListWithRawParameter("selectExecutionByNativeQuery", parameterMap, firstResult, maxResults));
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") public java.Util.List<org.camunda.bpm.engine.runtime.ProcessInstance> findProcessInstanceByNativeQuery(java.Util.Map<String, Object> parameterMap, int firstResult, int maxResults)
        public virtual IList<IProcessInstance> FindProcessInstanceByNativeQuery(
            IDictionary<string, object> parameterMap, int firstResult, int maxResults)
        {
            throw new NotImplementedException();
            //return ListExt.ConvertToListT<IProcessInstance>(DbEntityManager.SelectListWithRawParameter("selectExecutionByNativeQuery", parameterMap, firstResult, maxResults)) ;
        }

        public virtual long FindExecutionCountByNativeQuery(IDictionary<string, object> parameterMap)
        {            
            //return (long) DbEntityManager.SelectOne("selectExecutionCountByNativeQuery", parameterMap);
            return Count();
        }

        public virtual void UpdateExecutionSuspensionStateByProcessDefinitionId(string processDefinitionId, ISuspensionState suspensionState)
        {
            //IDictionary<string, object> parameters = new Dictionary<string, object>();
            //parameters["processDefinitionId"] = processDefinitionId;
            //parameters["suspensionState"] = suspensionState.StateCode;            
            //DbEntityManager.Update(typeof(ExecutionEntity), "updateExecutionSuspensionStateByParameters", ConfigureParameterizedQuery(parameters));
            var predicate = ParameterBuilder.True<ExecutionEntity>();
            if (!string.IsNullOrEmpty(processDefinitionId))
                predicate = predicate.AndParam(c => c.ProcessDefinitionId == processDefinitionId);

            Find(predicate).ForEach(d =>
            {
                d.Revision = d.RevisionNext;
                d.SuspensionState = suspensionState.StateCode;
            });
            //DbContext.SaveChanges();
        }

        public virtual void UpdateExecutionSuspensionStateByProcessInstanceId(string processInstanceId, ISuspensionState suspensionState)
        {
            //IDictionary<string, object> parameters = new Dictionary<string, object>();
            //parameters["processInstanceId"] = processInstanceId;
            //parameters["suspensionState"] = suspensionState.StateCode;
            //DbEntityManager.Update(typeof(ExecutionEntity), "updateExecutionSuspensionStateByParameters", ConfigureParameterizedQuery(parameters));

            //< update id = "updateExecutionSuspensionStateByParameters" parameterType = "org.camunda.bpm.engine.impl.db.ListQueryParameterObject" >
            //   update ${ prefix}
            //        ACT_RU_EXECUTION set
            //  REV_ = REV_ + 1,
            //  SUSPENSION_STATE_ = #{parameter.suspensionState, jdbcType=INTEGER}
            //< where >
            //  <if test = "parameter.processInstanceId != null" >
            //    PROC_INST_ID_ = #{parameter.processInstanceId, jdbcType=VARCHAR}
            //  </if>
            //  <if test = "parameter.processDefinitionId != null" >
            //    and PROC_DEF_ID_ = #{parameter.processDefinitionId, jdbcType=VARCHAR}
            //  </if>
            //  <if test = "parameter.processDefinitionKey != null" >
            //    and PROC_DEF_ID_ IN (
            //      SELECT ID_
            //      FROM ${ prefix}
            //        ACT_RE_PROCDEF PD
            //      WHERE PD.KEY_ = #{parameter.processDefinitionKey, jdbcType=VARCHAR}
            //      <if test = "parameter.isTenantIdSet" >
            //        <if test = "parameter.tenantId != null" >
            //          and PD.TENANT_ID_ = #{parameter.tenantId, jdbcType=VARCHAR}
            //        </if>
            //        <if test = "parameter.tenantId == null" >
            //          and PD.TENANT_ID_ is null
            //        </if>
            //      </if>
            //    )
            //    < bind name = "columnPrefix" value = "''" />
            //       < include refid = "org.camunda.bpm.engine.impl.persistence.entity.TenantEntity.queryTenantCheckWithPrefix" />
            //      </if>
            //    </ where >
            //  </ update >

            var predicate = ParameterBuilder.True<ExecutionEntity>();
            if (!string.IsNullOrEmpty(processInstanceId))
                predicate = predicate.AndParam(c => c.ProcessInstanceId == processInstanceId);

            var executions = Find(predicate);
            foreach (var exec in executions)
            {
                exec.Revision = exec.RevisionNext;
                exec.SuspensionState = suspensionState.StateCode;
            }

            //DbContext.SaveChanges();
        }

        public virtual void UpdateExecutionSuspensionStateByProcessDefinitionKey(string processDefinitionKey, ISuspensionState suspensionState)
        {
            //IDictionary<string, object> parameters = new Dictionary<string, object>();
            //parameters["processDefinitionKey"] = processDefinitionKey;
            //parameters["isTenantIdSet"] = false;
            //parameters["suspensionState"] = suspensionState.StateCode
            //DbEntityManager.Update(typeof(ExecutionEntity), "updateExecutionSuspensionStateByParameters", ConfigureParameterizedQuery(parameters)); ;
            var predicateProc = ParameterBuilder.True<ProcessDefinitionEntity>();
            var predicateExec = ParameterBuilder.True<ExecutionEntity>();
            if (!string.IsNullOrEmpty(processDefinitionKey))
            {
                predicateProc = predicateProc.AndParam(c => c.Key == processDefinitionKey);
                predicateExec = predicateExec.AndParam(c => DbContext.Set<ProcessDefinitionEntity>().Where(predicateProc).Select(p => p.Id).ToList().Contains(c.ProcessDefinitionId));
            }

            Find(predicateExec).ForEach(d =>
            {
                d.Revision = d.RevisionNext;
                d.SuspensionState = suspensionState.StateCode;
            });
            //DbContext.SaveChanges();
        }

        public virtual void UpdateExecutionSuspensionStateByProcessDefinitionKeyAndTenantId(string processDefinitionKey,
            string tenantId, ISuspensionState suspensionState)
        {
            //IDictionary<string, object> parameters = new Dictionary<string, object>();
            //parameters["processDefinitionKey"] = processDefinitionKey;
            //parameters["isTenantIdSet"] = true;
            //parameters["tenantId"] = tenantId;
            //parameters["suspensionState"] = suspensionState.StateCode;
            //DbEntityManager.Update(typeof(ExecutionEntity), "updateExecutionSuspensionStateByParameters", ConfigureParameterizedQuery(parameters));
            
            var predicateProc = ParameterBuilder.True<ProcessDefinitionEntity>();
            var predicateExec = ParameterBuilder.True<ExecutionEntity>();
            if (!string.IsNullOrEmpty(processDefinitionKey))
            {
                predicateProc = predicateProc.AndParam(c => c.Key == processDefinitionKey);
                if (!string.IsNullOrEmpty(tenantId))
                    predicateProc = predicateProc.AndParam(c => c.TenantId == tenantId);
                else
                    predicateProc = predicateProc.AndParam(c => c.TenantId == null);
                predicateExec =predicateExec.AndParam(c => DbContext.Set<ProcessDefinitionEntity>().Where(predicateProc).Select(p=>p.Id).ToList().Contains(c.ProcessDefinitionId));
            }

            Find(predicateExec).ForEach(d =>
            {
                d.Revision = d.RevisionNext;
                d.SuspensionState = suspensionState.StateCode;
            });
            //DbContext.SaveChanges();
        }

        // helper ///////////////////////////////////////////////////////////

        protected internal virtual void CreateDefaultAuthorizations(ExecutionEntity execution)
        {
            if (execution.IsProcessInstanceExecution && AuthorizationEnabled)
            {
                var provider = ResourceAuthorizationProvider;
                var authorizations = provider.NewProcessInstance(execution);
                SaveDefaultAuthorizations(authorizations);
            }
        }

        //protected internal virtual void ConfigureQuery<T1, TU1>(AbstractQuery<T1, TU1> query)
        //{
        //    AuthorizationManager.ConfigureExecutionQuery(query);
        //    TenantManager.ConfigureQuery(query);
        //}

        //protected internal virtual ListQueryParameterObject ConfigureParameterizedQuery(object parameter)
        //{
        //    return TenantManager.ConfigureQuery(parameter);
        //}
        public void UpdateExecutionById(ExecutionEntity entity)
        {
            Update(entity);
        }
    }
}
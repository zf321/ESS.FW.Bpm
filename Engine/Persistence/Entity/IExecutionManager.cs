using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.DataAccess;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{
    public interface IExecutionManager:IRepository<ExecutionEntity,string>
    {
        void DeleteExecution(ExecutionEntity execution);
        void DeleteProcessInstance(string processInstanceId, string deleteReason);
        void DeleteProcessInstance(string processInstanceId, string deleteReason, bool cascade, bool skipCustomListeners);
        void DeleteProcessInstance(string processInstanceId, string deleteReason, bool cascade, bool skipCustomListeners, bool externallyTerminated);
        void DeleteProcessInstancesByProcessDefinition(string processDefinitionId, string deleteReason, bool cascade, bool skipCustomListeners);
        IList<ExecutionEntity> FindChildExecutionsByParentExecutionId(string parentExecutionId);
        IList<ExecutionEntity> FindEventScopeExecutionsByActivityId(string activityRef, string parentExecutionId);
        ExecutionEntity FindExecutionById(string executionId);
        long FindProcessInstanceCountByProcessDefinitionId(string processDefinitionId);
        long FindExecutionCountByNativeQuery(IDictionary<string, object> parameterMap);
        IList<IExecution> FindExecutionsByNativeQuery(IDictionary<string, object> parameterMap, int firstResult, int maxResults);
        IList<ExecutionEntity> FindExecutionsByProcessInstanceId(string processInstanceId);
        IList<IProcessInstance> FindProcessInstanceByNativeQuery(IDictionary<string, object> parameterMap, int firstResult, int maxResults);

        IList<IProcessInstance> FindProcessInstancesByProcessInstanceId(string processInstanceId, Page page);
        IList<IProcessInstance> FindProcessInstancesByProcessDefinitionId(string processDefinitionId, Page page);
        IList<IProcessInstance> FindProcessInstancesByTenantIds(string[] tenantIds, string processDefinitionKey,Page page);
        IList<IProcessInstance> FindProcessInstancesByProcessDefinitionKey(string processDefinitionKey, Page page);
        IList<IProcessInstance> FindProcessInstancesBySuperProcessInstanceId(string processInstanceId, Page page);

        ExecutionEntity FindSubProcessInstanceBySuperCaseExecutionId(string superCaseExecutionId);
        ExecutionEntity FindSubProcessInstanceBySuperExecutionId(string superExecutionId);
        void InsertExecution(ExecutionEntity execution);
        void UpdateExecutionById(ExecutionEntity entity);
        void UpdateExecutionSuspensionStateByProcessDefinitionId(string processDefinitionId, ISuspensionState suspensionState);
        void UpdateExecutionSuspensionStateByProcessDefinitionKey(string processDefinitionKey, ISuspensionState suspensionState);
        void UpdateExecutionSuspensionStateByProcessDefinitionKeyAndTenantId(string processDefinitionKey, string tenantId, ISuspensionState suspensionState);
        void UpdateExecutionSuspensionStateByProcessInstanceId(string processInstanceId, ISuspensionState suspensionState);
    }
}
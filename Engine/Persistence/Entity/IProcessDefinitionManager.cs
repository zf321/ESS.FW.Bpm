using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.DataAccess;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{
    public interface IProcessDefinitionManager:IRepository<ProcessDefinitionEntity,string>
    {
        void DeleteProcessDefinition(IProcessDefinition processDefinition, string processDefinitionId, bool cascadeToHistory, bool cascadeToInstances, bool skipCustomListeners);
        void DeleteSubscriptionsForProcessDefinition(string processDefinitionId);
        ProcessDefinitionEntity FindDefinitionByDeploymentAndKey(string deploymentId, string definitionKey);
        ProcessDefinitionEntity FindDefinitionByKeyVersionAndTenantId(string definitionKey, int? definitionVersion, string tenantId);
        ProcessDefinitionEntity FindLatestDefinitionById(string id);
        ProcessDefinitionEntity FindLatestDefinitionByKey(string key);
        ProcessDefinitionEntity FindLatestDefinitionByKeyAndTenantId(string definitionKey, string tenantId);
        ProcessDefinitionEntity FindLatestProcessDefinitionById(string processDefinitionId);
        ProcessDefinitionEntity FindLatestProcessDefinitionByKey(string processDefinitionKey);
        ProcessDefinitionEntity FindLatestProcessDefinitionByKeyAndTenantId(string processDefinitionKey, string tenantId);
        string FindPreviousProcessDefinitionId(string processDefinitionKey, int? version, string tenantId);
        ProcessDefinitionEntity FindProcessDefinitionByDeploymentAndKey(string deploymentId, string processDefinitionKey);
        ProcessDefinitionEntity FindProcessDefinitionByKeyVersionAndTenantId(string processDefinitionKey, int? processDefinitionVersion, string tenantId);
        IList<ProcessDefinitionEntity> FindProcessDefinitionsByDeploymentId(string deploymentId);
        IList<IProcessDefinition> FindProcessDefinitionsByKeyIn(params string[] keys);
        ProcessDefinitionEntity GetCachedResourceDefinitionEntity(string definitionId);
        void InsertProcessDefinition(ProcessDefinitionEntity processDefinition);
        void UpdateProcessDefinitionSuspensionStateById(string processDefinitionId, ISuspensionState suspensionState);
        void UpdateProcessDefinitionSuspensionStateByKey(string processDefinitionKey, ISuspensionState suspensionState);
        void UpdateProcessDefinitionSuspensionStateByKeyAndTenantId(string processDefinitionKey, string tenantId, ISuspensionState suspensionState);
    }
}
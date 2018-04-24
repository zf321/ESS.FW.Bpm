using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Repository;

namespace ESS.FW.Bpm.Engine.Dmn.Impl.Entity.Repository
{
    public interface IDecisionDefinitionManager
    {
        void DeleteDecisionDefinitionsByDeploymentId(string deploymentId);
        DecisionDefinitionEntity FindDecisionDefinitionByDeploymentAndKey(string deploymentId, string decisionDefinitionKey);
        IList<IDecisionDefinition> FindDecisionDefinitionByDeploymentId(string deploymentId);
        DecisionDefinitionEntity FindDecisionDefinitionById(string decisionDefinitionId);
        
        DecisionDefinitionEntity FindLatestDecisionDefinitionByKey(string decisionDefinitionKey);
        DecisionDefinitionEntity FindDecisionDefinitionByKeyAndVersion(string decisionDefinitionKey, int? decisionDefinitionVersion);
        DecisionDefinitionEntity FindDecisionDefinitionByKeyVersionAndTenantId(string decisionDefinitionKey, int? decisionDefinitionVersion, string tenantId);
        DecisionDefinitionEntity FindDefinitionByDeploymentAndKey(string deploymentId, string definitionKey);
        DecisionDefinitionEntity FindDefinitionByKeyVersionAndTenantId(string definitionKey, int? definitionVersion, string tenantId);
        DecisionDefinitionEntity FindLatestDecisionDefinitionByKeyAndTenantId(string decisionDefinitionKey, string tenantId);
        DecisionDefinitionEntity FindLatestDefinitionById(string id);
        DecisionDefinitionEntity FindLatestDefinitionByKey(string key);
        DecisionDefinitionEntity FindLatestDefinitionByKeyAndTenantId(string definitionKey, string tenantId);
        string FindPreviousDecisionDefinitionId(string decisionDefinitionKey, int? version, string tenantId);
        DecisionDefinitionEntity GetCachedResourceDefinitionEntity(string definitionId);
        void InsertDecisionDefinition(DecisionDefinitionEntity decisionDefinition);
    }
}
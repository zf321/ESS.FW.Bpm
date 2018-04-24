using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Repository;

namespace ESS.FW.Bpm.Engine.Dmn.Impl.Entity.Repository
{
    public interface IDecisionRequirementsDefinitionManager
    {
        void DeleteDecisionRequirementsDefinitionsByDeploymentId(string deploymentId);
        DecisionRequirementsDefinitionEntity FindDecisionRequirementsDefinitionByDeploymentAndKey(string deploymentId, string decisionRequirementsDefinitionKey);
        IList<IDecisionRequirementsDefinition> FindDecisionRequirementsDefinitionByDeploymentId(string deploymentId);
        DecisionRequirementsDefinitionEntity FindDecisionRequirementsDefinitionById(string decisionRequirementsDefinitionId);
        DecisionRequirementsDefinitionEntity FindDefinitionByDeploymentAndKey(string deploymentId, string definitionKey);
        DecisionRequirementsDefinitionEntity FindDefinitionByKeyVersionAndTenantId(string definitionKey, int? definitionVersion, string tenantId);
        DecisionRequirementsDefinitionEntity FindLatestDecisionRequirementsDefinitionByKeyAndTenantId(string decisionRequirementsDefinitionKey, string tenantId);
        DecisionRequirementsDefinitionEntity FindLatestDefinitionById(string id);
        DecisionRequirementsDefinitionEntity FindLatestDefinitionByKey(string key);
        DecisionRequirementsDefinitionEntity FindLatestDefinitionByKeyAndTenantId(string definitionKey, string tenantId);
        string FindPreviousDecisionRequirementsDefinitionId(string decisionRequirementsDefinitionKey, int? version, string tenantId);
        DecisionRequirementsDefinitionEntity GetCachedResourceDefinitionEntity(string definitionId);
        void InsertDecisionRequirementsDefinition(DecisionRequirementsDefinitionEntity decisionRequirementsDefinition);
    }
}
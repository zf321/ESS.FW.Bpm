using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Dmn.Impl.Entity.Repository;
using ESS.FW.Bpm.Engine.exception.dmn;
using ESS.FW.Bpm.Engine.Impl.Util;

namespace ESS.FW.Bpm.Engine.Persistence.Deploy.Cache
{
    /// <summary>
    /// </summary>
    public class DecisionDefinitionCache : ResourceDefinitionCache<DecisionDefinitionEntity>
    {
        public DecisionDefinitionCache(ICacheFactory factory, int cacheCapacity, CacheDeployer cacheDeployer)
            : base(factory, cacheCapacity, cacheDeployer)
        {
        }

        protected internal override IAbstractResourceDefinitionManager<DecisionDefinitionEntity> Manager
        {
            get { return Context.CommandContext.DecisionDefinitionManager_2; }
        }

        public virtual DecisionDefinitionEntity FindDeployedDefinitionByKeyAndVersion(string definitionKey,
            int? definitionVersion)
        {
            var definition = ((DecisionDefinitionManager) Manager).FindDecisionDefinitionByKeyAndVersion(definitionKey,
                definitionVersion);

            CheckInvalidDefinitionByKeyAndVersion(definitionKey, definitionVersion, definition);
            definition = ResolveDefinition(definition);
            return definition;
        }

        protected internal override void CheckInvalidDefinitionId(string definitionId)
        {
            EnsureUtil.EnsureNotNull("Invalid decision definition id", "decisionDefinitionId", definitionId);
        }

        protected internal override void CheckDefinitionFound(string definitionId, DecisionDefinitionEntity definition)
        {
            EnsureUtil.EnsureNotNull(typeof(DecisionDefinitionNotFoundException),
                "no deployed decision definition found with id '" + definitionId + "'", "decisionDefinition", definition);
        }

        protected internal override void CheckInvalidDefinitionByKey(string definitionKey,
            DecisionDefinitionEntity definition)
        {
            EnsureUtil.EnsureNotNull(typeof(DecisionDefinitionNotFoundException),
                "no decision definition deployed with key '" + definitionKey + "'", "decisionDefinition", definition);
        }

        protected internal override void CheckInvalidDefinitionByKeyAndTenantId(string definitionKey, string tenantId,
            DecisionDefinitionEntity definition)
        {
            EnsureUtil.EnsureNotNull(typeof(DecisionDefinitionNotFoundException),
                "no decision definition deployed with key '" + definitionKey + "' and tenant-id '" + tenantId + "'",
                "decisionDefinition", definition);
        }

        protected internal virtual void CheckInvalidDefinitionByKeyAndVersion(string decisionDefinitionKey,
            int? decisionDefinitionVersion, DecisionDefinitionEntity decisionDefinition)
        {
            EnsureUtil.EnsureNotNull(typeof(DecisionDefinitionNotFoundException),
                "no decision definition deployed with key = '" + decisionDefinitionKey + "' and version = '" +
                decisionDefinitionVersion + "'", "decisionDefinition", decisionDefinition);
        }

        protected internal override void CheckInvalidDefinitionByKeyVersionAndTenantId(string definitionKey,
            int? definitionVersion, string tenantId, DecisionDefinitionEntity definition)
        {
            EnsureUtil.EnsureNotNull(typeof(DecisionDefinitionNotFoundException),
                "no decision definition deployed with key = '" + definitionKey + "', version = '" + definitionVersion +
                "' and tenant-id '" + tenantId + "'", "decisionDefinition", definition);
        }

        protected internal override void CheckInvalidDefinitionByDeploymentAndKey(string deploymentId,
            string definitionKey, DecisionDefinitionEntity definition)
        {
            EnsureUtil.EnsureNotNull(typeof(DecisionDefinitionNotFoundException),
                "no decision definition deployed with key = '" + definitionKey + "' in deployment = '" + deploymentId +
                "'", "decisionDefinition", definition);
        }

        protected internal override void CheckInvalidDefinitionWasCached(string deploymentId, string definitionId,
            DecisionDefinitionEntity definition)
        {
            EnsureUtil.EnsureNotNull(
                "deployment '" + deploymentId + "' didn't put decision definition '" + definitionId + "' in the cache",
                "cachedDecisionDefinition", definition);
        }
    }
}
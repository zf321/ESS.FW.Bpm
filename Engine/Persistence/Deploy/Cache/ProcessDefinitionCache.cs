using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Persistence.Deploy.Cache
{
    /// <summary>
    /// </summary>
    public class ProcessDefinitionCache : ResourceDefinitionCache<ProcessDefinitionEntity>
    {
        public ProcessDefinitionCache(ICacheFactory factory, int cacheCapacity, CacheDeployer cacheDeployer)
            : base(factory, cacheCapacity, cacheDeployer)
        {
        }

        protected internal override IAbstractResourceDefinitionManager<ProcessDefinitionEntity> Manager
        {
            get { return Context.CommandContext.ProcessDefinitionManager_2; }
        }

        public override ProcessDefinitionEntity ResolveDefinition(ProcessDefinitionEntity processDefinition)
        {
            var entity = base.ResolveDefinition(processDefinition);
            if (entity != null)
                entity.UpdateModifiedFieldsFromEntity(processDefinition);
            return entity;
        }

        protected internal override void CheckInvalidDefinitionId(string definitionId)
        {
            EnsureUtil.EnsureNotNull("Invalid process definition id", "processDefinitionId", definitionId);
        }

        protected internal override void CheckDefinitionFound(string definitionId, ProcessDefinitionEntity definition)
        {
            EnsureUtil.EnsureNotNull("no deployed process definition found with id '" + definitionId + "'",
                "processDefinition", definition);
        }

        protected internal override void CheckInvalidDefinitionByKey(string definitionKey,
            ProcessDefinitionEntity definition)
        {
            EnsureUtil.EnsureNotNull("no processes deployed with key '" + definitionKey + "'", "processDefinition",
                definition);
        }

        protected internal override void CheckInvalidDefinitionByKeyAndTenantId(string definitionKey, string tenantId,
            ProcessDefinitionEntity definition)
        {
            EnsureUtil.EnsureNotNull(
                "no processes deployed with key '" + definitionKey + "' and tenant-id '" + tenantId + "'",
                "processDefinition", definition);
        }

        protected internal override void CheckInvalidDefinitionByKeyVersionAndTenantId(string definitionKey,
            int? definitionVersion, string tenantId, ProcessDefinitionEntity definition)
        {
            EnsureUtil.EnsureNotNull(
                "no processes deployed with key = '" + definitionKey + "', version = '" + definitionVersion +
                "' and tenant-id = '" + tenantId + "'", "processDefinition", definition);
        }

        protected internal override void CheckInvalidDefinitionByDeploymentAndKey(string deploymentId,
            string definitionKey, ProcessDefinitionEntity definition)
        {
            EnsureUtil.EnsureNotNull(
                "no processes deployed with key = '" + definitionKey + "' in deployment = '" + deploymentId + "'",
                "processDefinition", definition);
        }

        protected internal override void CheckInvalidDefinitionWasCached(string deploymentId, string definitionId,
            ProcessDefinitionEntity definition)
        {
            EnsureUtil.EnsureNotNull(
                "deployment '" + deploymentId + "' didn't put process definition '" + definitionId + "' in the cache",
                "cachedProcessDefinition", definition);
        }
    }
}
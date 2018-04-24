




using ESS.FW.Bpm.Engine.Dmn.Impl.Entity.Repository;
using ESS.FW.Bpm.Engine.Impl.Util;

namespace ESS.FW.Bpm.Engine.Persistence.Deploy.Cache
{
    //using DecisionRequirementsDefinitionEntity = DecisionRequirementsDefinitionEntity;

    //static org.camunda.bpm.engine.impl.Util.EnsureUtil.ensureNotNull;

	/// <summary>
	/// 
	/// </summary>
	public class DecisionRequirementsDefinitionCache : ResourceDefinitionCache<DecisionRequirementsDefinitionEntity>
	{

	  public DecisionRequirementsDefinitionCache(ICacheFactory factory, int cacheCapacity, CacheDeployer cacheDeployer) : base(factory, cacheCapacity, cacheDeployer)
	  {
	  }
        protected internal override IAbstractResourceDefinitionManager<DecisionRequirementsDefinitionEntity> Manager
        {
            get
            {
                return context.Impl.Context.CommandContext.DecisionRequirementsDefinitionManager_2;
            }
        }
       
        protected internal override void CheckInvalidDefinitionId(string definitionId)
	  {
            EnsureUtil.EnsureNotNull("Invalid decision requirements definition id", "decisionRequirementsDefinitionId", definitionId);
	  }

	  protected internal override void CheckDefinitionFound(string definitionId, DecisionRequirementsDefinitionEntity definition)
	  {
            EnsureUtil.EnsureNotNull("no deployed decision requirements definition found with id '" + definitionId + "'", "decisionRequirementsDefinition", definition);
	  }

	  protected internal override void CheckInvalidDefinitionByKey(string definitionKey, DecisionRequirementsDefinitionEntity definition)
	  {
		// not needed
	  }

	  protected internal override void CheckInvalidDefinitionByKeyAndTenantId(string definitionKey, string tenantId, DecisionRequirementsDefinitionEntity definition)
	  {
		// not needed
	  }

	  protected internal override void CheckInvalidDefinitionByKeyVersionAndTenantId(string definitionKey, int? definitionVersion, string tenantId, DecisionRequirementsDefinitionEntity definition)
	  {
		// not needed
	  }

	  protected internal override void CheckInvalidDefinitionByDeploymentAndKey(string deploymentId, string definitionKey, DecisionRequirementsDefinitionEntity definition)
	  {
		// not needed
	  }

	  protected internal override void CheckInvalidDefinitionWasCached(string deploymentId, string definitionId, DecisionRequirementsDefinitionEntity definition)
	  {
            EnsureUtil.EnsureNotNull("deployment '" + deploymentId + "' didn't put decision requirements definition '" + definitionId + "' in the cache", "cachedDecisionRequirementsDefinition", definition);
	  }
	}

}
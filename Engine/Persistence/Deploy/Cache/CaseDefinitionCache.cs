


using ESS.FW.Bpm.Engine.Impl.Util;

namespace ESS.FW.Bpm.Engine.Persistence.Deploy.Cache
{

	//using CaseDefinitionNotFoundException = org.camunda.bpm.engine.exception.cmmn.CaseDefinitionNotFoundException;
	//using CaseDefinitionEntity = CaseDefinitionEntity;

 //   static org.camunda.bpm.engine.impl.Util.EnsureUtil.EnsureUtil.EnsureNotNull;

	/// <summary>
	/// 
	/// </summary>
	public class CaseDefinitionCache// : ResourceDefinitionCache<CaseDefinitionEntity>
	{

	  public CaseDefinitionCache(ICacheFactory factory, int cacheCapacity, CacheDeployer cacheDeployer) //: base(factory, cacheCapacity, cacheDeployer)
	  {
	  }

	 // public virtual CaseDefinitionEntity GetCaseDefinitionById(string caseDefinitionId)
	 // {
		//CheckInvalidDefinitionId(caseDefinitionId);
		//CaseDefinitionEntity caseDefinition = GetDefinition(caseDefinitionId);
		//if (caseDefinition == null)
		//{
		//  caseDefinition = FindDeployedDefinitionById(caseDefinitionId);

		//}
		//return caseDefinition;
	 // }

	 // protected internal override IAbstractResourceDefinitionManager<CaseDefinitionEntity> Manager
	 // {
		//  get
		//  {
		//	return Context.CommandContext.CaseDefinitionManager;
		//  }
	 // }

	 // protected internal override void CheckInvalidDefinitionId(string definitionId)
	 // {
		//EnsureUtil.EnsureNotNull("Invalid case definition id", "caseDefinitionId", definitionId);
	 // }

	 // protected internal override void CheckDefinitionFound(string definitionId, CaseDefinitionEntity definition)
	 // {
		//EnsureUtil.EnsureNotNull(typeof(CaseDefinitionNotFoundException), "no deployed case definition found with id '" + definitionId + "'", "caseDefinition", definition);
	 // }

	 // protected internal override void CheckInvalidDefinitionByKey(string definitionKey, CaseDefinitionEntity definition)
	 // {
		//EnsureUtil.EnsureNotNull(typeof(CaseDefinitionNotFoundException), "no case definition deployed with key '" + definitionKey + "'", "caseDefinition", definition);
	 // }

	 // protected internal override void CheckInvalidDefinitionByKeyAndTenantId(string definitionKey, string tenantId, CaseDefinitionEntity definition)
	 // {
		//EnsureUtil.EnsureNotNull(typeof(CaseDefinitionNotFoundException), "no case definition deployed with key '" + definitionKey + "' and tenant-id '" + tenantId + "'", "caseDefinition", definition);
	 // }

	 // protected internal override void CheckInvalidDefinitionByKeyVersionAndTenantId(string definitionKey, int? definitionVersion, string tenantId, CaseDefinitionEntity definition)
	 // {
		//EnsureUtil.EnsureNotNull(typeof(CaseDefinitionNotFoundException), "no case definition deployed with key = '" + definitionKey + "', version = '" + definitionVersion + "'" + " and tenant-id = '" + tenantId + "'", "caseDefinition", definition);
	 // }

	 // protected internal override void CheckInvalidDefinitionByDeploymentAndKey(string deploymentId, string definitionKey, CaseDefinitionEntity definition)
	 // {
		//EnsureUtil.EnsureNotNull(typeof(CaseDefinitionNotFoundException), "no case definition deployed with key = '" + definitionKey + "' in deployment = '" + deploymentId + "'", "caseDefinition", definition);
	 // }

	 // protected internal override void CheckInvalidDefinitionWasCached(string deploymentId, string definitionId, CaseDefinitionEntity definition)
	 // {
		//EnsureUtil.EnsureNotNull("deployment '" + deploymentId + "' didn't put case definition '" + definitionId + "' in the cache", "cachedCaseDefinition", definition);
	 // }
	}

}
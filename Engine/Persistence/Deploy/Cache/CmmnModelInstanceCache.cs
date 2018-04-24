//using System.Collections.Generic;
//using ESS.FW.Bpm.Engine.Cmmn.entity.repository;
//using ESS.FW.Bpm.Engine.Dmn.Impl.Entity.Repository;
//using ESS.FW.Bpm.Engine.Rrepository;

//
//
//
// *
//
// *
//
//
//
//
//
// */
//namespace ESS.FW.Bpm.Engine.Persistence.Deploy.Cache
//{



//	/// <summary>
//	/// 
//	/// </summary>
//	public class CmmnModelInstanceCache //: ModelInstanceCache<CmmnModelInstance, CaseDefinitionEntity>
//	{

//	 // public CmmnModelInstanceCache(ICacheFactory factory, int cacheCapacity, ResourceDefinitionCache<CaseDefinitionEntity> definitionCache) : base(factory, cacheCapacity, definitionCache)
//	 // {
//	 // }

//	 // protected internal override void throwLoadModelException(string definitionId, System.Exception e)
//	 // {
//		//throw LOG.loadModelException("CMMN", "case", definitionId, e);
//	 // }

//	 // protected internal override CmmnModelInstance readModelFromStream(InputStream cmmnResourceInputStream)
//	 // {
//		//return Cmmn.readModelFromStream(cmmnResourceInputStream);
//	 // }

//	 // protected internal override void logRemoveEntryFromDeploymentCacheFailure(string definitionId, System.Exception e)
//	 // {
//		//LOG.removeEntryFromDeploymentCacheFailure("case", definitionId, e);
//	 // }

//	 // protected internal override IList<IDecisionDefinition> getAllDefinitionsForDeployment(string deploymentId)
//	 // {
//		//return (new DecisionDefinitionQueryImpl()).DeploymentId(deploymentId).list();
//	 // }
//	}

//}
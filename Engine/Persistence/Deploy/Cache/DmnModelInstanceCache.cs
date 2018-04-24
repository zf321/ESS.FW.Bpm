using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Dmn.Impl.Entity.Repository;
using ESS.FW.Bpm.Model.Dmn;
using System.IO;

namespace ESS.FW.Bpm.Engine.Persistence.Deploy.Cache
{



	/// <summary>
	/// 
	/// </summary>
	public class DmnModelInstanceCache : ModelInstanceCache<IDmnModelInstance, DecisionDefinitionEntity>
	{

	  public DmnModelInstanceCache(ICacheFactory factory, int cacheCapacity, ResourceDefinitionCache<DecisionDefinitionEntity> definitionCache) : base(factory, cacheCapacity, definitionCache)
	  {
	  }

        protected internal override void ThrowLoadModelException(string definitionId, System.Exception e)
        {
            throw Log.LoadModelException("DMN", "decision", definitionId, e);
        }

        protected internal override IDmnModelInstance ReadModelFromStream(Stream cmmnResourceInputStream)
        {
            return Model.Dmn.Dmn.ReadModelFromStream(cmmnResourceInputStream);
        }

        protected internal override void LogRemoveEntryFromDeploymentCacheFailure(string definitionId, System.Exception e)
        {
            Log.RemoveEntryFromDeploymentCacheFailure("decision", definitionId, e);
        }

        protected internal override IList<IDecisionDefinition> GetAllDefinitionsForDeployment<IDecisionDefinition>(string deploymentId)
        {
            throw new NotImplementedException();
            //return (IList<IDecisionDefinition>) (new DecisionDefinitionQueryImpl()).DeploymentId(deploymentId).List().GetList();
        }
    }

}
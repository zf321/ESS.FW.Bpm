using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Model.Bpmn;

namespace ESS.FW.Bpm.Engine.Persistence.Deploy.Cache
{
    /// <summary>
    /// </summary>
    public class BpmnModelInstanceCache : ModelInstanceCache<IBpmnModelInstance, ProcessDefinitionEntity>
    {
        public BpmnModelInstanceCache(ICacheFactory factory, int cacheCapacity,
            ResourceDefinitionCache<ProcessDefinitionEntity> definitionCache)
            : base(factory, cacheCapacity, definitionCache)
        {
        }

        protected internal override void ThrowLoadModelException(string definitionId, System.Exception e)
        {
            throw Log.LoadModelException("BPMN", "process", definitionId, e);
        }

        protected internal override IBpmnModelInstance ReadModelFromStream(Stream bpmnResourceInputStream)
        {
            return Bpmn.ReadModelFromStream(bpmnResourceInputStream);
        }

        protected internal override void LogRemoveEntryFromDeploymentCacheFailure(string definitionId, System.Exception e)
        {
            Log.RemoveEntryFromDeploymentCacheFailure("process", definitionId, e);
        }

        protected internal override IList<TProcessDefinition> GetAllDefinitionsForDeployment<TProcessDefinition>(
            string deploymentId)
        {
            var commandContext = Context.CommandContext;
            var allDefinitionsForDeployment =
                commandContext.RunWithoutAuthorization<IList<IProcessDefinition>>(
                    //() => commandContext.ProcessDefinitionManager.Find(c => c.DeploymentId == deploymentId)
                    //    .ToList()
                    () => null
                        );
            return (IList<TProcessDefinition>) allDefinitionsForDeployment;
        }
        
    }
}
using System.Collections.Generic;
using System.IO;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Common.Cache;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.Cmd;
using ESS.FW.Bpm.Engine.Impl.DB;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Model.Xml;
using ESS.Shared.Entities.Bpm;

namespace ESS.FW.Bpm.Engine.Persistence.Deploy.Cache
{
    /// <summary>
    /// </summary>
    public abstract class ModelInstanceCache<TInstanceType, TDefinitionType> where TInstanceType : IModelInstance
        where TDefinitionType :class, IResourceDefinition,IDbEntity,new()
    {
        protected internal static readonly EnginePersistenceLogger Log = ProcessEngineLogger.PersistenceLogger;
        protected internal ResourceDefinitionCache<TDefinitionType> DefinitionCache;

        protected internal ICache<string, TInstanceType> InstanceCache;

        public ModelInstanceCache(ICacheFactory factory, int cacheCapacity,
            ResourceDefinitionCache<TDefinitionType> definitionCache)
        {
            InstanceCache = factory.CreateCache<TInstanceType>(cacheCapacity);
            DefinitionCache = definitionCache;
        }

        public virtual ICache<string, TInstanceType> Cache
        {
            get { return InstanceCache; }
        }

        public virtual TInstanceType FindBpmnModelInstanceForDefinition(TDefinitionType definitionEntity)
        {
            TInstanceType bpmnModelInstance = InstanceCache.Get((definitionEntity as IResourceDefinition).Id);
            if (bpmnModelInstance == null)
                bpmnModelInstance = LoadAndCacheBpmnModelInstance(definitionEntity);
            return bpmnModelInstance;
        }

        public virtual TInstanceType FindBpmnModelInstanceForDefinition(string definitionId)
        {
            TInstanceType bpmnModelInstance = InstanceCache.Get(definitionId);
            if (bpmnModelInstance == null)
            {
                var definition = DefinitionCache.FindDeployedDefinitionById(definitionId);
                bpmnModelInstance = LoadAndCacheBpmnModelInstance(definition);
            }
            return bpmnModelInstance;
        }


        protected internal virtual TInstanceType LoadAndCacheBpmnModelInstance(TDefinitionType definitionEntity)
        {
            var commandContext = Context.CommandContext;
            var bpmnResourceInputStream =
                commandContext.RunWithoutAuthorization(()=> new GetDeploymentResourceCmd(definitionEntity.DeploymentId, definitionEntity.ResourceName).Execute
                        (commandContext));

            try
            {
                var bpmnModelInstance = ReadModelFromStream(bpmnResourceInputStream);
                InstanceCache.Put((definitionEntity as IResourceDefinition).Id, bpmnModelInstance);
                return bpmnModelInstance;
            }
            catch (System.Exception e)
            {
                ThrowLoadModelException((definitionEntity as IResourceDefinition).Id, e);
            }
            return default(TInstanceType);
        }

        public virtual void RemoveAllDefinitionsByDeploymentId(string deploymentId)
        {
            // remove all definitions for a specific deployment
            var allDefinitionsForDeployment = GetAllDefinitionsForDeployment<ProcessDefinitionEntity>(deploymentId);
            foreach (IResourceDefinition definition in allDefinitionsForDeployment)
                try
                {
                    InstanceCache.Remove(definition.Id);
                    DefinitionCache.RemoveDefinitionFromCache(definition.Id);
                }
                catch (System.Exception e)
                {
                    LogRemoveEntryFromDeploymentCacheFailure(definition.Id, e);
                }
        }

        public virtual void Remove(string definitionId)
        {
            InstanceCache.Remove(definitionId);
        }

        public virtual void Clear()
        {
            InstanceCache.Clear();
        }

        protected internal abstract void ThrowLoadModelException(string definitionId, System.Exception e);

        protected internal abstract void LogRemoveEntryFromDeploymentCacheFailure(string definitionId, System.Exception e);

        protected internal abstract TInstanceType ReadModelFromStream(Stream stream);

        protected internal abstract IList<T> GetAllDefinitionsForDeployment<T>(string deploymentId)
            where T : IResourceDefinition;
        
    }
}
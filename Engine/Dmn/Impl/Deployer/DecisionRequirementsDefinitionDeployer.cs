
using ESS.FW.Bpm.Engine.Dmn.engine.impl.spi.transform;
using ESS.FW.Bpm.Engine.Dmn.Impl.Entity.Repository;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.Core.Model;
using ESS.FW.Bpm.Engine.Persistence.Deploy.Cache;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using System;
using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Dmn.Impl.Deployer
{


    /// <summary>
    /// <seealso cref="Deployer"/> responsible to parse DMN 1.1 XML files and create the proper
    /// <seealso cref="DecisionRequirementsDefinitionEntity"/>s.
    /// </summary>
    public class DecisionRequirementsDefinitionDeployer : AbstractDefinitionDeployer<DecisionRequirementsDefinitionEntity>
    {

        protected internal static readonly DecisionLogger Log = ProcessEngineLogger.DecisionLogger;

        protected internal IDmnTransformer transformer;

        protected internal override string[] ResourcesSuffixes
        {
            get
            {
                // since the DecisionDefinitionDeployer uses the result of this cacheDeployer, make sure that
                // it process the same DMN resources
                return DecisionDefinitionDeployer.DmnResourceSuffixes;
            }
        }

        protected internal override IList<DecisionRequirementsDefinitionEntity> TransformDefinitions(DeploymentEntity deployment, ResourceEntity resource, Bpm.Engine.Impl.Core.Model.Properties properties)
        {
            byte[] bytes = resource.Bytes;
            System.IO.MemoryStream inputStream = new System.IO.MemoryStream(bytes);

            try
            {
                DecisionRequirementsDefinitionEntity drd = transformer.createTransform().modelInstance(inputStream).transformDecisionRequirementsGraph<DecisionRequirementsDefinitionEntity>();

                return  new List<DecisionRequirementsDefinitionEntity>() { drd };

            }
            catch (System.Exception e)
            {
                throw Log.ExceptionParseDmnResource(resource.Name, e);
            }
        }

        protected internal override DecisionRequirementsDefinitionEntity FindDefinitionByDeploymentAndKey(string deploymentId, string definitionKey)
        {
            return DecisionRequirementsDefinitionManager.FindDecisionRequirementsDefinitionByDeploymentAndKey(deploymentId, definitionKey);
        }

        protected internal override DecisionRequirementsDefinitionEntity FindLatestDefinitionByKeyAndTenantId(string definitionKey, string tenantId)
        {
            return DecisionRequirementsDefinitionManager.FindLatestDecisionRequirementsDefinitionByKeyAndTenantId(definitionKey, tenantId);
        }

        protected internal override void PersistDefinition(DecisionRequirementsDefinitionEntity definition)
        {
            if (IsDecisionRequirementsDefinitionPersistable(definition))
            {
                DecisionRequirementsDefinitionManager.InsertDecisionRequirementsDefinition(definition);
            }
        }

        protected internal override void AddDefinitionToDeploymentCache(DeploymentCache deploymentCache, DecisionRequirementsDefinitionEntity definition)
        {
            if (IsDecisionRequirementsDefinitionPersistable(definition))
            {
                deploymentCache.AddDecisionRequirementsDefinition(definition);
            }
        }

        protected internal override void EnsureNoDuplicateDefinitionKeys(IList<DecisionRequirementsDefinitionEntity> definitions)
        {
            // ignore decision requirements definitions which will not be persistent
            List<DecisionRequirementsDefinitionEntity> persistableDefinitions = new List<DecisionRequirementsDefinitionEntity>();
            foreach (DecisionRequirementsDefinitionEntity definition in definitions)
            {
                if (IsDecisionRequirementsDefinitionPersistable(definition))
                {
                    persistableDefinitions.Add(definition);
                }
            }

            base.EnsureNoDuplicateDefinitionKeys(persistableDefinitions);
        }

        public static bool IsDecisionRequirementsDefinitionPersistable(DecisionRequirementsDefinitionEntity definition)
        {
            // persist no decision requirements definition for a single decision
            return definition.Decisions.Count> 1;
        }

        protected internal override void UpdateDefinitionByPersistedDefinition(DeploymentEntity deployment, DecisionRequirementsDefinitionEntity definition, DecisionRequirementsDefinitionEntity persistedDefinition)
        {
            // cannot update the definition if it is not persistent
            if (persistedDefinition != null)
            {
                base.UpdateDefinitionByPersistedDefinition(deployment, definition, persistedDefinition);
            }
        }

        //context ///////////////////////////////////////////////////////////////////////////////////////////

        protected internal virtual IDecisionRequirementsDefinitionManager DecisionRequirementsDefinitionManager
        {
            get
            {
                return CommandContext.DecisionRequirementsDefinitionManager;
            }
        }

        // getters/setters ///////////////////////////////////////////////////////////////////////////////////

        public virtual IDmnTransformer Transformer
        {
            get
            {
                return transformer;
            }
            set
            {
                this.transformer = value;
            }
        }


    }

}


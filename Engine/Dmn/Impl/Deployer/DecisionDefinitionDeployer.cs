using ESS.FW.Bpm.Engine.Dmn.engine;
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
    /// <seealso cref="DecisionDefinitionEntity"/>s. Since it uses the result of the
    /// <seealso cref="DecisionRequirementsDefinitionDeployer"/> to avoid duplicated parsing, the DecisionRequirementsDefinitionDeployer must
    /// process the deployment before this cacheDeployer.
    /// </summary>
    public class DecisionDefinitionDeployer : AbstractDefinitionDeployer<DecisionDefinitionEntity>
    {

        protected internal static readonly DecisionLogger Log = ProcessEngineLogger.DecisionLogger;

        public static readonly string[] DmnResourceSuffixes = new string[] { "dmn11.xml", "dmn" };

        protected internal IDmnTransformer transformer;

        protected internal override string[] ResourcesSuffixes
        {
            get
            {
                return DmnResourceSuffixes;
            }
        }

        protected internal override IList<DecisionDefinitionEntity> TransformDefinitions(DeploymentEntity deployment, ResourceEntity resource, Bpm.Engine.Impl.Core.Model.Properties properties)
        {
            IList<DecisionDefinitionEntity> decisions = new List<DecisionDefinitionEntity>();

            // get the decisions from the deployed drd instead of parse the DMN again
            DecisionRequirementsDefinitionEntity deployedDrd = FindDeployedDrdForResource(deployment, resource.Name);

            if (deployedDrd == null)
            {
                throw Log.ExceptionNoDrdForResource(resource.Name);
            }

            ICollection<IDmnDecision> decisionsOfDrd = deployedDrd.Decisions;
            foreach (IDmnDecision decisionOfDrd in decisionsOfDrd)
            {

                DecisionDefinitionEntity decisionEntity = (DecisionDefinitionEntity)decisionOfDrd;
                if (DecisionRequirementsDefinitionDeployer.IsDecisionRequirementsDefinitionPersistable(deployedDrd))
                {
                    decisionEntity.DecisionRequirementsDefinitionId = deployedDrd.Id;
                    decisionEntity.DecisionRequirementsDefinitionKey = deployedDrd.Key;
                }

                decisions.Add(decisionEntity);
            }

            if (!DecisionRequirementsDefinitionDeployer.IsDecisionRequirementsDefinitionPersistable(deployedDrd))
            {
                deployment.RemoveArtifact(deployedDrd);
            }

            return decisions;
        }

        protected internal virtual DecisionRequirementsDefinitionEntity FindDeployedDrdForResource(DeploymentEntity deployment, string resourceName)
        {
            IList<DecisionRequirementsDefinitionEntity> deployedDrds = deployment.GetDeployedArtifacts< DecisionRequirementsDefinitionEntity>(typeof(DecisionRequirementsDefinitionEntity));
            if (deployedDrds != null)
            {

                foreach (DecisionRequirementsDefinitionEntity deployedDrd in deployedDrds)
                {
                    if (deployedDrd.ResourceName.Equals(resourceName))
                    {
                        return deployedDrd;
                    }
                }
            }
            return null;
        }

        protected internal override DecisionDefinitionEntity FindDefinitionByDeploymentAndKey(string deploymentId, string definitionKey)
        {
            return DecisionDefinitionManager.FindDecisionDefinitionByDeploymentAndKey(deploymentId, definitionKey);
        }

        protected internal override DecisionDefinitionEntity FindLatestDefinitionByKeyAndTenantId(string definitionKey, string tenantId)
        {
            return DecisionDefinitionManager.FindLatestDecisionDefinitionByKeyAndTenantId(definitionKey, tenantId);
        }

        protected internal override void PersistDefinition(DecisionDefinitionEntity definition)
        {
            DecisionDefinitionManager.InsertDecisionDefinition(definition);
        }


        protected internal override void AddDefinitionToDeploymentCache(DeploymentCache deploymentCache, DecisionDefinitionEntity definition)
        {
            deploymentCache.AddDecisionDefinition(definition);
        }

        // context ///////////////////////////////////////////////////////////////////////////////////////////

        protected internal virtual IDecisionDefinitionManager DecisionDefinitionManager
        {
            get
            {
                return CommandContext.DecisionDefinitionManager;
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


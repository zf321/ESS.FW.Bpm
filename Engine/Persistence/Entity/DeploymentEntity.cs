using System;
using System.Collections;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.Dmn.Impl.Entity.Repository;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Repository.Impl;
using Newtonsoft.Json;
using ESS.Shared.Entities.Bpm;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{


    /// <summary>
	///  
	/// </summary>
	[Serializable]
    public class DeploymentEntity : IDeploymentWithDefinitions, IDbEntity
    {

        private const long SerialVersionUid = 1L;
        [NonSerialized]
        protected internal IDictionary<string, ResourceEntity> resources;
        public string Id { get; set; }
        public string Name { get; set; }
        public virtual bool IsValidatingSchema { get; set; }
        public virtual bool IsNew { get; set; }
        public DateTime DeploymentTime { get; set; }
        public string Source { get; set; }
        public string TenantId { get; set; }
        /// <summary>
        /// Will only be used during actual deployment to pass deployed artifacts (eg process definitions).
        /// Will be null otherwise.
        /// </summary>
        protected internal IDictionary<Type, IList> deployedArtifacts;

        public virtual ResourceEntity GetResource(string resourceName)
        {
            return Resources[resourceName];
        }

        public virtual void AddResource(ResourceEntity resource)
        {
            if (resources == null)
            {
                resources = new Dictionary<string, ResourceEntity>();
            }
            resources[resource.Name] = resource;
        }

        public virtual void ClearResources()
        {
            if (resources != null)
            {
                resources.Clear();
            }
        }
        [JsonIgnore]
        // lazy loading /////////////////////////////////////////////////////////////
        public virtual IDictionary<string, ResourceEntity> Resources
        {
            get
            {
                if (resources == null && Id != null)
                {
                    IList<ResourceEntity> resourcesList = context.Impl.Context.CommandContext.ResourceManager.FindResourcesByDeploymentId(Id);
                    resources = new Dictionary<string, ResourceEntity>();
                    foreach (ResourceEntity resource in resourcesList)
                    {
                        resources[resource.Name] = resource;
                    }
                }
                return resources;
            }
            set
            {
                resources = value;
            }
        }

        public virtual object GetPersistentState()
        {
            // properties of this entity are immutable
            // so always the same value is returned
            // so never will an update be issued for a DeploymentEntity
            return typeof(DeploymentEntity);
        }

        // Deployed artifacts manipulation //////////////////////////////////////////

        public virtual void AddDeployedArtifact(IResourceDefinitionEntity deployedArtifact)
        {
            if (deployedArtifacts == null)
            {
                deployedArtifacts = new Dictionary<Type, IList>();
            }

            Type clazz = deployedArtifact.GetType();
            IList artifacts = deployedArtifacts.ContainsKey(clazz) ? deployedArtifacts[clazz] : null;
            if (artifacts == null)
            {
                artifacts = new ArrayList();
                deployedArtifacts[clazz] = artifacts;
            }

            artifacts.Add(deployedArtifact);
        }

        public virtual IDictionary<Type, IList> DeployedArtifacts
        {
            get
            {
                return deployedArtifacts;
            }
        }
        
        public virtual IList<T> GetDeployedArtifacts<T>(Type clazz)
        {
            if (deployedArtifacts == null)
            {
                return null;
            }
            else
            {
                return ListExt.ConvertToListT<T>(deployedArtifacts[clazz]);
            }
        }

        public virtual void RemoveArtifact(IResourceDefinitionEntity notDeployedArtifact)
        {
            if (deployedArtifacts != null)
            {
                IList artifacts = deployedArtifacts[notDeployedArtifact.GetType()];
                if (artifacts != null)
                {
                    artifacts.Remove(notDeployedArtifact);
                    if (artifacts.Count == 0)
                    {
                        deployedArtifacts.Remove(notDeployedArtifact.GetType());
                    }
                }
            }
        }

        // getters and setters //////////////////////////////////////////////////////
        public IList<IProcessDefinition> DeployedProcessDefinitions
        {
            get
            {
                if(deployedArtifacts==null|| !deployedArtifacts.ContainsKey(typeof(ProcessDefinitionEntity)))
                {
                    return null;
                }
                return ListExt.ConvertToListT<IProcessDefinition>(deployedArtifacts[typeof(ProcessDefinitionEntity)]);
            }
        }
        [JsonIgnore]
        public IList<ICaseDefinition> DeployedCaseDefinitions
        {
            get
            {
                return null;
                throw new NotImplementedException("≤ª∆Ù”√Case");                
                //return deployedArtifacts == null ? null : (deployedArtifacts[typeof(CaseDefinitionEntity)]);
            }
        }

        public IList<IDecisionDefinition> DeployedDecisionDefinitions
        {
            get
            {
                if (deployedArtifacts == null || !deployedArtifacts.ContainsKey(typeof(DecisionDefinitionEntity)))
                    return null;
                return ListExt.ConvertToListT<IDecisionDefinition>(deployedArtifacts[typeof(DecisionDefinitionEntity)]);
                //return deployedArtifacts == null ? null : ListExt.ConvertToListT<IDecisionDefinition>(deployedArtifacts[typeof(DecisionDefinitionEntity)]);
            }
        }

        public IList<IDecisionRequirementsDefinition> DeployedDecisionRequirementsDefinitions
        {
            get
            {
                if (deployedArtifacts == null || !deployedArtifacts.ContainsKey(typeof(DecisionRequirementsDefinitionEntity)))
                    return null;
                return ListExt.ConvertToListT<IDecisionRequirementsDefinition>(deployedArtifacts[typeof(DecisionRequirementsDefinitionEntity)]);
                //return deployedArtifacts == null ? null : ListExt.ConvertToListT<IDecisionRequirementsDefinition>(deployedArtifacts[typeof(DecisionRequirementsDefinitionEntity)]);
            }
        }
        public override string ToString()
        {
            return this.GetType().Name + "[id=" + Id + ", name=" + Name + ", resources=" + resources + ", deploymentTime=" + DeploymentTime + ", validatingSchema=" + IsValidatingSchema + ", isNew=" + IsNew + ", source=" + Source + ", tenantId=" + TenantId + "]";
        }

    }

}
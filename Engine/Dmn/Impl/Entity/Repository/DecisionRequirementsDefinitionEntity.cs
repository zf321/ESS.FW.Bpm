using System;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Dmn.engine.impl;
using ESS.FW.Bpm.Engine.Impl.DB;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Repository.Impl;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using ESS.Shared.Entities.Bpm;

namespace ESS.FW.Bpm.Engine.Dmn.Impl.Entity.Repository
{
    [Serializable]
    public class DecisionRequirementsDefinitionEntity : DmnDecisionRequirementsGraphImpl, IDecisionRequirementsDefinition,
        IResourceDefinitionEntity, IDbEntity, IHasDbRevision
    {
        // firstVersion is true, when version == 1 or when
        // this definition does not have any previous definitions
        protected internal bool FirstVersion;

        protected internal string id;
        //protected internal string key;
        //protected internal string name;
        protected internal string previousDecisionRequirementsDefinitionId;

        public virtual string PreviousDecisionRequirementsDefinitionId
        {
            get
            {
                EnsurePreviousDecisionRequirementsDefinitionIdInitialized();
                return previousDecisionRequirementsDefinitionId;
            }
        }

        public virtual string PreviousDecisionDefinitionId
        {
            set { previousDecisionRequirementsDefinitionId = value; }
        }

        public virtual string Id
        {
            get { return id; }
            set { id = value; }
        }

        public virtual string Category { get; set; }
        
        

        public virtual int Version { get; set; }

        public virtual string ResourceName { get; set; }

        public virtual string DeploymentId { get; set; }

        public virtual string DiagramResourceName { get; set; }

        public virtual string TenantId { get; set; }

        public virtual object GetPersistentState()
        {
            return typeof(DecisionRequirementsDefinitionEntity); 
        }

        public virtual int Revision { set; get; } = 1;


        public virtual int RevisionNext
        {
            get { return Revision + 1; }
        }

        [NotMapped]
        public virtual IResourceDefinitionEntity PreviousDefinition
        {
            get
            {
                DecisionRequirementsDefinitionEntity previousDecisionDefinition = null;

                var previousDecisionDefinitionId = PreviousDecisionRequirementsDefinitionId;
                if (!ReferenceEquals(previousDecisionDefinitionId, null))
                {
                    previousDecisionDefinition = LoadDecisionRequirementsDefinition(previousDecisionDefinitionId);

                    if (previousDecisionDefinition == null)
                    {
                        ResetPreviousDecisionRequirementsDefinitionId();
                        previousDecisionDefinitionId = PreviousDecisionRequirementsDefinitionId;

                        if (!ReferenceEquals(previousDecisionDefinitionId, null))
                            previousDecisionDefinition = LoadDecisionRequirementsDefinition(previousDecisionDefinitionId);
                    }
                }

                return previousDecisionDefinition;
            }
        }

        /// <summary>
        ///     Returns the cached version if exists; does not update the entity from the database in that case
        /// </summary>
        protected internal virtual DecisionRequirementsDefinitionEntity LoadDecisionRequirementsDefinition(
            string decisionRequirementsDefinitionId)
        {
            var configuration = Context.ProcessEngineConfiguration;
            //DeploymentCache deploymentCache = configuration.DeploymentCache;

            //DecisionRequirementsDefinitionEntity decisionRequirementsDefinition =
            //    deploymentCache.findDecisionRequirementsDefinitionFromCache(decisionRequirementsDefinitionId);

            //if (decisionRequirementsDefinition == null)
            //{
            //    //CommandContext commandContext = Context.CommandContext;
            //    DecisionRequirementsDefinitionManager manager = commandContext.DecisionRequirementsDefinitionManager;
            //    decisionRequirementsDefinition =
            //        manager.findDecisionRequirementsDefinitionById(decisionRequirementsDefinitionId);

            //    if (decisionRequirementsDefinition != null)
            //    {
            //        decisionRequirementsDefinition =
            //            deploymentCache.resolveDecisionRequirementsDefinition(decisionRequirementsDefinition);
            //    }
            //}

            //return decisionRequirementsDefinition;
            return null;
        }

        protected internal virtual void ResetPreviousDecisionRequirementsDefinitionId()
        {
            previousDecisionRequirementsDefinitionId = null;
            EnsurePreviousDecisionRequirementsDefinitionIdInitialized();
        }

        protected internal virtual void EnsurePreviousDecisionRequirementsDefinitionIdInitialized()
        {
            if (ReferenceEquals(previousDecisionRequirementsDefinitionId, null) && !FirstVersion)
            {
                previousDecisionRequirementsDefinitionId =
                    Context.CommandContext.DecisionRequirementsDefinitionManager
                        .FindPreviousDecisionRequirementsDefinitionId(key, Version, TenantId);

                if (ReferenceEquals(previousDecisionRequirementsDefinitionId, null))
                    FirstVersion = true;
            }
        }
        [NotMapped]
        public virtual ICollection<DecisionDefinitionEntity> DecisionDefinitions { get; set; }

        [NotMapped]
        public int HistoryTimeToLive { get; set; }

        public override string ToString()
        {
            return "DecisionRequirementsDefinitionEntity [id=" + id + ", revision=" + Revision + ", name=" + name +
                   ", category=" + Category + ", key=" + key + ", version=" + Version + ", deploymentId=" + DeploymentId +
                   ", tenantId=" + TenantId + "]";
        }
    }
}
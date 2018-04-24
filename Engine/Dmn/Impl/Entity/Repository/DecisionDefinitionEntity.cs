using System;
using ESS.FW.Bpm.Engine.Dmn.engine.impl;
using ESS.FW.Bpm.Engine.Impl.DB;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Repository.Impl;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ESS.Shared.Entities.Bpm;

namespace ESS.FW.Bpm.Engine.Dmn.Impl.Entity.Repository
{
    //

    [Serializable]
    public class DecisionDefinitionEntity : DmnDecisionImpl, IDecisionDefinition, IResourceDefinitionEntity, IDbEntity,
        IHasDbRevision
    {
        // firstVersion is true, when version == 1 or when
        // this definition does not have any previous definitions
        protected internal bool FirstVersion;

        protected internal string previousDecisionDefinitionId;
        protected internal int version;

        // previous decision definition //////////////////////////////////////////////
        [NotMapped]
        public virtual DecisionDefinitionEntity PreviousDefinition
        {
            get
            {
                DecisionDefinitionEntity previousDecisionDefinition = null;

                var previousDecisionDefinitionId = PreviousDecisionDefinitionId;
                if (!ReferenceEquals(previousDecisionDefinitionId, null))
                {
                    previousDecisionDefinition = LoadDecisionDefinition(previousDecisionDefinitionId);

                    if (previousDecisionDefinition == null)
                    {
                        ResetPreviousDecisionDefinitionId();
                        previousDecisionDefinitionId = PreviousDecisionDefinitionId;

                        if (!ReferenceEquals(previousDecisionDefinitionId, null))
                            previousDecisionDefinition = LoadDecisionDefinition(previousDecisionDefinitionId);
                    }
                }

                return previousDecisionDefinition;
            }
        }

        public virtual string PreviousDecisionDefinitionId
        {
            get
            {
                EnsurePreviousDecisionDefinitionIdInitialized();
                return previousDecisionDefinitionId;
            }
            set { previousDecisionDefinitionId = value; }
        }

        public virtual string Id { get; set; }


        public virtual string Category { get; set; }

        


        public virtual int Version
        {
            get { return version; }
            set
            {
                version = value;
                FirstVersion = version == 1;
            }
        }


        public virtual string DeploymentId { get; set; }


        public virtual string ResourceName { get; set; }


        public virtual string DiagramResourceName { get; set; }


        public virtual string TenantId { get; set; }


        public virtual string DecisionRequirementsDefinitionId { get; set; }


        public virtual string DecisionRequirementsDefinitionKey { get; set; }


        public virtual object GetPersistentState()
        {
             return typeof(DecisionDefinitionEntity);
        }


        public virtual int Revision { get; set; } = 1;


        public virtual int RevisionNext
        {
            get { return Revision + 1; }
        }

        [NotMapped]
        IResourceDefinitionEntity IResourceDefinitionEntity.PreviousDefinition
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        ///     Returns the cached version if exists; does not update the entity from the database in that case
        /// </summary>
        protected internal virtual DecisionDefinitionEntity LoadDecisionDefinition(string decisionDefinitionId)
        {
            throw new NotImplementedException();
            var configuration = context.Impl.Context.ProcessEngineConfiguration;
            //DeploymentCache deploymentCache = configuration.DeploymentCache;

            //DecisionDefinitionEntity decisionDefinition = deploymentCache.findDecisionDefinitionFromCache(decisionDefinitionId);

            //if (decisionDefinition == null)
            //{
            //  CommandContext commandContext = Context.CommandContext;
            //  DecisionDefinitionManager decisionDefinitionManager = commandContext.DecisionDefinitionManager;
            //  decisionDefinition = decisionDefinitionManager.findDecisionDefinitionById(decisionDefinitionId);

            //  if (decisionDefinition != null)
            //  {
            //	decisionDefinition = deploymentCache.resolveDecisionDefinition(decisionDefinition);
            //  }
            //}

            //return decisionDefinition;
            return null;
        }

        protected internal virtual void ResetPreviousDecisionDefinitionId()
        {
            previousDecisionDefinitionId = null;
            EnsurePreviousDecisionDefinitionIdInitialized();
        }

        protected internal virtual void EnsurePreviousDecisionDefinitionIdInitialized()
        {
            if (ReferenceEquals(previousDecisionDefinitionId, null) && !FirstVersion)
                if (ReferenceEquals(previousDecisionDefinitionId, null))
                    FirstVersion = true;
        }
        [NotMapped]//[ForeignKey("DecisionRequirementsDefinitionId")]
        public virtual DecisionRequirementsDefinitionEntity DecisionRequirementsDefinition { get; set; }

        public int HistoryTimeToLive { get; set;  }

        public override string ToString()
        {
            return "DecisionDefinitionEntity{" + "id='" + Id + '\'' + ", name='" + Name + '\'' + ", category='" +
                   Category + '\'' + ", key='" + Key + '\'' + ", version=" + version +
                   ", decisionRequirementsDefinitionId='" + DecisionRequirementsDefinitionId + '\'' +
                   ", decisionRequirementsDefinitionKey='" + DecisionRequirementsDefinitionKey + '\'' +
                   ", deploymentId='" + DeploymentId + '\'' + ", tenantId='" + TenantId + '\'' + '}';
        }
    }
}
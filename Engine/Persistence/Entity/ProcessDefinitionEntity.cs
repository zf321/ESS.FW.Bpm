using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Form.Impl.Handler;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Parser;
using ESS.FW.Bpm.Engine.Impl.DB;
using ESS.FW.Bpm.Engine.Impl.Pvm;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Impl.Pvm.Runtime;
using ESS.FW.Bpm.Engine.Impl.task;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Repository.Impl;
using System.ComponentModel.DataAnnotations.Schema;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Task;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Persistence.Deploy.Cache;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;
using ESS.FW.Bpm.Engine.Persistence.Entity.Impl;
using Newtonsoft.Json;
using ESS.Shared.Entities.Bpm;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{

    /// <summary>
    ///  
    /// 
    /// </summary>
    public class ProcessDefinitionEntity : ProcessDefinitionImpl, IProcessDefinition, IResourceDefinitionEntity, IDbEntity, IHasDbRevision
    {

        private const long SerialVersionUid = 1L;
        protected internal static readonly EnginePersistenceLogger Log = ProcessEngineLogger.PersistenceLogger;

        protected internal int version;
        protected internal int? historyLevel;
        protected internal IStartFormHandler startFormHandler;
        protected internal string diagramResourceName;
        protected internal IDictionary<string, TaskDefinition> taskDefinitions;
        protected internal bool isIdentityLinksInitialized = false;
        protected internal IList<IdentityLinkEntity> definitionIdentityLinkEntities = new List<IdentityLinkEntity>();
        protected internal ISet<IExpression> candidateStarterUserIdExpressions = new HashSet<IExpression>();
        protected internal ISet<IExpression> candidateStarterGroupIdExpressions = new HashSet<IExpression>();

        // firstVersion is true, when version == 1 or when
        // this definition does not have any previous definitions
        protected internal bool firstVersion = false;
        protected internal string previousProcessDefinitionId;

        public ProcessDefinitionEntity() : base(null)
        {

        }
        public virtual int Revision { get; set; } = 1;
        public virtual string Category { get; set; }
        public virtual string Key { get; set; }
        public virtual int Version
        {
            get
            {
                return version;
            }
            set
            {
                this.version = value;
                firstVersion = (this.version == 1);
            }
        }
        public virtual string ResourceName { get; set; }
        public virtual bool HasStartFormKey { get; set; }
        public virtual int SuspensionState { get; set; } = SuspensionStateFields.Active.StateCode;

        public virtual string TenantId { get; set; }
        public virtual string VersionTag { get; set; }
        //public virtual ICollection<ExecutionEntity> Executions { get; set; }
        //public virtual ICollection<IncidentEntity> Incidents { get; set; }
        //public virtual ICollection<TaskEntity> Tasks { get; set; }
        protected internal virtual void EnsureNotSuspended()
        {
            if (Suspended)
            {
                throw Log.SuspendedEntityException("Process Definition", id);
            }
        }

        public override IPvmProcessInstance CreateProcessInstance()
        {
            return base.CreateProcessInstance();
        }

        public override IPvmProcessInstance CreateProcessInstance(string businessKey)
        {
            return base.CreateProcessInstance(businessKey);
        }

        public override IPvmProcessInstance CreateProcessInstance(string businessKey, string caseInstanceId)
        {
            return base.CreateProcessInstance(businessKey, caseInstanceId);
        }

        public override IPvmProcessInstance CreateProcessInstance(string businessKey, ActivityImpl initial)
        {
            return base.CreateProcessInstance(businessKey, initial);
        }
        protected internal override PvmExecutionImpl NewProcessInstance()
        {
            ExecutionEntity newExecution = ExecutionEntity.CreateNewExecution();

            if (TenantId != null)
            {
                newExecution.TenantId = TenantId;
            }

            return newExecution;
        }

        public override IPvmProcessInstance CreateProcessInstance(string businessKey, string caseInstanceId, ActivityImpl initial)
        {

            EnsureNotSuspended();

            ExecutionEntity processInstance = (ExecutionEntity)CreateProcessInstanceForInitial(initial);

            //// do not reset executions (CAM-2557)!
            processInstance.SetExecutions(new List<IActivityExecution>());//.SetExecutions(new ArrayList<ExecutionEntity>());


            processInstance.SetProcessDefinition(processDefinition);

            //// Do not initialize variable map (let it happen lazily)

            //// reset the process instance in order to have the db-generated process instance id available
            processInstance.SetProcessInstance(processInstance);

            //// initialize business key
            if (businessKey != null)
            {
                processInstance.BusinessKey = businessKey;
            }

            //// initialize case instance id
            if (caseInstanceId != null)
            {
                processInstance.CaseInstanceId = caseInstanceId;
            }

            if (TenantId != null)
            {
                processInstance.TenantId = TenantId;
            }

            return processInstance;
        }

        public virtual IdentityLinkEntity AddIdentityLink(string userId, string groupId)
        {

            IdentityLinkEntity identityLinkEntity = IdentityLinkEntity.NewIdentityLink();
            IdentityLinks.Add(identityLinkEntity);
            identityLinkEntity.ProcessDef = (this);
            identityLinkEntity.UserId = userId;
            identityLinkEntity.GroupId = groupId;
            identityLinkEntity.Type = IdentityLinkType.Candidate;
            identityLinkEntity.TenantId = TenantId;
            identityLinkEntity.Insert();
            return identityLinkEntity;
        }

        public virtual void DeleteIdentityLink(string userId, string groupId)
        {

            IList<IdentityLinkEntity> identityLinks = Context.CommandContext.IdentityLinkManager.FindIdentityLinkByProcessDefinitionUserAndGroup(id, userId, groupId);

            foreach (IdentityLinkEntity identityLink in identityLinks)
            {
                identityLink.Delete();
            }
        }

        [NotMapped]
        [JsonIgnore]
        public virtual IList<IdentityLinkEntity> IdentityLinks
        {
            get
            {

                if (!isIdentityLinksInitialized)
                {
                    definitionIdentityLinkEntities = Context.CommandContext.IdentityLinkManager.FindIdentityLinksByProcessDefinitionId(id);
                    isIdentityLinksInitialized = true;
                }

                return definitionIdentityLinkEntities;
            }
        }

        public override string ToString()
        {
            return "ProcessDefinitionEntity[" + id + "]";
        }

        /// <summary>
        /// Updates all modifiable fields from another process definition entity. </summary>
        /// <param name="updatingProcessDefinition"> </param>
        public virtual void UpdateModifiedFieldsFromEntity(ProcessDefinitionEntity updatingProcessDefinition)
        {
            //TODO 
            if ((this.Key == updatingProcessDefinition.Key) && this.DeploymentId == updatingProcessDefinition.DeploymentId)
            {
                // TODO: add a guard once the mismatch between revisions in deployment cache and database has been resolved
                this.Revision = updatingProcessDefinition.Revision;
                this.SuspensionState = updatingProcessDefinition.SuspensionState;
            }
            else
            {
                Log.LogUpdateUnrelatedProcessDefinitionEntity(this.Key, updatingProcessDefinition.Key, this.DeploymentId, updatingProcessDefinition.DeploymentId);
            }
        }

        // previous process definition //////////////////////////////////////////////
        [JsonIgnore]
        public virtual IResourceDefinitionEntity PreviousDefinition
        {
            get
            {
                ProcessDefinitionEntity previousProcessDefinition = null;

                string previousProcessDefinitionId = PreviousProcessDefinitionId;
                if (previousProcessDefinitionId != null)
                {

                    previousProcessDefinition = LoadProcessDefinition(previousProcessDefinitionId);

                    if (previousProcessDefinition == null)
                    {
                        ResetPreviousProcessDefinitionId();
                        previousProcessDefinitionId = PreviousProcessDefinitionId;

                        if (previousProcessDefinitionId != null)
                        {
                            previousProcessDefinition = LoadProcessDefinition(previousProcessDefinitionId);
                        }
                    }
                }

                return previousProcessDefinition;
            }
        }

        /// <summary>
        /// Returns the cached version if exists; does not update the entity from the database in that case
        /// </summary>
        protected internal virtual ProcessDefinitionEntity LoadProcessDefinition(string processDefinitionId)
        {

            ProcessEngineConfigurationImpl configuration = Context.ProcessEngineConfiguration;
            DeploymentCache deploymentCache = configuration.DeploymentCache;

            ProcessDefinitionEntity processDefinition = deploymentCache.FindProcessDefinitionFromCache(processDefinitionId);

            if (processDefinition == null)
            {
                CommandContext commandContext = Context.CommandContext;
                ProcessDefinitionManager processDefinitionManager = commandContext.ProcessDefinitionManager as ProcessDefinitionManager;
                processDefinition = processDefinitionManager.FindLatestProcessDefinitionById(processDefinitionId);

                if (processDefinition != null)
                {
                    processDefinition = deploymentCache.ResolveProcessDefinition(processDefinition);
                }
            }

            return processDefinition;

        }

        protected internal virtual string PreviousProcessDefinitionId
        {
            get
            {
                EnsurePreviousProcessDefinitionIdInitialized();
                return previousProcessDefinitionId;
            }
            set
            {
                this.previousProcessDefinitionId = value;
            }
        }

        protected internal virtual void ResetPreviousProcessDefinitionId()
        {
            previousProcessDefinitionId = null;
            EnsurePreviousProcessDefinitionIdInitialized();
        }


        protected internal virtual void EnsurePreviousProcessDefinitionIdInitialized()
        {

            if (previousProcessDefinitionId == null && !firstVersion)
            {
                previousProcessDefinitionId = Context.CommandContext.ProcessDefinitionManager.FindPreviousProcessDefinitionId(Key, version, TenantId);

                if (previousProcessDefinitionId == null)
                {
                    firstVersion = true;
                }
            }
        }

        // getters and setters //////////////////////////////////////////////////////

        public virtual object GetPersistentState()
        {
            IDictionary<string, object> persistentState = new Dictionary<string, object>();
            persistentState["suspensionState"] = this.SuspensionState;
            return persistentState;
        }


        [JsonIgnore]
        public override string Description
        {
            get
            {
                return GetProperty(BpmnParse.PropertynameDocumentation).ToString();
            }
        }


        [NotMapped]
        public virtual int? HistoryLevel
        {
            get
            {
                return historyLevel;
            }
            set
            {
                this.historyLevel = value;
            }
        }

        [NotMapped]
        public virtual IStartFormHandler StartFormHandler
        {
            get
            {
                return startFormHandler;
            }
            set
            {
                this.startFormHandler = value;
            }
        }

        [NotMapped]
        public virtual IDictionary<string, TaskDefinition> TaskDefinitions
        {
            get
            {
                return taskDefinitions;
            }
            set
            {
                this.taskDefinitions = value;
            }
        }



        public virtual bool GetHasStartFormKey()
        {
            return HasStartFormKey;
        }



        public virtual bool StartFormKey
        {
            set
            {
                this.HasStartFormKey = value;
            }
        }

        [NotMapped]
        public virtual bool GraphicalNotationDefined { get; set; }

        public virtual int RevisionNext
        {
            get
            {
                return Revision + 1;
            }
        }




        public virtual bool Suspended
        {
            get
            {
                return SuspensionState == SuspensionStateFields.Suspended.StateCode;
            }
        }

        public virtual ISet<IExpression> CandidateStarterUserIdExpressions
        {
            get
            {
                return candidateStarterUserIdExpressions;
            }
        }

        public virtual void AddCandidateStarterUserIdExpression(IExpression userId)
        {
            candidateStarterUserIdExpressions.Add(userId);
        }

        public virtual ISet<IExpression> CandidateStarterGroupIdExpressions
        {
            get
            {
                return candidateStarterGroupIdExpressions;
            }
        }

        public virtual void AddCandidateStarterGroupIdExpression(IExpression groupId)
        {
            candidateStarterGroupIdExpressions.Add(groupId);
        }

        internal IList<ActivityImpl> GetActivities()
        {
            return FlowActivities;
        }
        [NotMapped]
        public virtual int HistoryTimeToLive { get; set; }
    }

}
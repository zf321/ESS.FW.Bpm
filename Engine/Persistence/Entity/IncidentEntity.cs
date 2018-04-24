using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.DB;
using ESS.FW.Bpm.Engine.Impl.Incident;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Runtime;
using System.ComponentModel.DataAnnotations.Schema;
using ESS.FW.Bpm.Engine.History.Impl;
using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.Bpm.Engine.History.Impl.Producer;
using ESS.FW.DataAccess;
using Newtonsoft.Json;
using ESS.Shared.Entities.Bpm;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{
    /// <summary>
    /// 
    /// </summary>
    public class IncidentEntity : IIncident, IDbEntity, IHasDbRevision, IHasDbReferences
    {
        public static readonly string FailedJobHandlerType = "failedJob";
        public static readonly string ExternalTaskHandlerType = "failedExternalTask";

        public virtual IList<IncidentEntity> CreateRecursiveIncidents()
        {
            IList<IncidentEntity> createdIncidents = new List<IncidentEntity>();
            CreateRecursiveIncidents(Id, createdIncidents);
            return createdIncidents;
        }

        /// <summary>
        /// Instantiate recursive a new incident a super execution
        /// (i.e. super process instance) which is affected from this
        /// incident.
        /// For example: a super process instance called via CallActivity
        /// a new process instance on which an incident happened, so that
        /// the super process instance has an incident too. 
        /// </summary>
        protected internal virtual void CreateRecursiveIncidents(string rootCauseIncidentId, IList<IncidentEntity> createdIncidents)
        {
            //TODO 性能问题
            ExecutionEntity execution = Execution;

            if (execution != null)
            {

                ExecutionEntity superExecution = (ExecutionEntity)execution.ProcessInstance.SuperExecution;//.getSuperExecution();

                if (superExecution != null)
                {

                    // create a new incident
                    IncidentEntity newIncident = Create(IncidentType);
                    newIncident.Execution = superExecution;
                    newIncident.ActivityId = superExecution.CurrentActivityId;
                    newIncident.ProcessDefinitionId = superExecution.ProcessDefinitionId;
                    newIncident.TenantId = superExecution.TenantId;

                    // set cause and root cause
                    newIncident.CauseIncidentId = Id;
                    newIncident.RootCauseIncidentId = rootCauseIncidentId;

                    // insert new incident (and create a new historic incident)
                    Insert(newIncident);

                    // add new incident to result set
                    createdIncidents.Add(newIncident);

                    newIncident.CreateRecursiveIncidents(rootCauseIncidentId, createdIncidents);
                }
            }
        }

        public static IncidentEntity CreateAndInsertIncident(string incidentType, IncidentContext context, string message)
        {
            // create new incident
            IncidentEntity newIncident = Create(incidentType);
            newIncident.IncidentMessage = message;

            // set properties from incident context
            newIncident.Configuration = context.Configuration;
            newIncident.ActivityId = context.ActivityId;
            newIncident.ProcessDefinitionId = context.ProcessDefinitionId;
            newIncident.TenantId = context.TenantId;
            newIncident.JobDefinitionId = context.JobDefinitionId;

            if (context.ExecutionId != null)
            {
                // fetch execution
                ExecutionEntity execution = Engine.context.Impl.Context.CommandContext.ExecutionManager.FindExecutionById(context.ExecutionId);

                // link incident with execution
                newIncident.Execution = execution;
            }

            // insert new incident (and create a new historic incident)
            Insert(newIncident);

            return newIncident;
        }

        protected internal static IncidentEntity Create(string incidentType)
        {
            //string incidentId = context.Impl.Context.ProcessEngineConfiguration.IdGenerator.DbSqlSessionFactory.IdGenerator.NextId;
            string incidentId = context.Impl.Context.CommandContext.Scope.Resolve<IDGenerator>().NewGuid();

            // decorate new incident
            IncidentEntity newIncident = new IncidentEntity();
            newIncident.Id = incidentId;
            newIncident.IncidentTimestamp = ClockUtil.CurrentTime;
            newIncident.IncidentType = incidentType;
            newIncident.CauseIncidentId = incidentId;
            newIncident.RootCauseIncidentId = incidentId;

            return newIncident;
        }

        protected internal static void Insert(IncidentEntity incident)
        {
            // persist new incident
            context.Impl.Context.CommandContext.IncidentManager.Add(incident);

            incident.FireHistoricIncidentEvent(HistoryEventTypes.IncidentCreate);
        }

        public virtual void Delete()
        {
            Remove(false);
        }

        public virtual void Resolve()
        {
            Remove(true);
        }

        protected internal virtual void Remove(bool resolved)
        {

            ExecutionEntity execution = Execution;

            if (execution != null)
            {
                // Extract possible super execution of the assigned execution
                ExecutionEntity superExecution = null;
                if (execution.Id == execution.ProcessInstanceId)
                {
                    superExecution = (ExecutionEntity) execution.SuperExecution;
                }
                else
                {
                    //superExecution = execution.getProcessInstance().getSuperExecution();
                    superExecution = (ExecutionEntity)execution.ProcessInstance.SuperExecution;
                }

                if (superExecution != null)
                {
                    // get the incident, where this incident is the cause
                    IncidentEntity parentIncident = superExecution.GetIncidentByCauseIncidentId(Id);

                    if (parentIncident != null)
                    {
                        // remove the incident
                        parentIncident.Remove(resolved);
                    }
                }

                // remove link to execution
                execution.RemoveIncident(this);
            }

            // always delete the incident
            context.Impl.Context.CommandContext.IncidentManager.Delete(this);//.DbEntityManager.Delete(this);

            // update historic incident
            HistoryEventTypes eventType = resolved ? HistoryEventTypes.IncidentResolve : HistoryEventTypes.IncidentDelete;
            FireHistoricIncidentEvent(eventType);
        }

        protected internal virtual void FireHistoricIncidentEvent(HistoryEventTypes eventType)
        {
            ProcessEngineConfigurationImpl processEngineConfiguration = context.Impl.Context.ProcessEngineConfiguration;

            IHistoryLevel historyLevel = processEngineConfiguration.HistoryLevel;
            if (historyLevel.IsHistoryEventProduced(eventType, this))
            {

                HistoryEventProcessor.ProcessHistoryEvents(new HistoryEventCreatorAnonymousInnerClassHelper(this, eventType));
            }
        }

        private class HistoryEventCreatorAnonymousInnerClassHelper : HistoryEventProcessor.HistoryEventCreator
        {
            private readonly IncidentEntity _outerInstance;

            private HistoryEventTypes _eventType;

            public HistoryEventCreatorAnonymousInnerClassHelper(IncidentEntity outerInstance, HistoryEventTypes eventType)
            {
                this._outerInstance = outerInstance;
                this._eventType = eventType;
            }

            public override HistoryEvent CreateHistoryEvent(IHistoryEventProducer producer)
            {
                HistoryEvent @event = null;
                if (HistoryEventTypes.IncidentCreate.EventName.Equals(_eventType.EventName))
                {
                    @event = producer.CreateHistoricIncidentCreateEvt(_outerInstance);

                }
                else if (HistoryEventTypes.IncidentResolve.EventName.Equals(_eventType.EventName))
                {
                    @event = producer.CreateHistoricIncidentResolveEvt(_outerInstance);

                }
                else if (HistoryEventTypes.IncidentDelete.EventName.Equals(_eventType.EventName))
                {
                    @event = producer.CreateHistoricIncidentDeleteEvt(_outerInstance);
                }
                return @event;
            }
        }
        [JsonIgnore]
        public ISet<string> ReferencedEntityIds
        {
            get
            {
                ISet<string> referenceIds = new HashSet<string>();

                if (CauseIncidentId != null)
                {
                    referenceIds.Add(CauseIncidentId);
                }

                return referenceIds;
            }
        }

        public string Id { get; set; }


        public DateTime IncidentTimestamp { get; set; }


        public string IncidentType { get; set; }


        public string IncidentMessage { get; set; }


        public string ExecutionId { get; set; }


        public string ActivityId { get; set; }


        public string ProcessInstanceId { get; set; }

        [JsonIgnore]
        public virtual ProcessDefinitionEntity ProcessDefinition
        {
            get
            {
                if (ProcessDefinitionId != null)
                {
                    return context.Impl.Context.ProcessEngineConfiguration.DeploymentCache.FindDeployedProcessDefinitionById(ProcessDefinitionId);
                }
                return null;
            }
        }

        public string ProcessDefinitionId { get; set; }


        public string CauseIncidentId { get; set; }


        public string RootCauseIncidentId { get; set; }


        public string Configuration { get; set; }

        public string TenantId { get; set; }


        public virtual string JobDefinitionId { set; get; }


        [JsonIgnore]
        public virtual ExecutionEntity Execution
        {
            set
            {
                if (value != null)
                {
                    ExecutionId = value.Id;
                    ProcessInstanceId = value.ProcessInstanceId;
                    value.AddIncident(this);
                }
                else
                {
                    Execution.RemoveIncident(this);
                    ExecutionId = null;
                    ProcessInstanceId = null;
                }
            }
            get
            {
                if (ExecutionId != null)
                {
                    return context.Impl.Context.CommandContext.ExecutionManager.FindExecutionById(ExecutionId);
                }
                else
                {
                    return null;
                }
            }
        }


        public object GetPersistentState()
        {
            IDictionary<string, object> persistentState = new Dictionary<string, object>();
            persistentState["executionId"] = ExecutionId;
            persistentState["processDefinitionId"] = ProcessDefinitionId;
            persistentState["activityId"] = ActivityId;
            persistentState["jobDefinitionId"] = JobDefinitionId;
            return persistentState;
        }

        public int Revision { set; get; }


        public int RevisionNext
        {
            get
            {
                return Revision + 1;
            }
        }
        
        //public virtual ExecutionEntity ProcessInstanceExecution { get; set; }
        //public virtual ICollection<IncidentEntity> CauseIncidentIncidents { get; set; }
        //[NotMapped]////[ForeignKey("CauseIncidentId")]
        //public virtual IncidentEntity CauseIncidentIncident { get; set; }
        //public virtual ICollection<IncidentEntity> RootCauseIncidentIncidents { get; set; }
        //[NotMapped]////[ForeignKey("RootCauseIncidentId")]
        //public virtual IncidentEntity RootCauseIncidentIncident { get; set; }
        ////[ForeignKey("JobDefinitionId")]
        //public virtual JobDefinitionEntity JobDefinition { get; set; }
        public override string ToString()
        {
            return this.GetType().Name + "[id=" + Id + ", incidentTimestamp=" + IncidentTimestamp + ", incidentType=" + IncidentType + ", executionId=" + ExecutionId + ", activityId=" + ActivityId + ", processInstanceId=" + ProcessInstanceId + ", processDefinitionId=" + ProcessDefinitionId + ", causeIncidentId=" + CauseIncidentId + ", rootCauseIncidentId=" + RootCauseIncidentId + ", configuration=" + Configuration + ", tenantId=" + TenantId + ", incidentMessage=" + IncidentMessage + ", jobDefinitionId=" + JobDefinitionId + "]";
        }

        public override int GetHashCode()
        {
            const int prime = 31;
            int result = 1;
            result = prime * result + ((Id == null) ? 0 : Id.GetHashCode());
            return result;
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return true;
            }
            if (obj == null)
            {
                return false;
            }
            if (this.GetType() != obj.GetType())
            {
                return false;
            }
            IncidentEntity other = (IncidentEntity)obj;
            if (Id == null)
            {
                if (other.Id != null)
                {
                    return false;
                }
            }
            else if (!Id.Equals(other.Id))
            {
                return false;
            }
            return true;
        }

    }

}
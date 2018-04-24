using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using ESS.FW.Bpm.Engine.Impl.DB;
using ESS.FW.Bpm.Engine.Impl.Event;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.Shared.Entities.Bpm;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class EventSubscriptionEntity : IEventSubscription, IDbEntity, IHasDbRevision
    {

        private const long SerialVersionUid = 1L;

        // persistent state ///////////////////////////

        protected internal string activityId;
        private ExecutionEntity executionEneity;

        // runtime state /////////////////////////////
        [NotMapped]
        public ExecutionEntity Execution
        {
            get
            {
                if (executionEneity != null)
                {
                    return executionEneity;
                }
                if (ExecutionId != null)
                {
                    return context.Impl.Context.CommandContext.ExecutionManager.FindExecutionById(ExecutionId);
                }
                return null;
            }
            set {
                executionEneity = value;
                if (value != null)
                {
                    ExecutionId = value.Id;
                }
            }
        }
        protected internal ActivityImpl activity;
        protected internal EventSubscriptionJobDeclaration jobDeclaration;

        /////////////////////////////////////////////

        //only for mybatis
        public EventSubscriptionEntity()
        {
        }

        public EventSubscriptionEntity(EventType eventType)
        {
            this.Created = ClockUtil.CurrentTime;
            this.EventType = eventType.Name;
        }

        public EventSubscriptionEntity(ExecutionEntity executionEntity, EventType eventType) : this(eventType)
        {
            SetExecution(executionEntity);
            Activity = Execution.Activity as ActivityImpl;
            this.ProcessInstanceId = executionEntity.ProcessInstanceId;
            this.TenantId = executionEntity.TenantId;
        }

        // processing /////////////////////////////

        public virtual void EventReceived(object payload, bool processASync)
        {
            if (processASync)
            {
                ScheduleEventAsync(payload);
            }
            else
            {
                ProcessEventSync(payload);
            }
        }

        protected internal virtual void ProcessEventSync(object payload)
        {
            IEventHandler eventHandler = context.Impl.Context.ProcessEngineConfiguration.getEventHandler(EventType);
            EnsureUtil.EnsureNotNull("Could not find eventhandler for event of type '" + EventType + "'", "eventHandler", eventHandler);
            eventHandler.HandleEvent(this, payload, context.Impl.Context.CommandContext);
        }

        protected internal virtual void ScheduleEventAsync(object payload)
        {

            EventSubscriptionJobDeclaration asyncDeclaration = JobDeclaration;

            if (asyncDeclaration == null)
            {
                // fallback to sync if we couldn't find a job declaration
                ProcessEventSync(payload);
            }
            else
            {
                MessageEntity message = asyncDeclaration.CreateJobInstance(this);
                CommandContext commandContext = context.Impl.Context.CommandContext;
                commandContext.JobManager.Send(message);
            }
        }

        // persistence behavior /////////////////////

        public virtual void Delete()
        {
            context.Impl.Context.CommandContext.EventSubscriptionManager.DeleteEventSubscription(this);
            RemoveFromExecution();
        }

        public virtual void Insert()
        {
            context.Impl.Context.CommandContext.EventSubscriptionManager.Insert(this);
            AddToExecution();
        }


        public static EventSubscriptionEntity CreateAndInsert(ExecutionEntity executionEntity, EventType eventType, ActivityImpl activity)
        {
            return CreateAndInsert(executionEntity, eventType, activity, null);
        }

        public static EventSubscriptionEntity CreateAndInsert(ExecutionEntity executionEntity, EventType eventType, ActivityImpl activity, string configuration)
        {
            EventSubscriptionEntity eventSubscription = new EventSubscriptionEntity(executionEntity, eventType);
            eventSubscription.Activity = activity;
            eventSubscription.TenantId = executionEntity.TenantId;
            eventSubscription.Configuration = configuration;
            eventSubscription.Insert();
            return eventSubscription;
        }

        // referential integrity -> ExecutionEntity ////////////////////////////////////

        protected internal virtual void AddToExecution()
        {
            // add reference in execution
            ExecutionEntity execution = GetExecution();
            if (execution != null)
            {
                execution.AddEventSubscription(this);
            }
        }

        protected internal virtual void RemoveFromExecution()
        {
            // remove reference in execution
            ExecutionEntity execution = GetExecution();
            if (execution != null)
            {
                execution.RemoveEventSubscription(this);
            }
        }

        public virtual object GetPersistentState()
        {
            Dictionary<string, object> persistentState = new Dictionary<string, object>();
            persistentState["executionId"] = ExecutionId;
            persistentState["configuration"] = Configuration;
            persistentState["activityId"] = activityId;
            persistentState["eventName"] = EventName;
            return persistentState;
        }

        // getters & setters ////////////////////////////

        public virtual ExecutionEntity GetExecution()
        {
            if (Execution == null && ExecutionId != null)
            {
                Execution = context.Impl.Context.CommandContext.ExecutionManager.FindExecutionById(ExecutionId);
            }
            return Execution;
        }

        public virtual void SetExecution(ExecutionEntity execution)
        {
            if (execution != null)
            {
                this.Execution = execution;
                this.ExecutionId = execution.Id;
                AddToExecution();
            }
            else
            {
                RemoveFromExecution();
                this.ExecutionId = null;
                this.Execution = null;
            }
        }

        [NotMapped]
        public virtual ActivityImpl Activity
        {
            get
            {
                if (activity == null && activityId != null)
                {
                    ProcessDefinitionImpl processDefinition = ProcessDefinition;
                    activity = (ActivityImpl)processDefinition.FindActivity(activityId);
                }
                return activity;
            }
            set
            {
                this.activity = value;
                if (value != null)
                {
                    this.activityId = value.Id;
                }
            }
        }
        [NotMapped]
        public virtual ProcessDefinitionEntity ProcessDefinition
        {
            get
            {
                if (ExecutionId != null)
                {
                    ExecutionEntity execution = GetExecution();
                    return (ProcessDefinitionEntity)execution.ProcessDefinition;
                }
                else
                {
                    // this assumes that start event subscriptions have the process definition id
                    // as their configuration (which holds for message and signal start events)
                    string processDefinitionId = Configuration;
                    return context.Impl.Context.ProcessEngineConfiguration.DeploymentCache.FindDeployedProcessDefinitionById(processDefinitionId);
                }
            }
        }


        [NotMapped]
        public virtual EventSubscriptionJobDeclaration JobDeclaration
        {
            get
            {
                if (jobDeclaration == null)
                {
                    jobDeclaration = EventSubscriptionJobDeclaration.FindDeclarationForSubscription(this);
                }

                return jobDeclaration;
            }
        }

        public virtual string Id { get; set; }


        public virtual int Revision { get; set; } = 1;


        public virtual int RevisionNext
        {
            get
            {
                return Revision + 1;
            }
        }

        public virtual bool IsSubscriptionForEventType(EventType eventType)
        {
            return this.EventType == eventType.Name;
        }

        public virtual string EventType { get; set; }


        public virtual string EventName { get; set; }

        public virtual string ExecutionId { get; set; }


        public virtual string ProcessInstanceId { get; set; }


        public virtual string Configuration { get; set; }


        public virtual string ActivityId
        {
            get
            {
                return activityId;
            }
            set
            {
                this.activityId = value;
                this.activity = null;
            }
        }


        public virtual DateTime Created { get; set; }


        public virtual string TenantId { get; set; }


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
            EventSubscriptionEntity other = (EventSubscriptionEntity)obj;
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

        public override string ToString()
        {
            return this.GetType().Name + "[id=" + Id + ", eventType=" + EventType + ", eventName=" + EventName + ", executionId=" + ExecutionId + ", processInstanceId=" + ProcessInstanceId + ", activityId=" + activityId + ", tenantId=" + TenantId + ", configuration=" + Configuration + ", revision=" + Revision + ", created=" + Created + "]";
        }

    }

}
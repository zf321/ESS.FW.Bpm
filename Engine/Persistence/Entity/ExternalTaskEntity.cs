using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior;
using ESS.FW.Bpm.Engine.Impl.DB;
using ESS.FW.Bpm.Engine.Impl.Incident;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Externaltask;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using ESS.Shared.Entities.Bpm;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{

	/// <summary>
	/// 
	/// 
	/// </summary>
	public partial class ExternalTaskEntity : IExternalTask, IDbEntity, IHasDbRevision
	{

        protected internal static readonly EnginePersistenceLogger Log = ProcessEngineLogger.PersistenceLogger;
        private const string ExceptionName = "externalTask.exceptionByteArray";

        /// <summary>
        /// Note: <seealso cref="String#length()"/> counts Unicode supplementary
        /// characters twice, so for a String consisting only of those,
        /// the limit is effectively MAX_EXCEPTION_MESSAGE_LENGTH / 2
        /// </summary>
        public const int MaxExceptionMessageLength = 666;


        protected internal string errorMessage;

        public virtual ResourceEntity ErrorDetailsByteArray { get; set; }

	    //protected internal int SuspensionState = SuspensionStateFields.Active.StateCode;
        [JsonIgnore]
        [IgnoreDataMember]
        public ExecutionEntity Execution { get; set; }


        public virtual string Id { get; set; }
        public virtual string TopicName { get; set; }
        public virtual string WorkerId { get; set; }
        public virtual DateTime LockExpirationTime { get; set; }
        public virtual string ExecutionId { get; set; }
        public virtual string ProcessDefinitionKey { get; set; }
        public virtual string ActivityId { get; set; }
        public virtual string ActivityInstanceId { get; set; }
        public virtual int Revision { get; set; }
        public virtual int RevisionNext
        {
            get
            {
                return Revision + 1;
            }
        }
        public virtual int SuspensionState { get; set; }= SuspensionStateFields.Active.StateCode;
        public virtual bool Suspended
        {
            get
            {
                return SuspensionState == SuspensionStateFields.Suspended.StateCode;
            }
        }
        public virtual string ProcessInstanceId { get; set; }
        public virtual string ProcessDefinitionId { get; set; }
        public virtual string TenantId { get; set; }

	    public long Priority { get; set; }

	    public virtual int? Retries { get; set; }
        public virtual string ErrorMessage
        {
            get
            {
                return errorMessage;
            }
            set
            {
                if (value != null && value.Length > MaxExceptionMessageLength)
                {
                    this.errorMessage = value.Substring(0, MaxExceptionMessageLength);
                }
                else
                {
                    this.errorMessage = value;
                }
            }
        }

        public virtual bool AreRetriesLeft()
        {
            return Retries == null || Retries > 0;
        }




        public virtual object GetPersistentState()
        {
                IDictionary<string, object> persistentState = new Dictionary<string, object>();
                persistentState["topic"] = TopicName;
                persistentState["workerId"] = WorkerId;
                persistentState["lockExpirationTime"] = LockExpirationTime;
                persistentState["retries"] = Retries;
                persistentState["errorMessage"] = errorMessage;
                persistentState["executionId"] = ExecutionId;
                persistentState["processInstanceId"] = ProcessInstanceId;
                persistentState["processDefinitionId"] = ProcessDefinitionId;
                persistentState["processDefinitionKey"] = ProcessDefinitionKey;
                persistentState["activityId"] = ActivityId;
                persistentState["activityInstanceId"] = ActivityInstanceId;
                persistentState["suspensionState"] = SuspensionState;
                persistentState["tenantId"] = TenantId;
                persistentState["priority"] = Priority;

                if (ErrorDetailsByteArrayId != null)
                {
                    persistentState["errorDetailsByteArrayId"] = ErrorDetailsByteArrayId;
                }

                return persistentState;
        }

        public virtual void Insert()
        {
            context.Impl.Context.CommandContext.ExternalTaskManager.Add(this);

            GetExecution().AddExternalTask(this);
        }

        /// <summary>
        /// Method implementation relies on the command context object,
        /// therefore should be invoked from the commands only
        /// </summary>
        /// <returns> error details persisted in byte array table </returns>
        [NotMapped]
        [JsonIgnore]
        [IgnoreDataMember]
        public virtual string ErrorDetails
        {
            get
            {
                ResourceEntity byteArray = ErrorByteArray;
                return ExceptionUtil.GetExceptionStacktrace(byteArray);
            }
            set
            {
                throw new NotImplementedException();
                //EnsureUtil.EnsureNotNull("exception", value);

                //byte[] exceptionBytes = toByteArray(value);

                //ResourceEntity byteArray = ErrorByteArray;

                //if (byteArray == null)
                //{
                   
                //    byteArray = createExceptionByteArray(ExceptionName, exceptionBytes);
                //    errorDetailsByteArrayId = byteArray.Id;
                //    ErrorDetailsByteArray = byteArray;
                //}
                //else
                //{
                //    byteArray.Bytes = exceptionBytes;
                //}
            }
        }



        public virtual string ErrorDetailsByteArrayId { get; set; }

	    protected internal virtual ResourceEntity ErrorByteArray
        {
            get
            {
                EnsureErrorByteArrayInitialized();
                return ErrorDetailsByteArray;
            }
        }

        protected internal virtual void EnsureErrorByteArrayInitialized()
        {
            if (ErrorDetailsByteArray == null && ErrorDetailsByteArrayId != null)
            {
                ErrorDetailsByteArray = context.Impl.Context.CommandContext.ByteArrayManager.Get(ErrorDetailsByteArrayId);
            }
        }

        public virtual void Delete()
        {
            DeleteFromExecutionAndRuntimeTable();
            ProduceHistoricExternalTaskDeletedEvent();
        }

        protected internal virtual void DeleteFromExecutionAndRuntimeTable()
        {
            GetExecution().RemoveExternalTask(this);

            CommandContext commandContext = context.Impl.Context.CommandContext;

            commandContext.ExternalTaskManager.Delete(this);

            // Also delete the external tasks's error details byte array
            if (ErrorDetailsByteArrayId != null)
            {
                commandContext.ByteArrayManager.DeleteByteArrayById(ErrorDetailsByteArrayId);
            }
        }

        public virtual void Complete(IDictionary<string, object> variables)
        {
            EnsureActive();

            ExecutionEntity associatedExecution = GetExecution();

            if (variables != null)
            {
                associatedExecution.Variables = variables;
            }

            DeleteFromExecutionAndRuntimeTable();

            ProduceHistoricExternalTaskSuccessfulEvent();

            associatedExecution.Signal(null, null);
        }

        /// <summary>
        /// process failed state, make sure that binary entity is created for the errorMessage, shortError
        /// message does not exceed limit, handle properly retry counts and incidents
        /// </summary>
        /// <param name="errorMessage"> - short error message text </param>
        /// <param name="errorDetails"> - full error details </param>
        /// <param name="retries"> - updated value of retries left </param>
        /// <param name="retryDuration"> - used for lockExpirationTime calculation </param>
        public virtual void Failed(string errorMessage, string errorDetails, int retries, long retryDuration)
        {
            EnsureActive();

            this.errorMessage = errorMessage;
            if (errorDetails != null)
            {
                ErrorDetails = errorDetails;
            }
            throw new NotImplementedException();
            //this.lockExpirationTime = new DateTime(ClockUtil.CurrentTime.Time + retryDuration);
            RetriesAndManageIncidents = retries;
            ProduceHistoricExternalTaskFailedEvent();
        }

        public virtual void BpmnError(string errorCode)
        {
            EnsureActive();
            IActivityExecution activityExecution = GetExecution();
            BpmnError bpmnError = new BpmnError(errorCode);
            try
            {
                ExternalTaskActivityBehavior behavior = ((ExternalTaskActivityBehavior)activityExecution.Activity.ActivityBehavior);
                behavior.PropagateBpmnError(bpmnError, activityExecution);
            }
            catch (System.Exception ex)
            {
                throw ProcessEngineLogger.CmdLogger.ExceptionBpmnErrorPropagationFailed(errorCode, ex);
            }
        }

        public virtual int RetriesAndManageIncidents
        {
            set
            {

                if (AreRetriesLeft() && value <= 0)
                {
                    CreateIncident();
                }
                else if (!AreRetriesLeft() && value > 0)
                {
                    RemoveIncident();
                }

                Retries = value;
            }
        }

        protected internal virtual void CreateIncident()
        {
            throw new NotImplementedException();
            //          IncidentHandler incidentHandler = Context.ProcessEngineConfiguration.getIncidentHandler(Incident.EXTERNAL_TASK_HANDLER_TYPE);

            //incidentHandler.handleIncident(CreateIncidentContext(), errorMessage);
        }

        protected internal virtual void RemoveIncident()
        {
            throw new NotImplementedException();
            //IncidentHandler handler = Context.ProcessEngineConfiguration.getIncidentHandler(Incident.EXTERNAL_TASK_HANDLER_TYPE);

            //handler.resolveIncident(CreateIncidentContext());
        }

        protected internal virtual IncidentContext CreateIncidentContext()
        {
            IncidentContext context = new IncidentContext();
            context.ProcessDefinitionId = ProcessDefinitionId;
            context.ExecutionId = ExecutionId;
            context.ActivityId = ActivityId;
            context.TenantId = TenantId;
            context.Configuration = Id;
            return context;
        }

        public virtual void Lock(string workerId, long lockDuration)
        {
            this.WorkerId = workerId;
            //this.lockExpirationTime = new DateTime(ClockUtil.CurrentTime.Time + lockDuration);
        }

        public virtual ExecutionEntity GetExecution()
        {
            EnsureExecutionInitialized();
            return Execution;
        }

        public virtual void SetExecution(ExecutionEntity execution)
        {
            this.Execution = execution;
        }

        protected internal virtual void EnsureExecutionInitialized()
        {
            if (Execution == null)
            {
                Execution = context.Impl.Context.CommandContext.ExecutionManager.FindExecutionById(ExecutionId);
                EnsureUtil.EnsureNotNull("Cannot find execution with id " + ExecutionId + " for external task " + Id, "execution", Execution);
            }
        }

        protected internal virtual void EnsureActive()
        {
            //if (suspensionState == SuspensionStateFields.Suspended.StateCode)
            //{
            //  throw Log.suspendedEntityException(EntityTypes.EXTERNAL_TASK, id);
            //}
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return "ExternalTaskEntity [" + "id=" + Id + ", revision=" + Revision + ", topicName=" + TopicName + ", workerId=" + WorkerId + ", lockExpirationTime=" + LockExpirationTime + ", priority=" + Priority + ", errorMessage=" + errorMessage + ", errorDetailsByteArray=" + ErrorDetailsByteArray + ", errorDetailsByteArrayId=" + ErrorDetailsByteArrayId + ", executionId=" + ExecutionId + "]";
        }

        public virtual void Unlock()
        {
            WorkerId = null;
            //lockExpirationTime = null;
        }

        public static ExternalTaskEntity CreateAndInsert(ExecutionEntity execution, string topic, long priority)
        {
            ExternalTaskEntity externalTask = new ExternalTaskEntity();

            externalTask.TopicName = topic;
            externalTask.ExecutionId = execution.Id;
            externalTask.ProcessInstanceId = execution.ProcessInstanceId;
            externalTask.ProcessDefinitionId = execution.ProcessDefinitionId;
            externalTask.ActivityId = execution.ActivityId;
            externalTask.ActivityInstanceId = execution.ActivityInstanceId;
            externalTask.TenantId = execution.TenantId;
            externalTask.Priority = priority;

            ProcessDefinitionEntity processDefinition = (ProcessDefinitionEntity)execution.ProcessDefinition;
            externalTask.ProcessDefinitionKey = processDefinition.Key;

            externalTask.Insert();
            externalTask.ProduceHistoricExternalTaskCreatedEvent();

            return externalTask;
        }

        protected internal virtual void ProduceHistoricExternalTaskCreatedEvent()
        {
            CommandContext commandContext = context.Impl.Context.CommandContext;
            commandContext.HistoricExternalTaskLogManager.FireExternalTaskCreatedEvent(this);
        }

        protected internal virtual void ProduceHistoricExternalTaskFailedEvent()
        {
            CommandContext commandContext = context.Impl.Context.CommandContext;
            commandContext.HistoricExternalTaskLogManager.FireExternalTaskFailedEvent(this);
        }

        protected internal virtual void ProduceHistoricExternalTaskSuccessfulEvent()
        {
            CommandContext commandContext = context.Impl.Context.CommandContext;
            commandContext.HistoricExternalTaskLogManager.FireExternalTaskSuccessfulEvent(this);
        }

        protected internal virtual void ProduceHistoricExternalTaskDeletedEvent()
        {
            CommandContext commandContext = context.Impl.Context.CommandContext;
            commandContext.HistoricExternalTaskLogManager.FireExternalTaskDeletedEvent(this);
        }

	}

}
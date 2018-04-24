using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Batch.Impl;
using ESS.FW.Bpm.Engine.Batch.Impl.History;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Externaltask;
using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.Bpm.Engine.Impl.migration.instance;
using ESS.FW.Bpm.Engine.Impl.oplog;
using ESS.FW.Bpm.Engine.Impl.Pvm;
using ESS.FW.Bpm.Engine.Impl.Pvm.Runtime;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;
using ESS.FW.Bpm.Engine.Batch;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.DataAccess;
using ESS.FW.Bpm.Engine.Management;
using ESS.FW.Bpm.Engine.Impl.util;

namespace ESS.FW.Bpm.Engine.History.Impl.Producer
{


    /// <summary>
    /// 
    /// 
    /// 
    /// </summary>
    public class DefaultHistoryEventProducer : IHistoryEventProducer
    {

        protected internal virtual void InitActivityInstanceEvent(HistoricActivityInstanceEventEntity evt, ExecutionEntity execution, IHistoryEventType eventType)
        {
            IPvmScope eventSource = execution.Activity;
            if (eventSource == null)
            {
                eventSource = (IPvmScope)execution.EventSource;
            }
            string activityInstanceId = execution.ActivityInstanceId;

            string parentActivityInstanceId = null;
            ExecutionEntity parentExecution = (ExecutionEntity)execution.Parent;
            if (parentExecution != null && CompensationBehavior.IsCompensationThrowing(parentExecution) && execution.Activity != null)
            {
                parentActivityInstanceId = CompensationBehavior.GetParentActivityInstanceId(execution);
            }
            else
            {
                parentActivityInstanceId = execution.ParentActivityInstanceId;
            }

            InitActivityInstanceEvent(evt, execution, eventSource, activityInstanceId, parentActivityInstanceId, eventType);
        }

        protected internal virtual void InitActivityInstanceEvent(HistoricActivityInstanceEventEntity evt, MigratingActivityInstance migratingActivityInstance, IHistoryEventType eventType)
        {
            IPvmScope eventSource = migratingActivityInstance.TargetScope;
            string activityInstanceId = migratingActivityInstance.ActivityInstanceId;

            MigratingActivityInstance parentInstance = (MigratingActivityInstance)migratingActivityInstance.Parent;
            string parentActivityInstanceId = null;
            if (parentInstance != null)
            {
                parentActivityInstanceId = parentInstance.ActivityInstanceId;
            }

            ExecutionEntity execution = migratingActivityInstance.ResolveRepresentativeExecution();

            InitActivityInstanceEvent(evt, execution, eventSource, activityInstanceId, parentActivityInstanceId, eventType);
        }

        protected internal virtual void InitActivityInstanceEvent(HistoricActivityInstanceEventEntity evt, ExecutionEntity execution, IPvmScope eventSource, string activityInstanceId, string parentActivityInstanceId, IHistoryEventType eventType)
        {

            evt.Id = activityInstanceId;
            evt.EventType = eventType.EventName;
            evt.ActivityInstanceId = activityInstanceId;
            evt.ParentActivityInstanceId = parentActivityInstanceId;
            evt.ProcessDefinitionId = execution.ProcessDefinitionId;
            evt.ProcessInstanceId = execution.ProcessInstanceId;
            evt.ExecutionId = execution.Id;
            evt.TenantId = execution.TenantId;

            ProcessDefinitionEntity definition = execution.GetProcessDefinition();
            if (definition != null)
            {
                evt.ProcessDefinitionKey = definition.Key;
            }

            evt.ActivityId = eventSource.Id;
            evt.ActivityName = (string)eventSource.GetProperty("name");
            evt.ActivityType = (string)eventSource.GetProperty("type");

            // update sub process reference
            ExecutionEntity subProcessInstance = execution.GetSubProcessInstance();
            if (subProcessInstance != null)
            {
                evt.CalledProcessInstanceId = subProcessInstance.Id;
            }

            // update sub case reference
            //CaseExecutionEntity subCaseInstance = execution.GetSubCaseInstance();
            //if (subCaseInstance != null)
            //{
            //    evt.CalledCaseInstanceId = subCaseInstance.Id;
            //}
        }


        protected internal virtual void InitProcessInstanceEvent(HistoricProcessInstanceEventEntity evt, ExecutionEntity execution, IHistoryEventType eventType)
        {

            string processDefinitionId = execution.ProcessDefinitionId;
            string processInstanceId = execution.ProcessInstanceId;
            string executionId = execution.Id;
            // the given execution is the process instance!
            string caseInstanceId = execution.CaseInstanceId;
            string tenantId = execution.TenantId;

            ProcessDefinitionEntity definition = execution.GetProcessDefinition();
            string processDefinitionKey = null;
            if (definition != null)
            {
                processDefinitionKey = definition.Key;
            }

            evt.Id = processInstanceId;
            evt.EventType = eventType.EventName;
            evt.ProcessDefinitionKey = processDefinitionKey;
            evt.ProcessDefinitionId = processDefinitionId;
            evt.ProcessInstanceId = processInstanceId;
            evt.ExecutionId = executionId;
            evt.BusinessKey = execution.ProcessBusinessKey;
            evt.CaseInstanceId = caseInstanceId;
            evt.TenantId = tenantId;

            //if (execution.GetSuperCaseExecution() != null)
            //{
            //    evt.SuperCaseInstanceId = execution.GetSuperCaseExecution().CaseInstanceId;
            //}
            if (execution.SuperExecution != null)
            {
                evt.SuperProcessInstanceId = execution.SuperExecution.ProcessInstanceId;
            }
        }

        protected internal virtual void InitTaskInstanceEvent(HistoricTaskInstanceEventEntity evt, TaskEntity taskEntity, IHistoryEventType eventType)
        {

            string processDefinitionKey = null;
            ProcessDefinitionEntity definition = taskEntity.ProcessDefinition;
            if (definition != null)
            {
                processDefinitionKey = definition.Key;
            }

            string processDefinitionId = taskEntity.ProcessDefinitionId;
            string processInstanceId = taskEntity.ProcessInstanceId;
            string executionId = taskEntity.ExecutionId;

            string caseDefinitionKey = null;
            //CaseDefinitionEntity caseDefinition = taskEntity.CaseDefinition;
            //if (caseDefinition != null)
            //{
            //    caseDefinitionKey = caseDefinition.Key;
            //}

            string caseDefinitionId = taskEntity.CaseDefinitionId;
            string caseExecutionId = taskEntity.CaseExecutionId;
            string caseInstanceId = taskEntity.CaseInstanceId;
            string tenantId = taskEntity.TenantId;

            evt.Id = taskEntity.Id;
            evt.EventType = eventType.EventName;
            evt.TaskId = taskEntity.Id;

            evt.ProcessDefinitionKey = processDefinitionKey;
            evt.ProcessDefinitionId = processDefinitionId;
            evt.ProcessInstanceId = processInstanceId;
            evt.ExecutionId = executionId;

            evt.CaseDefinitionKey = caseDefinitionKey;
            evt.CaseDefinitionId = caseDefinitionId;
            evt.CaseExecutionId = caseExecutionId;
            evt.CaseInstanceId = caseInstanceId;

            evt.Assignee = taskEntity.Assignee;
            evt.Description = taskEntity.Description;
            evt.DueDate = taskEntity.DueDate;
            evt.FollowUpDate = taskEntity.FollowUpDate;
            evt.Name = taskEntity.Name;
            evt.Owner = taskEntity.Owner;
            evt.ParentTaskId = taskEntity.ParentTaskId;
            evt.Priority = taskEntity.Priority;
            evt.TaskDefinitionKey = taskEntity.TaskDefinitionKey;
            evt.TenantId = tenantId;

            ExecutionEntity execution = taskEntity.GetExecution();
            if (execution != null)
            {
                evt.ActivityInstanceId = execution.ActivityInstanceId;
            }

        }

        protected internal virtual void InitHistoricVariableUpdateEvt(HistoricVariableUpdateEventEntity evt, VariableInstanceEntity variableInstance, IHistoryEventType eventType)
        {

            // init properties
            evt.EventType = eventType.EventName;
            evt.TimeStamp = ClockUtil.CurrentTime;
            evt.VariableInstanceId = variableInstance.Id;
            evt.ProcessInstanceId = variableInstance.ProcessInstanceId;
            evt.ExecutionId = variableInstance.ExecutionId;
            evt.CaseInstanceId = variableInstance.CaseInstanceId;
            evt.CaseExecutionId = variableInstance.CaseExecutionId;
            evt.TaskId = variableInstance.TaskId;
            evt.Revision = variableInstance.Revision;
            evt.VariableName = variableInstance.Name;
            evt.SerializerName = variableInstance.SerializerName;
            evt.TenantId = variableInstance.TenantId;

            ExecutionEntity execution = variableInstance.Execution;
            if (execution != null)
            {
                ProcessDefinitionEntity definition = execution.GetProcessDefinition();
                if (definition != null)
                {
                    evt.ProcessDefinitionId = definition.Id;
                    evt.ProcessDefinitionKey = definition.Key;
                }
            }

            //CaseExecutionEntity caseExecution = variableInstance.CaseExecution;
            //if (caseExecution != null)
            //{
            //    CaseDefinitionEntity definition = (CaseDefinitionEntity)caseExecution.CaseDefinition;
            //    if (definition != null)
            //    {
            //        evt.CaseDefinitionId = definition.Id;
            //        evt.CaseDefinitionKey = definition.Key;
            //    }
            //}

            // copy value
            evt.TextValue = variableInstance.TextValue;
            evt.TextValue2 = variableInstance.TextValue2;
            evt.DoubleValue = variableInstance.DoubleValue;
            evt.LongValue = variableInstance.LongValue;
            if (variableInstance.ByteArrayId != null)
            {
                evt.ByteValue = variableInstance.ByteArrayValue;
                //evt.Value = variableInstance.Value;
                //evt.ByteArrayValue = variableInstance.ByteArrayValue;
                var r= evt.Value;
            }
        }

        protected internal virtual void InitUserOperationLogEvent(UserOperationLogEntryEventEntity evt, UserOperationLogContext context, UserOperationLogContextEntry contextEntry, PropertyChange propertyChange)
        {
            // init properties
            evt.DeploymentId = contextEntry.DeploymentId;
            evt.EntityType = contextEntry.EntityType;
            evt.OperationType = contextEntry.OperationType;
            evt.OperationId = context.OperationId;
            evt.UserId = context.UserId;
            evt.ProcessDefinitionId = contextEntry.ProcessDefinitionId;
            evt.ProcessDefinitionKey = contextEntry.ProcessDefinitionKey;
            evt.ProcessInstanceId = contextEntry.ProcessInstanceId;
            evt.ExecutionId = contextEntry.ExecutionId;
            evt.CaseDefinitionId = contextEntry.CaseDefinitionId;
            evt.CaseInstanceId = contextEntry.CaseInstanceId;
            evt.CaseExecutionId = contextEntry.CaseExecutionId;
            evt.TaskId = contextEntry.TaskId;
            evt.JobId = contextEntry.JobId;
            evt.JobDefinitionId = contextEntry.JobDefinitionId;
            evt.BatchId = contextEntry.BatchId;
            evt.Timestamp = ClockUtil.CurrentTime;

            // init property value
            evt.Property = propertyChange.PropertyName;
            evt.OrgValue = propertyChange.OrgValueString;
            evt.NewValue = propertyChange.NewValueString;
        }

        protected internal virtual void InitHistoricIncidentEvent(Event.HistoricIncidentEntity evt, IIncident incident, IHistoryEventType eventType)
        {
            // init properties
            evt.Id = incident.Id;
            evt.ProcessDefinitionId = incident.ProcessDefinitionId;
            evt.ProcessInstanceId = incident.ProcessInstanceId;
            evt.ExecutionId = incident.ExecutionId;
            evt.CreateTime = incident.IncidentTimestamp;
            evt.IncidentType = incident.IncidentType;
            evt.ActivityId = incident.ActivityId;
            evt.CauseIncidentId = incident.CauseIncidentId;
            evt.RootCauseIncidentId = incident.RootCauseIncidentId;
            evt.Configuration = incident.Configuration;
            evt.IncidentMessage = incident.IncidentMessage;
            evt.TenantId = incident.TenantId;
            evt.JobDefinitionId = incident.JobDefinitionId;

            IncidentEntity incidentEntity = (IncidentEntity)incident;
            ProcessDefinitionEntity definition = incidentEntity.ProcessDefinition;
            if (definition != null)
            {
                evt.ProcessDefinitionKey = definition.Key;
            }

            // init event type
            evt.EventType = eventType.EventName;

            // init state
            IIncidentState incidentState = IncidentStateFields.Deleted;
            if (HistoryEventTypes.IncidentDelete==eventType)
            {
                incidentState = IncidentStateFields.Deleted;
            }
            else if (HistoryEventTypes.IncidentResolve.Equals(eventType))
            {
                incidentState = IncidentStateFields.Resolved;
            }
            evt.IncidentState = incidentState.StateCode;
        }

        protected internal virtual HistoryEvent CreateHistoricVariableEvent(VariableInstanceEntity variableInstance, IVariableScope sourceVariableScope, HistoryEventTypes eventType)
        {
            string scopeActivityInstanceId = null;
            string sourceActivityInstanceId = null;

            if (variableInstance.ExecutionId!= null)
            {
                //ExecutionEntity scopeExecution = Context.CommandContext.DbEntityManager.SelectById(typeof(ExecutionEntity), variableInstance.ExecutionId);
                ExecutionEntity scopeExecution = Context.CommandContext.ExecutionManager.FindExecutionById(variableInstance.ExecutionId);
                if (variableInstance.TaskId== null && !variableInstance.IsConcurrentLocal)
                {
                    scopeActivityInstanceId = scopeExecution.ParentActivityInstanceId;

                }
                else
                {
                    scopeActivityInstanceId = scopeExecution.ActivityInstanceId;
                }
            }
            else if (!string.ReferenceEquals(variableInstance.CaseExecutionId, null))
            {
                scopeActivityInstanceId = variableInstance.CaseExecutionId;
            }

            ExecutionEntity sourceExecution = null;
            //CaseExecutionEntity sourceCaseExecution = null;
            if (sourceVariableScope is ExecutionEntity)
            {
                sourceExecution = (ExecutionEntity)sourceVariableScope;
                sourceActivityInstanceId = sourceExecution.ActivityInstanceId;

            }
            else if (sourceVariableScope is TaskEntity)
            {
                sourceExecution = ((TaskEntity)sourceVariableScope).GetExecution();
                if (sourceExecution != null)
                {
                    sourceActivityInstanceId = sourceExecution.ActivityInstanceId;
                }
                else
                {
                    //sourceCaseExecution = ((TaskEntity)sourceVariableScope).GetCaseExecution();
                    //if (sourceCaseExecution != null)
                    //{
                    //    sourceActivityInstanceId = sourceCaseExecution.Id;
                    //}
                }
            }
            //else if (sourceVariableScope is CaseExecutionEntity)
            //{
            //    sourceCaseExecution = (CaseExecutionEntity)sourceVariableScope;
            //    sourceActivityInstanceId = sourceCaseExecution.Id;
            //}

            // create event
            HistoricVariableUpdateEventEntity evt = NewVariableUpdateEventEntity(sourceExecution);
            // initialize
            InitHistoricVariableUpdateEvt(evt, variableInstance, eventType);
            // initialize sequence counter
            InitSequenceCounter(variableInstance, evt);

            // set scope activity instance id
            evt.ScopeActivityInstanceId = scopeActivityInstanceId;

            // set source activity instance id
            evt.ActivityInstanceId = sourceActivityInstanceId;

            return evt;
        }

        // event instance factory ////////////////////////

        protected internal virtual HistoricProcessInstanceEventEntity NewProcessInstanceEventEntity(ExecutionEntity execution)
        {
            return new HistoricProcessInstanceEventEntity();
        }

        protected internal virtual HistoricActivityInstanceEventEntity NewActivityInstanceEventEntity(ExecutionEntity execution)
        {
            return new HistoricActivityInstanceEventEntity();
        }

        protected internal virtual HistoricTaskInstanceEventEntity NewTaskInstanceEventEntity(IDelegateTask task)
        {
            return new HistoricTaskInstanceEventEntity();
        }

        protected internal virtual HistoricVariableUpdateEventEntity NewVariableUpdateEventEntity(ExecutionEntity execution)
        {
            return new HistoricVariableUpdateEventEntity();
        }

        protected internal virtual HistoricFormPropertyEventEntity NewHistoricFormPropertyEvent()
        {
            return new HistoricFormPropertyEventEntity();
        }

        protected internal virtual HistoricIncidentEntity NewIncidentEventEntity(IIncident incident)
        {
            return new HistoricIncidentEntity();
        }

        protected internal virtual HistoricJobLogEventEntity NewHistoricJobLogEntity(IJob job)
        {
            return new HistoricJobLogEventEntity();
        }

        protected internal virtual HistoricProcessInstanceEventEntity LoadProcessInstanceEventEntity(ExecutionEntity execution)
        {
            return NewProcessInstanceEventEntity(execution);
        }

        protected internal virtual HistoricActivityInstanceEventEntity LoadActivityInstanceEventEntity(ExecutionEntity execution)
        {
            return NewActivityInstanceEventEntity(execution);
        }

        protected internal virtual HistoricTaskInstanceEventEntity LoadTaskInstanceEvent(IDelegateTask task)
        {
            return NewTaskInstanceEventEntity(task);
        }

        protected internal virtual HistoricIncidentEntity LoadIncidentEvent(IIncident incident)
        {
            return NewIncidentEventEntity(incident);
        }

        // Implementation ////////////////////////////////

        public virtual HistoryEvent CreateProcessInstanceStartEvt(IDelegateExecution execution)
        {
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final org.camunda.bpm.persistence.entity.ExecutionEntity executionEntity = (org.camunda.bpm.persistence.entity.ExecutionEntity) execution;
            ExecutionEntity executionEntity = (ExecutionEntity)execution;

            // create event instance
            HistoricProcessInstanceEventEntity evt = NewProcessInstanceEventEntity(executionEntity);

            // initialize event
            InitProcessInstanceEvent(evt, executionEntity, HistoryEventTypes.ActivityInstanceStart);

            evt.StartActivityId = executionEntity.ActivityId;
            evt.StartTime = ClockUtil.CurrentTime;

            // set super process instance id
            ExecutionEntity superExecution = (ExecutionEntity) executionEntity.SuperExecution;
            if (superExecution != null)
            {
                evt.SuperProcessInstanceId = superExecution.ProcessInstanceId;
            }

            //state
            evt.State = HistoricProcessInstanceFields.StateActive;

            // set start user Id
            evt.StartUserId = Context.CommandContext.AuthenticatedUserId;

            return evt;
        }

        public virtual HistoryEvent CreateProcessInstanceUpdateEvt(IDelegateExecution execution)
        {
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final org.camunda.bpm.persistence.entity.ExecutionEntity executionEntity = (org.camunda.bpm.persistence.entity.ExecutionEntity) execution;
            ExecutionEntity executionEntity = (ExecutionEntity)execution;

            // create event instance
            HistoricProcessInstanceEventEntity evt = NewProcessInstanceEventEntity(executionEntity);

            // initialize event
            InitProcessInstanceEvent(evt, executionEntity, HistoryEventTypes.ActivityInstanceUpdate);

            if (executionEntity.IsSuspended)
            {
                evt.State = HistoricProcessInstanceFields.StateSuspended;
            }
            else
            {
                evt.State = HistoricProcessInstanceFields.StateActive;
            }

            return evt;
        }

        public virtual HistoryEvent CreateProcessInstanceMigrateEvt(IDelegateExecution execution)
        {
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final org.camunda.bpm.persistence.entity.ExecutionEntity executionEntity = (org.camunda.bpm.persistence.entity.ExecutionEntity) execution;
            ExecutionEntity executionEntity = (ExecutionEntity)execution;

            // create event instance
            HistoricProcessInstanceEventEntity evt = NewProcessInstanceEventEntity(executionEntity);

            // initialize event
            InitProcessInstanceEvent(evt, executionEntity, HistoryEventTypes.ProcessInstanceMigrate);

            return evt;
        }

        public virtual HistoryEvent CreateProcessInstanceEndEvt(IDelegateExecution execution)
        {
            ExecutionEntity executionEntity = (ExecutionEntity)execution;

            // create event instance
            HistoricProcessInstanceEventEntity evt = LoadProcessInstanceEventEntity(executionEntity);

            // initialize event
            InitProcessInstanceEvent(evt, executionEntity, HistoryEventTypes.ProcessInstanceEnd);

            DetermineEndState(executionEntity, evt);

            // set end activity id
            evt.EndActivityId = executionEntity.ActivityId;
            evt.EndTime = ClockUtil.CurrentTime;

            if (evt.StartTime != null)
            {
                evt.DurationInMillis = ((DateTime)evt.EndTime).Ticks - ((DateTime)evt.StartTime).Ticks;
            }

            // set delete reason (if applicable).
            if (!string.ReferenceEquals(executionEntity.DeleteReason, null))
            {
                evt.DeleteReason = executionEntity.DeleteReason;
            }

            return evt;
        }

        protected internal virtual void DetermineEndState(ExecutionEntity executionEntity, HistoricProcessInstanceEventEntity evt)
        {
            //determine state
            if (executionEntity.Activity != null)
            {
                evt.State = HistoricProcessInstanceFields.StateCompleted;
            }
            else if (executionEntity.Activity == null && executionEntity.ExternallyTerminated)
            {
                evt.State = HistoricProcessInstanceFields.StateExternallyTerminated;
            }
            else if (executionEntity.Activity == null && !executionEntity.ExternallyTerminated)
            {
                evt.State = HistoricProcessInstanceFields.StateInternallyTerminated;
            }
        }

        public virtual HistoryEvent CreateActivityInstanceStartEvt(IDelegateExecution execution)
        {
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final org.camunda.bpm.persistence.entity.ExecutionEntity executionEntity = (org.camunda.bpm.persistence.entity.ExecutionEntity) execution;
            ExecutionEntity executionEntity = (ExecutionEntity)execution;

            // create event instance
            HistoricActivityInstanceEventEntity evt = NewActivityInstanceEventEntity(executionEntity);

            // initialize event
            InitActivityInstanceEvent(evt, executionEntity, HistoryEventTypes.ActivityInstanceStart);

            // initialize sequence counter
            InitSequenceCounter(executionEntity, evt);

            evt.StartTime = ClockUtil.CurrentTime;

            return evt;
        }

        public virtual HistoryEvent CreateActivityInstanceUpdateEvt(IDelegateExecution execution)
        {
            return CreateActivityInstanceUpdateEvt(execution, null);
        }

        public virtual HistoryEvent CreateActivityInstanceUpdateEvt(IDelegateExecution execution, IDelegateTask task)
        {
            ExecutionEntity executionEntity = (ExecutionEntity)execution;

            // create event instance
            HistoricActivityInstanceEventEntity evt = LoadActivityInstanceEventEntity(executionEntity);

            // initialize event
            InitActivityInstanceEvent(evt, executionEntity, HistoryEventTypes.ActivityInstanceUpdate);

            // update ITask assignment
            if (task != null)
            {
                evt.TaskId = task.Id;
                evt.TaskAssignee = task.Assignee;
            }

            return evt;
        }

        public virtual HistoryEvent CreateActivityInstanceMigrateEvt(MigratingActivityInstance actInstance)
        {

            // create event instance
            HistoricActivityInstanceEventEntity evt = LoadActivityInstanceEventEntity(actInstance.ResolveRepresentativeExecution());

            // initialize event
            InitActivityInstanceEvent(evt, actInstance, HistoryEventTypes.ActivityInstanceMigrate);

            return evt;
        }


        public virtual HistoryEvent CreateActivityInstanceEndEvt(IDelegateExecution execution)
        {
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final org.camunda.bpm.persistence.entity.ExecutionEntity executionEntity = (org.camunda.bpm.persistence.entity.ExecutionEntity) execution;
            ExecutionEntity executionEntity = (ExecutionEntity)execution;

            // create event instance
            HistoricActivityInstanceEventEntity evt = LoadActivityInstanceEventEntity(executionEntity);
            evt.ActivityInstanceState = executionEntity.ActivityInstanceState;

            // initialize event
            InitActivityInstanceEvent(evt, (ExecutionEntity)execution, HistoryEventTypes.ActivityInstanceEnd);

            evt.EndTime = ClockUtil.CurrentTime;
            if (evt.StartTime != null)
            {
                evt.DurationInMillis = ((DateTime)evt.EndTime).Ticks - ((DateTime)evt.StartTime).Ticks;
            }

            return evt;
        }

        public virtual HistoryEvent CreateTaskInstanceCreateEvt(IDelegateTask task)
        {

            // create event instance
            HistoricTaskInstanceEventEntity evt = NewTaskInstanceEventEntity(task);

            // initialize event
            InitTaskInstanceEvent(evt, (TaskEntity)task, HistoryEventTypes.TaskInstanceCreate);

            evt.StartTime = ClockUtil.CurrentTime;

            return evt;
        }

        public virtual HistoryEvent CreateTaskInstanceUpdateEvt(IDelegateTask task)
        {

            // create event instance
            HistoricTaskInstanceEventEntity evt = LoadTaskInstanceEvent(task);

            // initialize event
            InitTaskInstanceEvent(evt, (TaskEntity)task, HistoryEventTypes.TaskInstanceUpdate);

            return evt;
        }

        public virtual HistoryEvent CreateTaskInstanceMigrateEvt(IDelegateTask task)
        {
            // create event instance
            HistoricTaskInstanceEventEntity evt = LoadTaskInstanceEvent(task);

            // initialize event
            InitTaskInstanceEvent(evt, (TaskEntity)task, HistoryEventTypes.TaskInstanceMigrate);

            return evt;
        }

        public virtual HistoryEvent CreateTaskInstanceCompleteEvt(IDelegateTask task, string deleteReason)
        {

            // create event instance
            HistoricTaskInstanceEventEntity evt = LoadTaskInstanceEvent(task);

            // initialize event
            InitTaskInstanceEvent(evt, (TaskEntity)task, HistoryEventTypes.TaskInstanceComplete);

            // set end time
            evt.EndTime = ClockUtil.CurrentTime;
            if (evt.StartTime != null)
            {
                evt.DurationInMillis = ((DateTime)evt.EndTime).Ticks - ((DateTime)evt.StartTime).Ticks;
            }

            // set delete reason
            evt.DeleteReason = deleteReason;

            return evt;
        }

        // User Operation Logs ///////////////////////////

        public virtual IList<HistoryEvent> CreateUserOperationLogEvents(UserOperationLogContext context)
        {
            IList<HistoryEvent> historyEvents = new List<HistoryEvent>();

            string operationId = Context.ProcessEngineConfiguration.IdGenerator.NewGuid();//.NextId;
            context.OperationId = operationId;

            foreach (UserOperationLogContextEntry entry in context.Entries)
            {
                foreach (PropertyChange propertyChange in entry.PropertyChanges)
                {
                    UserOperationLogEntryEventEntity evt = new UserOperationLogEntryEventEntity();

                    InitUserOperationLogEvent(evt, context, entry, propertyChange);

                    historyEvents.Add(evt);
                }
            }

            return historyEvents;
        }

        // variables /////////////////////////////////

        public virtual HistoryEvent CreateHistoricVariableCreateEvt(VariableInstanceEntity variableInstance, IVariableScope sourceVariableScope)
        {
            return CreateHistoricVariableEvent(variableInstance, sourceVariableScope, HistoryEventTypes.VariableInstanceCreate);
        }

        public virtual HistoryEvent CreateHistoricVariableDeleteEvt(VariableInstanceEntity variableInstance, IVariableScope sourceVariableScope)
        {
            return CreateHistoricVariableEvent(variableInstance, sourceVariableScope, HistoryEventTypes.VariableInstanceDelete);
        }

        public virtual HistoryEvent CreateHistoricVariableUpdateEvt(VariableInstanceEntity variableInstance, IVariableScope sourceVariableScope)
        {
            return CreateHistoricVariableEvent(variableInstance, sourceVariableScope, HistoryEventTypes.VariableInstanceUpdate);
        }

        public virtual HistoryEvent CreateHistoricVariableMigrateEvt(VariableInstanceEntity variableInstance)
        {
            return CreateHistoricVariableEvent(variableInstance, null, HistoryEventTypes.VariableInstanceMigrate);
        }

        // form Properties ///////////////////////////

        public virtual HistoryEvent CreateFormPropertyUpdateEvt(ExecutionEntity execution, string propertyId, string propertyValue, string taskId)
        {

            IDGenerator idGenerator = Context.ProcessEngineConfiguration.IdGenerator;

            HistoricFormPropertyEventEntity historicFormPropertyEntity = NewHistoricFormPropertyEvent();

            historicFormPropertyEntity.Id = idGenerator.NewGuid();//.NextId;
            historicFormPropertyEntity.EventType = HistoryEventTypes.FormPropertyUpdate.EventName;
            historicFormPropertyEntity.TimeStamp = ClockUtil.CurrentTime;
            historicFormPropertyEntity.ActivityInstanceId = execution.ActivityInstanceId;
            historicFormPropertyEntity.ExecutionId = execution.Id;
            historicFormPropertyEntity.ProcessDefinitionId = execution.ProcessDefinitionId;
            historicFormPropertyEntity.ProcessInstanceId = execution.ProcessInstanceId;
            historicFormPropertyEntity.PropertyId = propertyId;
            historicFormPropertyEntity.PropertyValue=propertyValue;
            historicFormPropertyEntity.TaskId = taskId;
            historicFormPropertyEntity.TenantId = execution.TenantId;

            ProcessDefinitionEntity definition = execution.GetProcessDefinition();
            if (definition != null)
            {
                historicFormPropertyEntity.ProcessDefinitionKey = definition.Key;
            }

            // initialize sequence counter
            InitSequenceCounter(execution, historicFormPropertyEntity);

            return historicFormPropertyEntity;
        }

        // Incidents //////////////////////////////////

        public virtual HistoryEvent CreateHistoricIncidentCreateEvt(IIncident incident)
        {
            return CreateHistoricIncidentEvt(incident, HistoryEventTypes.IncidentCreate);
        }

        public virtual HistoryEvent CreateHistoricIncidentResolveEvt(IIncident incident)
        {
            return CreateHistoricIncidentEvt(incident, HistoryEventTypes.IncidentResolve);
        }

        public virtual HistoryEvent CreateHistoricIncidentDeleteEvt(IIncident incident)
        {
            return CreateHistoricIncidentEvt(incident, HistoryEventTypes.IncidentDelete);
        }

        public virtual HistoryEvent CreateHistoricIncidentMigrateEvt(IIncident incident)
        {
            return CreateHistoricIncidentEvt(incident, HistoryEventTypes.IncidentMigrate);
        }

        protected internal virtual HistoryEvent CreateHistoricIncidentEvt(IIncident incident, HistoryEventTypes eventType)
        {
            // create event
            Event.HistoricIncidentEntity evt = LoadIncidentEvent(incident);
            // initialize
            InitHistoricIncidentEvent(evt, incident, eventType);

            if (HistoryEventTypes.IncidentCreate!=eventType)
            {
                evt.EndTime = ClockUtil.CurrentTime;
            }

            return evt;
        }

        // Historic identity link
        public virtual HistoryEvent CreateHistoricIdentityLinkAddEvent(IIdentityLink identityLink)
        {
            return CreateHistoricIdentityLinkEvt(identityLink, HistoryEventTypes.IdentityLinkAdd);
        }

        public virtual HistoryEvent CreateHistoricIdentityLinkDeleteEvent(IIdentityLink identityLink)
        {
            return CreateHistoricIdentityLinkEvt(identityLink, HistoryEventTypes.IdentityLinkDelete);
        }

        protected internal virtual HistoryEvent CreateHistoricIdentityLinkEvt(IIdentityLink identityLink, HistoryEventTypes eventType)
        {
            // create historic identity link event
            HistoricIdentityLinkLogEventEntity evt = NewIdentityLinkEventEntity();
            // Mapping all the values of identity link to HistoricIdentityLinkEvent
            InitHistoricIdentityLinkEvent(evt, identityLink, eventType);
            return evt;
        }

        protected internal virtual HistoricIdentityLinkLogEventEntity NewIdentityLinkEventEntity()
        {
            return new HistoricIdentityLinkLogEventEntity();
        }

        protected internal virtual void InitHistoricIdentityLinkEvent(HistoricIdentityLinkLogEventEntity evt, IIdentityLink identityLink, HistoryEventTypes eventType)
        {

            if (!string.ReferenceEquals(identityLink.TaskId, null))
            {
                TaskEntity task = Context.CommandContext.TaskManager.FindTaskById(identityLink.TaskId);

                evt.ProcessDefinitionId = task.ProcessDefinitionId;

                if (task.ProcessDefinition != null)
                {
                    evt.ProcessDefinitionKey = task.ProcessDefinition.Key;
                }
            }

            if (!string.ReferenceEquals(identityLink.ProcessDefId, null))
            {
                evt.ProcessDefinitionId = identityLink.ProcessDefId;

                ProcessDefinitionEntity definition = Context.ProcessEngineConfiguration.DeploymentCache.FindProcessDefinitionFromCache(identityLink.ProcessDefId);
                evt.ProcessDefinitionKey = definition.Key;
            }

            evt.Time = ClockUtil.CurrentTime;
            evt.Type = identityLink.Type;
            evt.UserId = identityLink.UserId;
            evt.GroupId = identityLink.GroupId;
            evt.TaskId = identityLink.TaskId;
            evt.TenantId = identityLink.TenantId;
            // There is a conflict in HistoryEventTypes for 'delete' keyword,
            // So HistoryEventTypes.IDENTITY_LINK_ADD /
            // HistoryEventTypes.IDENTITY_LINK_DELETE is provided with the event name
            // 'add-identity-link' /'delete-identity-link'
            // and changed to 'add'/'delete' (While inserting it into the database) on
            // Historic identity link add / delete event
            string operationType = "add";
            if (eventType.EventName.Equals(HistoryEventTypes.IdentityLinkDelete.EventName))
            {
                operationType = "delete";
            }

            evt.OperationType = operationType;
            evt.EventType = eventType.EventName;
            evt.AssignerId = Context.CommandContext.AuthenticatedUserId;
        }
        // Batch

        public virtual HistoryEvent CreateBatchStartEvent(IBatch batch)
        {
            return CreateBatchEvent((BatchEntity)batch, HistoryEventTypes.BatchStart);
        }

        public virtual HistoryEvent CreateBatchEndEvent(IBatch batch)
        {
            return CreateBatchEvent((BatchEntity)batch, HistoryEventTypes.BatchEnd);
        }

        protected internal virtual HistoryEvent CreateBatchEvent(BatchEntity batch, HistoryEventTypes eventType)
        {
            HistoricBatchEntity @event = new HistoricBatchEntity();

            @event.Id = batch.Id;
            @event.Type = batch.Type;
            @event.TotalJobs = batch.TotalJobs;
            @event.BatchJobsPerSeed = batch.BatchJobsPerSeed;
            @event.InvocationsPerBatchJob = batch.InvocationsPerBatchJob;
            @event.SeedJobDefinitionId = batch.SeedJobDefinitionId;
            @event.MonitorJobDefinitionId = batch.MonitorJobDefinitionId;
            @event.BatchJobDefinitionId = batch.BatchJobDefinitionId;
            @event.TenantId = batch.TenantId;
            @event.EventType = eventType.EventName;

            if (HistoryEventTypes.BatchStart.Equals(eventType))
            {
                @event.StartTime = ClockUtil.CurrentTime;
            }

            if (HistoryEventTypes.BatchEnd.Equals(eventType))
            {
                @event.EndTime = ClockUtil.CurrentTime;
            }

            return @event;
        }

        // Job Log

        public virtual HistoryEvent CreateHistoricJobLogCreateEvt(IJob job)
        {
            return CreateHistoricJobLogEvt(job, HistoryEventTypes.JobCreate);
        }

        public virtual HistoryEvent CreateHistoricJobLogFailedEvt(IJob job, System.Exception exception)
        {
            HistoricJobLogEventEntity @event = (HistoricJobLogEventEntity)CreateHistoricJobLogEvt(job, HistoryEventTypes.JobFail);

            if (exception != null)
            {
                // exception message
                @event.JobExceptionMessage = exception.Message;

                // stacktrace
                string exceptionStacktrace = ExceptionUtil.GetExceptionStacktrace(exception);
                byte[] exceptionBytes =StringUtil.ToByteArray(exceptionStacktrace);
                ResourceEntity byteArray = ExceptionUtil.CreateJobExceptionByteArray(exceptionBytes);
                @event.ExceptionByteArrayId = byteArray.Id;
            }

            return @event;
        }

        public virtual HistoryEvent CreateHistoricJobLogSuccessfulEvt(IJob job)
        {
            return CreateHistoricJobLogEvt(job, HistoryEventTypes.JobSuccess);
        }

        public virtual HistoryEvent CreateHistoricJobLogDeleteEvt(IJob job)
        {
            return CreateHistoricJobLogEvt(job, HistoryEventTypes.JobDelete);
        }

        protected internal virtual HistoryEvent CreateHistoricJobLogEvt(IJob job, HistoryEventTypes eventType)
        {
            HistoricJobLogEventEntity @event = NewHistoricJobLogEntity(job);
            InitHistoricJobLogEvent(@event, job, eventType);
            return @event;
        }

        protected internal virtual void InitHistoricJobLogEvent(HistoricJobLogEventEntity evt, IJob job, HistoryEventTypes eventType)
        {
            evt.TimeStamp = ClockUtil.CurrentTime;

            JobEntity jobEntity = (JobEntity)job;
            evt.JobId = jobEntity.Id;
            evt.JobDueDate = jobEntity.Duedate;
            evt.JobRetries = jobEntity.Retries;
            evt.JobPriority = jobEntity.Priority;

            IJobDefinition jobDefinition = jobEntity.JobDefinition;
            if (jobDefinition != null)
            {
                evt.JobDefinitionId = jobDefinition.Id;
                evt.JobDefinitionType = jobDefinition.JobType;
                evt.JobDefinitionConfiguration = jobDefinition.JobConfiguration;
            }
            else
            {
                // in case of async signal there does not exist a job definition
                // but we use the jobHandlerType as jobDefinitionType
                evt.JobDefinitionType = jobEntity.JobHandlerType;
            }

            evt.ActivityId = jobEntity.ActivityId;
            evt.ExecutionId = jobEntity.ExecutionId;
            evt.ProcessInstanceId = jobEntity.ProcessInstanceId;
            evt.ProcessDefinitionId = jobEntity.ProcessDefinitionId;
            evt.ProcessDefinitionKey = jobEntity.ProcessDefinitionKey;
            evt.DeploymentId = jobEntity.DeploymentId;
            evt.TenantId = jobEntity.TenantId;

            // initialize sequence counter
            InitSequenceCounter(jobEntity, evt);

            IJobState state = null;
            if (HistoryEventTypes.JobCreate.Equals(eventType))
            {
                state = JobStateFields.Created;
            }
            else if (HistoryEventTypes.JobFail.Equals(eventType))
            {
                state = JobStateFields.Failed;
            }
            else if (HistoryEventTypes.JobSuccess.Equals(eventType))
            {
                state = JobStateFields.Successful;
            }
            else if (HistoryEventTypes.JobDelete.Equals(eventType))
            {
                state = JobStateFields.Deleted;
            }
            evt.State = state.StateCode;
        }

        public virtual HistoryEvent CreateHistoricExternalTaskLogCreatedEvt(IExternalTask task)
        {
            return InitHistoricExternalTaskLog((ExternalTaskEntity)task, ExternalTaskStateFields.Created);
        }

        public virtual HistoryEvent CreateHistoricExternalTaskLogFailedEvt(IExternalTask task)
        {
            HistoricExternalTaskLogEntity @event = InitHistoricExternalTaskLog((ExternalTaskEntity)task, ExternalTaskStateFields.Failed);
            @event.ErrorMessage = task.ErrorMessage;
            string errorDetails = ((ExternalTaskEntity)task).ErrorDetails;
            if (!string.ReferenceEquals(errorDetails, null))
            {
                @event.ErrorDetails = errorDetails;
            }
            return @event;
        }

        public virtual HistoryEvent CreateHistoricExternalTaskLogSuccessfulEvt(IExternalTask task)
        {
            return InitHistoricExternalTaskLog((ExternalTaskEntity)task, ExternalTaskStateFields.Successful);
        }

        public virtual HistoryEvent CreateHistoricExternalTaskLogDeletedEvt(IExternalTask task)
        {
            return InitHistoricExternalTaskLog((ExternalTaskEntity)task, ExternalTaskStateFields.Deleted);
        }

        protected internal virtual HistoricExternalTaskLogEntity InitHistoricExternalTaskLog(ExternalTaskEntity entity, IExternalTaskState state)
        {
            HistoricExternalTaskLogEntity @event = new HistoricExternalTaskLogEntity();
            @event.TimeStamp = ClockUtil.CurrentTime;
            @event.ExternalTaskId = entity.Id;
            @event.TopicName = entity.TopicName;
            @event.WorkerId = entity.WorkerId;

            @event.Priority = entity.Priority;
            @event.Retries = entity.Retries;

            @event.ActivityId = entity.ActivityId;
            @event.ActivityInstanceId = entity.ActivityInstanceId;
            @event.ExecutionId = entity.ExecutionId;

            @event.ProcessInstanceId = entity.ProcessInstanceId;
            @event.ProcessDefinitionId = entity.ProcessDefinitionId;
            @event.ProcessDefinitionKey = entity.ProcessDefinitionKey;
            @event.TenantId = entity.TenantId;
            @event.State = state.StateCode;

            return @event;
        }

        // sequence counter //////////////////////////////////////////////////////

        protected internal virtual void InitSequenceCounter(ExecutionEntity execution, HistoryEvent @event)
        {
            InitSequenceCounter(execution.SequenceCounter, @event);
        }

        protected internal virtual void InitSequenceCounter(VariableInstanceEntity variable, HistoryEvent @event)
        {
            InitSequenceCounter(variable.SequenceCounter, @event);
        }

        protected internal virtual void InitSequenceCounter(JobEntity job, HistoryEvent @event)
        {
            InitSequenceCounter(job.SequenceCounter, @event);
        }

        protected internal virtual void InitSequenceCounter(long sequenceCounter, HistoryEvent @event)
        {
            @event.SequenceCounter = sequenceCounter;
        }


    }

}


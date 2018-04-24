using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.Bpm.Engine.History.Impl.Producer;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.runtime;
using ESS.FW.Bpm.Engine.Management.Impl;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Persistence.Entity.Impl;
using ESS.FW.Common.Utilities;
using System.Linq;
using ESS.FW.Bpm.Engine.Management;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///     
    /// </summary>
    public abstract class AbstractSetProcessInstanceStateCmd : AbstractSetStateCmd
    {
        protected internal readonly string ProcessInstanceId;
        protected internal bool IsProcessDefinitionTenantIdSet;
        protected internal string ProcessDefinitionId;
        protected internal string ProcessDefinitionKey;

        protected internal string ProcessDefinitionTenantId;

        public AbstractSetProcessInstanceStateCmd(IUpdateProcessInstanceSuspensionStateBuilder builder)
            : base(true, null)
        {
            ProcessInstanceId = builder.ProcessInstanceId;
            ProcessDefinitionId = builder.ProcessDefinitionId;
            ProcessDefinitionKey = builder.ProcessDefinitionKey;
            ProcessDefinitionTenantId = builder.processDefinitionTenantId;
            IsProcessDefinitionTenantIdSet = builder.ProcessDefinitionTenantIdSet;
        }

        protected internal override void CheckParameters(CommandContext commandContext)
        {
            if (ReferenceEquals(ProcessInstanceId, null) && ReferenceEquals(ProcessDefinitionId, null) &&
                ReferenceEquals(ProcessDefinitionKey, null))
                throw new ProcessEngineException(
                    "ProcessInstanceId, ProcessDefinitionId nor ProcessDefinitionKey cannot be null.");
        }

        protected internal override void CheckAuthorization(CommandContext commandContext)
        {
            foreach (var checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
                if (!ReferenceEquals(ProcessInstanceId, null))
                {
                    checker.CheckUpdateProcessInstanceById(ProcessInstanceId);
                }
                else
                {
                    if (!ReferenceEquals(ProcessDefinitionId, null))
                    {
                        checker.CheckUpdateProcessInstanceByProcessDefinitionId(ProcessDefinitionId);
                    }
                    else
                    {
                        if (!ReferenceEquals(ProcessDefinitionKey, null))
                            checker.CheckUpdateProcessInstanceByProcessDefinitionKey(ProcessDefinitionKey);
                    }
                }
        }

        protected internal override void UpdateSuspensionState(CommandContext commandContext,
            ISuspensionState suspensionState)
        {
            IExecutionManager executionManager = commandContext.ExecutionManager;
            ITaskManager taskManager = commandContext.TaskManager;
            IExternalTaskManager externalTaskManager = commandContext.ExternalTaskManager;

            if (!string.IsNullOrEmpty(ProcessInstanceId))
            {
                executionManager.UpdateExecutionSuspensionStateByProcessInstanceId(ProcessInstanceId, suspensionState);
                taskManager.UpdateTaskSuspensionStateByProcessInstanceId(ProcessInstanceId, suspensionState);
                externalTaskManager.UpdateExternalTaskSuspensionStateByProcessInstanceId(ProcessInstanceId, suspensionState);
            }
            else if (!string.IsNullOrEmpty(ProcessDefinitionId))
            {
                executionManager.UpdateExecutionSuspensionStateByProcessDefinitionId(ProcessDefinitionId, suspensionState);
                taskManager.UpdateTaskSuspensionStateByProcessDefinitionId(ProcessDefinitionId, suspensionState);
                externalTaskManager.UpdateExternalTaskSuspensionStateByProcessDefinitionId(ProcessDefinitionId, suspensionState);
            }
            else if (IsProcessDefinitionTenantIdSet)
            {
                executionManager.UpdateExecutionSuspensionStateByProcessDefinitionKeyAndTenantId(ProcessDefinitionKey, ProcessDefinitionTenantId, suspensionState);
                taskManager.UpdateTaskSuspensionStateByProcessDefinitionKeyAndTenantId(ProcessDefinitionKey, ProcessDefinitionTenantId, suspensionState);
                externalTaskManager.UpdateExternalTaskSuspensionStateByProcessDefinitionKeyAndTenantId(ProcessDefinitionKey, ProcessDefinitionTenantId, suspensionState);
            }
            else
            {
                executionManager.UpdateExecutionSuspensionStateByProcessDefinitionKey(ProcessDefinitionKey, suspensionState);
                taskManager.UpdateTaskSuspensionStateByProcessDefinitionKey(ProcessDefinitionKey, suspensionState);
                externalTaskManager.UpdateExternalTaskSuspensionStateByProcessDefinitionKey(ProcessDefinitionKey, suspensionState);
            }
        }

        protected internal override void TriggerHistoryEvent(CommandContext commandContext)
        {
            var historyLevel = commandContext.ProcessEngineConfiguration.HistoryLevel;
            var updatedProcessInstances = ObtainProcessInstances(commandContext);
            //suspension state is not updated synchronously
            if (NewSuspensionState != null && updatedProcessInstances != null)
            {
                foreach (IProcessInstance processInstance in updatedProcessInstances)
                {

                    if (historyLevel.IsHistoryEventProduced(HistoryEventTypes.ProcessInstanceUpdate, processInstance))
                    {
                        HistoryEventProcessor.ProcessHistoryEvents(new HistoryEventCreatorAnonymousInnerClass(this, processInstance));
                    }
                }
            }
        }

        protected internal virtual IList<IProcessInstance> ObtainProcessInstances(CommandContext commandContext)
        {
            #region
            //ProcessInstanceQueryImpl query = new ProcessInstanceQueryImpl();
            //if (processInstanceId != null)
            //{
            //    query.processInstanceId(processInstanceId);
            //}
            //else if (processDefinitionId != null)
            //{
            //    query.processDefinitionId(processDefinitionId);
            //}
            //else if (isProcessDefinitionTenantIdSet)
            //{
            //    query.processDefinitionKey(processDefinitionKey);
            //    if (processDefinitionTenantId != null)
            //    {
            //        query.tenantIdIn(processDefinitionTenantId);
            //    }
            //    else
            //    {
            //        query.withoutTenantId();
            //    }
            //}
            //else
            //{
            //    query.processDefinitionKey(processDefinitionKey);
            //}
            //List<ProcessInstance> result = new ArrayList<ProcessInstance>();
            ////result.addAll(commandContext.getExecutionManager().findProcessInstancesByQueryCriteria(query,null));

            //ExecutionManager manager = commandContext.getExecutionManager();

            //List<ProcessInstance> datas = manager.findProcessInstancesByQueryCriteria(query, null);

            //result.addAll(datas);


            //return result;

            #endregion

            if (!string.IsNullOrEmpty(ProcessInstanceId))
            {
                return commandContext.ExecutionManager.FindProcessInstancesByProcessInstanceId(ProcessInstanceId, null);
            }
            else if (!string.IsNullOrEmpty(ProcessDefinitionId))
            {
                return commandContext.ExecutionManager.FindProcessInstancesByProcessDefinitionId(ProcessDefinitionId, null);
            }
            else if (IsProcessDefinitionTenantIdSet)
            {
                return commandContext.ExecutionManager.FindProcessInstancesByTenantIds(ProcessDefinitionTenantId != null ? new List<string> { ProcessDefinitionTenantId }.ToArray() : null, ProcessDefinitionKey, null);
            }
            else
            {
                return commandContext.ExecutionManager.FindProcessInstancesByProcessDefinitionKey(ProcessDefinitionKey, null);
            }
        }

        protected internal override void LogUserOperation(CommandContext commandContext)
        {
            PropertyChange propertyChange = new PropertyChange(SuspensionStateProperty, null, NewSuspensionState.Name);
            commandContext.OperationLogManager.LogProcessInstanceOperation(LogEntryOperation, ProcessInstanceId, ProcessDefinitionId, ProcessDefinitionKey, new List<PropertyChange>() { propertyChange });// Collections.singletonList());
        }

        protected internal virtual IUpdateJobSuspensionStateBuilder CreateJobCommandBuilder()
        {
            var builder = new UpdateJobSuspensionStateBuilderImpl();

            if (!ReferenceEquals(ProcessInstanceId, null))
            {
                builder.ByProcessInstanceId(ProcessInstanceId);
            }
            else if (!ReferenceEquals(ProcessDefinitionId, null))
            {
                builder.ByProcessDefinitionId(ProcessDefinitionId);
            }
            else if (!ReferenceEquals(ProcessDefinitionKey, null))
            {
                builder.ByProcessDefinitionKey(ProcessDefinitionKey);

                if (IsProcessDefinitionTenantIdSet && !ReferenceEquals(ProcessDefinitionTenantId, null))
                    return builder.ProcessDefinitionTenantId(ProcessDefinitionTenantId);
                if (IsProcessDefinitionTenantIdSet)
                    return builder.ProcessDefinitionWithoutTenantId();
            }
            return builder;
        }

        protected internal new AbstractSetJobStateCmd NextCommand
        {
            get
            {
                var jobCommandBuilder = CreateJobCommandBuilder();

                return GetNextCommand(jobCommandBuilder);
            }
        }

        protected internal abstract AbstractSetJobStateCmd GetNextCommand(IUpdateJobSuspensionStateBuilder jobCommandBuilder);

        private class HistoryEventCreatorAnonymousInnerClass : HistoryEventProcessor.HistoryEventCreator
        {
            private readonly AbstractSetProcessInstanceStateCmd _outerInstance;
            private IProcessInstance _processInstance;

            public HistoryEventCreatorAnonymousInnerClass(AbstractSetProcessInstanceStateCmd outerInstance, IProcessInstance processInstance)
            {
                this._outerInstance = outerInstance;
                this._processInstance = processInstance;
            }

            public override HistoryEvent CreateHistoryEvent(IHistoryEventProducer producer)
            {
                HistoricProcessInstanceEventEntity processInstanceUpdateEvt = (HistoricProcessInstanceEventEntity)producer.CreateProcessInstanceUpdateEvt((IDelegateExecution)_processInstance);
                if (SuspensionStateFields.Suspended.StateCode == _outerInstance.NewSuspensionState.StateCode)
                {
                    processInstanceUpdateEvt.State = HistoricProcessInstanceFields.StateSuspended;
                }
                else
                {
                    processInstanceUpdateEvt.State = HistoricProcessInstanceFields.StateActive;
                }
                return processInstanceUpdateEvt;
            }
        }
    }
}
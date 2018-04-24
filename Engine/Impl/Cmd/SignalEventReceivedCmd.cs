using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Pvm;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Deploy.Cache;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Persistence.Entity.Impl;
using ESS.FW.Bpm.Engine.Common;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    
    


    /// <summary>
    ///     
    ///     
    /// </summary>
    public class SignalEventReceivedCmd : ICommand<object>
    {
        protected internal static readonly CommandLogger Log = ProcessEngineLogger.CmdLogger;

        protected internal readonly SignalEventReceivedBuilderImpl Builder;

        public SignalEventReceivedCmd(SignalEventReceivedBuilderImpl builder)
        {
            this.Builder = builder;
        }
        
        public virtual object Execute(CommandContext commandContext)
        {
            var signalName = Builder.SignalName;
            var executionId = Builder.ExecutionId;

            if (executionId== null)
                SendSignal(commandContext, signalName);
            else
                SendSignalToExecution(commandContext, signalName, executionId);
            return null;
        }

        protected internal virtual void SendSignal(CommandContext commandContext, string signalName)
        {
            var signalEventSubscriptions = FindSignalEventSubscriptions(commandContext,
                signalName);

            var catchSignalEventSubscription =
                FilterIntermediateSubscriptions(signalEventSubscriptions);
            var startSignalEventSubscriptions =
                FilterStartSubscriptions(signalEventSubscriptions);
            var processDefinitions =
                GetProcessDefinitionsOfSubscriptions(startSignalEventSubscriptions);

            CheckAuthorizationOfCatchSignals(commandContext, catchSignalEventSubscription);
            CheckAuthorizationOfStartSignals(commandContext, startSignalEventSubscriptions, processDefinitions);

            NotifyExecutions(catchSignalEventSubscription);
            StartProcessInstances(startSignalEventSubscriptions, processDefinitions);
        }

        protected internal virtual IList<EventSubscriptionEntity> FindSignalEventSubscriptions(
            CommandContext commandContext, string signalName)
        {
            IEventSubscriptionManager eventSubscriptionManager = commandContext.EventSubscriptionManager;

            if (Builder.IsTenantIdSet)
            {
                return eventSubscriptionManager.FindSignalEventSubscriptionsByEventNameAndTenantId(signalName,
                    Builder.TenantId);
            }
            return eventSubscriptionManager.FindSignalEventSubscriptionsByEventName(signalName);
        }

        protected internal virtual IDictionary<string, ProcessDefinitionEntity> GetProcessDefinitionsOfSubscriptions(
            IList<EventSubscriptionEntity> startSignalEventSubscriptions)
        {
            DeploymentCache deploymentCache = context.Impl.Context.ProcessEngineConfiguration.DeploymentCache;

            IDictionary<string, ProcessDefinitionEntity> processDefinitions =
                new Dictionary<string, ProcessDefinitionEntity>();

            foreach (var eventSubscription in startSignalEventSubscriptions)
            {
                var processDefinitionId = eventSubscription.Configuration;
                EnsureUtil.EnsureNotNull(
                    "Configuration of signal start event subscription '" + eventSubscription.Id +
                    "' contains no process definition id.", processDefinitionId);

                ProcessDefinitionEntity processDefinition =
                    deploymentCache.FindDeployedProcessDefinitionById(processDefinitionId);
                if (processDefinition != null && !processDefinition.Suspended)
                {
                    processDefinitions[eventSubscription.Id] = processDefinition;
                }
            }

            return processDefinitions;
        }

        protected internal virtual void SendSignalToExecution(CommandContext commandContext, string signalName,
            string executionId)
        {
            IExecutionManager executionManager = commandContext.ExecutionManager;
            ExecutionEntity execution = executionManager.FindExecutionById(executionId);
            EnsureUtil.EnsureNotNull("Cannot find execution with id '" + executionId + "'", "execution", execution);

            IEventSubscriptionManager eventSubscriptionManager = commandContext.EventSubscriptionManager;
            IList<EventSubscriptionEntity> signalEvents =
                eventSubscriptionManager.FindSignalEventSubscriptionsByNameAndExecution(signalName, executionId);
            EnsureUtil.EnsureNotEmpty("Execution '" + executionId + "' has not subscribed to a signal event with name '" + signalName + "'.",ListExt.ConvertToIlist(signalEvents));

            CheckAuthorizationOfCatchSignals(commandContext, signalEvents);
            NotifyExecutions(signalEvents);
        }
        
        protected internal virtual void CheckAuthorizationOfCatchSignals(CommandContext commandContext,
            IList<EventSubscriptionEntity> catchSignalEventSubscription)
        {
            // check authorization for each fetched signal event
            foreach (var @event in catchSignalEventSubscription)
            {
                var processInstanceId = @event.ProcessInstanceId;
                foreach (var checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
                    checker.CheckUpdateProcessInstanceById(processInstanceId);
            }
        }
        
        private void CheckAuthorizationOfStartSignals(CommandContext commandContext,
            IList<EventSubscriptionEntity> startSignalEventSubscriptions,
            IDictionary<string, ProcessDefinitionEntity> processDefinitions)
        {
            // check authorization for process definition
            foreach (var signalStartEventSubscription in startSignalEventSubscriptions)
            {
                var processDefinition = processDefinitions.ContainsKey(signalStartEventSubscription.Id)? processDefinitions[signalStartEventSubscription.Id]:null;
                if (processDefinition != null)
                    foreach (var checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
                        checker.CheckCreateProcessInstance(processDefinition);
            }
        }

        private void NotifyExecutions(IList<EventSubscriptionEntity> catchSignalEventSubscription)
        {
            foreach (var signalEventSubscriptionEntity in catchSignalEventSubscription)
                if (IsActiveEventSubscription(signalEventSubscriptionEntity))
                {
                    signalEventSubscriptionEntity.EventReceived(Builder.GetVariables(), false);
                }
        }

        private bool IsActiveEventSubscription(EventSubscriptionEntity signalEventSubscriptionEntity)
        {
            ExecutionEntity execution = signalEventSubscriptionEntity.Execution;
            return !execution.IsEnded && !execution.Canceled;
        }

        private void StartProcessInstances(IList<EventSubscriptionEntity> startSignalEventSubscriptions,
            IDictionary<string, ProcessDefinitionEntity> processDefinitions)
        {
            foreach (var signalStartEventSubscription in startSignalEventSubscriptions)
            {
                var processDefinition = processDefinitions.ContainsKey(signalStartEventSubscription.Id)? processDefinitions[signalStartEventSubscription.Id]:null;
                if (processDefinition != null)
                {
                    ActivityImpl signalStartEvent =
                        (ActivityImpl) processDefinition.FindActivity(signalStartEventSubscription.ActivityId);
                    IPvmProcessInstance processInstance =
                        processDefinition.CreateProcessInstanceForInitial(signalStartEvent);
                    processInstance.Start(Builder.GetVariables());
                }
            }
        }

        protected internal virtual IList<EventSubscriptionEntity> FilterIntermediateSubscriptions(
            IList<EventSubscriptionEntity> subscriptions)
        {
            IList<EventSubscriptionEntity> result = new List<EventSubscriptionEntity>();

            foreach (var subscription in subscriptions)
                if (!ReferenceEquals(subscription.ExecutionId, null))
                    result.Add(subscription);

            return result;
        }

        protected internal virtual IList<EventSubscriptionEntity> FilterStartSubscriptions(
            IList<EventSubscriptionEntity> subscriptions)
        {
            IList<EventSubscriptionEntity> result = new List<EventSubscriptionEntity>();

            foreach (var subscription in subscriptions)
                if (ReferenceEquals(subscription.ExecutionId, null))
                    result.Add(subscription);

            return result;
        }
    }
}
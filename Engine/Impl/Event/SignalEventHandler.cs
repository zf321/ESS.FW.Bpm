using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Impl.Cmd;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Pvm;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Deploy.Cache;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Event
{



    /// <summary>
    ///     
    /// </summary>
    public class SignalEventHandler : EventHandlerImpl
    {
        private static readonly CommandLogger Log = ProcessEngineLogger.CmdLogger;

        public SignalEventHandler() : base(EventType.Signal)
        {
        }

        protected internal virtual void HandleStartEvent(EventSubscriptionEntity eventSubscription, object payload,
            CommandContext commandContext)
        {
            var processDefinitionId = eventSubscription.Configuration;
            EnsureUtil.EnsureNotNull(
                "Configuration of signal start event subscription '" + eventSubscription.Id +
                "' contains no process definition id.", processDefinitionId);

            DeploymentCache deploymentCache = Context.ProcessEngineConfiguration.DeploymentCache;
            ProcessDefinitionEntity processDefinition =
                deploymentCache.FindDeployedProcessDefinitionById(processDefinitionId);
            if (processDefinition == null || processDefinition.Suspended)
            {
                // ignore event subscription
                Log.DebugIgnoringEventSubscription(eventSubscription, processDefinitionId);
            }
            else
            {
                ActivityImpl signalStartEvent = (ActivityImpl) processDefinition.FindActivity(eventSubscription.ActivityId);
                IPvmProcessInstance processInstance = processDefinition.CreateProcessInstanceForInitial(signalStartEvent);
                processInstance.Start();
            }
        }

        public override void HandleEvent(EventSubscriptionEntity eventSubscription, object payload,
            CommandContext commandContext)
        {
            if (!ReferenceEquals(eventSubscription.ExecutionId, null))
                HandleIntermediateEvent(eventSubscription, payload, commandContext);
            else
                HandleStartEvent(eventSubscription, payload, commandContext);
        }
    }
}
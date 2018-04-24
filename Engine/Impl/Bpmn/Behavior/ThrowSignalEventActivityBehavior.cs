using System.Collections.Generic;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Parser;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Persistence.Entity.Impl;

namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior
{
    /// <summary>
    ///     Defines activity behavior for signal end event and intermediate throw signal event.
    ///     
    /// </summary>
    public class ThrowSignalEventActivityBehavior : AbstractBpmnActivityBehavior
    {
        protected internal new static readonly BpmnBehaviorLogger Log = ProcessEngineLogger.BpmnBehaviorLogger;

        protected internal readonly EventSubscriptionDeclaration SignalDefinition;

        public ThrowSignalEventActivityBehavior(EventSubscriptionDeclaration signalDefinition)
        {
            this.SignalDefinition = signalDefinition;
        }

        public override void Execute(IActivityExecution execution)
        {
            var eventName = SignalDefinition.ResolveExpressionOfEventName(execution);
            // trigger all event subscriptions for the signal (start and intermediate)
            var signalEventSubscriptions = FindSignalEventSubscriptions(eventName, (execution as ExecutionEntity).TenantId);

            foreach (var signalEventSubscription in signalEventSubscriptions)
                if (IsActiveEventSubscription(signalEventSubscription))
                {
                    signalEventSubscription.EventReceived(null, SignalDefinition.Async);
                }
            Leave(execution);
        }

        protected internal virtual IList<EventSubscriptionEntity> FindSignalEventSubscriptions(string signalName,
            string tenantId)
        {
            IEventSubscriptionManager eventSubscriptionManager = Context.CommandContext.EventSubscriptionManager;

            if (!ReferenceEquals(tenantId, null))
            {
                return
                    eventSubscriptionManager.FindSignalEventSubscriptionsByEventNameAndTenantIdIncludeWithoutTenantId(
                        signalName, tenantId);
            }
            // find event subscriptions without tenant id
            //Preparing: select EVT.* from ACT_RU_EVENT_SUBSCR EVT left join ACT_RU_EXECUTION EXC on EVT.EXECUTION_ID_ = EXC.ID_ where (EVENT_TYPE_ = 'signal') and (EVENT_NAME_ = ?) and (EVT.EXECUTION_ID_ is null or EXC.SUSPENSION_STATE_ = 1) and EVT.TENANT_ID_ is null 
            return eventSubscriptionManager.FindSignalEventSubscriptionsByEventNameAndTenantId(signalName, null);
        }

        protected internal virtual bool IsActiveEventSubscription(EventSubscriptionEntity signalEventSubscriptionEntity)
        {
            return IsStartEventSubscription(signalEventSubscriptionEntity) ||
                   IsActiveIntermediateEventSubscription(signalEventSubscriptionEntity);
        }

        protected internal virtual bool IsStartEventSubscription(EventSubscriptionEntity signalEventSubscriptionEntity)
        {
            return ReferenceEquals(signalEventSubscriptionEntity.ExecutionId, null);
        }

        protected internal virtual bool IsActiveIntermediateEventSubscription(
            EventSubscriptionEntity signalEventSubscriptionEntity)
        {
            ExecutionEntity execution = signalEventSubscriptionEntity.Execution;
            return execution != null && !execution.IsEnded && !execution.Canceled;
        }
    }
}
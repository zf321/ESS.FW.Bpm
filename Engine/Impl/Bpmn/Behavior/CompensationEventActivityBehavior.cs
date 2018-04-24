using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Helper;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Parser;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;
using ESS.FW.Bpm.Engine.Impl.Pvm.Runtime;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior
{
    /// <summary>
    ///     Behavior for a compensation end event.
    /// </summary>
    /// <seealso cref= "IntermediateThrowCompensationEventActivityBehavior">
    /// 
    ///     
    /// </seealso>
    public class CompensationEventActivityBehavior : FlowNodeActivityBehavior
    {
        protected internal readonly CompensateEventDefinition CompensateEventDefinition;

        public CompensationEventActivityBehavior(CompensateEventDefinition compensateEventDefinition)
        {
            this.CompensateEventDefinition = compensateEventDefinition;
        }
        
        public override void Execute(IActivityExecution execution)
        {
            var eventSubscriptions = CollectEventSubscriptions(execution);
            if (eventSubscriptions.Count == 0)
                Leave(execution);
            else
                CompensationUtil.ThrowCompensationEvent(eventSubscriptions, execution, false);
        }

        protected internal virtual IList<EventSubscriptionEntity> CollectEventSubscriptions(IActivityExecution execution)
        {
            var activityRef = CompensateEventDefinition.ActivityRef;
            if (!ReferenceEquals(activityRef, null))
                return CompensationUtil.CollectCompensateEventSubscriptionsForActivity(execution, activityRef);
            return CompensationUtil.CollectCompensateEventSubscriptionsForScope(execution);
        }
        
        public override void Signal(IActivityExecution execution, string signalName, object signalData)
        {
            // join compensating executions -
            // only wait for non-event-scope executions cause a compensation event subprocess consume the compensation event and
            // do not have to compensate embedded subprocesses (which are still non-event-scope executions)

            if (((PvmExecutionImpl) execution).NonEventScopeExecutions.Count == 0)
                Leave(execution);
        }
    }
}
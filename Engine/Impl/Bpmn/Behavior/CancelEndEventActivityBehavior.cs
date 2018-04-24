using ESS.FW.Bpm.Engine.Impl.Bpmn.Helper;
using ESS.FW.Bpm.Engine.Impl.Pvm;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Impl.Util;

namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior
{
    /// <summary>
    ///     
    ///     
    /// </summary>
    public class CancelEndEventActivityBehavior : AbstractBpmnActivityBehavior
    {
        protected internal IPvmActivity cancelBoundaryEvent;

        public virtual IPvmActivity CancelBoundaryEvent
        {
            set { cancelBoundaryEvent = value; }
            get { return cancelBoundaryEvent; }
        }
        
        public override void Execute(IActivityExecution execution)
        {
            EnsureUtil.EnsureNotNull("Could not find cancel boundary event for cancel end event " + execution.Activity,
                "cancelBoundaryEvent", cancelBoundaryEvent);

            var compensateEventSubscriptions = CompensationUtil.CollectCompensateEventSubscriptionsForScope(execution);

            if (compensateEventSubscriptions.Count == 0)
                Leave(execution);
            else
                CompensationUtil.ThrowCompensationEvent(compensateEventSubscriptions, execution, false);
        }

        public override void DoLeave(IActivityExecution execution)
        {
            // continue via the appropriate cancel boundary event
            var eventScope = (ScopeImpl) cancelBoundaryEvent.EventScope;

            var boundaryEventScopeExecution = execution.FindExecutionForFlowScope(eventScope);
            boundaryEventScopeExecution.ExecuteActivity(cancelBoundaryEvent);
        }
        
        public override void Signal(IActivityExecution execution, string signalName, object signalData)
        {
            // join compensating executions
            if (!execution.HasChildren())
                Leave(execution);
        }
    }
}
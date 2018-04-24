using ESS.FW.Bpm.Engine.Impl.Bpmn.Parser;
using ESS.FW.Bpm.Engine.Impl.Core.Variable.Event;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Impl.Pvm.Runtime;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior
{
    /// <summary>
    ///      
    /// </summary>
    public class EventSubProcessStartConditionalEventActivityBehavior : EventSubProcessStartEventActivityBehavior,
        IConditionalEventBehavior
    {
        protected internal readonly ConditionalEventDefinition ConditionalEvent;

        public EventSubProcessStartConditionalEventActivityBehavior(ConditionalEventDefinition conditionalEvent)
        {
            this.ConditionalEvent = conditionalEvent;
        }

        public virtual ConditionalEventDefinition ConditionalEventDefinition
        {
            get { return ConditionalEvent; }
        }
        
        public virtual void LeaveOnSatisfiedCondition(EventSubscriptionEntity eventSubscription,
            VariableEvent variableEvent)
        {
            PvmExecutionImpl execution = eventSubscription.Execution;

            if (execution != null && !execution.IsEnded && execution.IsScope &&
                ConditionalEvent.TryEvaluate(variableEvent, execution))
            {
                ActivityImpl activity = eventSubscription.Activity;
                activity = (ActivityImpl)activity.FlowScope;
                execution.ExecuteEventHandlerActivity(activity);
            }
        }
    }
}


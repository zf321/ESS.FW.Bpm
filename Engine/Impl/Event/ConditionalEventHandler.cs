using ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior;
using ESS.FW.Bpm.Engine.Impl.Core.Variable.Event;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Event
{
    /// <summary>
    ///      
    /// </summary>
    public class ConditionalEventHandler : IEventHandler
    {
        public virtual string EventHandlerType
        {
            get { return EventType.Conditonal.Name; }
        }

        public virtual void HandleEvent(EventSubscriptionEntity eventSubscription, object payload,
            CommandContext commandContext)
        {
            VariableEvent variableEvent;
            if ((payload == null) || payload is VariableEvent)
                variableEvent = (VariableEvent) payload;
            else
                throw new ProcessEngineException("Payload have to be " + typeof(VariableEvent).FullName +
                                                 ", to evaluate condition.");

            ActivityImpl activity = eventSubscription.Activity;
            IActivityBehavior activityBehavior = activity.ActivityBehavior;
            if (activityBehavior is IConditionalEventBehavior)
            {
                var conditionalBehavior = (IConditionalEventBehavior)activityBehavior;
                conditionalBehavior.LeaveOnSatisfiedCondition(eventSubscription, variableEvent);
            }
            else
            {
                throw new ProcessEngineException("Conditional Event has not correct behavior: " + activityBehavior);
            }
        }
    }
}


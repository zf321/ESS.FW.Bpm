using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior
{
    /// <summary>
    ///     
    /// </summary>
    public class EventBasedGatewayActivityBehavior : FlowNodeActivityBehavior
    {
        public override void Execute(IActivityExecution execution)
        {
            // If conditional events exist after the event based gateway they should be evaluated.
            // If a condition is satisfied the event based gateway should be left,
            // otherwise the event based gateway is a wait state
            var eventBasedGateway = (ActivityImpl) execution.Activity;
            foreach (var act in eventBasedGateway.EventActivities)
            {
                IActivityBehavior activityBehavior = act.ActivityBehavior;
                if (activityBehavior is IConditionalEventBehavior)
                {
                    var conditionalEventBehavior = (IConditionalEventBehavior)activityBehavior;
                    var conditionalEventDefinition = conditionalEventBehavior.ConditionalEventDefinition;
                    if (conditionalEventDefinition.TryEvaluate(execution))
                    {
                        ((ExecutionEntity) execution).ExecuteEventHandlerActivity(
                            conditionalEventDefinition.ConditionalActivity);
                        return;
                    }
                }
            }
        }
    }
}
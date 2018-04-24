using ESS.FW.Bpm.Engine.Impl.Bpmn.Parser;
using ESS.FW.Bpm.Engine.Impl.Core.Variable.Event;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Impl.Pvm.Runtime;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior
{
    /// <summary>
    ///      
    /// </summary>
    public class IntermediateConditionalEventBehavior : IntermediateCatchEventActivityBehavior, IConditionalEventBehavior
    {
        protected internal readonly ConditionalEventDefinition ConditionalEvent;

        public IntermediateConditionalEventBehavior(ConditionalEventDefinition conditionalEvent,
            bool isAfterEventBasedGateway) : base(isAfterEventBasedGateway)
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

            if (execution != null && !execution.IsEnded && variableEvent != null && ConditionalEvent.TryEvaluate(variableEvent, execution) && execution.IsActive && execution.IsScope)
            {
                if (IsAfterEventBasedGateway)
                {
                    ActivityImpl activity = eventSubscription.Activity;
                    execution.ExecuteEventHandlerActivity(activity);
                }
                else
                {
                    Leave(execution);
                }
            }
        }
        
        public override void Execute(IActivityExecution execution)
        {
            if (IsAfterEventBasedGateway || ConditionalEvent.TryEvaluate(execution))
                Leave(execution);
        }
    }
}


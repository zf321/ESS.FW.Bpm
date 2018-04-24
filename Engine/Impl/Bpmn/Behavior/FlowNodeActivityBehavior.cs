using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;
using ESS.FW.Bpm.Engine.Impl.Pvm.Runtime.Operation;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior
{
    /// <summary>
    ///     Superclass for all 'connectable' BPMN 2.0 process elements: tasks, gateways and events.
    ///     This means that any subclass can be the source or target of a sequenceflow.
    ///     Corresponds with the notion of the 'flownode' in BPMN 2.0.
    /// </summary>
    public abstract class FlowNodeActivityBehavior : ISignallableActivityBehavior
    {
        protected internal static readonly BpmnBehaviorLogger Log = ProcessEngineLogger.BpmnBehaviorLogger;

        protected internal BpmnActivityBehavior BpmnActivityBehavior = new BpmnActivityBehavior();

        /// <summary>
        ///     Default behaviour: just leave the activity with no extra functionality.
        /// </summary>
        public virtual void Execute(IActivityExecution execution)
        {
            Leave(execution);
        }
        
        public virtual void Signal(IActivityExecution execution, string signalName, object signalData)
        {
            // concrete activity behaviors that do accept signals should override this method;
            throw Log.UnsupportedSignalException(execution.Activity.Id);
        }

        /// <summary>
        ///     Default way of leaving a BPMN 2.0 activity: evaluate the conditions on the
        ///     outgoing sequence flow and take those that evaluate to true.
        /// </summary>
        public virtual void Leave(IActivityExecution execution)
        {
            ((ExecutionEntity)execution).DispatchDelayedEventsAndPerformOperation(
                PvmAtomicOperationFields.ActivityLeave);
        }

        public virtual void DoLeave(IActivityExecution execution)
        {
            BpmnActivityBehavior.PerformDefaultOutgoingBehavior(execution);
        }

        protected internal virtual void LeaveIgnoreConditions(IActivityExecution activityContext)
        {
            BpmnActivityBehavior.PerformIgnoreConditionsOutgoingBehavior(activityContext);
        }
    }
}
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;

namespace ESS.FW.Bpm.Engine.Impl.Pvm.Runtime.Operation
{
    /// <summary>
    ///     In BPMN this is used for terminate end events
    ///     
    /// </summary>
    public class PvmAtomicOperationsTransitionInterruptFlowScope : PvmAtomicOperationInterruptScope
    {
        public virtual string CanonicalName
        {
            get { return "transition-interrupt-scope"; }
        }

        protected internal override void ScopeInterrupted(IActivityExecution execution)
        {
            ((PvmExecutionImpl)execution).DispatchDelayedEventsAndPerformOperation(PvmAtomicOperationFields.TransitionCreateScope);
        }

        protected internal override IPvmActivity GetInterruptingActivity(PvmExecutionImpl execution)
        {
            return execution.Transition.Destination;
        }
    }
}
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Core.Model;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;

namespace ESS.FW.Bpm.Engine.Impl.Pvm.Runtime.Operation
{
    /// <summary>
    ///      
    ///     
    ///     
    /// </summary>
    public abstract class AbstractPvmAtomicOperationTransitionNotifyListenerTake : AbstractPvmEventAtomicOperation
    {
        protected internal override string EventName
        {
            get { return ExecutionListenerFields.EventNameTake; }
        }

        protected internal override void EventNotificationsCompleted(PvmExecutionImpl execution)
        {
            var destination = execution.Transition.Destination;

            // check start behavior of next activity
            switch (destination.ActivityStartBehavior)
            {
                case ActivityStartBehavior.Default:
                    execution.Activity=(ActivityImpl) (destination);
                    execution.DispatchDelayedEventsAndPerformOperation(PvmAtomicOperationFields.TransitionCreateScope);
                    break;
                case ActivityStartBehavior.InterruptFlowScope:
                    execution.Activity=(null);
                    execution.PerformOperation(PvmAtomicOperationFields.TransitionInterruptFlowScope);
                    break;
                default:
                    throw new ProcessEngineException("Unsupported start behavior for activity '" + destination +
                                                     "' started from a sequence flow: " +
                                                     destination.ActivityStartBehavior);
            }
        }

        protected internal override CoreModelElement GetScope(PvmExecutionImpl execution)
        {
            return (CoreModelElement) execution.Transition;
        }
    }
}
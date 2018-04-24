using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Core.Model;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;

namespace ESS.FW.Bpm.Engine.Impl.Pvm.Runtime.Operation
{
    /// <summary>
    ///      
    /// </summary>
    public class PvmAtomicOperationTransitionNotifyListenerStart : PvmAtomicOperationActivityInstanceStart
    {
        protected internal override string EventName
        {
            get { return ExecutionListenerFields.EventNameStart; }
        }

        public override string CanonicalName
        {
            get { return "transition-notifiy-listener-start"; }
        }

        protected internal override CoreModelElement GetScope(PvmExecutionImpl execution)
        {
            return execution.Activity as CoreModelElement;
        }

        protected internal override void EventNotificationsCompleted(PvmExecutionImpl execution)
        {
            base.EventNotificationsCompleted(execution);

            var transition = execution.Transition;
            IPvmActivity destination;
            if (transition == null)
                destination = execution.Activity;
            else
                destination = transition.Destination;
            execution.Transition=(null);
            execution.Activity=(ActivityImpl) (destination);

            var executionStartContext = execution.ExecutionStartContext;
            if (executionStartContext != null)
            {
                executionStartContext.ExecutionStarted(execution);
                execution.DisposeExecutionStartContext();
            }

            execution.DispatchDelayedEventsAndPerformOperation(PvmAtomicOperationFields.ActivityExecute);
        }
    }
}
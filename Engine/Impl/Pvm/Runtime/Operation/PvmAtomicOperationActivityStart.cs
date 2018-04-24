using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Core.Model;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;

namespace ESS.FW.Bpm.Engine.Impl.Pvm.Runtime.Operation
{
    /// <summary>
    ///      
    /// </summary>
    public class PvmAtomicOperationActivityStart : PvmAtomicOperationActivityInstanceStart
    {
        protected internal override string EventName
        {
            get { return ExecutionListenerFields.EventNameStart; }
        }

        public override string CanonicalName
        {
            get { return "activity-start"; }
        }

        protected internal override void EventNotificationsCompleted(PvmExecutionImpl execution)
        {
            base.EventNotificationsCompleted(execution);

            var executionStartContext = execution.ExecutionStartContext;
            if (executionStartContext != null)
            {
                executionStartContext.ExecutionStarted(execution);
                execution.DisposeExecutionStartContext();
            }

            execution.DispatchDelayedEventsAndPerformOperation(PvmAtomicOperationFields.ActivityExecute);
        }

        protected internal override CoreModelElement GetScope(PvmExecutionImpl execution)
        {
            return execution.Activity as CoreModelElement;
        }
    }
}
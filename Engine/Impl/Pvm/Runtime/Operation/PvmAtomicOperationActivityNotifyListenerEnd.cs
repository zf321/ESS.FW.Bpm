using System;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Core.Model;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;

namespace ESS.FW.Bpm.Engine.Impl.Pvm.Runtime.Operation
{
    /// <summary>
    ///     
    /// </summary>
    public class PvmAtomicOperationActivityNotifyListenerEnd : PvmAtomicOperationActivityInstanceEnd
    {
        

        public override string CanonicalName
        {
            get { return "activity-notify-listener-end"; }
        }

        protected internal override string EventName
        {
            get { return ExecutionListenerFields.EventNameEnd; }
        }

        protected internal override CoreModelElement GetScope(PvmExecutionImpl execution)
        {
            return execution.Activity as CoreModelElement;
        }

        protected internal override void EventNotificationsCompleted(PvmExecutionImpl execution)
        {
            execution.DispatchDelayedEventsAndPerformOperation((e) =>
            {
                e.LeaveActivityInstance();
                e.ActivityInstanceId = null;
                e.PerformOperation(PvmAtomicOperationFields.ActivityEnd);
            });
        }
        
    }
}
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Core.Model;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;

namespace ESS.FW.Bpm.Engine.Impl.Pvm.Runtime.Operation
{
    /// <summary>
    ///     <para>This atomic operation simply fires the activity end event</para>
    ///     
    /// </summary>
    public class PvmAtomicOperationFireActivityEnd : AbstractPvmEventAtomicOperation
    {
        public override string CanonicalName
        {
            get { return "fire-activity-end"; }
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
            // nothing to do
        }
    }
}
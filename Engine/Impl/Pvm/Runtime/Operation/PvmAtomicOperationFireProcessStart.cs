using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Core.Model;

namespace ESS.FW.Bpm.Engine.Impl.Pvm.Runtime.Operation
{
    /// <summary>
    ///      
    ///     
    /// </summary>
    public class PvmAtomicOperationFireProcessStart : AbstractPvmEventAtomicOperation
    {
        protected internal override string EventName
        {
            get { return ExecutionListenerFields.EventNameStart; }
        }

        public override string CanonicalName
        {
            get { return "fire-process-start"; }
        }

        protected internal override CoreModelElement GetScope(PvmExecutionImpl execution)
        {
            return execution.ProcessDefinition;
        }

        protected internal override void EventNotificationsCompleted(PvmExecutionImpl execution)
        {
            // do nothing
        }
    }
}
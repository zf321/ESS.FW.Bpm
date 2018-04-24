using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;
using ESS.FW.Bpm.Engine.Impl.Pvm.Runtime;

namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior
{
    /// <summary>
    ///     
    /// </summary>
    public class EventSubProcessActivityBehavior : SubProcessActivityBehavior
    {
        public override void Complete(IActivityExecution scopeExecution)
        {
            // check whether legacy behavior needs to be performed.
            if (!LegacyBehavior.EventSubprocessComplete(scopeExecution))
                base.Complete(scopeExecution);
        }

        public override void ConcurrentChildExecutionEnded(IActivityExecution scopeExecution,
            IActivityExecution endedExecution)
        {
            // Check whether legacy behavior needs to be performed.
            // Legacy behavior means that the event subprocess is not a scope and as a result does not
            // join concurrent executions on it's own. Instead it delegates to the the subprocess activity behavior in which it is embedded.
            if (!LegacyBehavior.EventSubprocessConcurrentChildExecutionEnded(scopeExecution, endedExecution))
                base.ConcurrentChildExecutionEnded(scopeExecution, endedExecution);
        }
    }
}
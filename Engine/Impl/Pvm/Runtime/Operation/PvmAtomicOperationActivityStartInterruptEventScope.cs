using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;

namespace ESS.FW.Bpm.Engine.Impl.Pvm.Runtime.Operation
{
    /// <summary>
    ///     
    /// </summary>
    public class PvmAtomicOperationActivityStartInterruptEventScope : PvmAtomicOperationInterruptScope
    {
        public virtual string CanonicalName
        {
            get { return "activity-start-interrupt-scope"; }
        }

        protected internal override void ScopeInterrupted(IActivityExecution execution)
        {
            execution.ActivityInstanceId = null;
            ((PvmExecutionImpl)execution).PerformOperation(PvmAtomicOperationFields.ActivityStartCreateScope);
        }

        protected internal override IPvmActivity GetInterruptingActivity(PvmExecutionImpl execution)
        {
            var nextActivity = execution.NextActivity;
            execution.NextActivity = null;
            return nextActivity;
        }
    }
}
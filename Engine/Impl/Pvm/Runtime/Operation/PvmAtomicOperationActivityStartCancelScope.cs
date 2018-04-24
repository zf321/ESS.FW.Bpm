using ESS.FW.Bpm.Engine.Impl.Pvm.Process;

namespace ESS.FW.Bpm.Engine.Impl.Pvm.Runtime.Operation
{
    /// <summary>
    ///     Cancel scope operation performed when an execution starts at an <seealso cref="ActivityImpl#isCancelActivity()" />
    ///     activity. This is used when an execution is set to the activity without entering it through a transition.
    ///     See <seealso cref="PvmAtomicOperationCancelActivity" /> for more details on "cancel scope" behavior.
    ///     
    ///     
    /// </summary>
    public class PvmAtomicOperationActivityStartCancelScope : PvmAtomicOperationCancelActivity
    {
        public override string CanonicalName
        {
            get { return "activity-start-cancel-scope"; }
        }

        public override bool AsyncCapable
        {
            get { return false; }
        }

        protected internal override void ActivityCancelled(PvmExecutionImpl execution)
        {
            execution.ActivityInstanceId = null;
            execution.PerformOperation(PvmAtomicOperationFields.ActivityStartCreateScope);
        }

        protected internal virtual IPvmActivity GetCancellingActivity(PvmExecutionImpl execution)
        {
            return execution.NextActivity;
        }
    }
}
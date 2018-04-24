namespace ESS.FW.Bpm.Engine.Impl.Pvm.Runtime.Operation
{
    /// <summary>
    ///      
    /// </summary>
    public class PvmAtomicOperationTransitionCreateScope : PvmAtomicOperationCreateScope
    {
        public override string CanonicalName
        {
            get { return "transition-create-scope"; }
        }

        public override bool AsyncCapable
        {
            get { return true; }
        }

        public override bool IsAsync(PvmExecutionImpl execution)
        {
            IPvmActivity activity = execution.Activity;
            return activity.AsyncBefore;
        }

        protected internal override void ScopeCreated(PvmExecutionImpl execution)
        {
            execution.PerformOperation(PvmAtomicOperationFields.TransitionNotifyListenerStart);
        }
    }
}
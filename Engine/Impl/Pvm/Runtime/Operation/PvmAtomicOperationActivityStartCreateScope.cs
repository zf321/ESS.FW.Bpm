namespace ESS.FW.Bpm.Engine.Impl.Pvm.Runtime.Operation
{
    /// <summary>
    ///     
    /// </summary>
    public class PvmAtomicOperationActivityStartCreateScope : PvmAtomicOperationCreateScope
    {
        public override bool AsyncCapable
        {
            get { return true; }
        }

        public override string CanonicalName
        {
            get { return "activity-start-create-scope"; }
        }

        public override bool IsAsync(PvmExecutionImpl execution)
        {
            IPvmActivity activity = execution.Activity;
            return activity.AsyncBefore && !execution.HasProcessInstanceStartContext();
        }

        protected internal override void ScopeCreated(PvmExecutionImpl execution)
        {
            execution.PerformOperation(PvmAtomicOperationFields.ActivityStart);
        }
    }
}
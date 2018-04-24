namespace ESS.FW.Bpm.Engine.Impl.Pvm.Runtime.Operation
{
    /// <summary>
    ///     
    ///     
    /// </summary>
    public class PvmAtomicOperationActivityStartConcurrent : PvmAtomicOperationCreateConcurrentExecution
    {
        public override string CanonicalName
        {
            get { return "activity-start-concurrent"; }
        }

        public override bool AsyncCapable
        {
            get { return false; }
        }

        protected internal override void ConcurrentExecutionCreated(PvmExecutionImpl propagatingExecution)
        {
            propagatingExecution.ActivityInstanceId = null;
            propagatingExecution.PerformOperation(PvmAtomicOperationFields.ActivityStartCreateScope);
        }
    }
}
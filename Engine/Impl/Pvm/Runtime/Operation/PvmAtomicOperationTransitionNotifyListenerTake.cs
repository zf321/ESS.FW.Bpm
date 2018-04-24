namespace ESS.FW.Bpm.Engine.Impl.Pvm.Runtime.Operation
{
    /// <summary>
    ///      
    /// </summary>
    public class PvmAtomicOperationTransitionNotifyListenerTake : AbstractPvmAtomicOperationTransitionNotifyListenerTake
    {
        public override string CanonicalName
        {
            get { return "transition-notify-listener-take"; }
        }

        public override bool AsyncCapable
        {
            get { return true; }
        }

        public virtual bool isAsync(PvmExecutionImpl execution)
        {
            return execution.Activity.AsyncAfter;
        }
    }
}
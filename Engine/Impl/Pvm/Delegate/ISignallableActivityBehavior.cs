namespace ESS.FW.Bpm.Engine.Impl.Pvm.Delegate
{
    /// <summary>
    ///      
    /// </summary>
    public interface ISignallableActivityBehavior : IActivityBehavior
    {
        void Signal(IActivityExecution execution, string signalEvent, object signalData);
    }
}
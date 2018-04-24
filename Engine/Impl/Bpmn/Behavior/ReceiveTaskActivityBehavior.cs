using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;

namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior
{
    /// <summary>
    ///     A receive Task is a wait state that waits for the receival of some message.
    ///     Currently, the only message that is supported is the external trigger,
    ///     given by calling the <seealso cref="IRuntimeService#signal(String)" /> operation.
    ///     
    /// </summary>
    public class ReceiveTaskActivityBehavior : TaskActivityBehavior
    {
        protected override void PerformExecution(IActivityExecution execution)
        {
            Log.LogDebug("ReceiveTaskActivityBehavior.performExecution", "execution.Id" + execution.Id);
            // Do nothing: waitstate behavior
        }
        
        public override void Signal(IActivityExecution execution, string signalName, object data)
        {
            Log.LogDebug("ReceiveTaskActivityBehavior.signal", "execution.Id:" + execution.Id + "  signalName:" + signalName);
            Leave(execution);
        }
    }
}
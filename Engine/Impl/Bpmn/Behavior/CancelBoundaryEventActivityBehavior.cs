using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;
using ESS.FW.Bpm.Engine.Impl.Pvm.Runtime;

namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior
{
    /// <summary>
    ///     See CancelEndEventActivityBehavior: the cancel end event interrupts the scope and performs compensation.
    ///     
    /// </summary>
    public class CancelBoundaryEventActivityBehavior : BoundaryEventActivityBehavior
    {
        public override void Signal(IActivityExecution execution, string signalName, object signalData)
        {
            if (LegacyBehavior.SignalCancelBoundaryEvent(signalName))
                if (!execution.HasChildren())
                    Leave(execution);
                else
                    base.Signal(execution, signalName, signalData);
        }
    }
}
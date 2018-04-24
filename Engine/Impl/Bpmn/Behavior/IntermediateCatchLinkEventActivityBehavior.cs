using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;

namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior
{
    /// <summary>
    ///     
    /// </summary>
    public class IntermediateCatchLinkEventActivityBehavior : AbstractBpmnActivityBehavior
    {
        public override void Execute(IActivityExecution execution)
        {
            // a link event does not behave as a wait state
            Leave(execution);
        }
    }
}
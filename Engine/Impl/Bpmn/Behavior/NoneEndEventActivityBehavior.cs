using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;

namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior
{
    /// <summary>
    ///     
    /// </summary>
    public class NoneEndEventActivityBehavior : FlowNodeActivityBehavior
    {
        public override void Execute(IActivityExecution execution)
        {
            execution.End(true);
        }
    }
}
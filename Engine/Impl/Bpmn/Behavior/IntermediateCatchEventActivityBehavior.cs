using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;

namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior
{
    /// <summary>
    ///     
    ///     
    /// </summary>
    public class IntermediateCatchEventActivityBehavior : AbstractBpmnActivityBehavior
    {
        protected internal bool IsAfterEventBasedGateway;

        public IntermediateCatchEventActivityBehavior(bool isAfterEventBasedGateway)
        {
            this.IsAfterEventBasedGateway = isAfterEventBasedGateway;
        }

        public virtual bool AfterEventBasedGateway
        {
            get { return IsAfterEventBasedGateway; }
        }
        
        public override void Execute(IActivityExecution execution)
        {
            if (IsAfterEventBasedGateway)
                Leave(execution);
        }
        public override void Signal(IActivityExecution execution, string signalName, object signalData)
        {
            Leave(execution);
        }
    }
}
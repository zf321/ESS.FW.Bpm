using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;

namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior
{
    /// <summary>
    ///     
    ///     
    /// </summary>
    public class ErrorEndEventActivityBehavior : AbstractBpmnActivityBehavior
    {
        protected internal string errorCode;

        public ErrorEndEventActivityBehavior(string errorCode)
        {
            this.errorCode = errorCode;
        }

        public virtual string ErrorCode
        {
            get { return errorCode; }
            set { errorCode = value; }
        }
        
        public override void Execute(IActivityExecution execution)
        {
            PropagateError(errorCode, null, null, execution);
        }
    }
}
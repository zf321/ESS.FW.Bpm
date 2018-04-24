using ESS.FW.Bpm.Engine.Impl.Delegate;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;

namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Delegate
{
    /// <summary>
    ///     
    /// </summary>
    public class ActivityBehaviorInvocation : DelegateInvocation
    {
        private readonly IActivityBehavior _behaviorInstance;

        private readonly IActivityExecution _execution;

        public ActivityBehaviorInvocation(IActivityBehavior behaviorInstance, IActivityExecution execution)
            : base(execution, null)
        {
            this._behaviorInstance = behaviorInstance;
            this._execution = execution;
        }
        
        protected internal override void Invoke()
        {
            _behaviorInstance.Execute(_execution);
        }
    }
}
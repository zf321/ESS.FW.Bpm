using ESS.FW.Bpm.Engine.Impl.Delegate;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;

namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Delegate
{
    /// <summary>
    ///     
    /// </summary>
    public class ActivityBehaviorSignalInvocation : DelegateInvocation
    {
        private readonly ISignallableActivityBehavior _behaviorInstance;
        private readonly IActivityExecution _execution;
        private readonly object _signalData;
        private readonly string _signalName;

        public ActivityBehaviorSignalInvocation(ISignallableActivityBehavior behaviorInstance,
            IActivityExecution execution, string signalName, object signalData) : base(execution, null)
        {
            this._behaviorInstance = behaviorInstance;
            this._execution = execution;
            this._signalName = signalName;
            this._signalData = signalData;
        }
        
        protected internal override void Invoke()
        {
            _behaviorInstance.Signal(_execution, _signalName, _signalData);
        }
    }
}
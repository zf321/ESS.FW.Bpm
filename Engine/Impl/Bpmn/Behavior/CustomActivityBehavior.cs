using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Delegate;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;

namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior
{
    /// <summary>
    ///     
    /// </summary>
    public class CustomActivityBehavior : IActivityBehavior, ISignallableActivityBehavior
    {
        private readonly IActivityBehavior _delegateActivityBehavior;

        public CustomActivityBehavior(IActivityBehavior activityBehavior)
        {
            _delegateActivityBehavior = activityBehavior;
        }

        public virtual IActivityBehavior DelegateActivityBehavior
        {
            get { return _delegateActivityBehavior; }
        }
        
        public virtual void Execute(IActivityExecution execution)
        {
            Context.ProcessEngineConfiguration.DelegateInterceptor.HandleInvocation(
                new ActivityBehaviorInvocation(_delegateActivityBehavior, execution));
        }
        
        public virtual void Signal(IActivityExecution execution, string signalEvent, object signalData)
        {
            Context.ProcessEngineConfiguration.DelegateInterceptor.HandleInvocation(
                new ActivityBehaviorSignalInvocation((ISignallableActivityBehavior) _delegateActivityBehavior, execution,
                    signalEvent, signalData));
        }
    }
}
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Delegate;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;

namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior
{
    /// <summary>
    /// </summary>
    public class ServiceTaskMethodDelegateActivityBehavior : TaskActivityBehavior, IDelegateListener<IBaseDelegateExecution>
    {
        protected internal IJavaDelegate JavaDelegate;

        protected internal ServiceTaskMethodDelegateActivityBehavior()
        {
        }

        public ServiceTaskMethodDelegateActivityBehavior(IJavaDelegate javaDelegate)
        {
            this.JavaDelegate = javaDelegate;
        }
        
        public virtual void Notify(IBaseDelegateExecution execution)
        {
            Execute((IDelegateExecution)execution);
        }
        
        protected override void PerformExecution(IActivityExecution execution)
        {
            Execute(execution);
            Leave(execution);
        }
        
        public virtual void Execute(IDelegateExecution execution)
        {
            Context.ProcessEngineConfiguration.DelegateInterceptor.HandleInvocation(
                new JavaDelegateInvocation(JavaDelegate, execution));
        }
    }
}
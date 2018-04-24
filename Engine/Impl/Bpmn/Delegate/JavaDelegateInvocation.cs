using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Delegate;

namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Delegate
{
    /// <summary>
    ///     Class handling invocations of JavaDelegates
    ///     
    /// </summary>
    public class JavaDelegateInvocation : DelegateInvocation
    {
        protected internal readonly IJavaDelegate DelegateInstance;
        protected internal readonly IBaseDelegateExecution Execution;

        public JavaDelegateInvocation(IJavaDelegate delegateInstance, IDelegateExecution execution)
            : base(execution, null)
        {
            this.DelegateInstance = delegateInstance;
            this.Execution = execution;
        }
        
        protected internal override void Invoke()
        {
            DelegateInstance.Execute(Execution);
        }
    }
}
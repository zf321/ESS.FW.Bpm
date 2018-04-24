using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Delegate;

namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Delegate
{
    /// <summary>
    ///     Class handling invocations of ExecutionListeners
    ///     
    /// </summary>
    public class ExecutionListenerInvocation : DelegateInvocation
    {
        protected internal readonly IDelegateExecution Execution;

        protected internal readonly IDelegateListener<IBaseDelegateExecution> ExecutionListenerInstance;

        public ExecutionListenerInvocation(IDelegateListener<IBaseDelegateExecution> executionListenerInstance, IDelegateExecution execution)
            : base(execution, null)
        {
            this.ExecutionListenerInstance = executionListenerInstance;
            this.Execution = execution;
        }
        protected internal override void Invoke()
        {
            ExecutionListenerInstance.Notify(Execution);
        }
    }
}
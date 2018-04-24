using ESS.FW.Bpm.Engine.Delegate;

namespace ESS.FW.Bpm.Engine.Application
{
    /// <summary>
    ///     The context of an invocation.
    /// </summary>
    public class InvocationContext
    {
        protected internal readonly IBaseDelegateExecution execution;

        public InvocationContext(IBaseDelegateExecution execution)
        {
            this.execution = execution;
        }

        public virtual IBaseDelegateExecution Execution
        {
            get { return execution; }
        }

        public override string ToString()
        {
            return "InvocationContext [execution=" + execution + "]";
        }
    }
}
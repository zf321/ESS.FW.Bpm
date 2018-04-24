using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Delegate;
using ESS.FW.Bpm.Engine.Impl.Scripting;

namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Listener
{

    /// <summary>
    ///     An <seealso cref="IExecutionListener" /> which invokes a <seealso cref="ExecutableScript" /> when notified.
    ///     
    /// </summary>
    public class ScriptExecutionListener : IDelegateListener<IBaseDelegateExecution>
    {
        protected internal readonly ExecutableScript script;

        public ScriptExecutionListener(ExecutableScript script)
        {
            this.script = script;
        }

        public virtual ExecutableScript Script
        {
            get { return script; }
        }

        public virtual void Notify(IBaseDelegateExecution execution)
        {
            var invocation = new ScriptInvocation(script, execution);
            Context.ProcessEngineConfiguration.DelegateInterceptor.HandleInvocation(invocation);
        }
    }
}
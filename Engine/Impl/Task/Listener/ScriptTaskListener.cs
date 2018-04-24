using System;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Delegate;
using ESS.FW.Bpm.Engine.Impl.Scripting;

namespace ESS.FW.Bpm.Engine.Impl.task.listener
{

    /// <summary>
    ///     A <seealso cref="ITaskListener" /> which invokes a <seealso cref="ExecutableScript" /> when notified.
    ///     
    /// </summary>
    public class ScriptTaskListener : ITaskListener
    {

        protected internal readonly ExecutableScript script;

        public ScriptTaskListener(ExecutableScript script)
        {
            this.script = script;
        }

        public virtual ExecutableScript Script
        {
            get { return script; }
        }

        public virtual void Notify(IDelegateTask delegateTask)
        {
            var invocation = new ScriptInvocation(script, delegateTask);
            
                Context.ProcessEngineConfiguration.DelegateInterceptor.HandleInvocation(invocation);
        }
    }
}
using System;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Delegate;
using ESS.FW.Bpm.Engine.Impl.Scripting;

namespace ESS.FW.Bpm.Engine.Impl.Variable.listener
{
    /// <summary>
    ///     
    /// </summary>
    public class ScriptCaseVariableListener : ICaseVariableListener
    {
        protected internal readonly ExecutableScript script;

        public ScriptCaseVariableListener(ExecutableScript script)
        {
            this.script = script;
        }

        public virtual ExecutableScript Script
        {
            get { return script; }
        }
        
        public virtual void Notify(IDelegateCaseVariableInstance variableInstance)
        {
            var variableInstanceImpl = (DelegateCaseVariableInstanceImpl)variableInstance;

            var invocation = new ScriptInvocation(script, variableInstanceImpl.ScopeExecution);
            Context.ProcessEngineConfiguration.DelegateInterceptor.HandleInvocation(invocation);
        }
    }
}
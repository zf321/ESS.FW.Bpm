using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Scripting;

namespace ESS.FW.Bpm.Engine.Impl.Delegate
{
    /// <summary>
    ///     
    /// </summary>
    public class ScriptInvocation : DelegateInvocation
    {
        protected internal IVariableScope _scope;

        protected internal ExecutableScript _script;

        public ScriptInvocation(ExecutableScript script, IVariableScope scope) : this(script, scope, null)
        {
        }

        public ScriptInvocation(ExecutableScript script, IVariableScope scope, IBaseDelegateExecution contextExecution)
            : base(contextExecution, null)
        {
            this._script = script;
            this._scope = scope;
        }
        
        protected internal override void Invoke()
        {
            InvocationResult = Context.ProcessEngineConfiguration.ScriptingEnvironment.Execute(_script, _scope);
        }
    }
}
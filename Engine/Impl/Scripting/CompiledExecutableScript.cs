using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Javax.Script;

namespace ESS.FW.Bpm.Engine.Impl.Scripting
{
    public class CompiledExecutableScript : ExecutableScript
    {
        protected static readonly ScriptLogger log = ProcessEngineLogger.SCRIPT_LOGGER;
        public CompiledExecutableScript(string language) : base(language)
        {
        }
        public CompiledExecutableScript(string language, CompiledScript compiledScript) : base(language)
        {
            this.CompiledScript = compiledScript;
        }
        public CompiledScript CompiledScript { get; set; }
        protected  override object Evaluate(IScriptEngine scriptEngine, IVariableScope variableScope, IBindings bindings)
        {
            try
            {
                log.DebugEvaluatingCompiledScript(language);
                return CompiledScript.Eval(bindings);
            }
            catch (ScriptException e)
            {
                //if (e.getCause() instanceof BpmnError) {
                //    throw (BpmnError)e.getCause();
                //}
                throw new ScriptEvaluationException("Unable to evaluate script: " + e.getMessage(), e);
            }
        }
    }
}

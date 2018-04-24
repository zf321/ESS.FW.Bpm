using ESS.FW.Bpm.Engine.Delegate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESS.FW.Bpm.Engine.Impl.Javax.Script;

namespace ESS.FW.Bpm.Engine.Impl.Scripting
{
    public abstract class DynamicExecutableScript : ExecutableScript
    {
        protected IExpression scriptExpression;

        protected DynamicExecutableScript(IExpression scriptExpression, String language):base(language)
        {
            this.scriptExpression = scriptExpression;
        }

        protected override object Evaluate(IScriptEngine scriptEngine, IVariableScope variableScope, IBindings bindings)
        {
            String source = GetScriptSource(variableScope);
            try
            {
                return scriptEngine.Eval(source, bindings);
            }
            catch (ScriptException e)
            {
                throw new ScriptEvaluationException("Unable to evaluate script: " + e.getMessage(), e);
            }
        }
        protected String EvaluateExpression(IVariableScope variableScope)
        {
            return (String)scriptExpression.GetValue(variableScope);
        }

        public abstract String GetScriptSource(IVariableScope variableScope);
    }
}

using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Exception;
using ESS.FW.Bpm.Engine.Impl.Core.Variable.mapping.value;
using ESS.FW.Bpm.Engine.Impl.Delegate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Engine.Impl.Scripting
{
    public class ScriptValueProvider: IParameterValueProvider
    {
        protected ExecutableScript script;
        public ScriptValueProvider(ExecutableScript script)
        {
            this.script = script;
        }
        public Object GetValue(IVariableScope variableScope)
        {
            ScriptInvocation invocation = new ScriptInvocation(script, variableScope);
            try
            {
                Context
                .ProcessEngineConfiguration
                .DelegateInterceptor
                .HandleInvocation(invocation);
            }
            catch (RuntimeException e)
            {
                throw e;
            }
            catch (System.Exception e)
            {
                throw new ProcessEngineException(e);
            }

            return invocation.InvocationResult;
        }
    }
}

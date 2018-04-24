using ESS.FW.Bpm.Engine.Delegate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Engine.Impl.Scripting
{
    public class DynamicSourceExecutableScript: DynamicExecutableScript
    {
        public DynamicSourceExecutableScript(String language, IExpression scriptSourceExpression):base(scriptSourceExpression,language)
        {
        }

        public override String GetScriptSource(IVariableScope variableScope)
        {
            return EvaluateExpression(variableScope);
        }
    }
}

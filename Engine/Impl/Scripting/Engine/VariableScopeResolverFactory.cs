using ESS.FW.Bpm.Engine.Delegate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Engine.Impl.Scripting.Engine
{
    public class VariableScopeResolverFactory: IResolverFactory
    {
        public IResolver CreateResolver(IVariableScope variableScope)
        {
            if (variableScope != null)
            {
                return new VariableScopeResolver(variableScope);
            }
            return null;
        }
    }
}

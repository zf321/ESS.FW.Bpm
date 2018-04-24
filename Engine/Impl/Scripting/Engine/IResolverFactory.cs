using ESS.FW.Bpm.Engine.Delegate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Engine.Impl.Scripting.Engine
{
    public interface IResolverFactory
    {
        IResolver CreateResolver(IVariableScope variableScope);
    }
}

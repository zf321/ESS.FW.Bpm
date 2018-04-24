using ESS.FW.Bpm.Engine.Impl.Javax.Script;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Engine.Dmn.Engine.impl
{
    public interface ICachedCompiledScriptSupport
    {
        void CacheCompiledScript(CompiledScript compiledScript);

        CompiledScript GetCachedCompiledScript();
    }
}

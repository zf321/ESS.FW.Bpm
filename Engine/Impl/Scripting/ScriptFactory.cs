using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESS.FW.Bpm.Engine.Impl.EL;

namespace ESS.FW.Bpm.Engine.Impl.Scripting
{
    public class ScriptFactory
    {
        public ExecutableScript CreateScriptFromResource(String language, String resource)
        {
            return new ResourceExecutableScript(language, resource);
        }

        public ExecutableScript CreateScriptFromResource(String language, IExpression resourceExpression)
        {
            return new DynamicResourceExecutableScript(language, resourceExpression);
        }

        public ExecutableScript CreateScriptFromSource(String language, String source)
        {
            return new SourceExecutableScript(language, source);
        }

        public ExecutableScript CreateScriptFromSource(String language, IExpression sourceExpression)
        {
            return new DynamicSourceExecutableScript(language, sourceExpression);
        }
    }
}

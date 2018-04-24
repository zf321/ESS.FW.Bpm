using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Engine.Impl.Scripting
{
    public class ScriptLogger: ProcessEngineLogger
    {
        public void DebugEvaluatingCompiledScript(String language)
        {
            LogDebug(
                "001", "Evaluating compiled script {0} in language", language);
        }

        public void DebugCompiledScriptUsing(String language)
        {
            LogDebug(
                "002", "Compiled script using {0} script language", language);
        }

        public void debugEvaluatingNonCompiledScript(String scriptSource)
        {
            LogDebug(
                "001", "Evaluating non-compiled script {0}", scriptSource);
        }
    }
}

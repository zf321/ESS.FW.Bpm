using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Engine.Impl.Scripting.Env
{
    public interface IScriptEnvResolver
    {
        /**
   * Resolves a set of environment scripts for a given script language.
   *
   * @param the script language to resolve env scripts for.
   * @return an array of environment script sources or null if this
   * resolver does not provide any scripts for the given language
   */
        string[] Resolve(string language);
    }
}

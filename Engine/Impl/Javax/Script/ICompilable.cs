using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Engine.Impl.Javax.Script
{
    public interface ICompilable
    {
        /**
     * Compiles the script (source represented as a <code>String</code>) for
     * later execution.
     *
     * @param script The source of the script, represented as a <code>String</code>.
     *
     * @return An subclass of <code>CompiledScript</code> to be executed later using one
     * of the <code>eval</code> methods of <code>CompiledScript</code>.
     *
     * @throws ScriptException if compilation fails.
     * @throws NullPointerException if the argument is null.
     *
     */

        CompiledScript Compile(String script);

        /**
         * Compiles the script (source read from <code>Reader</code>) for
         * later execution.  Functionality is identical to
         * <code>compile(String)</code> other than the way in which the source is
         * passed.
         *
         * @param script The reader from which the script source is obtained.
         *
         * @return An implementation of <code>CompiledScript</code> to be executed
         * later using one of its <code>eval</code> methods of <code>CompiledScript</code>.
         *
         * @throws ScriptException if compilation fails.
         * @throws NullPointerException if argument is null.
         */
        CompiledScript Compile(Stream script);
    }
}

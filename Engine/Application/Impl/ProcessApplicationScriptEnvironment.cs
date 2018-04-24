
using ESS.FW.Bpm.Engine.Impl.Javax.Script;
using ESS.FW.Bpm.Engine.Impl.Scripting;
using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Application.Impl
{
    /// <summary>
    ///     
    /// </summary>
    public class ProcessApplicationScriptEnvironment
    {
        protected internal IDictionary<string, IList<ExecutableScript>> environmentScripts =
            new Dictionary<string, IList<ExecutableScript>>();

        protected internal IProcessApplicationInterface ProcessApplication;

        protected internal ScriptEngineResolver processApplicationScriptEngineResolver;

        public ProcessApplicationScriptEnvironment(IProcessApplicationInterface processApplication)
        {
            this.ProcessApplication = processApplication;
        }

        ///     Returns a map of cached environment scripts per script language.

        /// <summary>
        /// </summary>
        //public virtual IDictionary<string, IList<ExecutableScript>> EnvironmentScripts
        //{
        //    get { return environmentScripts; }
        //}

        //protected internal virtual ClassLoader ProcessApplicationClassloader
        //{
        //    get { return processApplication.ProcessApplicationClassloader; }
        //}

        /// <summary>
        ///     <para>Returns an instance of <seealso cref="ScriptEngine" /> for the given <code>scriptEngineName</code>.</para>
        ///     <para>
        ///         Iff the given parameter <code>cache</code> is set <code>true</code>,
        ///         then the instance <seealso cref="ScriptEngine" /> will be cached.
        ///     </para>
        /// </summary>
        /// <param name="scriptEngineName"> the name of the <seealso cref="ScriptEngine" /> to return </param>
        /// <param name="cache">
        ///     a boolean value which indicates whether the <seealso cref="ScriptEngine" /> should
        ///     be cached or not.
        /// </param>
        /// <returns> a <seealso cref="ScriptEngine" /> </returns>
        public virtual IScriptEngine GetScriptEngineForName(string scriptEngineName, bool cache)
        {
            if (processApplicationScriptEngineResolver == null)
            {
                lock (this)
                {
                    if (processApplicationScriptEngineResolver == null)
                    {
                        processApplicationScriptEngineResolver =
                            new ScriptEngineResolver(new ScriptEngineManager());
                    }
                }
            }
            return processApplicationScriptEngineResolver.GetScriptEngine(scriptEngineName, cache);
        }
    }
}
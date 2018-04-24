using ESS.FW.Bpm.Engine.Impl.Javax.Script;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Engine.Impl.Scripting
{
    public class ScriptEngineResolver
    {
        protected internal readonly ScriptEngineManager scriptEngineManager;

        protected internal IDictionary<string, IScriptEngine> cachedEngines = new Dictionary<string, IScriptEngine>();

        public ScriptEngineResolver(ScriptEngineManager scriptEngineManager)
        {
            this.scriptEngineManager = scriptEngineManager;
        }

        public virtual void AddScriptEngineFactory(IScriptEngineFactory scriptEngineFactory)
        {
            scriptEngineManager.RegisterEngineName(scriptEngineFactory.EngineName, scriptEngineFactory);
        }

        public virtual ScriptEngineManager GetScriptEngineManager()
        {
            return scriptEngineManager;
        }


        /// <summary>
        /// Returns a cached script engine or creates a new script engine if no such engine is currently cached.
        /// </summary>
        /// <param name="language"> the language (such as 'groovy' for the script engine) </param>
        /// <returns> the cached engine or null if no script engine can be created for the given language </returns>
        public virtual IScriptEngine GetScriptEngine(string language, bool resolveFromCache)
        {
            IScriptEngine scriptEngine = null;
            if (resolveFromCache)
            {
                scriptEngine = cachedEngines.ContainsKey(language)?cachedEngines[language]:null;

                if (scriptEngine == null)
                {
                    scriptEngine = scriptEngineManager.GetEngineByName(language);

                    if (scriptEngine != null)
                    {
                        if (ScriptingEngines.GROOVY_SCRIPTING_LANGUAGE.Equals(language))
                        {
                            ConfigureGroovyScriptEngine(scriptEngine);
                        }
                        if (language == ScriptingEngines.CSScriptLanguage)
                        {
                            ConfigureCSScritEngine(scriptEngine);
                        }
                        if (IsCachable(scriptEngine))
                        {
                            cachedEngines[language] = scriptEngine;
                        }
                    }
                }
            }
            else
            {
                scriptEngine = scriptEngineManager.GetEngineByName(language);
            }

            return scriptEngine;
        }

        /// <summary>
        /// Allows checking whether the script engine can be cached.
        /// </summary>
        /// <param name="scriptEngine"> the script engine to check. </param>
        /// <returns> true if the script engine may be cached. </returns>
        protected internal virtual bool IsCachable(IScriptEngine scriptEngine)
        {
            // Check if script-engine supports multithreading. If true it can be cached.
            object threadingParameter = scriptEngine.GetFactory().GetParameter("THREADING");
            return threadingParameter != null;
        }

        /// <summary>
        /// Allows providing custom configuration for the groovy script engine. </summary>
        /// <param name="scriptEngine"> the groovy script engine to configure. </param>
        protected internal virtual void ConfigureGroovyScriptEngine(IScriptEngine scriptEngine)
        {

            // make sure Groovy compiled scripts only hold weak references to java methods
            scriptEngine.GetContext().SetAttribute("#jsr223.groovy.engine.keep.globals", "weak", ScriptContext_Fields.ENGINE_SCOPE);
        }
        protected virtual void ConfigureCSScritEngine(IScriptEngine scriptEngine)
        {
            scriptEngine.GetContext().SetAttribute("#CSharp.CS.engine.keep.globals", "weak", ScriptContext_Fields.ENGINE_SCOPE);
        }


    }

}

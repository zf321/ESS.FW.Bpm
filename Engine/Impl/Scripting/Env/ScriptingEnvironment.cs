using ESS.FW.Bpm.Engine.Application;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Javax.Script;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Engine.Impl.Scripting.Env
{
    /**
 * <p>The scripting environment contains scripts that provide an environment to
 * a user provided script.The environment may contain additional libraries
 * or imports.</p>
 *
 * <p>The environment performs lazy initialization of scripts: the first time a script of a given
 * script language is executed, the environment will use the {@link ScriptEnvResolver ScriptEnvResolvers
}
 * for resolving the environment scripts for that language.The scripts (if any) are then pre-compiled
* and cached for reuse.</p>
 *
 */
    public class ScriptingEnvironment
    {
        /// <summary>
        /// the cached environment scripts per script language </summary>
        protected internal IDictionary<string, IList<ExecutableScript>> env = new Dictionary<string, IList<ExecutableScript>>();

        /// <summary>
        /// the resolvers </summary>
        protected internal IList<IScriptEnvResolver> envResolvers;

        /// <summary>
        /// the script factory used for compiling env scripts </summary>
        protected internal ScriptFactory scriptFactory;

        /// <summary>
        /// the scripting engines </summary>
        protected internal ScriptingEngines scriptingEngines;

        public ScriptingEnvironment(ScriptFactory scriptFactory, IList<IScriptEnvResolver> scriptEnvResolvers, ScriptingEngines scriptingEngines)
        {
            this.scriptFactory = scriptFactory;
            this.envResolvers = scriptEnvResolvers;
            this.scriptingEngines = scriptingEngines;
        }

        /// <summary>
        /// execute a given script in the environment
        /// </summary>
        /// <param name="script"> the <seealso cref="ExecutableScript"/> to execute </param>
        /// <param name="scope"> the scope in which to execute the script </param>
        /// <returns> the result of the script evaluation </returns>
        public virtual object Execute(ExecutableScript script, IVariableScope scope)
        {

            // get script engine
            IScriptEngine scriptEngine = scriptingEngines.GetScriptEngineForLanguage(script.Language);

            // create bindings
            IBindings bindings = scriptingEngines.CreateBindings(scriptEngine, scope);

            return Execute(script, scope, bindings, scriptEngine);
        }

        public virtual object Execute(ExecutableScript script, IVariableScope scope, IBindings bindings, IScriptEngine scriptEngine)
        {
            string scriptLanguage = script.Language;
            // first, evaluate the env scripts (if any)
            IList<ExecutableScript> envScripts = getEnvScripts(scriptLanguage);
            foreach (ExecutableScript envScript in envScripts)
            {
                envScript.Execute(scriptEngine, scope, bindings);
            }

            // next evaluate the actual script
            return script.Execute(scriptEngine, scope, bindings);
        }

        protected internal virtual IDictionary<string, IList<ExecutableScript>> getEnv(string language)
        {
            ProcessEngineConfigurationImpl config = Context.ProcessEngineConfiguration;
            IProcessApplicationReference processApplication = Context.CurrentProcessApplication;

            IDictionary<string, IList<ExecutableScript>> result = null;
            if (config.IsEnableFetchScriptEngineFromProcessApplication)
            {
                if (processApplication != null)
                {
                    result = GetPaEnvScripts(processApplication);
                }
            }

            return result != null ? result : env;
        }

        protected internal virtual IDictionary<string, IList<ExecutableScript>> GetPaEnvScripts(IProcessApplicationReference pa)
        {
            try
            {
                IProcessApplicationInterface processApplication = pa.ProcessApplication;
                IProcessApplicationInterface rawObject = processApplication.RawObject;

                if (rawObject is AbstractProcessApplication)
                {
                    AbstractProcessApplication abstractProcessApplication = (AbstractProcessApplication)rawObject;
                    return abstractProcessApplication.EnvironmentScripts;
                }
                return null;
            }
            catch (ProcessApplicationUnavailableException e)
            {
                throw new ProcessEngineException("Process Application is unavailable.", e);
            }
        }

        /// <summary>
        /// Returns the env scripts for the given language. Performs lazy initialization of the env scripts.
        /// </summary>
        /// <param name="scriptLanguage"> the language </param>
        /// <returns> a list of executable environment scripts. Never null. </returns>
        protected internal virtual IList<ExecutableScript> getEnvScripts(string scriptLanguage)
        {
            IDictionary<string, IList<ExecutableScript>> environment = getEnv(scriptLanguage);
            IList<ExecutableScript> envScripts = environment.ContainsKey(scriptLanguage) ? environment[scriptLanguage] : null;
            if (envScripts == null)
            {
                lock (this)
                {
                    envScripts = environment.ContainsKey(scriptLanguage) ? environment[scriptLanguage] : null;
                    if (envScripts == null)
                    {
                        envScripts = initEnvForLanguage(scriptLanguage);
                        environment[scriptLanguage] = envScripts;
                    }
                }
            }
            return envScripts;
        }

        /// <summary>
        /// Initializes the env scripts for a given language.
        /// </summary>
        /// <param name="language"> the language </param>
        /// <returns> the list of env scripts. Never null. </returns>
        protected internal virtual IList<ExecutableScript> initEnvForLanguage(string language)
        {

            IList<ExecutableScript> scripts = new List<ExecutableScript>();
            foreach (IScriptEnvResolver resolver in envResolvers)
            {
                string[] resolvedScripts = resolver.Resolve(language);
                if (resolvedScripts != null)
                {
                    foreach (string resolvedScript in resolvedScripts)
                    {
                        scripts.Add(scriptFactory.CreateScriptFromSource(language, resolvedScript));
                    }
                }
            }

            return scripts;
        }


    }
}

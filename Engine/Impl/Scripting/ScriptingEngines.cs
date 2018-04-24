using ESS.FW.Bpm.Engine.Application;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Dmn.engine.impl.spi.el;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Javax.Script;
using ESS.FW.Bpm.Engine.Impl.Scripting.Engine;
using ESS.FW.Bpm.Engine.Impl.Util;

namespace ESS.FW.Bpm.Engine.Impl.Scripting
{
    /// <summary>
	/// <para>Manager for JSR-223 <seealso cref="ScriptEngine"/> handling.</para>
	/// 
	/// <para><strong>Resolving a script engine:</strong>
	/// This class supports resolving a script engine for a given 'language name' (eg. 'groovy').
	/// If the configuration option <seealso cref="#enableScriptEngineCaching"/> is set to true,
	/// the class will attempt to cache 'cachable' script engines. We assume a <seealso cref="ScriptEngine"/> is
	/// 'cachable' if it declares to be threadsafe (see <seealso cref="#isCachable(ScriptEngine)"/>)</para>
	/// 
	/// <para><strong>Custom Bindings:</strong> this class supports custom <seealso cref="Bindings"/>
	/// implementations through the <seealso cref="#scriptBindingsFactory"/>. See <seealso cref="ScriptBindingsFactory"/>.</para>
	/// </p>
	/// 
	/// @author Tom Baeyens
	/// @author Daniel Meyer
	/// </summary>
	public class ScriptingEngines : IDmnScriptEngineResolver
    {

        public const string DEFAULT_SCRIPTING_LANGUAGE = "juel";
        public const string GROOVY_SCRIPTING_LANGUAGE = "groovy";//不支持
        public const string CSScriptLanguage = "cs";//新增cs-scripts

        protected internal ScriptEngineResolver scriptEngineResolver;
        protected internal ScriptBindingsFactory scriptBindingsFactory;

        protected internal bool enableScriptEngineCaching = true;

        public ScriptingEngines(ScriptBindingsFactory scriptBindingsFactory) : this(new ScriptEngineManager())
        {
            this.scriptBindingsFactory = scriptBindingsFactory;
        }

        public ScriptingEngines(ScriptEngineManager scriptEngineManager)
        {
            this.scriptEngineResolver = new ScriptEngineResolver(scriptEngineManager);
        }

        public virtual bool IsEnableScriptEngineCaching()
        {
            return enableScriptEngineCaching;
        }

        public virtual void SetEnableScriptEngineCaching(bool enableScriptEngineCaching)
        {
            this.enableScriptEngineCaching = enableScriptEngineCaching;
        }

        public virtual ScriptEngineManager GetScriptEngineManager()
        {
            return scriptEngineResolver.GetScriptEngineManager();
        }

        public virtual ScriptingEngines AddScriptEngineFactory(IScriptEngineFactory scriptEngineFactory)
        {
            scriptEngineResolver.AddScriptEngineFactory(scriptEngineFactory);
            return this;
        }

        /// <summary>
        /// Loads the given script engine by language name. Will throw an exception if no script engine can be loaded for the given language name.
        /// </summary>
        /// <param name="language"> the name of the script language to lookup an implementation for </param>
        /// <returns> the script engine </returns>
        /// <exception cref="ProcessEngineException"> if no such engine can be found. </exception>
        public virtual IScriptEngine GetScriptEngineForLanguage(string language)
        {

            if (language != null)
            {
                language = language.ToLower();
            }

            IProcessApplicationReference pa = Context.CurrentProcessApplication;
            ProcessEngineConfigurationImpl config = Context.ProcessEngineConfiguration;

            IScriptEngine engine = null;
            if (config.IsEnableFetchScriptEngineFromProcessApplication)
            {
                if (pa != null)
                {
                    engine = GetPaScriptEngine(language, pa);
                }
            }

            if (engine == null)
            {
                engine = GetGlobalScriptEngine(language);
            }

            return engine;
        }

        protected internal virtual IScriptEngine GetPaScriptEngine(string language, IProcessApplicationReference pa)
        {
            try
            {
                IProcessApplicationInterface processApplication = pa.ProcessApplication;
                IProcessApplicationInterface rawObject = processApplication.RawObject;

                if (rawObject is AbstractProcessApplication)
                {
                    AbstractProcessApplication abstractProcessApplication = (AbstractProcessApplication)rawObject;
                    return abstractProcessApplication.GetScriptEngineForName(language, enableScriptEngineCaching);
                }
                return null;
            }
            catch (ProcessApplicationUnavailableException e)
            {
                throw new ProcessEngineException("Process Application is unavailable.", e);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        protected internal virtual IScriptEngine GetGlobalScriptEngine(string language)
        {

            IScriptEngine scriptEngine = scriptEngineResolver.GetScriptEngine(language, enableScriptEngineCaching);

            EnsureUtil.EnsureNotNull("Can't find scripting engine for '" + language + "'", "scriptEngine", scriptEngine);

            return scriptEngine;
        }

        /// <summary>
        /// override to build a spring aware ScriptingEngines </summary>
        /// <param name="engineBindin"> </param>
        /// <param name="scriptEngine">  </param>
        public virtual IBindings CreateBindings(IScriptEngine scriptEngine, IVariableScope variableScope)
        {
            return scriptBindingsFactory.CreateBindings(variableScope, scriptEngine.CreateBindings());
        }

        public virtual ScriptBindingsFactory GetScriptBindingsFactory()
        {
            return scriptBindingsFactory;
        }

        public virtual void SetScriptBindingsFactory(ScriptBindingsFactory scriptBindingsFactory)
        {
            this.scriptBindingsFactory = scriptBindingsFactory;
        }
    }

}

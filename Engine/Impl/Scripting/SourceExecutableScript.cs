using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Javax.Script;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Engine.Impl.Scripting
{
    public class SourceExecutableScript : CompiledExecutableScript
    {
        /** The source of the script. */
        protected String scriptSource;

        /** Flag to signal if the script should be compiled */
        protected bool shouldBeCompiled = true;

        public SourceExecutableScript(String language, String source):base(language)
        {
            scriptSource = source;
        }
        protected override object Evaluate(IScriptEngine engine, IVariableScope variableScope, IBindings bindings)
        {
            if (shouldBeCompiled)
            {
                CompileScript(engine);
            }

            if (CompiledScript != null)
            {
                return base.Evaluate(engine, variableScope, bindings);
            }
            else
            {
                return EvaluateScript(engine, bindings);
            }
        }
        protected internal virtual void CompileScript(IScriptEngine engine)
        {
            ProcessEngineConfigurationImpl processEngineConfiguration = Context.ProcessEngineConfiguration;
            //if (processEngineConfiguration.IsEnableScriptEngineCaching && processEngineConfiguration.IsEnableScriptCompilation())
            if(true)
            {

                if (CompiledScript == null && shouldBeCompiled)
                {
                    lock (this)
                    {
                        if (CompiledScript == null && shouldBeCompiled)
                        {
                            // try to compile script
                            CompiledScript = Compile(engine, language, scriptSource);

                            // either the script was successfully compiled or it can't be
                            // compiled but we won't try it again
                            shouldBeCompiled = false;
                        }
                    }
                }

            }
            else
            {
                // if script compilation is disabled abort
                shouldBeCompiled = false;
            }
        }

        public virtual CompiledScript Compile(IScriptEngine scriptEngine, string language, string src)
        {
            if (scriptEngine is ICompilable && !scriptEngine.GetFactory().LanguageName.Equals("ecmascript", StringComparison.OrdinalIgnoreCase)/*.EqualsIgnoreCase("ecmascript")*/)
            {
                ICompilable compilingEngine = (ICompilable)scriptEngine;

                try
                {
                    CompiledScript compiledScript = compilingEngine.Compile(src);

                    log.DebugCompiledScriptUsing(language);

                    return compiledScript;

                }
                catch (ScriptException e)
                {
                    throw new ScriptCompilationException("Unable to compile script: " + e.Message, e);

                }

            }
            else
            {
                // engine does not support compilation
                return null;
            }

        }

        protected  virtual object EvaluateScript(IScriptEngine engine, IBindings bindings)
        {
            try
            {
                log.debugEvaluatingNonCompiledScript(scriptSource);
                return engine.Eval(scriptSource, bindings);
            }
            catch (ScriptException e)
            {
                if (e.InnerException is BpmnError)
                {
                    throw (BpmnError)e.InnerException;
                }
                throw new ScriptEvaluationException("Unable to evaluate script: " + e.Message, e);
            }
        }

        public virtual string GetScriptSource()
        {
            return scriptSource;
        }

        /// <summary>
        /// Sets the script source code. And invalidates any cached compilation result.
        /// </summary>
        /// <param name="scriptSource">
        ///          the new script source code </param>
        public virtual void SetScriptSource(string scriptSource)
        {
            this.CompiledScript = null;
            shouldBeCompiled = true;
            this.scriptSource = scriptSource;
        }

        public virtual bool IsShouldBeCompiled()
        {
            return shouldBeCompiled;
        }
    }
}

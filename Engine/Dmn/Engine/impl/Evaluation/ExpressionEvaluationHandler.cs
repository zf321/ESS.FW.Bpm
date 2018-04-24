using System;
using ESS.FW.Bpm.Engine.Dmn.engine.impl.spi.el;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Variable.Context;
using ESS.FW.Bpm.Engine.Dmn.Feel;
using ESS.FW.Bpm.Engine.Impl.Juel;
using ESS.FW.Bpm.Engine.Impl.Javax.Script;
using ESS.FW.Bpm.Engine.Dmn.Engine.impl;
using ESS.FW.Bpm.Engine.Dmn.Engine.impl.El;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl.evaluation
{
    public class ExpressionEvaluationHandler
    {
        protected internal static readonly DmnEngineLogger Log = DmnLogger.ENGINE_LOGGER;
        protected internal readonly IELProvider elProvider;
        protected internal readonly IFeelEngine feelEngine;

        protected internal readonly IDmnScriptEngineResolver ScriptEngineResolver;

        public ExpressionEvaluationHandler(DefaultDmnEngineConfiguration configuration)
        {
            ScriptEngineResolver = configuration.ScriptEngineResolver;
            elProvider = configuration.ElProvider;
            feelEngine = configuration.FeelEngine;
        }

        public virtual object EvaluateExpression(string expressionLanguage, DmnExpressionImpl expression,
            IVariableContext variableContext)
        {
            var expressionText = GetExpressionTextForLanguage(expression, expressionLanguage);
            if (!ReferenceEquals(expressionText, null))
            {
                if (IsFeelExpressionLanguage(expressionLanguage))
                {
                    //TODO FeelSimpleExpression
                    //throw new NotImplementedException("Feel 语法未实现");
                    return EvaluateFeelSimpleExpression(expressionText, variableContext);
                }
                if (IsElExpression(expressionLanguage))//juel
                {
                    return EvaluateElExpression(expressionLanguage, expressionText, variableContext, expression);
                }
                //TODO ScriptExpression
                //throw new NotImplementedException("Script 语法未实现");
                return EvaluateScriptExpression(expressionLanguage, variableContext, expressionText, expression);
            }
            return null;
        }

        protected internal virtual object EvaluateScriptExpression(string expressionLanguage,
            IVariableContext variableContext, string expressionText,
            ICachedCompiledScriptSupport cachedCompiledScriptSupport)
        {
            IScriptEngine scriptEngine = GetScriptEngineForName(expressionLanguage);
            // wrap script engine bindings + variable context and pass enhanced
            // bindings to the script engine.
            IBindings bindings = VariableContextScriptBindings.Wrap(scriptEngine.CreateBindings(), variableContext);
            bindings.Put("variableContext", variableContext);

            try
            {
                if (scriptEngine is ICompilable)
                {
                    CompiledScript compiledScript = cachedCompiledScriptSupport.GetCachedCompiledScript();
                    if (compiledScript == null)
                    {
                        lock (cachedCompiledScriptSupport)
                        {
                            compiledScript = cachedCompiledScriptSupport.GetCachedCompiledScript();

                            if (compiledScript == null)
                            {
                                ICompilable compilableScriptEngine = (ICompilable)scriptEngine;
                                compiledScript = compilableScriptEngine.Compile(expressionText);

                                cachedCompiledScriptSupport.CacheCompiledScript(compiledScript);
                            }
                        }
                    }

                    return compiledScript.Eval(bindings);
                }
                return scriptEngine.Eval(expressionText, bindings);
            }
            catch (ScriptException e)
            {
                throw Log.unableToEvaluateExpression(expressionText, scriptEngine.GetFactory().LanguageName, e);
            }
        }

        protected internal virtual object EvaluateElExpression(string expressionLanguage, string expressionText,
            IVariableContext variableContext, CachedExpressionSupport cachedExpressionSupport)
        {
            try
            {
                var elExpression = cachedExpressionSupport.CachedExpression;

                if (elExpression == null)
                    lock (cachedExpressionSupport)
                    {
                        elExpression = cachedExpressionSupport.CachedExpression;
                        if (elExpression == null)
                        {
                            elExpression = elProvider.CreateExpression(expressionText);
                            cachedExpressionSupport.CachedExpression = elExpression;
                        }
                    }

                return elExpression.GetValue(variableContext);
            }
            // yes, we catch all exceptions
            catch (System.Exception e)
            {
                throw Log.unableToEvaluateExpression(expressionText, expressionLanguage, e);
            }
        }

        protected internal virtual object EvaluateFeelSimpleExpression(string expressionText,
            IVariableContext variableContext)
        {
            return feelEngine.EvaluateSimpleExpression<object>(expressionText, variableContext);
        }

        // helper ///////////////////////////////////////////////////////////////////

        protected internal virtual string GetExpressionTextForLanguage(DmnExpressionImpl expression,
            string expressionLanguage)
        {
            var expressionText = expression.Expression;
            if (!ReferenceEquals(expressionText, null))
            {
                if (IsJuelExpression(expressionLanguage) && !Common.Utils.StringUtil.IsExpression(expressionText))
                    return "${" + expressionText + "}";
                return expressionText;
            }
            return null;
        }

        private bool IsJuelExpression(string expressionLanguage)
        {
            return DefaultDmnEngineConfiguration.JUEL_EXPRESSION_LANGUAGE.Equals(expressionLanguage,
                StringComparison.CurrentCultureIgnoreCase);
        }

        protected internal virtual IScriptEngine GetScriptEngineForName(string expressionLanguage)
        {
            EnsureUtil.EnsureNotNull("expressionLanguage", expressionLanguage);
            IScriptEngine scriptEngine = ScriptEngineResolver.GetScriptEngineForLanguage(expressionLanguage);
            if (scriptEngine != null)
            {
                return scriptEngine;
            }
            throw Log.noScriptEngineFoundForLanguage(expressionLanguage);
        }

        protected internal virtual bool IsElExpression(string expressionLanguage)
        {
            return IsJuelExpression(expressionLanguage);
        }

        public virtual bool IsFeelExpressionLanguage(string expressionLanguage)
        {
            EnsureUtil.EnsureNotNull("expressionLanguage", expressionLanguage);
            return expressionLanguage.Equals(DefaultDmnEngineConfiguration.FEEL_EXPRESSION_LANGUAGE) ||
                   expressionLanguage.ToLower()
                       .Equals(DefaultDmnEngineConfiguration.FEEL_EXPRESSION_LANGUAGE_ALTERNATIVE);
        }
    }
}
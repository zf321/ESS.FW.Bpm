using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Engine.Impl.EL;
using ESS.FW.Bpm.Engine.Impl.Javax.EL;
using ESS.FW.Bpm.Engine.Impl.Javax.Script.Engine;
using ESS.FW.Bpm.Engine.Impl.Scripting;
using ESS.FW.Bpm.Engine.Impl.util;

namespace ESS.FW.Bpm.Engine.Impl.Util
{

    /// <summary>
    ///     
    /// </summary>
    public sealed class ScriptUtil
    {
        /// <summary>
        ///     Returns the configured script factory in the context or a new one.
        /// </summary>
        public static ScriptFactory ScriptFactory
        {
            get
            {
                var processEngineConfiguration = Context.ProcessEngineConfiguration;
                if (processEngineConfiguration != null)
                {
                    return processEngineConfiguration.ScriptFactory;
                }
                return new ScriptFactory();
            }
        }

        /// <summary>
        ///     Creates a new <seealso cref="ExecutableScript" /> from a source or resource. It excepts static and
        ///     dynamic sources and resources. Dynamic means that the source or resource is an expression
        ///     which will be evaluated during execution.
        /// </summary>
        /// <param name="language"> the language of the script </param>
        /// <param name="source"> the source code of the script or an expression which evaluates to the source code </param>
        /// <param name="resource"> the resource path of the script code or an expression which evaluates to the resource path </param>
        /// <param name="expressionManager"> the expression manager to use to generate the expressions of dynamic scripts </param>
        /// <returns> the newly created script </returns>
        /// <exception cref="NotValidException"> if language is null or empty or both of source and resource are null or empty </exception>
        public static ExecutableScript GetScript(string language, string source, string resource,
            ExpressionManager expressionManager)
        {
            return GetScript(language, source, resource, expressionManager, ScriptFactory);
        }

        /// <summary>
        ///     Creates a new <seealso cref="ExecutableScript" /> from a source or resource. It excepts static and
        ///     dynamic sources and resources. Dynamic means that the source or resource is an expression
        ///     which will be evaluated during execution.
        /// </summary>
        /// <param name="language"> the language of the script </param>
        /// <param name="source"> the source code of the script or an expression which evaluates to the source code </param>
        /// <param name="resource"> the resource path of the script code or an expression which evaluates to the resource path </param>
        /// <param name="expressionManager"> the expression manager to use to generate the expressions of dynamic scripts </param>
        /// <param name="scriptFactory"> the script factory used to create the script </param>
        /// <returns> the newly created script </returns>
        /// <exception cref="NotValidException"> if language is null or empty or both of source and resource are invalid </exception>
        public static ExecutableScript GetScript(string language, string source, string resource,
            ExpressionManager expressionManager, ScriptFactory scriptFactory)
        {
            EnsureUtil.EnsureNotEmpty(typeof(NotValidException), "Script language", language);
            EnsureUtil.EnsureAtLeastOneNotNull(typeof(NotValidException), "No script source or resource was given", source,
                resource);
            if (!ReferenceEquals(resource, null) && resource.Length > 0)
            {
                return GetScriptFromResource(language, resource, expressionManager, scriptFactory);
            }
            return GetScriptFormSource(language, source, expressionManager, scriptFactory);
        }

        /// <summary>
        ///     Creates a new <seealso cref="ExecutableScript" /> from a source. It excepts static and dynamic sources.
        ///     Dynamic means that the source is an expression which will be evaluated during execution.
        /// </summary>
        /// <param name="language"> the language of the script </param>
        /// <param name="source"> the source code of the script or an expression which evaluates to the source code </param>
        /// <param name="expressionManager"> the expression manager to use to generate the expressions of dynamic scripts </param>
        /// <param name="scriptFactory"> the script factory used to create the script </param>
        /// <returns> the newly created script </returns>
        /// <exception cref="NotValidException"> if language is null or empty or source is null </exception>
        public static ExecutableScript GetScriptFormSource(string language, string source,
            ExpressionManager expressionManager, ScriptFactory scriptFactory)
        {
            EnsureUtil.EnsureNotEmpty(typeof(NotValidException), "Script language", language);
            EnsureUtil.EnsureNotNull(typeof(NotValidException), "Script source", source);
            if (IsDynamicScriptExpression(language, source))
            {
                IExpression sourceExpression = expressionManager.CreateExpression(source);
                return GetScriptFromSourceExpression(language, sourceExpression, scriptFactory);
            }
            return GetScriptFromSource(language, source, scriptFactory);
        }

        /// <summary>
        ///     Creates a new <seealso cref="ExecutableScript" /> from a static source.
        /// </summary>
        /// <param name="language"> the language of the script </param>
        /// <param name="source"> the source code of the script </param>
        /// <param name="scriptFactory"> the script factory used to create the script </param>
        /// <returns> the newly created script </returns>
        /// <exception cref="NotValidException"> if language is null or empty or source is null </exception>
        public static ExecutableScript GetScriptFromSource(string language, string source, ScriptFactory scriptFactory)
        {
            EnsureUtil.EnsureNotEmpty(typeof(NotValidException), "Script language", language);
            EnsureUtil.EnsureNotNull(typeof(NotValidException), "Script source", source);
            return scriptFactory.CreateScriptFromSource(language, source);
        }

        /// <summary>
        ///     Creates a new <seealso cref="ExecutableScript" /> from a dynamic source. Dynamic means that the source
        ///     is an expression which will be evaluated during execution.
        /// </summary>
        /// <param name="language"> the language of the script </param>
        /// <param name="sourceExpression"> the expression which evaluates to the source code </param>
        /// <param name="scriptFactory"> the script factory used to create the script </param>
        /// <returns> the newly created script </returns>
        /// <exception cref="NotValidException"> if language is null or empty or sourceExpression is null </exception>
        public static ExecutableScript GetScriptFromSourceExpression(string language, IExpression sourceExpression,
            ScriptFactory scriptFactory)
        {
            EnsureUtil.EnsureNotEmpty(typeof(NotValidException), "Script language", language);
            EnsureUtil.EnsureNotNull(typeof(NotValidException), "Script source expression", sourceExpression);
            return scriptFactory.CreateScriptFromSource(language, sourceExpression);
        }

        /// <summary>
        ///     Creates a new <seealso cref="ExecutableScript" /> from a resource. It excepts static and dynamic resources.
        ///     Dynamic means that the resource is an expression which will be evaluated during execution.
        /// </summary>
        /// <param name="language"> the language of the script </param>
        /// <param name="resource"> the resource path of the script code or an expression which evaluates to the resource path </param>
        /// <param name="expressionManager"> the expression manager to use to generate the expressions of dynamic scripts </param>
        /// <param name="scriptFactory"> the script factory used to create the script </param>
        /// <returns> the newly created script </returns>
        /// <exception cref="NotValidException"> if language or resource are null or empty </exception>
        public static ExecutableScript GetScriptFromResource(string language, string resource,
            ExpressionManager expressionManager, ScriptFactory scriptFactory)
        {
            EnsureUtil.EnsureNotEmpty(typeof(NotValidException), "Script language", language);
            EnsureUtil.EnsureNotEmpty(typeof(NotValidException), "Script resource", resource);
            if (IsDynamicScriptExpression(language, resource))
            {
                IExpression resourceExpression = expressionManager.CreateExpression(resource);
                return GetScriptFromResourceExpression(language, resourceExpression, scriptFactory);
            }
            return GetScriptFromResource(language, resource, scriptFactory);
        }

        /// <summary>
        ///     Creates a new <seealso cref="ExecutableScript" /> from a static resource.
        /// </summary>
        /// <param name="language"> the language of the script </param>
        /// <param name="resource"> the resource path of the script code </param>
        /// <param name="scriptFactory"> the script factory used to create the script </param>
        /// <returns> the newly created script </returns>
        /// <exception cref="NotValidException"> if language or resource are null or empty </exception>
        public static ExecutableScript GetScriptFromResource(string language, string resource,
            ScriptFactory scriptFactory)
        {
            EnsureUtil.EnsureNotEmpty(typeof(NotValidException), "Script language", language);
            EnsureUtil.EnsureNotEmpty(typeof(NotValidException), "Script resource", resource);
            return scriptFactory.CreateScriptFromResource(language, resource);
        }

        /// <summary>
        ///     Creates a new <seealso cref="ExecutableScript" /> from a dynamic resource. Dynamic means that the source
        ///     is an expression which will be evaluated during execution.
        /// </summary>
        /// <param name="language"> the language of the script </param>
        /// <param name="resourceExpression"> the expression which evaluates to the resource path </param>
        /// <param name="scriptFactory"> the script factory used to create the script </param>
        /// <returns> the newly created script </returns>
        /// <exception cref="NotValidException"> if language is null or empty or resourceExpression is null </exception>
        public static ExecutableScript GetScriptFromResourceExpression(string language, IExpression resourceExpression,
            ScriptFactory scriptFactory)
        {
            EnsureUtil.EnsureNotEmpty(typeof(NotValidException), "Script language", language);
            EnsureUtil.EnsureNotNull(typeof(NotValidException), "Script resource expression", resourceExpression);
            return scriptFactory.CreateScriptFromResource(language, resourceExpression);
        }

        /// <summary>
        ///     Checks if the value is an expression for a dynamic script source or resource.
        /// </summary>
        /// <param name="language"> the language of the script </param>
        /// <param name="value"> the value to check </param>
        /// <returns> true if the value is an expression for a dynamic script source/resource, otherwise false </returns>
        public static bool IsDynamicScriptExpression(string language, string value)
        {
            return StringUtil.IsExpression(value) &&
                   (!ReferenceEquals(language, null) && !JuelScriptEngineFactory.names.Contains(language.ToLower()));
        }
    }
}
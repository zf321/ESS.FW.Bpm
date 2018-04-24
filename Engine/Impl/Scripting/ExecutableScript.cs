using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Javax.Script;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Engine.Impl.Scripting
{
    /// <summary>
	/// <para>Represents an executable script.</para>
	/// </summary>
	public abstract class ExecutableScript
    {

        /// <summary>
        /// The language of the script. Used to resolve the
        /// <seealso cref="ScriptEngine"/>. 
        /// </summary>
        protected internal readonly string language;

        protected internal ExecutableScript(string language)
        {
            this.language = language;
        }

        /// <summary>
        /// The language in which the script is written. </summary>
        /// <returns> the language </returns>
        public virtual string Language
        {
            get { return language; }
        }

        /// <summary>
        /// <para>Evaluates the script using the provided engine and bindings</para>
        /// </summary>
        /// <param name="scriptEngine"> the script engine to use for evaluating the script. </param>
        /// <param name="variableScope"> the variable scope of the execution </param>
        /// <param name="bindings"> the bindings to use for evaluating the script. </param>
        /// <exception cref="ProcessEngineException"> in case the script cannot be evaluated. </exception>
        /// <returns> the result of the script evaluation </returns>
        public virtual object Execute(IScriptEngine scriptEngine, IVariableScope variableScope, IBindings bindings)
        {
            return Evaluate(scriptEngine, variableScope, bindings);
        }

        protected  abstract object Evaluate(IScriptEngine scriptEngine, IVariableScope variableScope, IBindings bindings);

    }

}

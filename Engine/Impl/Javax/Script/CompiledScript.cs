using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Engine.Impl.Javax.Script
{
    /// <summary>
	/// Extended by classes that store results of compilations.  State
	/// might be stored in the form of Java classes, Java class files or scripting
	/// language opcodes.  The script may be executed repeatedly
	/// without reparsing.
	/// <br><br>
	/// Each <code>CompiledScript</code> is associated with a <code>ScriptEngine</code> -- A call to an  <code>eval</code>
	/// method of the <code>CompiledScript</code> causes the execution of the script by the
	/// <code>ScriptEngine</code>.  Changes in the state of the <code>ScriptEngine</code> caused by execution
	/// of tne <code>CompiledScript</code>  may visible during subsequent executions of scripts by the engine.
	/// 
	/// @author Mike Grogan
	/// @since 1.6
	/// </summary>
	public abstract class CompiledScript
    {

        /// <summary>
        /// Executes the program stored in this <code>CompiledScript</code> object.
        /// </summary>
        /// <param name="context"> A <code>ScriptContext</code> that is used in the same way as
        /// the <code>ScriptContext</code> passed to the <code>eval</code> methods of
        /// <code>ScriptEngine</code>.
        /// </param>
        /// <returns> The value returned by the script execution, if any.  Should return <code>null</code>
        /// if no value is returned by the script execution.
        /// </returns>
        /// <exception cref="ScriptException"> if an error occurs. </exception>
        /// <exception cref="NullPointerException"> if context is null. </exception>

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public abstract Object eval(ScriptContext context) throws ScriptException;
        public abstract object Eval(IScriptContext context);

        /// <summary>
        /// Executes the program stored in the <code>CompiledScript</code> object using
        /// the supplied <code>Bindings</code> of attributes as the <code>ENGINE_SCOPE</code> of the
        /// associated <code>ScriptEngine</code> during script execution.  If bindings is null,
        /// then the effect of calling this method is same as that of eval(getEngine().getContext()).
        /// <para>.
        /// The <code>GLOBAL_SCOPE</code> <code>Bindings</code>, <code>Reader</code> and <code>Writer</code>
        /// associated with the default <code>ScriptContext</code> of the associated <code>ScriptEngine</code> are used.
        /// 
        /// </para>
        /// </summary>
        /// <param name="bindings"> The bindings of attributes used for the <code>ENGINE_SCOPE</code>.
        /// </param>
        /// <returns> The return value from the script execution
        /// </returns>
        /// <exception cref="ScriptException"> if an error occurs. </exception>
        public virtual object Eval(IBindings bindings)
        {
            try
            {
                IScriptContext ctxt = GetEngine().GetContext();

                if (bindings != null)
                {
                    SimpleScriptContext tempctxt = new SimpleScriptContext();
                    tempctxt.SetBindings(bindings, ScriptContext_Fields.ENGINE_SCOPE);
                    tempctxt.SetBindings(ctxt.GetBindings(ScriptContext_Fields.GLOBAL_SCOPE), ScriptContext_Fields.GLOBAL_SCOPE);
                    tempctxt.SetWriter(ctxt.GetWriter());
                    tempctxt.SetReader(ctxt.GetReader());
                    tempctxt.SetErrorWriter(ctxt.GetErrorWriter());
                    ctxt = tempctxt;
                }

                return Eval(ctxt);
            }
            catch(ScriptException ex)
            {
                throw ex;
            }
            catch (System.Exception ex)
            {
                throw new ScriptException(ex);
            }
            
        }


        /// <summary>
        /// Executes the program stored in the <code>CompiledScript</code> object.  The
        /// default <code>ScriptContext</code> of the associated <code>ScriptEngine</code> is used.
        /// The effect of calling this method is same as that of eval(getEngine().getContext()).
        /// </summary>
        /// <returns> The return value from the script execution
        /// </returns>
        /// <exception cref="ScriptException"> if an error occurs. </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public Object eval() throws ScriptException
        public virtual object Eval()
        {
            return Eval(GetEngine().GetContext());
        }

        /// <summary>
        /// Returns the <code>ScriptEngine</code> whose <code>compile</code> method created this <code>CompiledScript</code>.
        /// The <code>CompiledScript</code> will execute in this engine.
        /// </summary>
        /// <returns> The <code>ScriptEngine</code> that created this <code>CompiledScript</code> </returns>
        public abstract IScriptEngine GetEngine();

    }

}

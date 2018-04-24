using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESS.FW.Bpm.Engine.Impl.Juel;

namespace ESS.FW.Bpm.Engine.Impl.Javax.Script
{
    /// <summary>
	/// Provides a standard implementation for several of the variants of the <code>eval</code>
	/// method.
	/// <br><br>
	/// <code><b>eval(Reader)</b></code><para><code><b>eval(String)</b></code><p>
	/// </para>
	/// <code><b>eval(String, Bindings)</b></code><para><code><b>eval(Reader, Bindings)</b></code>
	/// <br><br> are implemented using the abstract methods
	/// <br><br>
	/// <code><b>eval(Reader,ScriptContext)</b></code> or
	/// <code><b>eval(String, ScriptContext)</b></code>
	/// <br><br>
	/// with a <code>SimpleScriptContext</code>.
	/// <br><br>
	/// A <code>SimpleScriptContext</code> is used as the default <code>ScriptContext</code>
	/// of the <code>AbstractScriptEngine</code>..
	/// 
	/// @author Mike Grogan
	/// @since 1.6
	/// </para>
	/// </summary>
	public abstract class AbstractScriptEngine : IScriptEngine
    {

        /// <summary>
        /// The default <code>ScriptContext</code> of this <code>AbstractScriptEngine</code>.
        /// </summary>

        protected internal IScriptContext context;

        /// <summary>
        /// Creates a new instance of AbstractScriptEngine using a <code>SimpleScriptContext</code>
        /// as its default <code>ScriptContext</code>.
        /// </summary>
        public AbstractScriptEngine()
        {

            context = new SimpleScriptContext();

        }

        /// <summary>
        /// Creates a new instance using the specified <code>Bindings</code> as the
        /// <code>ENGINE_SCOPE</code> <code>Bindings</code> in the protected <code>context</code> field.
        /// </summary>
        /// <param name="n"> The specified <code>Bindings</code>. </param>
        /// <exception cref="NullPointerException"> if n is null. </exception>
        public AbstractScriptEngine(IBindings n) : this()
        {

            if (n == null)
            {
                throw new System.NullReferenceException("n is null");
            }
            context.SetBindings(n, ScriptContext_Fields.ENGINE_SCOPE);
        }

        /// <summary>
        /// Sets the value of the protected <code>context</code> field to the specified
        /// <code>ScriptContext</code>.
        /// </summary>
        /// <param name="ctxt"> The specified <code>ScriptContext</code>. </param>
        /// <exception cref="NullPointerException"> if ctxt is null. </exception>
        public virtual void SetContext(IScriptContext ctxt)
        {
            if (ctxt == null)
            {
                throw new System.NullReferenceException("null context");
            }
            context = ctxt;
        }

        /// <summary>
        /// Returns the value of the protected <code>context</code> field.
        /// </summary>
        /// <returns> The value of the protected <code>context</code> field. </returns>
        public virtual IScriptContext GetContext()
        {
            return context;
        }

        /// <summary>
        /// Returns the <code>Bindings</code> with the specified scope value in
        /// the protected <code>context</code> field.
        /// </summary>
        /// <param name="scope"> The specified scope
        /// </param>
        /// <returns> The corresponding <code>Bindings</code>.
        /// </returns>
        /// <exception cref="IllegalArgumentException"> if the value of scope is
        /// invalid for the type the protected <code>context</code> field. </exception>
        public virtual IBindings GetBindings(int scope)
        {

            if (scope == ScriptContext_Fields.GLOBAL_SCOPE)
            {
                return context.GetBindings(ScriptContext_Fields.GLOBAL_SCOPE);
            }
            else if (scope == ScriptContext_Fields.ENGINE_SCOPE)
            {
                return context.GetBindings(ScriptContext_Fields.ENGINE_SCOPE);
            }
            else
            {
                throw new System.ArgumentException("Invalid scope value.");
            }
        }

        /// <summary>
        /// Sets the <code>Bindings</code> with the corresponding scope value in the
        /// <code>context</code> field.
        /// </summary>
        /// <param name="bindings"> The specified <code>Bindings</code>. </param>
        /// <param name="scope"> The specified scope.
        /// </param>
        /// <exception cref="IllegalArgumentException"> if the value of scope is
        /// invalid for the type the <code>context</code> field. </exception>
        /// <exception cref="NullPointerException"> if the bindings is null and the scope is
        /// <code>ScriptContext.ENGINE_SCOPE</code> </exception>
        public virtual void SetBindings(IBindings bindings, int scope)
        {

            if (scope == ScriptContext_Fields.GLOBAL_SCOPE)
            {
                context.SetBindings(bindings, ScriptContext_Fields.GLOBAL_SCOPE);
            }
            else if (scope == ScriptContext_Fields.ENGINE_SCOPE)
            {
                context.SetBindings(bindings, ScriptContext_Fields.ENGINE_SCOPE);
            }
            else
            {
                throw new System.ArgumentException("Invalid scope value.");
            }
        }

        /// <summary>
        /// Sets the specified value with the specified key in the <code>ENGINE_SCOPE</code>
        /// <code>Bindings</code> of the protected <code>context</code> field.
        /// </summary>
        /// <param name="key"> The specified key. </param>
        /// <param name="value"> The specified value.
        /// </param>
        /// <exception cref="NullPointerException"> if key is null. </exception>
        /// <exception cref="IllegalArgumentException"> if key is empty. </exception>
        public virtual void Put(string key, object value)
        {

            IBindings nn = GetBindings(ScriptContext_Fields.ENGINE_SCOPE);
            if (nn != null)
            {
                nn.Put(key, value);
            }

        }

        /// <summary>
        /// Gets the value for the specified key in the <code>ENGINE_SCOPE</code> of the
        /// protected <code>context</code> field.
        /// </summary>
        /// <returns> The value for the specified key.
        /// </returns>
        /// <exception cref="NullPointerException"> if key is null. </exception>
        /// <exception cref="IllegalArgumentException"> if key is empty. </exception>
        public virtual object Get(string key)
        {

            IBindings nn = GetBindings(ScriptContext_Fields.ENGINE_SCOPE);
            if (nn != null)
            {
                return nn.Get(key);
            }

            return null;
        }


        /// <summary>
        /// <code>eval(Reader, Bindings)</code> calls the abstract
        /// <code>eval(Reader, ScriptContext)</code> method, passing it a <code>ScriptContext</code>
        /// whose Reader, Writers and Bindings for scopes other that <code>ENGINE_SCOPE</code>
        /// are identical to those members of the protected <code>context</code> field.  The specified
        /// <code>Bindings</code> is used instead of the <code>ENGINE_SCOPE</code>
        /// 
        /// <code>Bindings</code> of the <code>context</code> field.
        /// </summary>
        /// <param name="reader"> A <code>Reader</code> containing the source of the script. </param>
        /// <param name="bindings"> A <code>Bindings</code> to use for the <code>ENGINE_SCOPE</code>
        /// while the script executes.
        /// </param>
        /// <returns> The return value from <code>eval(Reader, ScriptContext)</code> </returns>
        /// <exception cref="ScriptException"> if an error occurs in script. </exception>
        /// <exception cref="NullPointerException"> if any of the parameters is null. </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public Object eval(java.io.Reader reader, Bindings bindings) throws ScriptException
        public virtual object Eval(Stream reader, IBindings bindings)
        {

            IScriptContext ctxt = GetScriptContext(bindings);

            return Eval(reader, ctxt);
        }


        /// <summary>
        /// Same as <code>eval(Reader, Bindings)</code> except that the abstract
        /// <code>eval(String, ScriptContext)</code> is used.
        /// </summary>
        /// <param name="script"> A <code>String</code> containing the source of the script.
        /// </param>
        /// <param name="bindings"> A <code>Bindings</code> to use as the <code>ENGINE_SCOPE</code>
        /// while the script executes.
        /// </param>
        /// <returns> The return value from <code>eval(String, ScriptContext)</code> </returns>
        /// <exception cref="ScriptException"> if an error occurs in script. </exception>
        /// <exception cref="NullPointerException"> if any of the parameters is null. </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public Object eval(String script, Bindings bindings) throws ScriptException
        public virtual object Eval(string script, IBindings bindings)
        {

            IScriptContext ctxt = GetScriptContext(bindings);

            return Eval(script, ctxt);
        }

        /// <summary>
        /// <code>eval(Reader)</code> calls the abstract
        /// <code>eval(Reader, ScriptContext)</code> passing the value of the <code>context</code>
        /// field.
        /// </summary>
        /// <param name="reader"> A <code>Reader</code> containing the source of the script. </param>
        /// <returns> The return value from <code>eval(Reader, ScriptContext)</code> </returns>
        /// <exception cref="ScriptException"> if an error occurs in script. </exception>
        /// <exception cref="NullPointerException"> if any of the parameters is null. </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public Object eval(java.io.Reader reader) throws ScriptException
        public virtual object Eval(Stream reader)
        {


            return Eval(reader, context);
        }

        /// <summary>
        /// Same as <code>eval(Reader)</code> except that the abstract
        /// <code>eval(String, ScriptContext)</code> is used.
        /// </summary>
        /// <param name="script"> A <code>String</code> containing the source of the script. </param>
        /// <returns> The return value from <code>eval(String, ScriptContext)</code> </returns>
        /// <exception cref="ScriptException"> if an error occurrs in script. </exception>
        /// <exception cref="NullPointerException"> if any of the parameters is null. </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public Object eval(String script) throws ScriptException
        public virtual object Eval(string script)
        {


            return Eval(script, context);
        }

        /// <summary>
        /// Returns a <code>SimpleScriptContext</code>.  The <code>SimpleScriptContext</code>:
        /// <br><br>
        /// <ul>
        /// <li>Uses the specified <code>Bindings</code> for its <code>ENGINE_SCOPE</code>
        /// </li>
        /// <li>Uses the <code>Bindings</code> returned by the abstract <code>getGlobalScope</code>
        /// method as its <code>GLOBAL_SCOPE</code>
        /// </li>
        /// <li>Uses the Reader and Writer in the default <code>ScriptContext</code> of this
        /// <code>ScriptEngine</code>
        /// </li>
        /// </ul>
        /// <br><br>
        /// A <code>SimpleScriptContext</code> returned by this method is used to implement eval methods
        /// using the abstract <code>eval(Reader,Bindings)</code> and <code>eval(String,Bindings)</code>
        /// versions.
        /// </summary>
        /// <param name="nn"> Bindings to use for the <code>ENGINE_SCOPE</code> </param>
        /// <returns> The <code>SimpleScriptContext</code> </returns>
        protected internal virtual IScriptContext GetScriptContext(IBindings nn)
        {

            SimpleScriptContext ctxt = new SimpleScriptContext();
            IBindings gs = GetBindings(ScriptContext_Fields.GLOBAL_SCOPE);

            if (gs != null)
            {
                ctxt.SetBindings(gs, ScriptContext_Fields.GLOBAL_SCOPE);
            }

            if (nn != null)
            {
                ctxt.SetBindings(nn, ScriptContext_Fields.ENGINE_SCOPE);
            }
            else
            {
                throw new System.NullReferenceException("Engine scope Bindings may not be null.");
            }

            ctxt.SetReader(context.GetReader());
            ctxt.SetWriter(context.GetWriter());
            ctxt.SetErrorWriter(context.GetErrorWriter());

            return ctxt;

        }

        public abstract object Eval(string script, IScriptContext context);

        public abstract object Eval(Stream reader, IScriptContext context);


        public abstract IBindings CreateBindings();

        public abstract IScriptEngineFactory GetFactory();

    }

}

using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.Impl.Juel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Engine.Impl.Javax.Script
{
    /// <summary>
    /// Simple implementation of ScriptContext.
    /// 
    /// @author Mike Grogan
    /// @since 1.6
    /// </summary>
    public class SimpleScriptContext : IScriptContext
    {

        /// <summary>
        /// This is the writer to be used to output from scripts.
        /// By default, a <code>PrintWriter</code> based on <code>System.out</code>
        /// is used. Accessor methods getWriter, setWriter are used to manage
        /// this field. </summary>
        /// <seealso cref= java.lang.System#out </seealso>
        /// <seealso cref= java.io.PrintWriter </seealso>
        protected internal Stream writer;

        /// <summary>
        /// This is the writer to be used to output errors from scripts.
        /// By default, a <code>PrintWriter</code> based on <code>System.err</code> is
        /// used. Accessor methods getErrorWriter, setErrorWriter are used to manage
        /// this field. </summary>
        /// <seealso cref= java.lang.System#err </seealso>
        /// <seealso cref= java.io.PrintWriter </seealso>
        protected internal Stream errorWriter;

        /// <summary>
        /// This is the reader to be used for input from scripts.
        /// By default, a <code>InputStreamReader</code> based on <code>System.in</code>
        /// is used and default charset is used by this reader. Accessor methods
        /// getReader, setReader are used to manage this field. </summary>
        /// <seealso cref= java.lang.System#in </seealso>
        /// <seealso cref= java.io.InputStreamReader </seealso>
        protected internal Stream reader;


        /// <summary>
        /// This is the engine scope bindings.
        /// By default, a <code>SimpleBindings</code> is used. Accessor
        /// methods setBindings, getBindings are used to manage this field. </summary>
        /// <seealso cref= SimpleBindings </seealso>
        protected internal IBindings engineScope;

        /// <summary>
        /// This is the global scope bindings.
        /// By default, a null value (which means no global scope) is used. Accessor
        /// methods setBindings, getBindings are used to manage this field.
        /// </summary>
        protected internal IBindings globalScope;


        public SimpleScriptContext()
        {
            engineScope = new SimpleBindings();
            globalScope = null;
            //reader = new StreamReader(); // new InputStreamReader(System.in);
            //writer = new PrintWriter(System.out, true);
            //errorWriter = new PrintWriter(System.err, true);
            reader = new MemoryStream();
            writer = new MemoryStream();
            errorWriter = new MemoryStream();
        }

        /// <summary>
        /// Sets a <code>Bindings</code> of attributes for the given scope.  If the value
        /// of scope is <code>ENGINE_SCOPE</code> the given <code>Bindings</code> replaces the
        /// <code>engineScope</code> field.  If the value
        /// of scope is <code>GLOBAL_SCOPE</code> the given <code>Bindings</code> replaces the
        /// <code>globalScope</code> field.
        /// </summary>
        /// <param name="bindings"> The <code>Bindings</code> of attributes to set. </param>
        /// <param name="scope"> The value of the scope in which the attributes are set.
        /// </param>
        /// <exception cref="IllegalArgumentException"> if scope is invalid. </exception>
        /// <exception cref="NullPointerException"> if the value of scope is <code>ENGINE_SCOPE</code> and
        /// the specified <code>Bindings</code> is null. </exception>
        public virtual void SetBindings(IBindings bindings, int scope)
        {

            switch (scope)
            {

                case ScriptContext_Fields.ENGINE_SCOPE:
                    if (bindings == null)
                    {
                        throw new System.NullReferenceException("Engine scope cannot be null.");
                    }
                    engineScope = bindings;
                    break;
                case ScriptContext_Fields.GLOBAL_SCOPE:
                    globalScope = bindings;
                    break;
                default:
                    throw new System.ArgumentException("Invalid scope value.");
            }
        }


        /// <summary>
        /// Retrieves the value of the attribute with the given name in
        /// the scope occurring earliest in the search order.  The order
        /// is determined by the numeric value of the scope parameter (lowest
        /// scope values first.)
        /// </summary>
        /// <param name="name"> The name of the the attribute to retrieve. </param>
        /// <returns> The value of the attribute in the lowest scope for
        /// which an attribute with the given name is defined.  Returns
        /// null if no attribute with the name exists in any scope. </returns>
        /// <exception cref="NullPointerException"> if the name is null. </exception>
        /// <exception cref="IllegalArgumentException"> if the name is empty. </exception>
        public virtual object GetAttribute(string name)
        {
            if (engineScope.ContainsKey(name))
            {
                return GetAttribute(name, ScriptContext_Fields.ENGINE_SCOPE);
            }
            else if (globalScope != null && globalScope.ContainsKey(name))
            {
                return GetAttribute(name, ScriptContext_Fields.GLOBAL_SCOPE);
            }

            return null;
        }

        /// <summary>
        /// Gets the value of an attribute in a given scope.
        /// </summary>
        /// <param name="name"> The name of the attribute to retrieve. </param>
        /// <param name="scope"> The scope in which to retrieve the attribute. </param>
        /// <returns> The value of the attribute. Returns <code>null</code> is the name
        /// does not exist in the given scope.
        /// </returns>
        /// <exception cref="IllegalArgumentException">
        ///         if the name is empty or if the value of scope is invalid. </exception>
        /// <exception cref="NullPointerException"> if the name is null. </exception>
        public virtual object GetAttribute(string name, int scope)
        {

            switch (scope)
            {

                case ScriptContext_Fields.ENGINE_SCOPE:
                    return engineScope.Get(name);

                case ScriptContext_Fields.GLOBAL_SCOPE:
                    if (globalScope != null)
                    {
                        return globalScope.Get(name);
                    }
                    return null;

                default:
                    throw new System.ArgumentException("Illegal scope value.");
            }
        }

        /// <summary>
        /// Remove an attribute in a given scope.
        /// </summary>
        /// <param name="name"> The name of the attribute to remove </param>
        /// <param name="scope"> The scope in which to remove the attribute
        /// </param>
        /// <returns> The removed value. </returns>
        /// <exception cref="IllegalArgumentException">
        ///         if the name is empty or if the scope is invalid. </exception>
        /// <exception cref="NullPointerException"> if the name is null. </exception>
        public virtual object RemoveAttribute(string name, int scope)
        {

            switch (scope)
            {

                case ScriptContext_Fields.ENGINE_SCOPE:
                    if (GetBindings(ScriptContext_Fields.ENGINE_SCOPE) != null)
                    {
                        return GetBindings(ScriptContext_Fields.ENGINE_SCOPE).Remove(name);
                    }
                    return null;

                case ScriptContext_Fields.GLOBAL_SCOPE:
                    if (GetBindings(ScriptContext_Fields.GLOBAL_SCOPE) != null)
                    {
                        return GetBindings(ScriptContext_Fields.GLOBAL_SCOPE).Remove(name);
                    }
                    return null;

                default:
                    throw new System.ArgumentException("Illegal scope value.");
            }
        }

        /// <summary>
        /// Sets the value of an attribute in a given scope.
        /// </summary>
        /// <param name="name"> The name of the attribute to set </param>
        /// <param name="value"> The value of the attribute </param>
        /// <param name="scope"> The scope in which to set the attribute
        /// </param>
        /// <exception cref="IllegalArgumentException">
        ///         if the name is empty or if the scope is invalid. </exception>
        /// <exception cref="NullPointerException"> if the name is null. </exception>
        public virtual void SetAttribute(string name, object value, int scope)
        {

            switch (scope)
            {

                case ScriptContext_Fields.ENGINE_SCOPE:
                    engineScope.Put(name, value);
                    return;

                case ScriptContext_Fields.GLOBAL_SCOPE://全局？
                    if (globalScope != null)
                    {
                        globalScope.Put(name, value);
                    }
                    return;

                default:
                    throw new System.ArgumentException("Illegal scope value.");
            }
        }

        /// <summary>
        /// {@inheritDoc} </summary>
        //public virtual Writer getWriter()
        //{
        //    return writer;
        //}

        /// <summary>
        /// {@inheritDoc} </summary>
        public virtual Stream GetReader()
        {
            return reader;
        }

        /// <summary>
        /// {@inheritDoc} </summary>
        public virtual void SetReader(Stream reader)
        {
            this.reader = reader;
        }

        /// <summary>
        /// {@inheritDoc} </summary>
        public virtual void SetWriter(Stream writer)
        {
            this.writer = writer;
        }
        public Stream GetWriter()
        {
            return this.writer;
        }
        public virtual void SetErrorWriter(Stream error)
        {
            this.errorWriter = error;
        }

        /// <summary>
        /// {@inheritDoc} </summary>
        public virtual Stream GetErrorWriter()
        {
            return errorWriter;
        }

        /// <summary>
        /// {@inheritDoc} </summary>
        public virtual void GetErrorWriter(Stream writer)
        {
            this.errorWriter = writer;
        }

        /// <summary>
        /// Get the lowest scope in which an attribute is defined. </summary>
        /// <param name="name"> Name of the attribute
        /// . </param>
        /// <returns> The lowest scope.  Returns -1 if no attribute with the given
        /// name is defined in any scope. </returns>
        /// <exception cref="NullPointerException"> if name is null. </exception>
        /// <exception cref="IllegalArgumentException"> if name is empty. </exception>
        public virtual int GetAttributesScope(string name)
        {
            if (engineScope.ContainsKey(name))
            {
                return ScriptContext_Fields.ENGINE_SCOPE;
            }
            else if (globalScope != null && globalScope.ContainsKey(name))
            {
                return ScriptContext_Fields.GLOBAL_SCOPE;
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// Returns the value of the <code>engineScope</code> field if specified scope is
        /// <code>ENGINE_SCOPE</code>.  Returns the value of the <code>globalScope</code> field if the specified scope is
        /// <code>GLOBAL_SCOPE</code>.
        /// </summary>
        /// <param name="scope"> The specified scope </param>
        /// <returns> The value of either the  <code>engineScope</code> or <code>globalScope</code> field. </returns>
        /// <exception cref="IllegalArgumentException"> if the value of scope is invalid. </exception>
        public virtual IBindings GetBindings(int scope)
        {
            if (scope == ScriptContext_Fields.ENGINE_SCOPE)
            {
                return engineScope;
            }
            else if (scope == ScriptContext_Fields.GLOBAL_SCOPE)
            {
                return globalScope;
            }
            else
            {
                throw new System.ArgumentException("Illegal scope value.");
            }
        }

        /// <summary>
        /// {@inheritDoc} </summary>
        public virtual IList<int?> GetScopes()
        {
            return scopes;
        }

        private static IList<int?> scopes;
        static SimpleScriptContext()
        {
            scopes = new List<int?>(2);
            scopes.Add(ScriptContext_Fields.ENGINE_SCOPE);
            scopes.Add(ScriptContext_Fields.GLOBAL_SCOPE);
            //scopes = Collections.UnmodifiableList(scopes);//变成只读List？
        }
    }

}

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
    /// The interface whose implementing classes are used to connect Script Engines
    /// with objects, such as scoped Bindings, in hosting applications.  Each scope is a set
    /// of named attributes whose values can be set and retrieved using the
    /// <code>ScriptContext</code> methods. ScriptContexts also expose Readers and Writers
    /// that can be used by the ScriptEngines for input and output.
    /// 
    /// @author Mike Grogan
    /// @since 1.6
    /// </summary>
    public interface IScriptContext
    {


        /// <summary>
        /// EngineScope attributes are visible during the lifetime of a single
        /// <code>ScriptEngine</code> and a set of attributes is maintained for each
        /// engine.
        /// </summary>

        /// <summary>
        /// GlobalScope attributes are visible to all engines created by same ScriptEngineFactory.
        /// </summary>


        /// <summary>
        /// Associates a <code>Bindings</code> instance with a particular scope in this
        /// <code>ScriptContext</code>.  Calls to the <code>getAttribute</code> and
        /// <code>setAttribute</code> methods must map to the <code>get</code> and
        /// <code>put</code> methods of the <code>Bindings</code> for the specified scope.
        /// </summary>
        /// <param name="bindings"> The <code>Bindings</code> to associate with the given scope </param>
        /// <param name="scope"> The scope
        /// </param>
        /// <exception cref="IllegalArgumentException"> If no <code>Bindings</code> is defined for the
        /// specified scope value in ScriptContexts of this type. </exception>
        /// <exception cref="NullPointerException"> if value of scope is <code>ENGINE_SCOPE</code> and
        /// the specified <code>Bindings</code> is null.
        ///  </exception>
        void SetBindings(IBindings bindings, int scope);

        /// <summary>
        /// Gets the <code>Bindings</code>  associated with the given scope in this
        /// <code>ScriptContext</code>.
        /// </summary>
        /// <returns> The associated <code>Bindings</code>.  Returns <code>null</code> if it has not
        /// been set.
        /// </returns>
        /// <exception cref="IllegalArgumentException"> If no <code>Bindings</code> is defined for the
        /// specified scope value in <code>ScriptContext</code> of this type. </exception>
        IBindings GetBindings(int scope);

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
        void SetAttribute(string name, object value, int scope);

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
        object GetAttribute(string name, int scope);

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
        object RemoveAttribute(string name, int scope);

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
        object GetAttribute(string name);


        /// <summary>
        /// Get the lowest scope in which an attribute is defined. </summary>
        /// <param name="name"> Name of the attribute
        /// . </param>
        /// <returns> The lowest scope.  Returns -1 if no attribute with the given
        /// name is defined in any scope. </returns>
        /// <exception cref="NullPointerException"> if name is null. </exception>
        /// <exception cref="IllegalArgumentException"> if name is empty. </exception>
        int GetAttributesScope(string name);

        /// <summary>
        /// Returns the <code>Writer</code> for scripts to use when displaying output.
        /// </summary>
        /// <returns> The <code>Writer</code>. </returns>
        Stream GetWriter();


        /// <summary>
        /// Returns the <code>Writer</code> used to display error output.
        /// </summary>
        /// <returns> The <code>Writer</code> </returns>
        Stream GetErrorWriter();

        /// <summary>
        /// Sets the <code>Writer</code> for scripts to use when displaying output.
        /// </summary>
        /// <param name="writer"> The new <code>Writer</code>. </param>
        void SetWriter(Stream writer);


        /// <summary>
        /// Sets the <code>Writer</code> used to display error output.
        /// </summary>
        /// <param name="writer"> The <code>Writer</code>. </param>
        void SetErrorWriter(Stream writer);

        /// <summary>
        /// Returns a <code>Reader</code> to be used by the script to read
        /// input.
        /// </summary>
        /// <returns> The <code>Reader</code>. </returns>
        Stream GetReader();


        /// <summary>
        /// Sets the <code>Reader</code> for scripts to read input
        /// . </summary>
        /// <param name="reader"> The new <code>Reader</code>. </param>
        void SetReader(Stream reader);

        /// <summary>
        /// Returns immutable <code>List</code> of all the valid values for
        /// scope in the ScriptContext.
        /// </summary>
        /// <returns> list of scope values </returns>
        IList<int?> GetScopes();
    }

    public static class ScriptContext_Fields
    {
        public const int ENGINE_SCOPE = 100;
        public const int GLOBAL_SCOPE = 200;
    }

}

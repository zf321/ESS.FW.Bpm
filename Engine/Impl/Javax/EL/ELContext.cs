using System;
using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Impl.Javax.EL
{
    /// <summary>
    ///     Context information for expression evaluation. To evaluate an <seealso cref="Expression" />, an ELContext
    ///     must be provided. The ELContext holds:
    ///     <ul>
    ///         <li>
    ///             a reference to the base <seealso cref="ELResolver" /> that will be consulted to resolve model objects
    ///             and their properties
    ///         </li>
    ///         <li>a reference to <seealso cref="FunctionMapper" /> that will be used to resolve EL Functions.</li>
    ///         <li>a reference to <seealso cref="VariableMapper" /> that will be used to resolve EL Variables.</li>
    ///         <li>a collection of all the relevant context objects for use by ELResolvers</li>
    ///         <li>
    ///             state information during the evaluation of an expression, such as whether a property has been
    ///             resolved yet
    ///         </li>
    ///     </ul>
    ///     The collection of context objects is necessary because each ELResolver may need access to a
    ///     different context object. For example, JSP and Faces resolvers need access to a
    ///     javax.servlet.jsp.JspContext and a javax.faces.context.FacesContext, respectively. Creation of
    ///     ELContext objects is controlled through the underlying technology. For example, in JSP the
    ///     JspContext.getELContext() factory method is used. Some technologies provide the ability to add an
    ///     <seealso cref="ELContextListener" /> so that applications and frameworks can ensure their own context
    ///     objects are attached to any newly created ELContext. Because it stores state during expression
    ///     evaluation, an ELContext object is not thread-safe. Care should be taken to never share an
    ///     ELContext instance between two or more threads.
    /// </summary>
    [Serializable]
    public abstract class ELContext
    {
        private readonly IDictionary<Type, object> _context = new Dictionary<Type, object>();

        /// <summary>
        ///     Retrieves the ELResolver associated with this context. The ELContext maintains a reference to
        ///     the ELResolver that will be consulted to resolve variables and properties during an
        ///     expression evaluation. This method retrieves the reference to the resolver. Once an ELContext
        ///     is constructed, the reference to the ELResolver associated with the context cannot be
        ///     changed.
        /// </summary>
        /// <returns>
        ///     The resolver to be consulted for variable and property resolution during expression
        ///     evaluation.
        /// </returns>
        public abstract ELResolver ELResolver { get; }

        /// <summary>
        ///     Retrieves the FunctionMapper associated with this ELContext.
        /// </summary>
        /// <returns> The function mapper to be consulted for the resolution of EL functions. </returns>
        public abstract FunctionMapper FunctionMapper { get; }

        /// <summary>
        ///     Get the Locale stored by a previous invocation to <seealso cref="#setLocale(Locale)" />. If this method
        ///     returns non null, this Locale must be used for all localization needs in the implementation.
        ///     The Locale must not be cached to allow for applications that change Locale dynamically.
        /// </summary>
        /// <returns>
        ///     The Locale in which this instance is operating. Used primarily for message
        ///     localization.
        /// </returns>
        //public virtual Locale Locale { get; set; }
        /// <summary>
        ///     Retrieves the VariableMapper associated with this ELContext.
        /// </summary>
        /// <returns> The variable mapper to be consulted for the resolution of EL variables. </returns>
        public abstract VariableMapper VariableMapper { get; }

        /// <summary>
        ///     Returns whether an <seealso cref="ELResolver" /> has successfully resolved a given (base, property)
        ///     pair. The <seealso cref="CompositeELResolver" /> checks this property to determine whether it should
        ///     consider or skip other component resolvers.
        /// </summary>
        /// <returns> The variable mapper to be consulted for the resolution of EL variables. </returns>
        /// <seealso cref= CompositeELResolver
        /// </seealso>
        public virtual bool PropertyResolved { get; set; }

        /// <summary>
        ///     Returns the context object associated with the given key. The ELContext maintains a
        ///     collection of context objects relevant to the evaluation of an expression. These context
        ///     objects are used by ELResolvers. This method is used to retrieve the context with the given
        ///     key from the collection. By convention, the object returned will be of the type specified by
        ///     the key. However, this is not required and the key is used strictly as a unique identifier.
        /// </summary>
        /// <param name="key">
        ///     The unique identifier that was used to associate the context object with this
        ///     ELContext.
        /// </param>
        /// <returns>
        ///     The context object associated with the given key, or null if no such context was
        ///     found.
        /// </returns>
        /// <exception cref="NullPointerException">
        ///     if key is null.
        /// </exception>
        public virtual object GetContext(Type key)
        {
            try
            {
                var result = new object();
                _context.TryGetValue(key, out result);
                return result;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        ///     Associates a context object with this ELContext. The ELContext maintains a collection of
        ///     context objects relevant to the evaluation of an expression. These context objects are used
        ///     by ELResolvers. This method is used to add a context object to that collection. By
        ///     convention, the contextObject will be of the type specified by the key. However, this is not
        ///     required and the key is used strictly as a unique identifier.
        /// </summary>
        /// <param name="key">
        ///     The key used by an <seealso cref="ELResolver" /> to identify this context object.
        /// </param>
        /// <param name="contextObject">
        ///     The context object to add to the collection.
        /// </param>
        /// <exception cref="NullPointerException">
        ///     if key is null or contextObject is null.
        /// </exception>
        public virtual void PutContext(Type key, object contextObject)
        {
            _context[key] = contextObject;
        }
    }
}
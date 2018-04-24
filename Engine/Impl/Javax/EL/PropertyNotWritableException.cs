
 
 

namespace ESS.FW.Bpm.Engine.Impl.Javax.EL
{
    /// <summary>
    ///     Thrown when a property could not be written to while setting the value on a
    ///     <seealso cref="ValueExpression" />. For example, this could be triggered by trying to set a map value on an
    ///     unmodifiable map.
    /// </summary>
    public class PropertyNotWritableException : ELException
    {
        

        /// <summary>
        ///     Creates a PropertyNotWritableException with no detail message.
        /// </summary>
        public PropertyNotWritableException()
        {
        }

        /// <summary>
        ///     Creates a PropertyNotWritableException with the provided detail message.
        /// </summary>
        /// <param name="message">
        ///     the detail message
        /// </param>
        public PropertyNotWritableException(string message) : base(message)
        {
        }

        /// <summary>
        ///     Creates a PropertyNotWritableException with the given root cause.
        /// </summary>
        /// <param name="cause">
        ///     the originating cause of this exception
        /// </param>
        public PropertyNotWritableException(System.Exception cause) : base(cause)
        {
        }

        /// <summary>
        ///     Creates a PropertyNotWritableException with the given detail message and root cause.
        /// </summary>
        /// <param name="message">
        ///     the detail message
        /// </param>
        /// <param name="cause">
        ///     the originating cause of this exception
        /// </param>
        public PropertyNotWritableException(string message, System.Exception cause) : base(message, cause)
        {
        }
    }
}
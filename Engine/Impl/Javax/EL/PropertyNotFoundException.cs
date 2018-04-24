 
 

namespace ESS.FW.Bpm.Engine.Impl.Javax.EL
{
    /// <summary>
    ///     Thrown when a property could not be found while evaluating a <seealso cref="ValueExpression" /> or
    ///     <seealso cref="MethodExpression" />. For example, this could be triggered by an index out of bounds while
    ///     setting an array value, or by an unreadable property while getting the value of a JavaBeans
    ///     property.
    /// </summary>
    public class PropertyNotFoundException : ELException
    {
        

        /// <summary>
        ///     Creates a PropertyNotFoundException with no detail message.
        /// </summary>
        public PropertyNotFoundException()
        {
        }

        /// <summary>
        ///     Creates a PropertyNotFoundException with the provided detail message.
        /// </summary>
        /// <param name="message">
        ///     the detail message
        /// </param>
        public PropertyNotFoundException(string message) : base(message)
        {
        }

        /// <summary>
        ///     Creates a PropertyNotFoundException with the given root cause.
        /// </summary>
        /// <param name="cause">
        ///     the originating cause of this exception
        /// </param>
        public PropertyNotFoundException(System.Exception cause) : base(cause)
        {
        }

        /// <summary>
        ///     Creates a PropertyNotFoundException with the given detail message and root cause.
        /// </summary>
        /// <param name="message">
        ///     the detail message
        /// </param>
        /// <param name="cause">
        ///     the originating cause of this exception
        /// </param>
        public PropertyNotFoundException(string message, System.Exception cause) : base(message, cause)
        {
        }
    }
}
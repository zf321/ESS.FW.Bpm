
 
 

namespace ESS.FW.Bpm.Engine.Impl.Javax.EL
{
    /// <summary>
    ///     Thrown when a method could not be found while evaluating a <seealso cref="MethodExpression" />.
    /// </summary>
    public class MethodNotFoundException : ELException
    {
        

        /// <summary>
        ///     Creates a MethodNotFoundException with no detail message.
        /// </summary>
        public MethodNotFoundException()
        {
        }

        /// <summary>
        ///     Creates a MethodNotFoundException with the provided detail message.
        /// </summary>
        /// <param name="message">
        ///     the detail message
        /// </param>
        public MethodNotFoundException(string message) : base(message)
        {
        }

        /// <summary>
        ///     Creates a MethodNotFoundException with the given root cause.
        /// </summary>
        /// <param name="cause">
        ///     the originating cause of this exception
        /// </param>
        public MethodNotFoundException(System.Exception cause) : base(cause)
        {
        }

        /// <summary>
        ///     Creates a MethodNotFoundException with the given detail message and root cause.
        /// </summary>
        /// <param name="message">
        ///     the detail message
        /// </param>
        /// <param name="cause">
        ///     the originating cause of this exception
        /// </param>
        public MethodNotFoundException(string message, System.Exception cause) : base(message, cause)
        {
        }
    }
}
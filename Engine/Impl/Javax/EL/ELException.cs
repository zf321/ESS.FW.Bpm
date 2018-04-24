
 

using System;

namespace ESS.FW.Bpm.Engine.Impl.Javax.EL
{
    /// <summary>
    ///     Represents any of the exception conditions that can arise during expression evaluation.
    /// </summary>
    public class ELException : ProcessEngineException
    {
        

        /// <summary>
        ///     Creates an ELException with no detail message.
        /// </summary>
        public ELException()
        {
        }

        /// <summary>
        ///     Creates an ELException with the provided detail message.
        /// </summary>
        /// <param name="message">
        ///     the detail message
        /// </param>
        public ELException(string message) : base(message)
        {
        }

        /// <summary>
        ///     Creates an ELException with the given cause.
        /// </summary>
        /// <param name="cause">
        ///     the originating cause of this exception
        /// </param>
        public ELException(System.Exception cause) : base((System.Exception) cause)
        {
        }

        /// <summary>
        ///     Creates an ELException with the given detail message and root cause.
        /// </summary>
        /// <param name="message">
        ///     the detail message
        /// </param>
        /// <param name="cause">
        ///     the originating cause of this exception
        /// </param>
        public ELException(string message, System.Exception cause) : base(message, cause)
        {
        }
    }
}
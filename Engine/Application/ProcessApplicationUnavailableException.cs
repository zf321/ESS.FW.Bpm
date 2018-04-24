using System;

namespace ESS.FW.Bpm.Engine.Application
{
    /// <summary>
    ///     <para>
    ///         Checked exception thrown by a <seealso cref="IProcessApplicationReference" /> if the referenced
    ///         process application is unavailable.
    ///     </para>
    ///     
    /// </summary>
    public class ProcessApplicationUnavailableException : System.Exception
    {
        private const long SerialVersionUid = 1L;

        public ProcessApplicationUnavailableException()
        {
        }

        public ProcessApplicationUnavailableException(string message, System.Exception cause) : base(message, cause)
        {
        }

        public ProcessApplicationUnavailableException(string message) : base(message)
        {
        }

        public ProcessApplicationUnavailableException(System.Exception cause) : base(cause.Message)
        {
        }
    }
}
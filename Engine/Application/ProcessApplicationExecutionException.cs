using System;

namespace ESS.FW.Bpm.Engine.Application
{
    /// <summary>
    ///     
    /// </summary>
    public class ProcessApplicationExecutionException : System.Exception
    {
        private const long SerialVersionUid = 1L;

        public ProcessApplicationExecutionException()
        {
        }

        public ProcessApplicationExecutionException(string message, System.Exception cause) : base(message, cause)
        {
        }

        public ProcessApplicationExecutionException(string message) : base(message)
        {
        }

        public ProcessApplicationExecutionException(System.Exception cause) : base(cause.Message)
        {
        }
    }
}
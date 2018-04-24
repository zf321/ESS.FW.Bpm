using System;
using ESS.FW.Bpm.Engine.Authorization;

namespace ESS.FW.Bpm.Engine.exception
{
    /// <summary>
    ///     <para>This exception is thrown, if an operation is not allowed to be executed.</para>
    ///     
    /// </summary>
    public class NotAllowedException : BadUserRequestException
    {
        

        public NotAllowedException()
        {
        }

        public NotAllowedException(string message, System.Exception cause) : base(message, cause)
        {
        }

        public NotAllowedException(string message) : base(message)
        {
        }

        public NotAllowedException(System.Exception cause) : base(cause)
        {
        }
    }
}
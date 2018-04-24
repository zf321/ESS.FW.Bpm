using System;
using ESS.FW.Bpm.Engine.Authorization;

namespace ESS.FW.Bpm.Engine.exception
{
    /// <summary>
    ///     <para>This exception is thrown, if a given value is not valid.</para>
    ///     
    /// </summary>
    public class NotValidException : BadUserRequestException
    {
        

        public NotValidException()
        {
        }

        public NotValidException(string message, System.Exception cause) : base(message, cause)
        {
        }

        public NotValidException(string message) : base(message)
        {
        }

        public NotValidException(System.Exception cause) : base(cause)
        {
        }
    }
}
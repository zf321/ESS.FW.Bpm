using System;
using ESS.FW.Bpm.Engine.Authorization;

namespace ESS.FW.Bpm.Engine.exception
{
    /// <summary>
    ///     <para>This exception is thrown, if an entity (case execution, case definition) is not found.</para>
    ///     
    /// </summary>
    public class NotFoundException : BadUserRequestException
    {
        

        public NotFoundException()
        {
        }

        public NotFoundException(string message, System.Exception cause) : base(message, cause)
        {
        }

        public NotFoundException(string message) : base(message)
        {
        }

        public NotFoundException(System.Exception cause) : base( cause)
        {
        }
    }
}
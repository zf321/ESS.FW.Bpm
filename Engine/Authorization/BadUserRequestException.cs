using System;

namespace ESS.FW.Bpm.Engine.Authorization
{
    /// <summary>
    ///     <para>
    ///         Exception resulting from a bad user request. A bad user request is
    ///         an interaction where the user requests some non-existing state or
    ///         attempts to perform an illegal action on some entity.
    ///     </para>
    ///     <para>
    ///         <strong>Examples:</strong>
    ///         <ul>
    ///             <li>cancelling a non-existing process instance</li>
    ///             <li>triggering a suspended execution...</li>
    ///         </ul>
    ///     </para>
    ///     
    /// </summary>
    public class BadUserRequestException : ProcessEngineException
    {
        private const long SerialVersionUid = 1L;

        public BadUserRequestException()
        {
        }

        public BadUserRequestException(string message, System.Exception cause) : base(message, cause)
        {
        }

        public BadUserRequestException(string message) : base(message)
        {
        }

        public BadUserRequestException(System.Exception cause) : base(cause)
        {
        }
    }
}
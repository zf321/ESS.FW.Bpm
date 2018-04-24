using ESS.FW.Bpm.Engine.exception;

namespace ESS.FW.Bpm.Engine
{
    /// <summary>
    ///     This exception is thrown, if an operation that requires a non-suspended entity (execution, task, process
    ///     definition)
    ///     is executed on a suspended one.
    ///     
    /// </summary>
    public class SuspendedEntityInteractionException : NotAllowedException
    {
        private const long SerialVersionUid = 1L;

        public SuspendedEntityInteractionException(string message) : base(message)
        {
        }
    }
}
namespace ESS.FW.Bpm.Engine
{
    /// <summary>
    ///     Exception that is thrown when an optimistic locking occurs in the datastore
    ///     caused by concurrent access of the same data entry.
    ///      
    ///     
    /// </summary>
    public class OptimisticLockingException : ProcessEngineException
    {
        private const long SerialVersionUid = 1L;

        public OptimisticLockingException(string message) : base(message)
        {
        }
    }
}
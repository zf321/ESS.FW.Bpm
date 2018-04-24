namespace ESS.FW.Bpm.Engine.exception.dmn
{
    /// <summary>
    ///     <para>This exception is thrown when a <seealso cref="DmnModelInstance" /> is not found.</para>
    /// </summary>
    public class DmnModelInstanceNotFoundException : DecisionException
    {
        

        public DmnModelInstanceNotFoundException()
        {
        }

        public DmnModelInstanceNotFoundException(string message, System.Exception cause) : base(message, cause)
        {
        }

        public DmnModelInstanceNotFoundException(string message) : base(message)
        {
        }

        public DmnModelInstanceNotFoundException(System.Exception cause) : base(cause)
        {
        }
    }
}
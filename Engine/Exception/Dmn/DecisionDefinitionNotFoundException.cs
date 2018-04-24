namespace ESS.FW.Bpm.Engine.exception.dmn
{
    /// <summary>
    ///     <para>This exception is thrown when a specific decision definition is not found.</para>
    /// </summary>
    public class DecisionDefinitionNotFoundException : DecisionException
    {
        

        public DecisionDefinitionNotFoundException()
        {
        }

        public DecisionDefinitionNotFoundException(string message, System.Exception cause) : base(message, cause)
        {
        }

        public DecisionDefinitionNotFoundException(string message) : base(message)
        {
        }

        public DecisionDefinitionNotFoundException(System.Exception cause) : base(cause)
        {
        }
    }
}
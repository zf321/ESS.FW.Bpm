using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine
{
    /// <summary>
    ///     
    /// </summary>
    public class MismatchingMessageCorrelationException : ProcessEngineException
    {
        private const long SerialVersionUid = 1L;

        public MismatchingMessageCorrelationException(string message) : base(message)
        {
        }

        public MismatchingMessageCorrelationException(string messageName, string reason)
            : this("Cannot correlate message '" + messageName + "': " + reason)
        {
        }

        public MismatchingMessageCorrelationException(string messageName, string businessKey,
            IDictionary<string, object> correlationKeys)
            : this(
                "Cannot correlate message '" + messageName + "' with process instance business key '" + businessKey +
                "' and correlation keys " + correlationKeys)
        {
        }

        public MismatchingMessageCorrelationException(string messageName, string businessKey,
            IDictionary<string, object> correlationKeys, string reason)
            : this(
                "Cannot correlate message '" + messageName + "' with process instance business key '" + businessKey +
                "' and correlation keys " + correlationKeys + ": " + reason)
        {
        }
    }
}
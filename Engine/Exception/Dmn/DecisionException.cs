using System;

namespace ESS.FW.Bpm.Engine.exception.dmn
{
    /// <summary>
    ///     <para>This exception is thrown when something happens related to a decision.</para>
    /// </summary>
    public class DecisionException : ProcessEngineException
    {
        

        public DecisionException()
        {
        }

        public DecisionException(string message, System.Exception cause) : base(message, cause)
        {
        }

        public DecisionException(string message) : base(message)
        {
        }

        public DecisionException(System.Exception cause) : base(cause)
        {
        }
    }
}
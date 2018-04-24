using System;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl
{
    /// <summary>
    ///     Exception throw for errors during the result creation of
    ///     a decision.
    /// </summary>
    public class DmnDecisionResultException : DmnEngineException
    {
        public DmnDecisionResultException(string message) : base(message)
        {
        }

        public DmnDecisionResultException(string message, System.Exception cause) : base(message, cause)
        {
        }
    }
}
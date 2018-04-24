using System;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl
{
    /// <summary>
    ///     Exception throw for errors during the evaluation of a decision.
    /// </summary>
    public class DmnEvaluationException : DmnEngineException
    {
        public DmnEvaluationException(string message, System.Exception cause) : base(message, cause)
        {
        }

        public DmnEvaluationException(string message) : base(message)
        {
        }
    }
}
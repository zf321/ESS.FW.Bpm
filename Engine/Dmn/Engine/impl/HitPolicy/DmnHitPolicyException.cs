using System;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl.hitpolicy
{
    /// <summary>
    ///     Exception throw if a hit policy is not applicable for a
    ///     decision result.
    /// </summary>
    public class DmnHitPolicyException : DmnEngineException
    {
        public DmnHitPolicyException(string message) : base(message)
        {
        }

        public DmnHitPolicyException(string message, System.Exception cause) : base(message, cause)
        {
        }
    }
}
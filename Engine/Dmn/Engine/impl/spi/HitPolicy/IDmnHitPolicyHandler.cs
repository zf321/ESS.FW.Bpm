using ESS.FW.Bpm.Engine.Dmn.engine.@delegate;
using ESS.FW.Bpm.Engine.Dmn.engine.impl.hitpolicy;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl.spi.hitpolicy
{
    /// <summary>
    ///     Handler for a DMN decision table hit policy.
    /// </summary>
    public interface IDmnHitPolicyHandler
    {
        /// <returns> the implemented hit policy and aggregator </returns>
        HitPolicyEntry HitPolicyEntry { get; }

        /// <summary>
        ///     Applies hit policy. Depending on the hit policy this can mean filtering and sorting of matching rules or
        ///     aggregating results.
        /// </summary>
        /// <param name="decisionTableEvaluationEvent"> the evaluation event of the decision table </param>
        /// <returns>
        ///     the final evaluation result
        /// </returns>
        /// <exception cref="DmnEngineException">
        ///     if the hit policy cannot be applied to the decision outputs
        /// </exception>
        IDmnDecisionTableEvaluationEvent apply(IDmnDecisionTableEvaluationEvent decisionTableEvaluationEvent);
    }
}
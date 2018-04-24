using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Dmn.engine.@delegate
{
    /// <summary>
    ///     Event which represents the evaluation of a decision
    /// </summary>
    public interface IDmnDecisionEvaluationEvent
    {
        /// <returns> the result of the evaluated decision </returns>
        IDmnDecisionLogicEvaluationEvent DecisionResult { get; }

        /// <returns> the collection of required decision results </returns>
        ICollection<IDmnDecisionLogicEvaluationEvent> RequiredDecisionResults { get; }

        /// <returns> the number of executed decision elements during the evaluation </returns>
        long ExecutedDecisionElements { get; }
    }
}
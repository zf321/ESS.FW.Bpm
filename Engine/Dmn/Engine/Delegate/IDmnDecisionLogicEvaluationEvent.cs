namespace ESS.FW.Bpm.Engine.Dmn.engine.@delegate
{
    /// <summary>
    ///     Event which represents the evaluation of a decision.
    /// </summary>
    public interface IDmnDecisionLogicEvaluationEvent
    {
        /// <returns> the evaluated decision </returns>
        IDmnDecision Decision { get; }

        /// <returns> the number of executed decision elements during the evaluation </returns>
        long ExecutedDecisionElements { get; }
    }
}
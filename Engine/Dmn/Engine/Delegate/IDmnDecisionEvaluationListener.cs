namespace ESS.FW.Bpm.Engine.Dmn.engine.@delegate
{
    /// <summary>
    ///     A listener which will be notified after a decision
    ///     was evaluated.
    /// </summary>
    public interface IDmnDecisionEvaluationListener
    {
        /// <summary>
        ///     Will be called after a decision was evaluated.
        /// </summary>
        /// <param name="evaluationEvent"> the evaluation event </param>
        void Notify(IDmnDecisionEvaluationEvent evaluationEvent);
    }
}
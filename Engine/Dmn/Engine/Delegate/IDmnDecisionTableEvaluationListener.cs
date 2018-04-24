namespace ESS.FW.Bpm.Engine.Dmn.engine.@delegate
{
    /// <summary>
    ///     A listener which will be notified after a decision table
    ///     was evaluated.
    /// </summary>
    public interface IDmnDecisionTableEvaluationListener
    {
        /// <summary>
        ///     Will be called after a decision table was evaluated.
        /// </summary>
        /// <param name="evaluationEvent"> the evaluation event </param>
        void Notify(IDmnDecisionTableEvaluationEvent evaluationEvent);
    }
}
using ESS.FW.Bpm.Engine.Dmn.engine.@delegate;

namespace ESS.FW.Bpm.Engine.Dmn.engine.spi
{
    /// <summary>
    ///     DMN engine metric collector which records the executed decision elements
    ///     since since its creation.
    /// </summary>
    public interface IDmnEngineMetricCollector : IDmnDecisionTableEvaluationListener
    {
        /// <returns> the number of executed decision elements since creation of this engine </returns>
        long ExecutedDecisionElements { get; }

        /// <summary>
        ///     Resets the executed decision elements to 0.
        /// </summary>
        /// <returns> the number of executed decision elements before resetting. </returns>
        long ClearExecutedDecisionElements();
    }
}
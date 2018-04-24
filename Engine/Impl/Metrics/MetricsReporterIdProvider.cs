namespace ESS.FW.Bpm.Engine.Impl.Metrics
{
    /// <summary>
    ///     
    /// </summary>
    public interface IMetricsReporterIdProvider
    {
        /// <summary>
        ///     Provides an id that identifies the metrics reported as part of the given engine's
        ///     process execution. May return null.
        /// </summary>
        string ProvideId(IProcessEngine processEngine);
    }
}
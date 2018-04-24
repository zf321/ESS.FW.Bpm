namespace ESS.FW.Bpm.Engine.Impl.Metrics
{
    /// <summary>
    ///     
    /// </summary>
    public class MetricsLogger : ProcessEngineLogger
    {
        public virtual void CouldNotDetermineIp(System.Exception e)
        {
            LogWarn("001", "Could not determine local IP address for generating an engine id", e);
        }

        /// <param name="e"> </param>
        public virtual void CouldNotCollectAndLogMetrics(System.Exception e)
        {
            LogWarn("002", "Could not collect and log metrics", e);
        }
    }
}
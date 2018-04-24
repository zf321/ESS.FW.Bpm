using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Delegate;

namespace ESS.FW.Bpm.Engine.Impl.Metrics.Parser
{
    /// <summary>
    ///     
    /// </summary>
    public class MetricsExecutionListener : IDelegateListener<IBaseDelegateExecution>
    {
        protected internal string MetricsName;

        public MetricsExecutionListener(string metricsName)
        {
            this.MetricsName = metricsName;
        }
        
        public virtual void Notify(IBaseDelegateExecution execution)
        {
            Context.ProcessEngineConfiguration.MetricsRegistry.MarkOccurrence(MetricsName);
        }
    }
}
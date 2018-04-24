using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Delegate;

namespace ESS.FW.Bpm.Engine.Impl.Metrics.Parser
{
    /// <summary>
    ///     
    /// </summary>
    public class MetricsCaseExecutionListener : ICaseExecutionListener
    {
        public virtual void Notify(IDelegateCaseExecution caseExecution)
        {
            Context.ProcessEngineConfiguration.MetricsRegistry.MarkOccurrence(Engine.Management.Metrics.ActivtyInstanceStart);
        }
    }
}
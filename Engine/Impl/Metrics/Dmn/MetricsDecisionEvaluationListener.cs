using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Dmn.engine.@delegate;

namespace ESS.FW.Bpm.Engine.Impl.Metrics.Dmn
{
    public class MetricsDecisionEvaluationListener : IDmnDecisionEvaluationListener
    {
        public virtual void Notify(IDmnDecisionEvaluationEvent evaluationEvent)
        {
            var processEngineConfiguration = Context.ProcessEngineConfiguration;

            if ((processEngineConfiguration != null) && processEngineConfiguration.MetricsEnabled)
                processEngineConfiguration.MetricsRegistry.MarkOccurrence(Engine.Management.Metrics.ExecutedDecisionElements,
                    evaluationEvent.ExecutedDecisionElements);
        }
    }
}
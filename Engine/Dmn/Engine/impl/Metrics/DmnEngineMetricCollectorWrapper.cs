using ESS.FW.Bpm.Engine.Dmn.engine.@delegate;
using ESS.FW.Bpm.Engine.Dmn.engine.spi;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl.metrics
{
    public class DmnEngineMetricCollectorWrapper : IDmnEngineMetricCollector, IDmnDecisionEvaluationListener
    {
        protected internal readonly IDmnEngineMetricCollector collector;

        public DmnEngineMetricCollectorWrapper(IDmnEngineMetricCollector collector)
        {
            this.collector = collector;
        }

        public virtual void Notify(IDmnDecisionEvaluationEvent evaluationEvent)
        {
            notifyCollector(evaluationEvent.DecisionResult);

            foreach (var @event in evaluationEvent.RequiredDecisionResults)
                notifyCollector(@event);
        }

        public virtual void Notify(IDmnDecisionTableEvaluationEvent evaluationEvent)
        {
            // the wrapper listen for decision evaluation events
        }

        public virtual long ExecutedDecisionElements
        {
            get { return collector.ExecutedDecisionElements; }
        }

        public virtual long ClearExecutedDecisionElements()
        {
            return collector.ClearExecutedDecisionElements();
        }

        protected internal virtual void notifyCollector(IDmnDecisionLogicEvaluationEvent evaluationEvent)
        {
            if (evaluationEvent is IDmnDecisionTableEvaluationEvent)
                collector.Notify((IDmnDecisionTableEvaluationEvent) evaluationEvent);
            // ignore other evaluation events since the collector is implemented as decision table evaluation listener
        }
    }
}
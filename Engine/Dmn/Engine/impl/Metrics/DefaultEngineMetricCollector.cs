using System;
using ESS.FW.Bpm.Engine.Dmn.engine.@delegate;
using ESS.FW.Bpm.Engine.Dmn.engine.spi;
using ESS.FW.Bpm.Engine.Common;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl.metrics
{
    public class DefaultEngineMetricCollector : IDmnEngineMetricCollector, IDmnDecisionEvaluationListener
    {
        protected AtomicLong executedDecisionElements = new AtomicLong();
        public void Notify(IDmnDecisionEvaluationEvent evaluationEvent)
        {
            var executedDecisionElements = evaluationEvent.ExecutedDecisionElements;
            this.executedDecisionElements.GetAndAdd(executedDecisionElements);
        }

        public virtual void Notify(IDmnDecisionTableEvaluationEvent evaluationEvent)
        {
            // collector is registered as decision evaluation listener
        }

        public virtual long ExecutedDecisionElements
        {
            get { return executedDecisionElements.GetValue(); }
        }

        public virtual long ClearExecutedDecisionElements()
        {
            return executedDecisionElements.GetAndSet(0);
        }

    }
}
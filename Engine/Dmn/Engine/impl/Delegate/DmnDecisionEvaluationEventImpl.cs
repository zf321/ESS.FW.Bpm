using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Dmn.engine.@delegate;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl.@delegate
{
    public class DmnDecisionEvaluationEventImpl : IDmnDecisionEvaluationEvent
    {
        protected internal IDmnDecisionLogicEvaluationEvent decisionResult;
        protected internal long executedDecisionElements;

        protected internal ICollection<IDmnDecisionLogicEvaluationEvent> requiredDecisionResults =
            new List<IDmnDecisionLogicEvaluationEvent>();

        public virtual IDmnDecisionLogicEvaluationEvent DecisionResult
        {
            get { return decisionResult; }
            set { decisionResult = value; }
        }


        public virtual ICollection<IDmnDecisionLogicEvaluationEvent> RequiredDecisionResults
        {
            get { return requiredDecisionResults; }
            set { requiredDecisionResults = value; }
        }


        public virtual long ExecutedDecisionElements
        {
            get { return executedDecisionElements; }
            set { executedDecisionElements = value; }
        }


        public override string ToString()
        {
            var dmnDecision = decisionResult.Decision;
            return "DmnDecisionEvaluationEventImpl{" + " key=" + dmnDecision.Key + ", name=" + dmnDecision.Name +
                   ", decisionLogic=" + dmnDecision.DecisionLogic + ", requiredDecisionResults=" +
                   requiredDecisionResults + ", executedDecisionElements=" + executedDecisionElements + '}';
        }
    }
}
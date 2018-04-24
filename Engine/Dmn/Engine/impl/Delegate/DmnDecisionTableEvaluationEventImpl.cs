using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Dmn.engine.@delegate;
using ESS.FW.Bpm.Engine.Variable.Type;
using ESS.FW.Bpm.Engine.Variable.Value;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl.@delegate
{
    public class DmnDecisionTableEvaluationEventImpl : IDmnDecisionTableEvaluationEvent
    {
        protected internal string collectResultName;
        protected internal ITypedValue collectResultValue;

        protected internal IDmnDecision decision;
        protected internal long executedDecisionElements;
        protected internal IList<IDmnEvaluatedInput> inputs = new List<IDmnEvaluatedInput>();
        protected internal IList<IDmnEvaluatedDecisionRule> matchingRules = new List<IDmnEvaluatedDecisionRule>();

        public virtual IDmnDecision DecisionTable
        {
            get { return Decision; }
            set { decision = value; }
        }

        public virtual IDmnDecision Decision
        {
            get { return decision; }
        }


        public virtual IList<IDmnEvaluatedInput> Inputs
        {
            get { return inputs; }
            set { inputs = value; }
        }


        public virtual IList<IDmnEvaluatedDecisionRule> MatchingRules
        {
            get { return matchingRules; }
            set { matchingRules = value; }
        }


        public virtual string CollectResultName
        {
            get { return collectResultName; }
            set { collectResultName = value; }
        }


        public virtual ITypedValue CollectResultValue
        {
            get { return collectResultValue; }
            set { collectResultValue = value; }
        }


        public virtual long ExecutedDecisionElements
        {
            get { return executedDecisionElements; }
            set { executedDecisionElements = value; }
        }


        public override string ToString()
        {
            return "DmnDecisionTableEvaluationEventImpl{" + " key=" + decision.Key + ", name=" + decision.Name +
                   ", decisionLogic=" + decision.DecisionLogic + ", inputs=" + inputs + ", matchingRules=" +
                   matchingRules + ", collectResultName='" + collectResultName + '\'' + ", collectResultValue=" +
                   collectResultValue + ", executedDecisionElements=" + executedDecisionElements + '}';
        }
    }
}
using System;
using ESS.FW.Bpm.Engine.Dmn.engine.@delegate;
using ESS.FW.Bpm.Engine.Variable.Type;
using ESS.FW.Bpm.Engine.Variable.Value;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl.@delegate
{
    public class DmnDecisionLiteralExpressionEvaluationEventImpl : IDmnDecisionLiteralExpressionEvaluationEvent
    {
        protected internal IDmnDecision decision;

        protected internal long executedDecisionElements;

        protected internal string outputName;
        protected internal ITypedValue outputValue;

        public virtual IDmnDecision Decision
        {
            get { return decision; }
            set { decision = value; }
        }


        public virtual string OutputName
        {
            get { return outputName; }
            set { outputName = value; }
        }


        public virtual ITypedValue OutputValue
        {
            get { return outputValue; }
            set { outputValue = value; }
        }


        public virtual long ExecutedDecisionElements
        {
            get { return executedDecisionElements; }
            set { executedDecisionElements = value; }
        }


        public override string ToString()
        {
            return "DmnDecisionLiteralExpressionEvaluationEventImpl [" + " key=" + decision.Key + ", name=" +
                   decision.Name + ", decisionLogic=" + decision.DecisionLogic + ", outputName=" + outputName +
                   ", outputValue=" + outputValue + ", executedDecisionElements=" + executedDecisionElements + "]";
        }
    }
}
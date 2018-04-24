using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Dmn.engine.@delegate;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl.@delegate
{
    public class DmnEvaluatedDecisionRuleImpl : IDmnEvaluatedDecisionRule
    {
        protected internal string id;
        protected internal IDictionary<string, IDmnEvaluatedOutput> outputEntries;

        public DmnEvaluatedDecisionRuleImpl(DmnDecisionTableRuleImpl matchingRule)
        {
            id = matchingRule.Id;
        }

        public virtual string Id
        {
            get { return id; }
            set { id = value; }
        }


        public virtual IDictionary<string, IDmnEvaluatedOutput> OutputEntries
        {
            get { return outputEntries; }
            set { outputEntries = value; }
        }


        public override string ToString()
        {
            return "DmnEvaluatedDecisionRuleImpl{" + "id='" + id + '\'' + ", outputEntries=" + outputEntries + '}';
        }
    }
}
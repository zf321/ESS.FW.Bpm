using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Dmn.engine.@delegate;
using ESS.FW.Bpm.Engine.Dmn.engine.impl.@delegate;
using ESS.FW.Bpm.Engine.Dmn.engine.impl.spi.hitpolicy;
using ESS.FW.Bpm.Model.Dmn;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl.hitpolicy
{
    public class FirstHitPolicyHandler : IDmnHitPolicyHandler
    {
        protected internal static readonly HitPolicyEntry HIT_POLICY = new HitPolicyEntry(HitPolicy.First,
            BuiltinAggregator.COUNT);

        public virtual IDmnDecisionTableEvaluationEvent apply(
            IDmnDecisionTableEvaluationEvent decisionTableEvaluationEvent)
        {
            if (decisionTableEvaluationEvent.MatchingRules.Count > 0)
            {
                var firstMatchedRule = decisionTableEvaluationEvent.MatchingRules[0];
                ((DmnDecisionTableEvaluationEventImpl) decisionTableEvaluationEvent).MatchingRules =
                    new List<IDmnEvaluatedDecisionRule> {firstMatchedRule};
                //Collections.singletonList(firstMatchedRule);
            }
            return decisionTableEvaluationEvent;
        }

        public virtual HitPolicyEntry HitPolicyEntry
        {
            get { return HIT_POLICY; }
        }

        public override string ToString()
        {
            return "FirstHitPolicyHandler{}";
        }
    }
}
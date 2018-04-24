using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Dmn.engine.@delegate;
using ESS.FW.Bpm.Engine.Dmn.engine.impl.@delegate;
using ESS.FW.Bpm.Engine.Dmn.engine.impl.spi.hitpolicy;
using ESS.FW.Bpm.Model.Dmn;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl.hitpolicy
{
    public class AnyHitPolicyHandler : IDmnHitPolicyHandler
    {
        public static readonly DmnHitPolicyLogger LOG = DmnLogger.HIT_POLICY_LOGGER;

        protected internal static readonly HitPolicyEntry HIT_POLICY = new HitPolicyEntry(HitPolicy.Any,
            BuiltinAggregator.COUNT);

        public virtual HitPolicyEntry HitPolicyEntry
        {
            get { return HIT_POLICY; }
        }

        public virtual IDmnDecisionTableEvaluationEvent apply(
            IDmnDecisionTableEvaluationEvent decisionTableEvaluationEvent)
        {
            var matchingRules = decisionTableEvaluationEvent.MatchingRules;

            if (matchingRules.Count > 0)
                if (allOutputsAreEqual(matchingRules))
                {
                    var firstMatchingRule = matchingRules[0];
                    ((DmnDecisionTableEvaluationEventImpl) decisionTableEvaluationEvent).MatchingRules =
                        new List<IDmnEvaluatedDecisionRule> {firstMatchingRule};
                    //Collections.singletonList(firstMatchingRule);
                }
                else
                {
                    throw LOG.anyHitPolicyRequiresThatAllOutputsAreEqual(matchingRules);
                }

            return decisionTableEvaluationEvent;
        }

        protected internal virtual bool allOutputsAreEqual(IList<IDmnEvaluatedDecisionRule> matchingRules)
        {
            var firstOutputEntries = matchingRules[0].OutputEntries;
            if (firstOutputEntries == null)
            {
                for (var i = 1; i < matchingRules.Count; i++)
                    if (matchingRules[i].OutputEntries != null)
                        return false;
            }
            else
            {
                for (var i = 1; i < matchingRules.Count; i++)
                    if (!firstOutputEntries.Equals(matchingRules[i].OutputEntries))
                        return false;
            }
            return true;
        }

        public override string ToString()
        {
            return "AnyHitPolicyHandler{}";
        }
    }
}
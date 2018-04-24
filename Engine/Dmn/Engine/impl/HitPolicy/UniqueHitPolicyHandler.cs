using ESS.FW.Bpm.Engine.Dmn.engine.@delegate;
using ESS.FW.Bpm.Engine.Dmn.engine.impl.spi.hitpolicy;
using ESS.FW.Bpm.Model.Dmn;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl.hitpolicy
{
    public class UniqueHitPolicyHandler : IDmnHitPolicyHandler
    {
        public static readonly DmnHitPolicyLogger LOG = DmnLogger.HIT_POLICY_LOGGER;

        protected internal static readonly HitPolicyEntry HIT_POLICY = new HitPolicyEntry(HitPolicy.Unique,
            BuiltinAggregator.COUNT);

        public virtual IDmnDecisionTableEvaluationEvent apply(
            IDmnDecisionTableEvaluationEvent decisionTableEvaluationEvent)
        {
            var matchingRules = decisionTableEvaluationEvent.MatchingRules;

            if (matchingRules.Count < 2)
                return decisionTableEvaluationEvent;
            throw LOG.uniqueHitPolicyOnlyAllowsSingleMatchingRule(matchingRules);
        }

        public virtual HitPolicyEntry HitPolicyEntry
        {
            get { return HIT_POLICY; }
        }

        public override string ToString()
        {
            return "UniqueHitPolicyHandler{}";
        }
    }
}
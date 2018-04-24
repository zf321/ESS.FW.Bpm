using ESS.FW.Bpm.Engine.Dmn.engine.@delegate;
using ESS.FW.Bpm.Engine.Dmn.engine.impl.spi.hitpolicy;
using ESS.FW.Bpm.Model.Dmn;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl.hitpolicy
{
    public class RuleOrderHitPolicyHandler : IDmnHitPolicyHandler
    {
        protected internal static readonly HitPolicyEntry HIT_POLICY = new HitPolicyEntry(HitPolicy.RuleOrder,
            BuiltinAggregator.COUNT);

        public virtual HitPolicyEntry HitPolicyEntry
        {
            get { return HIT_POLICY; }
        }

        public virtual IDmnDecisionTableEvaluationEvent apply(
            IDmnDecisionTableEvaluationEvent decisionTableEvaluationEvent)
        {
            return decisionTableEvaluationEvent;
        }

        public override string ToString()
        {
            return "RuleOrderHitPolicyHandler{}";
        }
    }
}
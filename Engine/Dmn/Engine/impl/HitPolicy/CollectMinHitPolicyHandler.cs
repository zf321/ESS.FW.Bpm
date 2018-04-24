using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Model.Dmn;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl.hitpolicy
{
    public class CollectMinHitPolicyHandler : AbstractCollectNumberHitPolicyHandler
    {
        protected internal static readonly HitPolicyEntry HIT_POLICY = new HitPolicyEntry(HitPolicy.Collect,
            BuiltinAggregator.MIN);

        public override HitPolicyEntry HitPolicyEntry
        {
            get { return HIT_POLICY; }
        }

        protected internal override BuiltinAggregator Aggregator
        {
            get { return BuiltinAggregator.MIN; }
        }

        protected internal override int? aggregateIntegerValues(IList<int> intValues)
        {
            return intValues.Min();
        }

        protected internal override long? aggregateLongValues(IList<long> longValues)
        {
            return longValues.Min();
        }

        protected internal override double? aggregateDoubleValues(IList<double> doubleValues)
        {
            return doubleValues.Min();
        }

        public override string ToString()
        {
            return "CollectMinHitPolicyHandler{}";
        }
    }
}
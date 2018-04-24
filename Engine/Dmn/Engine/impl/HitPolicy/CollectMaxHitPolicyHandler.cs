using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Model.Dmn;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl.hitpolicy
{
    public class CollectMaxHitPolicyHandler : AbstractCollectNumberHitPolicyHandler
    {
        protected internal static readonly HitPolicyEntry HIT_POLICY = new HitPolicyEntry(HitPolicy.Collect,
            BuiltinAggregator.MAX);

        public override HitPolicyEntry HitPolicyEntry
        {
            get { return HIT_POLICY; }
        }

        protected internal override BuiltinAggregator Aggregator
        {
            get { return BuiltinAggregator.MAX; }
        }

        protected internal override int? aggregateIntegerValues(IList<int> intValues)
        {
            return intValues.Max();
        }

        protected internal override long? aggregateLongValues(IList<long> longValues)
        {
            return longValues.Max();
        }

        protected internal override double? aggregateDoubleValues(IList<double> doubleValues)
        {
            return doubleValues.Max();
        }

        public override string ToString()
        {
            return "CollectMaxHitPolicyHandler{}";
        }
    }
}
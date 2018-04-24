using System.Collections.Generic;
using ESS.FW.Bpm.Model.Dmn;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl.hitpolicy
{
    public class CollectSumHitPolicyHandler : AbstractCollectNumberHitPolicyHandler
    {
        protected internal static readonly HitPolicyEntry HIT_POLICY = new HitPolicyEntry(HitPolicy.Collect,
            BuiltinAggregator.SUM);

        public override HitPolicyEntry HitPolicyEntry
        {
            get { return HIT_POLICY; }
        }

        protected internal override BuiltinAggregator Aggregator
        {
            get { return BuiltinAggregator.SUM; }
        }

        protected internal override int? aggregateIntegerValues(IList<int> intValues)
        {
            var sum = 0;
            foreach (int? intValue in intValues)
                if (intValue != null)
                    sum += intValue.Value;
            return sum;
        }

        protected internal override long? aggregateLongValues(IList<long> longValues)
        {
            var sum = 0L;
            foreach (long? longValue in longValues)
                if (longValue != null)
                    sum += longValue.Value;
            return sum;
        }

        protected internal override double? aggregateDoubleValues(IList<double> doubleValues)
        {
            var sum = 0.0;
            foreach (double? doubleValue in doubleValues)
                if (doubleValue != null)
                    sum += doubleValue.Value;
            return sum;
        }

        public override string ToString()
        {
            return "CollectSumHitPolicyHandler{}";
        }
    }
}
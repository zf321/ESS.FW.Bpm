using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Variable;
using ESS.FW.Bpm.Engine.Variable.Type;
using ESS.FW.Bpm.Engine.Variable.Value;
using ESS.FW.Bpm.Model.Dmn;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl.hitpolicy
{
    public class CollectCountHitPolicyHandler : AbstractCollectNumberHitPolicyHandler
    {
        protected internal static readonly HitPolicyEntry HIT_POLICY = new HitPolicyEntry(HitPolicy.Collect,
            BuiltinAggregator.COUNT);

        public override HitPolicyEntry HitPolicyEntry
        {
            get { return HIT_POLICY; }
        }

        protected internal override BuiltinAggregator Aggregator
        {
            get { return BuiltinAggregator.COUNT; }
        }

        protected internal override ITypedValue aggregateValues(IList<ITypedValue> values)
        {
            return Variables.IntegerValue(values.Count);
            //return 0;
        }

        protected internal override int? aggregateIntegerValues(IList<int> intValues)
        {
            // not used
            return 0;
        }

        protected internal override long? aggregateLongValues(IList<long> longValues)
        {
            // not used
            return 0L;
        }

        protected internal override double? aggregateDoubleValues(IList<double> doubleValues)
        {
            // not used
            return 0.0;
        }

        public override string ToString()
        {
            return "CollectCountHitPolicyHandler{}";
        }
    }
}


using ESS.FW.Bpm.Model.Dmn;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl.hitpolicy
{
    /// <summary>
    ///     Represents the hit policy and the aggregator of a decision table.
    ///     
    /// </summary>
    public class HitPolicyEntry
    {
        protected internal readonly BuiltinAggregator aggregator;

        protected internal readonly HitPolicy hitPolicy;

        /// <summary>
        ///     决策表相关，
        /// </summary>
        /// <param name="hitPolicy"></param>
        /// <param name="builtinAggregator"></param>
        public HitPolicyEntry(HitPolicy hitPolicy, BuiltinAggregator builtinAggregator)
        {
            this.hitPolicy = hitPolicy;
            aggregator = builtinAggregator;
        }

        public virtual HitPolicy HitPolicy
        {
            get { return hitPolicy; }
        }

        public virtual BuiltinAggregator Aggregator
        {
            get { return aggregator; }
        }

        public override bool Equals(object o)
        {
            if (this == o)
                return true;
            if ((o == null) || (GetType() != o.GetType()))
                return false;

            var that = (HitPolicyEntry) o;

            if (hitPolicy != that.hitPolicy)
                return false;
            return aggregator == that.aggregator;
        }

        public override int GetHashCode()
        {
            var result = hitPolicy != null ? hitPolicy.GetHashCode() : 0;
            result = 31*result + (aggregator != null ? aggregator.GetHashCode() : 0);
            return result;
        }
    }
}


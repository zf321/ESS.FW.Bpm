using ESS.FW.Bpm.Model.Dmn;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl.spi.hitpolicy
{
    /// <summary>
    ///     Registry of hit policy handlers
    /// </summary>
    public interface IDmnHitPolicyHandlerRegistry
    {
        /// <summary>
        ///     Get a hit policy for a <seealso cref="HitPolicy" /> and <seealso cref="BuiltinAggregator" /> combination.
        /// </summary>
        /// <param name="hitPolicy"> the hit policy </param>
        /// <param name="builtinAggregator"> the aggregator or null if not required </param>
        /// <returns> the handler which is registered for this hit policy, or null if none exist </returns>
        IDmnHitPolicyHandler getHandler(HitPolicy hitPolicy, BuiltinAggregator builtinAggregator);

        /// <summary>
        ///     Register a hit policy handler for a <seealso cref="HitPolicy" /> and <seealso cref="BuiltinAggregator" />
        ///     combination.
        /// </summary>
        /// <param name="hitPolicy"> the hit policy </param>
        /// <param name="builtinAggregator"> the aggregator or null if not required </param>
        /// <param name="hitPolicyHandler"> the hit policy handler to registry </param>
        void addHandler(HitPolicy hitPolicy, BuiltinAggregator builtinAggregator, IDmnHitPolicyHandler hitPolicyHandler);
    }
}
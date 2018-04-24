using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Dmn.engine.impl.spi.hitpolicy;
using ESS.FW.Bpm.Model.Dmn;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl.hitpolicy
{
    public class DefaultHitPolicyHandlerRegistry : IDmnHitPolicyHandlerRegistry
    {
        protected internal static readonly IDictionary<HitPolicyEntry, IDmnHitPolicyHandler> handlers = DefaultHandlers;

        protected internal static IDictionary<HitPolicyEntry, IDmnHitPolicyHandler> DefaultHandlers
        {
            get
            {
                IDictionary<HitPolicyEntry, IDmnHitPolicyHandler> handlers =
                    new Dictionary<HitPolicyEntry, IDmnHitPolicyHandler>();
                //null默认为BuiltinAggregator.Sum
                handlers[new HitPolicyEntry(HitPolicy.Unique, BuiltinAggregator.SUM)] = new UniqueHitPolicyHandler();
                handlers[new HitPolicyEntry(HitPolicy.First, BuiltinAggregator.SUM)] = new FirstHitPolicyHandler();
                handlers[new HitPolicyEntry(HitPolicy.Any, BuiltinAggregator.SUM)] = new AnyHitPolicyHandler();
                handlers[new HitPolicyEntry(HitPolicy.RuleOrder, BuiltinAggregator.SUM)] =
                    new RuleOrderHitPolicyHandler();
                handlers[new HitPolicyEntry(HitPolicy.Collect, BuiltinAggregator.SUM)] = new CollectHitPolicyHandler();

                handlers[new HitPolicyEntry(HitPolicy.Collect, BuiltinAggregator.COUNT)] =
                    new CollectCountHitPolicyHandler();
                handlers[new HitPolicyEntry(HitPolicy.Collect, BuiltinAggregator.SUM)] =
                    new CollectSumHitPolicyHandler();
                handlers[new HitPolicyEntry(HitPolicy.Collect, BuiltinAggregator.MIN)] =
                    new CollectMinHitPolicyHandler();
                handlers[new HitPolicyEntry(HitPolicy.Collect, BuiltinAggregator.MAX)] =
                    new CollectMaxHitPolicyHandler();

                return handlers;
            }
        }

        //TODO hitPolicy为null 默认会给HitPolicy.Unique ，BuiltinAggregator默认值SUM,找不到key改为返回Null
        public virtual IDmnHitPolicyHandler getHandler(HitPolicy hitPolicy, BuiltinAggregator builtinAggregator)
        {
            if (handlers.ContainsKey(new HitPolicyEntry(hitPolicy, builtinAggregator)))
                return handlers[new HitPolicyEntry(hitPolicy, builtinAggregator)];
            return null;
            //return handlers[new HitPolicyEntry(hitPolicy, builtinAggregator)];
        }

        public virtual void addHandler(HitPolicy hitPolicy, BuiltinAggregator builtinAggregator,
            IDmnHitPolicyHandler hitPolicyHandler)
        {
            handlers[new HitPolicyEntry(hitPolicy, builtinAggregator)] = hitPolicyHandler;
        }

    }
}
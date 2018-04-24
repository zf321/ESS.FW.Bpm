using ESS.FW.Bpm.Engine.Dmn.engine.impl.spi.hitpolicy;
using ESS.FW.Bpm.Engine.Dmn.engine.impl.spi.transform;
using ESS.FW.Bpm.Model.Dmn;
using ESS.FW.Bpm.Model.Dmn.instance;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl.transform
{
    public class DmnDecisionTableTransformHandler : IDmnElementTransformHandler<IDecisionTable, DmnDecisionTableImpl>
    {
        protected internal static readonly DmnTransformLogger Log = DmnLogger.TRANSFORM_LOGGER;

        public virtual DmnDecisionTableImpl HandleElement(IDmnElementTransformContext context,
            IDecisionTable decisionTable)
        {
            return CreateFromDecisionTable(context, decisionTable);
        }

        protected internal virtual DmnDecisionTableImpl CreateFromDecisionTable(IDmnElementTransformContext context,
            IDecisionTable decisionTable)
        {
            var dmnDecisionTable = CreateDmnElement(context, decisionTable);
            //TODO dmnDecisionTable.HitPolicyHandler赋值
            dmnDecisionTable.HitPolicyHandler = getHitPolicyHandler(context, decisionTable, dmnDecisionTable);

            return dmnDecisionTable;
        }

        protected internal virtual DmnDecisionTableImpl CreateDmnElement(IDmnElementTransformContext context,
            IDecisionTable decisionTable)
        {
            //TODO 坑 创建了空对象DmnDecisionTableImpl
            return new DmnDecisionTableImpl();
        }

        //TODO 决策表规则解析 约束决策表所有规则的 and或or等关系 aggregation默认为SUM问题待解决(可以为null)
        protected internal virtual IDmnHitPolicyHandler getHitPolicyHandler(IDmnElementTransformContext context,
            IDecisionTable decisionTable, DmnDecisionTableImpl dmnDecisionTable)
        {
            var hitPolicy = decisionTable.HitPolicy;
            if (hitPolicy == null)
                hitPolicy = HitPolicy.Unique;
            var aggregation = decisionTable.Aggregation;
            var hitPolicyHandler = context.HitPolicyHandlerRegistry.getHandler(hitPolicy, aggregation);
            if (hitPolicyHandler != null)
                return hitPolicyHandler;
            //return null;
            throw Log.HitPolicyNotSupported(dmnDecisionTable, hitPolicy, aggregation);
        }
    }
}
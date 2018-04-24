using ESS.FW.Bpm.Engine.Dmn.engine.impl.spi.transform;
using ESS.FW.Bpm.Model.Dmn.instance;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl.transform
{
    public class DmnDecisionTableRuleTransformHandler : IDmnElementTransformHandler<IRule, DmnDecisionTableRuleImpl>
    {
        public virtual DmnDecisionTableRuleImpl HandleElement(IDmnElementTransformContext context, IRule rule)
        {
            return CreateFromRule(context, rule);
        }

        protected internal virtual DmnDecisionTableRuleImpl CreateFromRule(IDmnElementTransformContext context,
            IRule rule)
        {
            var decisionTableRule = CreateDmnElement(context, rule);

            decisionTableRule.Id = rule.Id;
            decisionTableRule.Name = rule.Label;

            return decisionTableRule;
        }

        protected internal virtual DmnDecisionTableRuleImpl CreateDmnElement(IDmnElementTransformContext context,
            IRule rule)
        {
            return new DmnDecisionTableRuleImpl();
        }
    }
}
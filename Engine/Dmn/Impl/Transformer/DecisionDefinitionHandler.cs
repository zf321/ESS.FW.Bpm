using ESS.FW.Bpm.Engine.Dmn.engine.impl;
using ESS.FW.Bpm.Engine.Dmn.engine.impl.spi.transform;
using ESS.FW.Bpm.Engine.Dmn.engine.impl.transform;
using ESS.FW.Bpm.Engine.Dmn.Impl.Entity.Repository;
using ESS.FW.Bpm.Model.Dmn.instance;

namespace ESS.FW.Bpm.Engine.Dmn.Impl.Transformer
{
    public class DecisionDefinitionHandler : DmnDecisionTransformHandler
    {
        protected internal override DmnDecisionImpl CreateDmnElement()
        {
            return new DecisionDefinitionEntity();
        }

        protected internal override DmnDecisionImpl CreateFromDecision(IDmnElementTransformContext context,
            IDecision decision)
        {
            var decisionDefinition = (DecisionDefinitionEntity)base.CreateFromDecision(context, decision);

            string category = context.ModelInstance.Definitions.Namespace;
            decisionDefinition.Category = category;

            return decisionDefinition;
        }
    }
}
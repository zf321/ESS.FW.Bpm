using ESS.FW.Bpm.Engine.Dmn.engine.impl;
using ESS.FW.Bpm.Engine.Dmn.engine.impl.spi.transform;
using ESS.FW.Bpm.Engine.Dmn.engine.impl.transform;
using ESS.FW.Bpm.Engine.Dmn.Impl.Entity.Repository;
using ESS.FW.Bpm.Model.Dmn.instance;

namespace ESS.FW.Bpm.Engine.Dmn.Impl.Transformer
{
    public class DecisionRequirementsDefinitionTransformHandler : DmnDecisionRequirementsGraphTransformHandler
    {
        protected internal override DmnDecisionRequirementsGraphImpl CreateFromDefinitions(
            IDmnElementTransformContext context, IDefinitions definitions)
        {
            var entity = (DecisionRequirementsDefinitionEntity)base.CreateFromDefinitions(context, definitions);

            entity.Category = definitions.Namespace;

            return entity;
        }

        protected internal override DmnDecisionRequirementsGraphImpl CreateDmnElement()
        {
            return new DecisionRequirementsDefinitionEntity();
        }
    }
}
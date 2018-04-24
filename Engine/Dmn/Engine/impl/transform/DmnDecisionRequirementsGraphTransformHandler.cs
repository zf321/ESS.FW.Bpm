using ESS.FW.Bpm.Engine.Dmn.engine.impl.spi.transform;
using ESS.FW.Bpm.Model.Dmn.instance;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl.transform
{
    public class DmnDecisionRequirementsGraphTransformHandler :
        IDmnElementTransformHandler<IDefinitions, DmnDecisionRequirementsGraphImpl>
    {
        public virtual DmnDecisionRequirementsGraphImpl HandleElement(IDmnElementTransformContext context,
            IDefinitions definitions)
        {
            return CreateFromDefinitions(context, definitions);
        }

        protected internal virtual DmnDecisionRequirementsGraphImpl CreateFromDefinitions(
            IDmnElementTransformContext context, IDefinitions definitions)
        {
            var drd = CreateDmnElement();

            drd.Key = definitions.Id;
            drd.Name = definitions.Name;

            return drd;
        }

        protected internal virtual DmnDecisionRequirementsGraphImpl CreateDmnElement()
        {
            return new DmnDecisionRequirementsGraphImpl();
        }
    }
}
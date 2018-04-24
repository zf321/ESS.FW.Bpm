using ESS.FW.Bpm.Engine.Dmn.engine.impl.spi.transform;
using ESS.FW.Bpm.Model.Dmn.instance;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl.transform
{
    public class DmnDecisionTransformHandler : IDmnElementTransformHandler<IDecision, DmnDecisionImpl>
    {
        public virtual DmnDecisionImpl HandleElement(IDmnElementTransformContext context, IDecision decision)
        {
            return CreateFromDecision(context, decision);
        }

        protected internal virtual DmnDecisionImpl CreateFromDecision(IDmnElementTransformContext context,
            IDecision decision)
        {
            var decisionEntity = CreateDmnElement();

            decisionEntity.Key = decision.Id;
            decisionEntity.Name = decision.Name;
            return decisionEntity;
        }

        protected internal virtual DmnDecisionImpl CreateDmnElement()
        {
            return new DmnDecisionImpl();
        }
    }
}
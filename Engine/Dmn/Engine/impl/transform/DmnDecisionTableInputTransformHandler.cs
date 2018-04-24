using ESS.FW.Bpm.Engine.Dmn.engine.impl.spi.transform;
using ESS.FW.Bpm.Model.Dmn.instance;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl.transform
{
    public class DmnDecisionTableInputTransformHandler : IDmnElementTransformHandler<IInput, DmnDecisionTableInputImpl>
    {
        public virtual DmnDecisionTableInputImpl HandleElement(IDmnElementTransformContext context, IInput input)
        {
            return CreateFromInput(context, input);
        }

        //TODO Input 底层赋值
        protected internal virtual DmnDecisionTableInputImpl CreateFromInput(IDmnElementTransformContext context,
            IInput input)
        {
            var decisionTableInput = CreateDmnElement(context, input);

            decisionTableInput.Id = input.Id;
            decisionTableInput.Name = input.Label;
            decisionTableInput.InputVariable = input.CamundaInputVariable;

            return decisionTableInput;
        }

        protected internal virtual DmnDecisionTableInputImpl CreateDmnElement(IDmnElementTransformContext context,
            IInput input)
        {
            return new DmnDecisionTableInputImpl();
        }
    }
}
using ESS.FW.Bpm.Engine.Dmn.engine.impl.spi.transform;
using ESS.FW.Bpm.Engine.Dmn.engine.impl.spi.type;
using ESS.FW.Bpm.Engine.Dmn.engine.impl.type;
using ESS.FW.Bpm.Model.Dmn.instance;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl.transform
{
    public class DmnDecisionTableOutputTransformHandler :
        IDmnElementTransformHandler<IOutput, DmnDecisionTableOutputImpl>
    {
        public virtual DmnDecisionTableOutputImpl HandleElement(IDmnElementTransformContext context, IOutput output)
        {
            return CreateFromOutput(context, output);
        }

        protected internal virtual DmnDecisionTableOutputImpl CreateFromOutput(IDmnElementTransformContext context,
            IOutput output)
        {
            var decisionTableOutput = CreateDmnElement(context, output);

            decisionTableOutput.Id = output.Id;
            decisionTableOutput.Name = output.Label;
            decisionTableOutput.OutputName = output.Name;
            decisionTableOutput.TypeDefinition = GetTypeDefinition(context, output);

            return decisionTableOutput;
        }

        protected internal virtual DmnDecisionTableOutputImpl CreateDmnElement(IDmnElementTransformContext context,
            IOutput output)
        {
            return new DmnDecisionTableOutputImpl();
        }

        protected internal virtual IDmnTypeDefinition GetTypeDefinition(IDmnElementTransformContext context,
            IOutput output)
        {
            var typeRef = output.TypeRef;
            if (!ReferenceEquals(typeRef, null))
            {
                var transformer = context.DataTypeTransformerRegistry.GetTransformer(typeRef);
                return new DmnTypeDefinitionImpl(typeRef, transformer);
            }
            return new DefaultTypeDefinition();
        }
    }
}
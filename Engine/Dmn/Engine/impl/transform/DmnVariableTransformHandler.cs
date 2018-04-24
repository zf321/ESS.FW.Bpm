using ESS.FW.Bpm.Engine.Dmn.engine.impl.spi.transform;
using ESS.FW.Bpm.Model.Dmn.instance;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl.transform
{

    public class DmnVariableTransformHandler : IDmnElementTransformHandler<IVariable, DmnVariableImpl>
    {
        public virtual DmnVariableImpl HandleElement(IDmnElementTransformContext context, IVariable variable)
        {
            return CreateFromVariable(context, variable);
        }

        protected internal virtual DmnVariableImpl CreateFromVariable(IDmnElementTransformContext context,
            IVariable variable)
        {
            var dmnVariable = CreateDmnElement(context, variable);

            dmnVariable.Id = variable.Id;
            dmnVariable.Name = variable.Name;

            var typeDefinition = DmnExpressionTransformHelper.CreateTypeDefinition(context, variable);
            dmnVariable.TypeDefinition = typeDefinition;

            return dmnVariable;
        }

        protected internal virtual DmnVariableImpl CreateDmnElement(IDmnElementTransformContext context,
            IVariable variable)
        {
            return new DmnVariableImpl();
        }
    }
}
using ESS.FW.Bpm.Engine.Dmn.engine.impl.spi.transform;
using ESS.FW.Bpm.Model.Dmn.instance;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl.transform
{

    public class DmnDecisionTableInputExpressionTransformHandler :
        IDmnElementTransformHandler<IInputExpression, DmnExpressionImpl>
    {
        public virtual DmnExpressionImpl HandleElement(IDmnElementTransformContext context,
            IInputExpression inputExpression)
        {
            return CreateFromInputExpression(context, inputExpression);
        }

        protected internal virtual DmnExpressionImpl CreateFromInputExpression(IDmnElementTransformContext context,
            IInputExpression inputExpression)
        {
            var dmnExpression = CreateDmnElement(context, inputExpression);

            dmnExpression.Id = inputExpression.Id;
            dmnExpression.Name = inputExpression.Label;
            dmnExpression.TypeDefinition = DmnExpressionTransformHelper.CreateTypeDefinition(context, inputExpression);
            dmnExpression.ExpressionLanguage = DmnExpressionTransformHelper.GetExpressionLanguage(context,
                inputExpression);
            dmnExpression.Expression = DmnExpressionTransformHelper.GetExpression(inputExpression);

            return dmnExpression;
        }

        protected internal virtual DmnExpressionImpl CreateDmnElement(IDmnElementTransformContext context,
            IInputExpression inputExpression)
        {
            return new DmnExpressionImpl();
        }
    }
}
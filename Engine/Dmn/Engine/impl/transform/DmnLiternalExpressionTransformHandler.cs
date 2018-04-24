using ESS.FW.Bpm.Engine.Dmn.engine.impl.spi.transform;
using ESS.FW.Bpm.Model.Dmn.instance;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl.transform
{

    public class DmnLiternalExpressionTransformHandler :
        IDmnElementTransformHandler<ILiteralExpression, DmnExpressionImpl>
    {
        public virtual DmnExpressionImpl HandleElement(IDmnElementTransformContext context,
            ILiteralExpression literalExpression)
        {
            return CreateFromLiteralExpressionEntry(context, literalExpression);
        }

        protected internal virtual DmnExpressionImpl CreateFromLiteralExpressionEntry(
            IDmnElementTransformContext context, ILiteralExpression literalExpression)
        {
            var dmnExpression = CreateDmnElement(context, literalExpression);

            dmnExpression.Id = literalExpression.Id;
            dmnExpression.Name = literalExpression.Label;
            dmnExpression.ExpressionLanguage = DmnExpressionTransformHelper.GetExpressionLanguage(context,
                literalExpression);
            dmnExpression.Expression = DmnExpressionTransformHelper.GetExpression(literalExpression);

            return dmnExpression;
        }

        protected internal virtual DmnExpressionImpl CreateDmnElement(IDmnElementTransformContext context,
            ILiteralExpression inputEntry)
        {
            return new DmnExpressionImpl();
        }
    }
}
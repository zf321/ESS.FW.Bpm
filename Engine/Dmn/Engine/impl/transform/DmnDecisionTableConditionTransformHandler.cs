using ESS.FW.Bpm.Engine.Dmn.engine.impl.spi.transform;
using ESS.FW.Bpm.Model.Dmn.instance;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl.transform
{

    public class DmnDecisionTableConditionTransformHandler : IDmnElementTransformHandler<IInputEntry, DmnExpressionImpl>
    {
        public virtual DmnExpressionImpl HandleElement(IDmnElementTransformContext context, IInputEntry inputEntry)
        {
            return CreateFromInputEntry(context, inputEntry);
        }

        protected internal virtual DmnExpressionImpl CreateFromInputEntry(IDmnElementTransformContext context,
            IInputEntry inputEntry)
        {
            var condition = CreateDmnElement(context, inputEntry);

            condition.Id = inputEntry.Id;
            condition.Name = inputEntry.Label;
            condition.ExpressionLanguage = DmnExpressionTransformHelper.GetExpressionLanguage(context, inputEntry);
            condition.Expression = DmnExpressionTransformHelper.GetExpression(inputEntry);

            return condition;
        }

        protected internal virtual DmnExpressionImpl CreateDmnElement(IDmnElementTransformContext context,
            IInputEntry inputEntry)
        {
            return new DmnExpressionImpl();
        }
    }
}
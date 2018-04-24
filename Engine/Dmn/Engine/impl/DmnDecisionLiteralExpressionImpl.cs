namespace ESS.FW.Bpm.Engine.Dmn.engine.impl
{
    public class DmnDecisionLiteralExpressionImpl : IDmnDecisionLogic
    {
        protected internal DmnExpressionImpl expression;

        protected internal DmnVariableImpl variable;

        public virtual DmnVariableImpl Variable
        {
            get { return variable; }
            set { variable = value; }
        }

        public virtual DmnExpressionImpl Expression
        {
            get { return expression; }
            set { expression = value; }
        }

        public override string ToString()
        {
            return "DmnDecisionLiteralExpressionImpl [variable=" + variable + ", expression=" + expression + "]";
        }
    }
}
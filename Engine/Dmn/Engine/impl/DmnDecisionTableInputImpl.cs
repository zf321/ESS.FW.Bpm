namespace ESS.FW.Bpm.Engine.Dmn.engine.impl
{
    public class DmnDecisionTableInputImpl
    {
        public const string DEFAULT_INPUT_VARIABLE_NAME = "cellInput";

        protected internal DmnExpressionImpl expression;

        public string id;
        protected internal string inputVariable;
        public string name;

        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }


        public virtual string Id
        {
            get { return id; }
            set { id = value; }
        }


        public virtual DmnExpressionImpl Expression
        {
            set { expression = value; }
            get { return expression; }
        }


        public virtual string InputVariable
        {
            get
            {
                if (inputVariable != null)
                    return inputVariable;
                return DEFAULT_INPUT_VARIABLE_NAME;
            }
            set { inputVariable = value; }
        }


        public override string ToString()
        {
            return "DmnDecisionTableInputImpl{" + "id='" + id + '\'' + ", name='" + name + '\'' + ", expression=" +
                   expression + ", inputVariable='" + inputVariable + '\'' + '}';
        }
    }
}
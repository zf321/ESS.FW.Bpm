using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl
{
    public class DmnDecisionTableRuleImpl
    {
        protected internal IList<DmnExpressionImpl> conclusions = new List<DmnExpressionImpl>();

        protected internal IList<DmnExpressionImpl> conditions = new List<DmnExpressionImpl>();

        public string id;
        public string name;

        public virtual string Id
        {
            get { return id; }
            set { id = value; }
        }


        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }


        public virtual IList<DmnExpressionImpl> Conditions
        {
            get { return conditions; }
            set { conditions = value; }
        }


        public virtual IList<DmnExpressionImpl> Conclusions
        {
            get { return conclusions; }
            set { conclusions = value; }
        }


        public override string ToString()
        {
            return "DmnDecisionTableRuleImpl{" + "id='" + id + '\'' + ", name='" + name + '\'' + ", conditions=" +
                   conditions + ", conclusions=" + conclusions + '}';
        }
    }
}
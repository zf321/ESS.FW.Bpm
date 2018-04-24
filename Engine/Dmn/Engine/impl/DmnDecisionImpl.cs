using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl
{
    public class DmnDecisionImpl : IDmnDecision
    {


        public virtual ICollection<IDmnDecision> RequiredDecisions { get; set; } = new List<IDmnDecision>();

        public virtual string Key { get; set; }


        public virtual string Name { get; set; }


        public virtual IDmnDecisionLogic DecisionLogic { set; get; }
        

        public virtual bool DecisionTable
        {
            get { return (DecisionLogic != null) && DecisionLogic is DmnDecisionTableImpl; }
        }

        public override string ToString()
        {
            return "DmnDecisionTableImpl{" + " key= " + Key + ", name= " + Name + ", requiredDecision=" +
                   RequiredDecisions + ", decisionLogic=" + DecisionLogic + '}';
        }
    }
}
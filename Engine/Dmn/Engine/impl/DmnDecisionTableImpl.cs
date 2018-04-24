using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Dmn.engine.impl.spi.hitpolicy;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl
{
    public class DmnDecisionTableImpl : IDmnDecisionLogic
    {
        protected internal IDmnHitPolicyHandler hitPolicyHandler;

        protected internal IList<DmnDecisionTableInputImpl> inputs = new List<DmnDecisionTableInputImpl>();
        protected internal IList<DmnDecisionTableOutputImpl> outputs = new List<DmnDecisionTableOutputImpl>();
        protected internal IList<DmnDecisionTableRuleImpl> rules = new List<DmnDecisionTableRuleImpl>();

        public virtual IDmnHitPolicyHandler HitPolicyHandler
        {
            get { return hitPolicyHandler; }
            set { hitPolicyHandler = value; }
        }


        public virtual IList<DmnDecisionTableInputImpl> Inputs
        {
            get { return inputs; }
            set { inputs = value; }
        }


        public virtual IList<DmnDecisionTableOutputImpl> Outputs
        {
            get { return outputs; }
            set { outputs = value; }
        }


        public virtual IList<DmnDecisionTableRuleImpl> Rules
        {
            get { return rules; }
            set { rules = value; }
        }


        public override string ToString()
        {
            return "DmnDecisionTableImpl{" + " hitPolicyHandler=" + hitPolicyHandler + ", inputs=" + inputs +
                   ", outputs=" + outputs + ", rules=" + rules + '}';
        }
    }
}
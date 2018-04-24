using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl
{
    public class DmnDecisionRequirementsGraphImpl : IDmnDecisionRequirementsGraph
    {
        protected internal IDictionary<string, IDmnDecision> decisions = new Dictionary<string, IDmnDecision>();

        protected internal string key;
        protected internal string name;

        public virtual ICollection<string> DecisionKeys
        {
            get { return decisions.Keys; }
        }

        public virtual string Key
        {
            get { return key; }
            set { key = value; }
        }


        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }

        public virtual IDmnDecision getDecision(string key)
        {
            return decisions[key];
        }

        public ICollection<IDmnDecision> Decisions
        {
            get { return decisions.Values; }
        }

        ISet<string> IDmnDecisionRequirementsGraph.DecisionKeys
        {
            get
            {
                ISet<string> r = new HashSet<string>();
                foreach (var item in DecisionKeys)
                    r.Add(item);
                return r;
            }
        }

        public virtual ICollection<IDmnDecision> GetDecisions()
        {
            return decisions.Values;
        }

        public virtual void SetDecisions(IDictionary<string, IDmnDecision> decisions)
        {
            this.decisions = decisions;
        }

        public virtual void AddDecision(IDmnDecision decision)
        {
            decisions[decision.Key] = decision;
        }

        public override string ToString()
        {
            return "DmnDecisionRequirementsGraphImpl [key=" + key + ", name=" + name + ", decisions=" + decisions + "]";
        }
    }
}
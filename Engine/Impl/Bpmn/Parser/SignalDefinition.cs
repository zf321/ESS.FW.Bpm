using System;
using ESS.FW.Bpm.Engine.Delegate;

namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Parser
{
    /// <summary>
    ///     Represents a bpmn signal definition
    ///     
    /// </summary>
    [Serializable]
    public class SignalDefinition
    {
        private const long SerialVersionUid = 1L;

        protected internal string id;
        protected internal IExpression name;


        public virtual string Id
        {
            get { return id; }
            set { id = value; }
        }


        public virtual string Name
        {
            get { return name.ExpressionText; }
        }

        public virtual IExpression Expression
        {
            get { return name; }
            set { name = value; }
        }
    }
}
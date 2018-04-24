

using ESS.FW.Bpm.Engine.Delegate;

namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Parser
{
    /// <summary>
    ///     Implementation of the BPMN 2.0 'message'
    ///     
    /// </summary>
    public class MessageDefinition
    {
        protected internal string id;
        protected internal IExpression Name;

        public MessageDefinition(string id, IExpression name)
        {
            this.id = id;
            this.Name = name;
        }

        public virtual string Id
        {
            get { return id; }
        }

        public virtual IExpression Expression
        {
            get { return Name; }
        }
    }
}
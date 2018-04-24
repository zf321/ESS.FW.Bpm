namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Parser
{
    /// <summary>
    ///     Represents an 'escalation' element.
    ///     
    /// </summary>
    public class Escalation
    {
        protected internal readonly string id;
        protected internal string escalationCode;
        protected internal string name;

        public Escalation(string id)
        {
            this.id = id;
        }

        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }


        public virtual string EscalationCode
        {
            get { return escalationCode; }
            set { escalationCode = value; }
        }


        public virtual string Id
        {
            get { return id; }
        }
    }
}
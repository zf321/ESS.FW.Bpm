using ESS.FW.Bpm.Engine.Impl.Pvm;

namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Parser
{
    /// <summary>
    ///     Represents an escalation event definition that reference an 'escalation' element.
    ///     
    /// </summary>
    public class EscalationEventDefinition
    {
        protected internal readonly bool cancelActivity;

        protected internal readonly IPvmActivity escalationHandler;

        protected internal string escalationCode;
        protected internal string escalationCodeVariable;

        public EscalationEventDefinition(IPvmActivity escalationHandler, bool cancelActivity)
        {
            this.escalationHandler = escalationHandler;
            this.cancelActivity = cancelActivity;
        }

        public virtual string EscalationCode
        {
            get { return escalationCode; }
            set { escalationCode = value; }
        }

        public virtual IPvmActivity EscalationHandler
        {
            get { return escalationHandler; }
        }

        public virtual bool CancelActivity
        {
            get { return cancelActivity; }
        }


        public virtual string EscalationCodeVariable
        {
            get { return escalationCodeVariable; }
            set { escalationCodeVariable = value; }
        }
    }
}
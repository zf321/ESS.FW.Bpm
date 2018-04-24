using ESS.FW.Bpm.Engine.Delegate;

namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Parser
{
    /// <summary>
    ///     Represents a BPMN Error definition, whereas <seealso cref="BpmnError" /> represents an
    ///     actual instance of an Error.
    ///     
    /// </summary>
    public class Error
    {
        protected internal string errorCode;

        protected internal string id;

        public virtual string Id
        {
            get { return id; }
            set { id = value; }
        }


        public virtual string ErrorCode
        {
            get { return errorCode; }
            set { errorCode = value; }
        }
    }
}
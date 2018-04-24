using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    /// </summary>
    public class CompleteExternalTaskCmd : HandleExternalTaskCmd
    {
        protected internal IDictionary<string, object> Variables;

        public CompleteExternalTaskCmd(string externalTaskId, string workerId, IDictionary<string, object> variables)
            : base(externalTaskId, workerId)
        {
            this.Variables = variables;
        }

        public override string ErrorMessageOnWrongWorkerAccess
        {
            get { return "External ITask " + ExternalTaskId + " cannot be completed by worker '" + WorkerId; }
        }

        protected internal override object Execute(ExternalTaskEntity externalTask)
        {
            externalTask.Complete(Variables);
            return null;
        }
    }
}
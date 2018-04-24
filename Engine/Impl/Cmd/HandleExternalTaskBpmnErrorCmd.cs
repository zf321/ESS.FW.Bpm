using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///     Command to handle an external ITask BPMN error.
    ///     
    /// </summary>
    public class HandleExternalTaskBpmnErrorCmd : HandleExternalTaskCmd
    {
        /// <summary>
        ///     The error code of the corresponding bpmn error.
        /// </summary>
        protected internal string ErrorCode;

        public HandleExternalTaskBpmnErrorCmd(string externalTaskId, string workerId, string errorCode)
            : base(externalTaskId, workerId)
        {
            this.ErrorCode = errorCode;
        }

        public override string ErrorMessageOnWrongWorkerAccess
        {
            get
            {
                return "Bpmn error of External ITask " + ExternalTaskId + " cannot be reported by worker '" + WorkerId;
            }
        }

        protected internal override void ValidateInput()
        {
            base.ValidateInput();
            EnsureUtil.EnsureNotNull("errorCode", ErrorCode);
        }

        protected internal override object Execute(ExternalTaskEntity externalTask)
        {
            externalTask.BpmnError(ErrorCode);
            return null;
        }
    }
}


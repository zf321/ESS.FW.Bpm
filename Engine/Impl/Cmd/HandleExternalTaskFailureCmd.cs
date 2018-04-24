using System;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///     
    /// </summary>
    public class HandleExternalTaskFailureCmd : HandleExternalTaskCmd
    {
        protected internal string ErrorDetails;

        protected internal string ErrorMessage;
        protected internal int Retries;
        protected internal long RetryDuration;

        public HandleExternalTaskFailureCmd(string externalTaskId, string workerId, string errorMessage, int retries,
            long retryDuration) : this(externalTaskId, workerId, errorMessage, null, retries, retryDuration)
        {
        }

        /// <summary>
        ///     Overloaded constructor to support short and full error messages
        /// </summary>
        /// <param name="externalTaskId"> </param>
        /// <param name="workerId"> </param>
        /// <param name="errorMessage"> </param>
        /// <param name="errorDetails"> </param>
        /// <param name="retries"> </param>
        /// <param name="retryDuration"> </param>
        public HandleExternalTaskFailureCmd(string externalTaskId, string workerId, string errorMessage,
            string errorDetails, int retries, long retryDuration) : base(externalTaskId, workerId)
        {
            this.ErrorMessage = errorMessage;
            this.ErrorDetails = errorDetails;
            this.Retries = retries;
            this.RetryDuration = retryDuration;
        }

        public override string ErrorMessageOnWrongWorkerAccess
        {
            get { return "Failure of External ITask " + ExternalTaskId + " cannot be reported by worker '" + WorkerId; }
        }

        protected internal override  object Execute(ExternalTaskEntity externalTask)
        {
            externalTask.Failed(ErrorMessage, ErrorDetails, Retries, RetryDuration);
            return null;
        }

        protected internal override void ValidateInput()
        {
            base.ValidateInput();
            EnsureUtil.EnsureGreaterThanOrEqual("retries", Retries, 0);
            EnsureUtil.EnsureGreaterThanOrEqual("retryDuration", RetryDuration, 0);
        }

    }
}
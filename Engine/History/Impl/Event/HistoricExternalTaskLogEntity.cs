using System;
using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.History.Impl.Event
{

    [Serializable]
    public class HistoricExternalTaskLogEntity : HistoryEvent, IHistoricExternalTaskLog
    {
        private const long SerialVersionUid = 1L;
        private const string ExceptionName = "historicExternalTaskLog.exceptionByteArray";

        protected internal string errorMessage;


        public virtual string ErrorDetailsByteArrayId { get; set; }


        public virtual string ErrorDetails
        {
            get
            {
                var byteArray = ErrorByteArray;
                return ExceptionUtil.GetExceptionStacktrace(byteArray);
            }
            set
            {
                EnsureUtil.EnsureNotNull("exception", value);
                var exceptionBytes = value.GetBytes();
                ResourceEntity byteArray = ExceptionUtil.CreateExceptionByteArray(ExceptionName, exceptionBytes);
                ErrorDetailsByteArrayId = byteArray.Id;
            }
        }


        protected internal virtual ResourceEntity ErrorByteArray
        {
            get
            {
                if (!ReferenceEquals(ErrorDetailsByteArrayId, null))
                    return null;
                return null;
            }
        }


        public virtual int State { get; set; }

        public virtual DateTime TimeStamp { get; set; }


        public virtual string ExternalTaskId { get; set; }


        public virtual string TopicName { get; set; }


        public virtual string WorkerId { get; set; }


        public virtual int? Retries { get; set; }


        public virtual string ErrorMessage
        {
            get { return errorMessage; }
            set
            {
                // note: it is not a clean way to truncate where the history event is produced, since truncation is only
                //   relevant for relational history databases that follow our schema restrictions;
                //   a similar problem exists in ExternalTaskEntity#setErrorMessage where truncation may not be required for custom
                //   persistence implementations
                if (!ReferenceEquals(value, null) && (value.Length > ExternalTaskEntity.MaxExceptionMessageLength))
                    errorMessage = value.Substring(0, ExternalTaskEntity.MaxExceptionMessageLength);
                else
                    errorMessage = value;
            }
        }

        public virtual string ActivityId { get; set; }


        public virtual string ActivityInstanceId { get; set; }


        public virtual string TenantId { get; set; }


        public virtual long Priority { get; set; }


        public virtual bool CreationLog
        {
            get { return State == ExternalTaskStateFields.Created.StateCode; }
        }

        public virtual bool FailureLog
        {
            get { return State == ExternalTaskStateFields.Failed.StateCode; }
        }

        public virtual bool SuccessLog
        {
            get { return State == ExternalTaskStateFields.Successful.StateCode; }
        }

        public virtual bool DeletionLog
        {
            get { return State == ExternalTaskStateFields.Deleted.StateCode; }
        }
    }
}
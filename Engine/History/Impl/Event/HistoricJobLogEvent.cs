using System;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.History.Impl.Event
{
    /// <summary>
    ///     
    /// </summary>
    [Serializable]
    public class HistoricJobLogEventEntity : HistoryEvent,IHistoricJobLog
    {
        private const long SerialVersionUid = 1L;


        protected internal string jobExceptionMessage;

        public virtual DateTime TimeStamp { get; set; }


        public virtual string JobId { get; set; }


        public virtual DateTime? JobDueDate { get; set; }


        public virtual int JobRetries { get; set; }


        public virtual long JobPriority { get; set; }


        public virtual string JobExceptionMessage
        {
            get { return jobExceptionMessage; }
            set
            {
                // note: it is not a clean way to truncate where the history event is produced, since truncation is only
                //   relevant for relational history databases that follow our schema restrictions;
                //   a similar problem exists in JobEntity#setExceptionMessage where truncation may not be required for custom
                //   persistence implementations
                //if (!ReferenceEquals(value, null) && value.Length > JobEntity.MAX_EXCEPTION_MESSAGE_LENGTH)
                //{
                //    jobExceptionMessage = value.Substring(0, JobEntity.MAX_EXCEPTION_MESSAGE_LENGTH);
                //}
                //else
                //{
                //    jobExceptionMessage = value;
                //}
            }
        }


        public virtual string ExceptionByteArrayId { get; set; }


        public virtual string ExceptionStacktrace
        {
            get
            {
                var byteArray = ExceptionByteArray;
                return ExceptionUtil.GetExceptionStacktrace(byteArray);
            }
        }

        protected internal virtual ResourceEntity ExceptionByteArray
        {
            get
            {
                if (!ReferenceEquals(ExceptionByteArrayId, null))
                {
                    //return context.Impl.Context.CommandContext.HistoricJobLogManager.Get(exceptionByteArrayId);
                }

                return null;
            }
        }

        public virtual string JobDefinitionId { get; set; }


        public virtual string JobDefinitionType { get; set; }


        public virtual string JobDefinitionConfiguration { get; set; }


        public virtual string ActivityId { get; set; }


        public virtual string DeploymentId { get; set; }


        public virtual int State { get; set; }


        public virtual string TenantId { get; set; }


        public virtual bool CreationLog
        {
            get { return State == JobStateFields.Created.StateCode; }
        }

        public virtual bool FailureLog
        {
            get { return State == JobStateFields.Failed.StateCode; }
        }

        public virtual bool SuccessLog
        {
            get { return State == JobStateFields.Successful.StateCode; }
        }

        public virtual bool DeletionLog
        {
            get { return State == JobStateFields.Deleted.StateCode; }
        }
    }
}
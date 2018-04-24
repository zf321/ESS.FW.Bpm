using System;

namespace ESS.FW.Bpm.Engine.History.Impl.Event
{
    /// <summary>
    ///     
    /// </summary>
    [Serializable]
    public class HistoricCaseInstanceEventEntity : HistoricScopeInstanceEvent, IHistoricCaseInstance
    {

        /// <summary>
        ///     the business key of the case instance
        /// </summary>
        public string BusinessKey { get; set; }

        /// <summary>
        ///     the id of the user that created the case instance
        /// </summary>
        public string CreateUserId { get; set; }

        /// <summary>
        ///     the current state of the case instance
        /// </summary>
        public int State{ get; set; }

    /// <summary>
    ///     the case instance which started this case instance
    /// </summary>
    public string SuperCaseInstanceId { get; set; }

        /// <summary>
        ///     the process instance which started this case instance
        /// </summary>
        public string SuperProcessInstanceId { get; set; }

        /// <summary>
        ///     id of the tenant which belongs to the case instance
        /// </summary>
        public string TenantId { get; set; }


        public virtual DateTime? CreateTime
        {
            get { return StartTime; }
            set { StartTime = value; }
        }


        public virtual DateTime? CloseTime
        {
            get { return EndTime; }
            set { EndTime = value; }
        }


        public virtual bool Active
        {
            get { throw new NotImplementedException(); /*return state ==  ACTIVE.StateCode;*/ }
        }

        public virtual bool Completed
        {
            get { throw new NotImplementedException(); /*return state == COMPLETED.StateCode;*/ }
        }

        public virtual bool Terminated
        {
            get { throw new NotImplementedException(); /*return state == TERMINATED.StateCode;*/ }
        }

        public virtual bool Failed
        {
            get { throw new NotImplementedException(); /*return state == FAILED.StateCode; */}
        }

        public virtual bool Suspended
        {
            get { throw new NotImplementedException(); /*return state == SUSPENDED.StateCode;*/ }
        }

        public virtual bool Closed
        {
            get { throw new NotImplementedException(); /*return state == CLOSED.StateCode;*/ }
        }

        public override string ToString()
        {
            return GetType().Name + "[businessKey=" + BusinessKey + ", startUserId=" + CreateUserId +
                   ", superCaseInstanceId=" + SuperCaseInstanceId + ", superProcessInstanceId=" + SuperProcessInstanceId +
                   ", durationInMillis=" + durationInMillis + ", createTime=" + StartTime + ", closeTime=" + EndTime +
                   ", id=" + Id + ", eventType=" + EventType + ", caseExecutionId=" + CaseExecutionId +
                   ", caseDefinitionId=" + CaseDefinitionId + ", caseInstanceId=" + CaseInstanceId + ", tenantId=" +
                   TenantId + "]";
        }
    }
}
using System;

namespace ESS.FW.Bpm.Engine.History.Impl.Event
{
    /// <summary>
    ///     <para><seealso cref="HistoryEvent" /> implementation for events that happen in a case activity.</para>
    ///     
    /// </summary>
    [Serializable]
    public class HistoricCaseActivityInstanceEventEntity : HistoricScopeInstanceEvent, IHistoricCaseActivityInstance
    {
        // getters and setters //////////////////////////////////////////////////////

        public override string CaseExecutionId
        {
            get { return Id; }
        }

        /// <summary>
        ///     the id of the case activity
        /// </summary>
        public virtual string CaseActivityId { get; set; }


        /// <summary>
        ///     the name of the case activity
        /// </summary>
        public virtual string CaseActivityName { get; set; }


        /// <summary>
        ///     the type of the case activity
        /// </summary>
        public virtual string CaseActivityType { get; set; }


        /// <summary>
        ///     the state of this case activity instance
        /// </summary>
        public virtual int CaseActivityInstanceState { get; set; }


        /// <summary>
        ///     the id of the parent case activity instance
        /// </summary>
        public virtual string ParentCaseActivityInstanceId { get; set; }


        /// <summary>
        ///     the id of the called ITask in case of a human ITask
        /// </summary>
        public virtual string TaskId { get; set; }

        /// <summary>
        ///     the id of the called process in case of a process ITask
        /// </summary>
        public virtual string CalledProcessInstanceId { get; set; }


        /// <summary>
        ///     the id of the called case in case of a case ITask
        /// </summary>
        public virtual string CalledCaseInstanceId { get; set; }


        /// <summary>
        ///     id of the tenant which belongs to the case activity instance
        /// </summary>
        public virtual string TenantId { get; set; }


        public virtual DateTime? CreateTime
        {
            get { return StartTime; }
            set { StartTime = value; }
        }


        /// <summary>
        ///     the flag whether this case activity is required
        /// </summary>
        public virtual bool Required { get; set; }


        public virtual bool Available
        {
            get {throw new NotImplementedException(); /*return caseActivityInstanceState == AVAILABLE.StateCode;*/ }
        }

        public virtual bool Enabled
        {
            get { throw new NotImplementedException();/* return caseActivityInstanceState == ENABLED.StateCode;*/ }
        }

        public virtual bool Disabled
        {
            get { throw new NotImplementedException();/* return caseActivityInstanceState == DISABLED.StateCode;*/ }
        }

        public virtual bool Active
        {
            get { throw new NotImplementedException();/* return caseActivityInstanceState == ACTIVE.StateCode;*/ }
        }

        public virtual bool IsSuspended
        {
            get { throw new NotImplementedException();/* return caseActivityInstanceState == SUSPENDED.StateCode;*/ }
        }

        public virtual bool Completed
        {
            get { throw new NotImplementedException(); /*return caseActivityInstanceState == COMPLETED.StateCode; */
            }
        }

        public virtual bool Terminated
        {
            get { throw new NotImplementedException(); /*return caseActivityInstanceState == TERMINATED.StateCode;*/ }
        }

        public override string ToString()
        {
            return GetType().Name + "[caseActivityId=" + CaseActivityId + ", caseActivityName=" + CaseActivityName +
                   ", caseActivityInstanceId=" + Id + ", caseActivityInstanceState=" + CaseActivityInstanceState +
                   ", parentCaseActivityInstanceId=" + ParentCaseActivityInstanceId + ", taskId=" + TaskId +
                   ", calledProcessInstanceId=" + CalledProcessInstanceId + ", calledCaseInstanceId=" +
                   CalledCaseInstanceId + ", durationInMillis=" + durationInMillis + ", createTime=" + StartTime +
                   ", endTime=" + EndTime + ", eventType=" + EventType + ", caseExecutionId=" + CaseExecutionId +
                   ", caseDefinitionId=" + CaseDefinitionId + ", caseInstanceId=" + CaseInstanceId + ", tenantId=" +
                   TenantId + "]";
        }
    }
}
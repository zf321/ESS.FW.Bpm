using System;

namespace ESS.FW.Bpm.Engine.History.Impl.Event
{
    /// <summary>
    ///     <para><seealso cref="HistoryEvent" /> signifying a top-level event in a process instance.</para>
    ///     
    ///     
    /// </summary>
    public class HistoricProcessInstanceEventEntity : HistoricScopeInstanceEvent, IHistoricProcessInstance
    {
        


        // getters / setters ////////////////////////////////////////

        /// <summary>
        ///     id of the activity which started the process instance
        /// </summary>
        public virtual string EndActivityId { get; set; }

        /// <summary>
        ///     id of the activity which ended the process instance
        /// </summary>
        public virtual string StartActivityId { get; set; }


        /// <summary>
        ///     the business key of the process instance
        /// </summary>
        public virtual string BusinessKey { get; set; }


        /// <summary>
        ///     the id of the user that started the process instance
        /// </summary>
        public virtual string StartUserId { get; set; }


        /// <summary>
        ///     the id of the super process instance
        /// </summary>
        public virtual string SuperProcessInstanceId { get; set; }


        /// <summary>
        ///     the id of the super case instance
        /// </summary>
        public virtual string SuperCaseInstanceId { get; set; }


        /// <summary>
        ///     the reason why this process instance was cancelled (deleted)
        /// </summary>
        public virtual string DeleteReason { get; set; }

        /// <summary>
        ///     id of the tenant which belongs to the process instance
        /// </summary>
        public virtual string TenantId { get; set; }


        public virtual string State { get; set; }


        public override string ToString()
        {
            return GetType().Name + "[businessKey=" + BusinessKey + ", startUserId=" + StartUserId +
                   ", superProcessInstanceId=" + SuperProcessInstanceId + ", superCaseInstanceId=" + SuperCaseInstanceId +
                   ", deleteReason=" + DeleteReason + ", durationInMillis=" + durationInMillis + ", startTime=" +
                   StartTime + ", endTime=" + EndTime + ", endActivityId=" + EndActivityId + ", startActivityId=" +
                   StartActivityId + ", id=" + Id + ", eventType=" + EventType + ", executionId=" + ExecutionId +
                   ", processDefinitionId=" + ProcessDefinitionId + ", processInstanceId=" + ProcessInstanceId +
                   ", tenantId=" + TenantId + "]";
        }
    }
}
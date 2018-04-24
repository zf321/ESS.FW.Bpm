using System;

namespace ESS.FW.Bpm.Engine.History.Impl.Event
{
    /// <summary>
    /// </summary>
    [Serializable]
    public class HistoricTaskInstanceEventEntity : HistoricScopeInstanceEvent, IHistoricTaskInstance
    {
        private const long SerialVersionUid = 1L;

        protected internal string taskId;


        public virtual string TaskId
        {
            set { taskId = value; }
            get { return taskId; }
        }

        // getters and setters //////////////////////////////////////////////////////

        public virtual string DeleteReason { get; set; }

        public virtual string Assignee { get; set; }


        public virtual string Owner { get; set; }


        public virtual string Name { get; set; }


        public virtual string Description { get; set; }


        public virtual DateTime? DueDate { get; set; }


        public virtual DateTime? FollowUpDate { get; set; }

        public virtual int Priority { get; set; }


        public virtual string ParentTaskId { get; set; }


        public virtual string TaskDefinitionKey { get; set; }


        public virtual string ActivityInstanceId { get; set; }


        public virtual string TenantId { get; set; }


        public override string ToString()
        {
            return GetType()
                       .Name + "[taskId" + taskId + ", assignee=" + Assignee + ", owner=" + Owner + ", name=" +
                   Name + ", description=" + Description + ", dueDate=" + DueDate + ", followUpDate=" + FollowUpDate +
                   ", priority=" + Priority + ", parentTaskId=" + ParentTaskId + ", deleteReason=" + DeleteReason +
                   ", taskDefinitionKey=" + TaskDefinitionKey + ", durationInMillis=" + durationInMillis +
                   ", startTime=" + StartTime + ", endTime=" + EndTime + ", id=" + Id + ", eventType=" + EventType +
                   ", executionId=" + ExecutionId + ", processDefinitionId=" + ProcessDefinitionId +
                   ", processInstanceId=" + ProcessInstanceId + ", activityInstanceId=" + ActivityInstanceId +
                   ", tenantId=" + TenantId + "]";
        }
    }
}
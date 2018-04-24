using System;
using ESS.FW.Bpm.Engine.Impl.Pvm.Runtime;
using System.ComponentModel.DataAnnotations.Schema;
using ESS.Shared.Entities.Bpm;

namespace ESS.FW.Bpm.Engine.History.Impl.Event
{
    /// <summary>
    ///     <para><seealso cref="HistoryEvent" /> implementation for events that happen in an activity.</para>
    ///     
    ///     
    ///     
    /// </summary>
    [Serializable]
    public class HistoricActivityInstanceEventEntity : HistoricScopeInstanceEvent, IHistoricActivityInstance, IDbEntity
    {
        private const long SerialVersionUid = 1L;
        

        /// <summary>
        ///     the id of this activity instance
        /// </summary>
        //protected internal string ActivityInstanceId;

        /// <summary>
        ///     the state of this activity instance
        /// </summary>
        //protected internal int ActivityInstanceState;

        /// <summary>
        ///     the name of the activity
        /// </summary>
        //protected internal string ActivityName;

        /// <summary>
        ///     the type of the activity (startEvent, serviceTask ...)
        /// </summary>
        //protected internal string ActivityType;

        /// <summary>
        ///     the id of the child case instance
        /// </summary>
        //protected internal string CalledCaseInstanceId;

        /// <summary>
        ///     the id of the child process instance
        /// </summary>
        //protected internal string CalledProcessInstanceId;

        /// <summary>
        ///     the id of the parent activity instance
        /// </summary>
        //protected internal string ParentActivityInstanceId;
        
        

        // getters and setters //////////////////////////////////////////////////////

        public virtual string Assignee
        {
            get { return TaskAssignee; }
        }

        /// <summary>
        ///     the id of the activity
        /// </summary>
        public virtual string ActivityId { get; set; }


        public virtual string ActivityType { get; set; }


        public virtual string ActivityName { get; set; }

        //[NotMapped]
        public virtual string ActivityInstanceId { get; set; }


        public virtual string ParentActivityInstanceId { get; set; }


        public virtual string CalledProcessInstanceId { get; set; }


        public virtual string CalledCaseInstanceId { get; set; }


        public virtual string TaskId { get; set; }


        public virtual string TaskAssignee { get; set; }


        public virtual int ActivityInstanceState { get; set; }


        public virtual bool CompleteScope
        {
            get { return ActivityInstanceStateFields.ScopeComplete.StateCode == ActivityInstanceState; }
        }

        public virtual bool Canceled
        {
            get { return ActivityInstanceStateFields.Canceled.StateCode == ActivityInstanceState; }
        }

        /// <summary>
        ///     id of the tenant which belongs to the activity instance
        /// </summary>
        public virtual string TenantId { get; set; }


        public override string ToString()
        {
            return GetType().Name + "[activityId=" + ActivityId + ", activityName=" + ActivityName + ", activityType=" +
                   ActivityType + ", activityInstanceId=" + ActivityInstanceId + ", activityInstanceState=" +
                   ActivityInstanceState + ", parentActivityInstanceId=" + ParentActivityInstanceId +
                   ", calledProcessInstanceId=" + CalledProcessInstanceId + ", calledCaseInstanceId=" +
                   CalledCaseInstanceId + ", taskId=" + TaskId + ", taskAssignee=" + TaskAssignee +
                   ", durationInMillis=" + durationInMillis + ", startTime=" + StartTime + ", endTime=" + EndTime +
                   ", eventType=" + EventType + ", executionId=" + ExecutionId + ", processDefinitionId=" +
                   ProcessDefinitionId + ", processInstanceId=" + ProcessInstanceId + ", tenantId=" + TenantId + "]";
        }
    }
}
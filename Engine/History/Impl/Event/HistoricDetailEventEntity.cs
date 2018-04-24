using System;
using ESS.FW.Bpm.Engine.context.Impl;

namespace ESS.FW.Bpm.Engine.History.Impl.Event
{
    /// <summary>
    ///     
    /// </summary>
    [Serializable]
    public class HistoricDetailEventEntity : HistoryEvent, IHistoricDetail
    {
        private const long SerialVersionUid = 1L;

        // getters and setters //////////////////////////////////////////////////////

        public virtual string ActivityInstanceId { get; set; }


        public virtual string TaskId { get; set; }


        public virtual DateTime TimeStamp { get; set; }


        public virtual string TenantId { get; set; }

        public DateTime Time
        {
            get { return TimeStamp; }
        }

        public string UserOperationId
        {
            get { throw new NotImplementedException(); }
        }

        public virtual void Delete()
        {
            Context.CommandContext.GetDbEntityManager<HistoricDetailEventEntity>().Delete(this);
        }

        public override string ToString()
        {
            return GetType().Name + "[activityInstanceId=" + ActivityInstanceId + ", taskId=" + TaskId + ", timestamp=" +
                   TimeStamp + ", eventType=" + EventType + ", executionId=" + ExecutionId + ", processDefinitionId=" +
                   ProcessDefinitionId + ", processInstanceId=" + ProcessInstanceId + ", id=" + Id + ", tenantId=" +
                   TenantId + "]";
        }
    }
}
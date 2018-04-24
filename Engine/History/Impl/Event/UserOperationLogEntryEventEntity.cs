using ESS.FW.Bpm.Engine.Impl.DB;
using System;

namespace ESS.FW.Bpm.Engine.History.Impl.Event
{
    /// <summary>
    ///     
    /// </summary>
    [Serializable]
    public class UserOperationLogEntryEventEntity : HistoryEvent, IUserOperationLogEntry
    {
        private const long SerialVersionUid = 1L;

        //public override string Id { get; set; }

        public virtual string TenantId { get; set; }

        public virtual string OperationId { get; set; }

        public virtual string OperationType { get; set; }

        public virtual string TaskId { get; set; }

        public virtual string UserId { get; set; }

        public virtual DateTime Timestamp { get; set; }

        public virtual string Property { get; set; }

        public virtual string OrgValue { get; set; }

        public virtual string NewValue { get; set; }


        public virtual string EntityType { get; set; }


        public virtual string JobId { get; set; }


        public virtual string JobDefinitionId { get; set; }


        public virtual string DeploymentId { get; set; }


        public virtual string BatchId { get; set; }


        public override string ToString()
        {
            return GetType().Name + "[taskId" + TaskId + ", deploymentId" + DeploymentId + ", processDefinitionKey =" +
                   ProcessDefinitionKey + ", jobId = " + JobId + ", jobDefinitionId = " + JobDefinitionId +
                   ", batchId = " + BatchId + ", operationId =" + OperationId + ", operationType =" + OperationType +
                   ", userId =" + UserId + ", timestamp =" + Timestamp + ", property =" + Property + ", orgValue =" +
                   OrgValue + ", newValue =" + NewValue + ", id=" + Id + ", eventType=" + EventType + ", executionId=" +
                   ExecutionId + ", processDefinitionId=" + ProcessDefinitionId + ", processInstanceId=" +
                   ProcessInstanceId + ", tenantId=" + TenantId + ", entityType=" + EntityType + "]";
        }
    }
}
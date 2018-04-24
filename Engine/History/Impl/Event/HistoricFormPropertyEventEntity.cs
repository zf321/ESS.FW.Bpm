using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESS.FW.Bpm.Engine.History.Impl.Event
{
    /// <summary>
    ///     
    /// </summary>
    [Serializable]
    public class HistoricFormPropertyEventEntity : HistoricDetailEventEntity, IHistoricDetail
    {
        private const long SerialVersionUid = 1L;

        [Column("NAME")]
        public virtual string PropertyId { get; set; }

        [NotMapped]
        public virtual string PropertyValue { get; set; }


        public override string ToString()
        {
            return GetType().Name + "[propertyId=" + PropertyId + ", propertyValue=" + PropertyValue +
                   ", activityInstanceId=" + ActivityInstanceId + ", eventType=" + EventType + ", executionId=" +
                   ExecutionId + ", id=" + Id + ", processDefinitionId=" + ProcessDefinitionId + ", processInstanceId=" +
                   ProcessInstanceId + ", taskId=" + TaskId + ", tenantId=" + TenantId + "]";
        }
    }
}
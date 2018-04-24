using System;

namespace ESS.FW.Bpm.Engine.History.Impl.Event
{
    /// <summary>
    ///     
    /// </summary>
    [Serializable]
    public class HistoricIdentityLinkLogEventEntity : HistoryEvent, IHistoricIdentityLinkLog
    {
        private const long SerialVersionUid = 1L;

        public virtual DateTime Time { get; set; }


        public virtual string Type { get; set; }


        public virtual string UserId { get; set; }


        public virtual string GroupId { get; set; }


        public virtual string TaskId { get; set; }


        public virtual string OperationType { get; set; }


        public virtual string AssignerId { get; set; }


        public virtual string TenantId { get; set; }
    }
}
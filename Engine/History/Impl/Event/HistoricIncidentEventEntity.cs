using System;

namespace ESS.FW.Bpm.Engine.History.Impl.Event
{
    /// <summary>
    ///     
    /// </summary>
    [Serializable]
    public class HistoricIncidentEntity : HistoryEvent, IHistoricIncident
    {
        private const long SerialVersionUid = 1L;

        public virtual DateTime CreateTime { get; set; }


        public virtual DateTime EndTime { get; set; }


        public virtual string IncidentType { get; set; }


        public virtual string ActivityId { get; set; }


        public virtual string CauseIncidentId { get; set; }


        public virtual string RootCauseIncidentId { get; set; }


        public virtual string Configuration { get; set; }


        public virtual string IncidentMessage { get; set; }


        public virtual int IncidentState { get; set; }

        public virtual string TenantId { get; set; }


        public virtual string JobDefinitionId { get; set; }


        public virtual bool Open
        {
            get { return IncidentStateFields.Default.StateCode == IncidentState; }
        }

        public virtual bool Deleted
        {
            get { return IncidentStateFields.Deleted.StateCode == IncidentState; }
        }

        public virtual bool Resolved
        {
            get { return IncidentStateFields.Resolved.StateCode == IncidentState; }
        }
    }
}
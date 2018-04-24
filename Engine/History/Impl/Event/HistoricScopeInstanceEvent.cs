using System;

namespace ESS.FW.Bpm.Engine.History.Impl.Event
{
    /// <summary>
    ///     
    ///      Christian Lipphardt
    /// </summary>
    [Serializable]
    public class HistoricScopeInstanceEvent : HistoryEvent
    {
        private const long SerialVersionUid = 1L;

        protected internal long? durationInMillis;

        // getters / setters ////////////////////////////////////

        public virtual DateTime? EndTime { get; set; }


        public virtual DateTime? StartTime { get; set; }


        public virtual long? DurationInMillis
        {
            get
            {
                if (durationInMillis != null)
                    return durationInMillis;
                if ((StartTime != null) && (EndTime != null))
                    return (long)(((DateTime)EndTime) - ((DateTime)StartTime)).TotalMilliseconds;
                return null;
            }
            set { durationInMillis = value; }
        }


        public virtual long? DurationRaw
        {
            get { return durationInMillis; }
        }
    }
}
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.History.Impl;
using ESS.FW.Bpm.Engine.History.Impl.Event;

namespace Engine.Tests.Standalone.History
{
    public class RecordHistoryLevel : IHistoryLevel
    {
        protected internal IList<ProducedHistoryEvent> producedHistoryEvents = new List<ProducedHistoryEvent>();

        protected internal IList<HistoryEventTypes> recordedHistoryEventTypes = new List<HistoryEventTypes>();

        public RecordHistoryLevel()
        {
        }

        public RecordHistoryLevel(params HistoryEventTypes[] filterHistoryEventType)
        {
           Collections.AddAll(this.RecordedHistoryEventTypes, filterHistoryEventType);
        }

        public virtual IList<HistoryEventTypes> RecordedHistoryEventTypes
        {
            get { return recordedHistoryEventTypes; }
        }

        public virtual IList<ProducedHistoryEvent> ProducedHistoryEvents
        {
            get { return producedHistoryEvents; }
        }

        public virtual int Id
        {
            get { return 42; }
        }

        public virtual string Name
        {
            get { return "recordHistoryLevel"; }
        }

        public virtual bool IsHistoryEventProduced(HistoryEventTypes eventType, object entity)
        {
            if (recordedHistoryEventTypes.Count == 0 || recordedHistoryEventTypes.Contains(eventType))
                producedHistoryEvents.Add(new ProducedHistoryEvent(eventType, entity));
            return true;
        }

        public class ProducedHistoryEvent
        {
            public readonly object Entity;

            public readonly HistoryEventTypes EventType;

            public ProducedHistoryEvent(HistoryEventTypes eventType, object entity)
            {
                EventType = eventType;
                Entity = entity;
            }
        }
    }
}
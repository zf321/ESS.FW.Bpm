using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.Bpm.Engine.History.Impl.Producer;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.History.Impl.Parser
{
    /// <summary>
    ///     
    /// </summary>
    public class ActivityInstanceUpdateListener : HistoryTaskListener
    {
        public ActivityInstanceUpdateListener(IHistoryEventProducer historyEventProducer, IHistoryLevel historyLevel)
            : base(historyEventProducer, historyLevel)
        {
        }

        protected internal override HistoryEvent CreateHistoryEvent(IDelegateTask task, ExecutionEntity execution)
        {
            if (HistoryLevel.IsHistoryEventProduced(HistoryEventTypes.ActivityInstanceUpdate, execution))
                return EventProducer.CreateActivityInstanceUpdateEvt(execution, task);
            return null;
        }
    }
}
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.Bpm.Engine.History.Impl.Producer;

namespace ESS.FW.Bpm.Engine.History.Impl.Parser
{
    /// <summary>
    ///     
    /// </summary>
    public class ActivityInstanceStartListener : HistoryExecutionListener
    {
        public ActivityInstanceStartListener(IHistoryEventProducer historyEventProducer, IHistoryLevel historyLevel)
            : base(historyEventProducer, historyLevel)
        {
        }

        protected internal override HistoryEvent CreateHistoryEvent(IDelegateExecution execution)
        {
            if (HistoryLevel.IsHistoryEventProduced(HistoryEventTypes.ActivityInstanceStart, execution))
                // Todo: IHistoryEventProducer 实现
                if (EventProducer != null)
                    return EventProducer.CreateActivityInstanceStartEvt(execution);
            return null;
        }
    }
}
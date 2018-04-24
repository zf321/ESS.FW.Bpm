using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.Bpm.Engine.History.Impl.Producer;

namespace ESS.FW.Bpm.Engine.History.Impl.Parser
{
    /// <summary>
    ///     
    /// </summary>
    public class ActivityInstanceEndListener : HistoryExecutionListener
    {
        public ActivityInstanceEndListener(IHistoryEventProducer historyEventProducer, IHistoryLevel historyLevel)
            : base(historyEventProducer, historyLevel)
        {
        }

        protected internal override HistoryEvent CreateHistoryEvent(IDelegateExecution execution)
        {
            if (HistoryLevel.IsHistoryEventProduced(HistoryEventTypes.ActivityInstanceEnd, execution))
            {
                // Todo: IHistoryEventProducer 实现
                if(EventProducer!=null)
                    return EventProducer.CreateActivityInstanceEndEvt(execution);
            }
            return null;
        }
    }
}
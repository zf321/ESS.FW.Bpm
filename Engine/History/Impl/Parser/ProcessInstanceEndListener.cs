using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.Bpm.Engine.History.Impl.Producer;

namespace ESS.FW.Bpm.Engine.History.Impl.Parser
{
    /// <summary>
    ///     
    /// </summary>
    public class ProcessInstanceEndListener : HistoryExecutionListener
    {
        public ProcessInstanceEndListener(IHistoryEventProducer historyEventProducer, IHistoryLevel historyLevel)
            : base(historyEventProducer, historyLevel)
        {
        }

        protected internal override HistoryEvent CreateHistoryEvent(IDelegateExecution execution)
        {
            if (HistoryLevel.IsHistoryEventProduced(HistoryEventTypes.ProcessInstanceEnd, execution))
                return EventProducer.CreateProcessInstanceEndEvt(execution);
            return null;
        }
    }
}
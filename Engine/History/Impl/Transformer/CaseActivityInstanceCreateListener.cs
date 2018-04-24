using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.Bpm.Engine.History.Impl.Producer;

namespace ESS.FW.Bpm.Engine.History.Impl.Transformer
{
    /// <summary>
    ///     
    /// </summary>
    public class CaseActivityInstanceCreateListener : HistoryCaseExecutionListener
    {
        public CaseActivityInstanceCreateListener(ICmmnHistoryEventProducer historyEventProducer,
            IHistoryLevel historyLevel) : base(historyEventProducer, historyLevel)
        {
        }

        protected internal override HistoryEvent CreateHistoryEvent(IDelegateCaseExecution caseExecution)
        {
            if (HistoryLevel.IsHistoryEventProduced(HistoryEventTypes.CaseActivityInstanceCreate, caseExecution))
                return EventProducer.CreateCaseActivityInstanceCreateEvt(caseExecution);
            return null;
        }
    }
}
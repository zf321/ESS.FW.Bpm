using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.Bpm.Engine.History.Impl.Producer;

namespace ESS.FW.Bpm.Engine.History.Impl.Transformer
{
    /// <summary>
    ///     
    /// </summary>
    public class CaseActivityInstanceEndListener : HistoryCaseExecutionListener
    {
        public CaseActivityInstanceEndListener(ICmmnHistoryEventProducer historyEventProducer,
            IHistoryLevel historyLevel)
            : base(historyEventProducer, historyLevel)
        {
        }

        protected internal override HistoryEvent CreateHistoryEvent(IDelegateCaseExecution caseExecution)
        {
            if (HistoryLevel.IsHistoryEventProduced(HistoryEventTypes.CaseActivityInstanceEnd, caseExecution))
                return EventProducer.CreateCaseActivityInstanceEndEvt(caseExecution);
            return null;
        }
    }
}
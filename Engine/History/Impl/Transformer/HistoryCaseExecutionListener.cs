using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.Bpm.Engine.History.Impl.Producer;

namespace ESS.FW.Bpm.Engine.History.Impl.Transformer
{
    /// <summary>
    ///     
    /// </summary>
    public abstract class HistoryCaseExecutionListener : ICaseExecutionListener
    {
        protected internal ICmmnHistoryEventProducer EventProducer;
        protected internal IHistoryLevel HistoryLevel;

        public HistoryCaseExecutionListener(ICmmnHistoryEventProducer historyEventProducer, IHistoryLevel historyLevel)
        {
            EventProducer = historyEventProducer;
            this.HistoryLevel = historyLevel;
        }

//JAVA TO C# CONVERTER WARNING: MethodInfo 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void notify(org.camunda.bpm.engine.delegate.DelegateCaseExecution caseExecution) throws Exception
        public virtual void Notify(IDelegateCaseExecution caseExecution)
        {
            var historyEvent = CreateHistoryEvent(caseExecution);

            if (historyEvent != null)
                Context.ProcessEngineConfiguration.HistoryEventHandler.HandleEvent(historyEvent);
        }

        protected internal abstract HistoryEvent CreateHistoryEvent(IDelegateCaseExecution caseExecution);
    }
}
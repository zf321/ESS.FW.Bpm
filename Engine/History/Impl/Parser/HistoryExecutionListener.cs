using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.Bpm.Engine.History.Impl.Producer;

namespace ESS.FW.Bpm.Engine.History.Impl.Parser
{
    /// <summary>
    ///     <para>
    ///         An <seealso cref="IExecutionListener" /> implementation that delegates to a
    ///         <seealso cref="IHistoryEventProducer" />.
    ///     </para>
    ///     <para>
    ///         This allows plugging the history as an execution listener into process
    ///         execution and make sure history events are generated as we move through the
    ///         process.
    ///         
    ///     </para>
    /// </summary>
    public abstract class HistoryExecutionListener : IDelegateListener<IBaseDelegateExecution>
    {
        protected internal readonly IHistoryEventProducer EventProducer;
        protected internal IHistoryLevel HistoryLevel;

        public HistoryExecutionListener(IHistoryEventProducer historyEventProducer, IHistoryLevel historyLevel)
        {
            EventProducer = historyEventProducer;
            this.HistoryLevel = historyLevel;
        }

//JAVA TO C# CONVERTER WARNING: MethodInfo 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void notify(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
        public virtual void Notify(IBaseDelegateExecution execution)
        {
            // get the event handler
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.history.handler.HistoryEventHandler historyEventHandler = org.camunda.bpm.engine.impl.context.Context.GetProcessEngineConfiguration().getHistoryEventHandler();
            var historyEventHandler = Context.ProcessEngineConfiguration.HistoryEventHandler;

            // delegate creation of the history event to the producer
            var historyEvent = CreateHistoryEvent((IDelegateExecution) execution);

            if (historyEvent != null)
                historyEventHandler.HandleEvent(historyEvent);
        }

        protected internal abstract HistoryEvent CreateHistoryEvent(IDelegateExecution execution);
    }
}
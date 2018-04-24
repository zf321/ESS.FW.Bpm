using System;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.Bpm.Engine.History.Impl.Producer;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.History.Impl.Parser
{
    /// <summary>
    ///     <para>
    ///         A <seealso cref="ITaskListener" /> implementation that delegates to a
    ///         <seealso cref="IHistoryEventProducer" />.
    ///         
    ///     </para>
    /// </summary>
    public abstract class HistoryTaskListener : ITaskListener
    {
        protected internal readonly IHistoryEventProducer EventProducer;
        protected internal IHistoryLevel HistoryLevel;

        public HistoryTaskListener(IHistoryEventProducer historyEventProducer, IHistoryLevel historyLevel)
        {
            EventProducer = historyEventProducer;
            this.HistoryLevel = historyLevel;
        }

        public virtual void Notify(IDelegateTask task)
        {      
            // get the event handler
            var historyEventHandler = Context.ProcessEngineConfiguration.HistoryEventHandler;

            ExecutionEntity execution = ((TaskEntity)task).GetExecution();

            if (execution != null)
            {

                // delegate creation of the history event to the producer
                HistoryEvent historyEvent = CreateHistoryEvent(task, execution);

                if (historyEvent != null)
                {
                    // pass the event to the handler
                    historyEventHandler.HandleEvent(historyEvent);
                }

            }
        }

        protected internal abstract HistoryEvent CreateHistoryEvent(IDelegateTask ITask, ExecutionEntity execution);
    }
}
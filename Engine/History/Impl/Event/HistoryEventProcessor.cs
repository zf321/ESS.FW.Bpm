using System.Collections.Generic;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.History.Impl.Producer;

namespace ESS.FW.Bpm.Engine.History.Impl.Event
{
    /// <summary>
    ///     <para>The <seealso cref="HistoryEventProcessor" /> should be used to process an history event.</para>
    ///     <para>
    ///         The <seealso cref="HistoryEvent" /> will be created with the help of the
    ///         <seealso cref="IHistoryEventProducer" />
    ///         from the <seealso cref="ProcessEngineConfiguration" /> and the given implementation of the
    ///         <seealso cref="HistoryEventCreator" /> which uses the producer object to create an
    ///         <seealso cref="HistoryEvent" />. The <seealso cref="HistoryEvent" /> will be handled by the
    ///         <seealso cref="HistoryEventHandler" /> from the <seealso cref="ProcessEngineConfiguration" />.
    ///     </para>
    ///     
    ///     
    ///         
    /// </summary>
    public class HistoryEventProcessor
    {
        /// <summary>
        ///     Process an <seealso cref="HistoryEvent" /> and handle them directly after creation.
        ///     The <seealso cref="HistoryEvent" /> is created with the help of the given
        ///     <seealso cref="HistoryEventCreator" /> implementation.
        /// </summary>
        /// <param name="creator"> the creator is used to create the <seealso cref="HistoryEvent" /> which should be thrown </param>
        public static void ProcessHistoryEvents(HistoryEventCreator creator)
        {
            
            var historyEventProducer = Context.ProcessEngineConfiguration.HistoryEventProducer;
            var historyEventHandler = Context.ProcessEngineConfiguration.HistoryEventHandler;

            IList<HistoryEvent> eventList = new List<HistoryEvent>();
            if (historyEventProducer != null)
            {
                var singleEvent = creator.CreateHistoryEvent(historyEventProducer);
                if (singleEvent != null)
                    historyEventHandler.HandleEvent(singleEvent);

                eventList = creator.CreateHistoryEvents(historyEventProducer);
            }

            historyEventHandler.HandleEvents(eventList);
        }

        /// <summary>
        ///     The <seealso cref="HistoryEventCreator" /> interface which is used to interchange the implementation
        ///     of the creation of different HistoryEvents.
        /// </summary>
        public class HistoryEventCreator
        {
            /// <summary>
            ///     Creates the <seealso cref="HistoryEvent" /> with the help off the given
            ///     <seealso cref="IHistoryEventProducer" />.
            /// </summary>
            /// <param name="producer"> the producer which is used for the creation </param>
            /// <returns> the created <seealso cref="HistoryEvent" /> </returns>
            public virtual HistoryEvent CreateHistoryEvent(IHistoryEventProducer producer)
            {
                return null;
            }

            public virtual IList<HistoryEvent> CreateHistoryEvents(IHistoryEventProducer producer)
            {
                return new List<HistoryEvent>();
            }
        }
    }
}
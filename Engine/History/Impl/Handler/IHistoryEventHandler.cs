using System.Collections.Generic;
using ESS.FW.Bpm.Engine.History.Impl.Event;

namespace ESS.FW.Bpm.Engine.History.Impl.Handler
{
    /// <summary>
    ///     <para>The interface for implementing an history event handler.</para>
    ///     <para>
    ///         The <seealso cref="IHistoryEventHandler" /> is responsible for consuming the event. Many different
    ///         implementations of this interface can be imagined. Some implementations might persist the
    ///         event to a database, others might persist the event to a message queue and handle it
    ///         asynchronously.
    ///     </para>
    ///     <para>
    ///         The default implementation of this interface is <seealso cref="DbHistoryEventHandler" /> which
    ///         persists events to a database.
    ///     </para>
    ///     
    /// </summary>
    public interface IHistoryEventHandler
    {
        /// <summary>
        ///     Called by the process engine when an history event is fired.
        /// </summary>
        /// <param name="historyEvent"> the <seealso cref="HistoryEvent" /> that is about to be fired. </param>
        void HandleEvent(HistoryEvent historyEvent);

        /// <summary>
        ///     Called by the process engine when an history event is fired.
        /// </summary>
        /// <param name="historyEvents"> the <seealso cref="HistoryEvent" /> that is about to be fired. </param>
        void HandleEvents(IList<HistoryEvent> historyEvents);
    }
}
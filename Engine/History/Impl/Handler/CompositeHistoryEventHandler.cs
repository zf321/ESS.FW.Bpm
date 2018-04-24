using System.Collections.Generic;
using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.Bpm.Engine.Impl.Util;

namespace ESS.FW.Bpm.Engine.History.Impl.Handler
{
    /// <summary>
    ///     A <seealso cref="IHistoryEventHandler" /> implementation which delegates to a list of
    ///     <seealso cref="IHistoryEventHandler" />.
    /// </summary>
    public class CompositeHistoryEventHandler : IHistoryEventHandler
    {
        /// <summary>
        ///     The list of <seealso cref="IHistoryEventHandler" /> which consume the event.
        /// </summary>
        protected internal readonly IList<IHistoryEventHandler> HistoryEventHandlers = new List<IHistoryEventHandler>();

        /// <summary>
        ///     Non-argument constructor for default initialization.
        /// </summary>
        public CompositeHistoryEventHandler()
        {
        }

        /// <summary>
        ///     Constructor that takes a varargs parameter <seealso cref="IHistoryEventHandler" /> that
        ///     consume the event.
        /// </summary>
        /// <param name="historyEventHandlers">
        ///     the list of <seealso cref="IHistoryEventHandler" /> that consume the event.
        /// </param>
        public CompositeHistoryEventHandler(params IHistoryEventHandler[] historyEventHandlers)
        {
            initializeHistoryEventHandlers(historyEventHandlers);
        }

        /// <summary>
        ///     Constructor that takes a list of <seealso cref="IHistoryEventHandler" /> that consume
        ///     the event.
        /// </summary>
        /// <param name="historyEventHandlers">
        ///     the list of <seealso cref="IHistoryEventHandler" /> that consume the event.
        /// </param>
        public CompositeHistoryEventHandler(IList<IHistoryEventHandler> historyEventHandlers)
        {
            initializeHistoryEventHandlers(historyEventHandlers);
        }

        public virtual void HandleEvent(HistoryEvent historyEvent)
        {
            foreach (var historyEventHandler in HistoryEventHandlers)
                historyEventHandler.HandleEvent(historyEvent);
        }

        public virtual void HandleEvents(IList<HistoryEvent> historyEvents)
        {
            foreach (var historyEvent in historyEvents)
                HandleEvent(historyEvent);
        }

        /// <summary>
        ///     Initialize <seealso cref="#historyEventHandlers" /> with data transfered from constructor
        /// </summary>
        /// <param name="historyEventHandlers"> </param>
        private void initializeHistoryEventHandlers(IList<IHistoryEventHandler> historyEventHandlers)
        {
            EnsureUtil.EnsureNotNull("History event handler", historyEventHandlers);
            foreach (var historyEventHandler in historyEventHandlers)
            {
                EnsureUtil.EnsureNotNull("History event handler", historyEventHandler);
                HistoryEventHandlers.Add(historyEventHandler);
            }
        }

        /// <summary>
        ///     Adds the <seealso cref="IHistoryEventHandler" /> to the list of
        ///     <seealso cref="IHistoryEventHandler" /> that consume the event.
        /// </summary>
        /// <param name="historyEventHandler">
        ///     the <seealso cref="IHistoryEventHandler" /> that consume the event.
        /// </param>
        public virtual void Add(IHistoryEventHandler historyEventHandler)
        {
            EnsureUtil.EnsureNotNull("History event handler", historyEventHandler);
            HistoryEventHandlers.Add(historyEventHandler);
        }
    }
}
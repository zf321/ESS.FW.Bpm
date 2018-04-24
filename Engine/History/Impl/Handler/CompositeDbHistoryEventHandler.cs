using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.History.Impl.Handler
{
    /// <summary>
    ///     A <seealso cref="CompositeHistoryEventHandler" /> implementation which additionally adds
    ///     to the list of <seealso cref="IHistoryEventHandler" /> the <seealso cref="DbHistoryEventHandler" />
    ///     which persists events to a database.
    ///     Alexander Tyatenkov
    /// </summary>
    public class CompositeDbHistoryEventHandler : CompositeHistoryEventHandler
    {
        /// <summary>
        ///     Non-argument constructor that adds <seealso cref="DbHistoryEventHandler" /> to the
        ///     list of <seealso cref="IHistoryEventHandler" />.
        /// </summary>
        public CompositeDbHistoryEventHandler()
        {
            addDefaultDbHistoryEventHandler();
        }

        /// <summary>
        ///     Constructor that takes a varargs parameter <seealso cref="IHistoryEventHandler" /> that
        ///     consume the event and adds <seealso cref="DbHistoryEventHandler" /> to the list of
        ///     <seealso cref="IHistoryEventHandler" />.
        /// </summary>
        /// <param name="historyEventHandlers">
        ///     the list of <seealso cref="IHistoryEventHandler" /> that consume the event.
        /// </param>
        public CompositeDbHistoryEventHandler(params IHistoryEventHandler[] historyEventHandlers)
            : base(historyEventHandlers)
        {
            addDefaultDbHistoryEventHandler();
        }

        /// <summary>
        ///     Constructor that takes a list of <seealso cref="IHistoryEventHandler" /> that consume
        ///     the event and adds <seealso cref="DbHistoryEventHandler" /> to the list of
        ///     <seealso cref="IHistoryEventHandler" />.
        /// </summary>
        /// <param name="historyEventHandlers">
        ///     the list of <seealso cref="IHistoryEventHandler" /> that consume the event.
        /// </param>
        public CompositeDbHistoryEventHandler(IList<IHistoryEventHandler> historyEventHandlers)
            : base(historyEventHandlers)
        {
            addDefaultDbHistoryEventHandler();
        }

        /// <summary>
        ///     Add <seealso cref="DbHistoryEventHandler" /> to the list of
        ///     <seealso cref="IHistoryEventHandler" />.
        /// </summary>
        private void addDefaultDbHistoryEventHandler()
        {
            HistoryEventHandlers.Add(new DbHistoryEventHandler());
        }
    }
}
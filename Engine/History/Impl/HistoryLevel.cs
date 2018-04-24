using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.Bpm.Engine.History.Impl.Producer;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.History.Impl
{
    /// <summary>
    ///     <para>
    ///         The history level controls what kind of data is logged to the history database.
    ///         More formally, it controls which history events are produced by the <seealso cref="IHistoryEventProducer" />.
    ///     </para>
    ///     <para>
    ///         <strong>Built-in history levels:</strong> The process engine provides a set of built-in history levels
    ///         as default configuration. The built-in history levels are:
    ///         <ul>
    ///             <li>
    ///                 <seealso cref="#HISTORY_LEVEL_NONE" />
    ///             </li>
    ///             <li>
    ///                 <seealso cref="#HISTORY_LEVEL_ACTIVITY" />
    ///             </li>
    ///             <li>
    ///                 <seealso cref="#HISTORY_LEVEL_AUDIT" />
    ///             </li>
    ///             <li>
    ///                 <seealso cref="#HISTORY_LEVEL_FULL" />
    ///             </li>
    ///         </ul>
    ///         This class provides singleton instances of these history levels as constants.
    ///     </para>
    ///     <para>
    ///         <strong>Custom history levels:</strong>In order to implement a custom history level,
    ///         the following steps are necessary:
    ///         <ul>
    ///             <li>
    ///                 Provide a custom implementation of this interface. Note: Make sure you choose unique values for
    ///                 <seealso cref="#getName()" /> and <seealso cref="#getId()" />
    ///             </li>
    ///             <li>
    ///                 Add an instance of the custom implementation through
    ///                 <seealso cref="ProcessEngineConfigurationImpl#setCustomHistoryLevels(java.Util.List)" />
    ///             </li>
    ///             <li>
    ///                 use the name of your history level (as returned by <seealso cref="#getName()" /> as value for
    ///                 <seealso cref="ProcessEngineConfiguration#setHistory(String)" />
    ///             </li>
    ///         </ul>
    ///     </para>
    ///     
    ///     @since 7.2
    /// </summary>
    public interface IHistoryLevel
    {
        /// <summary>
        ///     An unique id identifying the history level.
        ///     The id is used internally to uniquely identify the history level and also stored in the database.
        /// </summary>
        int Id { get; }

        /// <summary>
        ///     An unique name identifying the history level.
        ///     The name of the history level can be used when configuring the process engine.
        /// </summary>
        /// <seealso cref=
        /// <seealso cref="ProcessEngineConfiguration#setHistory(String)" />
        /// </seealso>
        string Name { get; }

        /// <summary>
        ///     Returns true if a given history event should be produced.
        /// </summary>
        /// <param name="eventType"> the type of the history event which is about to be produced </param>
        /// <param name="entity">
        ///     the runtime structure used to produce the history event. Examples <seealso cref="ExecutionEntity" />,
        ///     <seealso cref="TaskEntity" />, <seealso cref="VariableInstanceEntity" />, ... If a 'null' value is provided, the
        ///     implementation
        ///     should return true if events of this type should be produced "in general".
        /// </param>
        bool IsHistoryEventProduced(HistoryEventTypes eventType, object entity);
    }

    public static class HistoryLevelFields
    {
        public static readonly IHistoryLevel HistoryLevelNone = new HistoryLevelNone();
        public static readonly IHistoryLevel HistoryLevelActivity = new HistoryLevelActivity();
        public static readonly IHistoryLevel HistoryLevelAudit = new HistoryLevelAudit();
        public static readonly IHistoryLevel HistoryLevelFull = new HistoryLevelFull();
    }
}
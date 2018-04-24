using ESS.FW.Bpm.Engine.Impl.migration.instance;
using ESS.FW.Bpm.Engine.Impl.migration.instance.parser;

namespace ESS.FW.Bpm.Engine.Impl.Pvm.Delegate
{
    /// <summary>
    ///     
    /// </summary>
    public interface IMigrationObserverBehavior
    {
        /// <summary>
        ///     Implement to perform activity-specific migration behavior that is not
        ///     covered by the regular migration procedure. Called after the scope execution and any ancestor executions
        ///     have been migrated to their target activities and process definition.
        /// </summary>
        void MigrateScope(IActivityExecution scopeExecution);

        /// <summary>
        ///     Callback to implement behavior specific parsing (e.g. adding additional dependent entities).
        /// </summary>
        void OnParseMigratingInstance(MigratingInstanceParseContext parseContext,
            MigratingActivityInstance migratingInstance);
    }
}
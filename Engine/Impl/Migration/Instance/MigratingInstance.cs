namespace ESS.FW.Bpm.Engine.Impl.migration.instance
{
    /// <summary>
    ///     
    /// </summary>
    public interface IMigratingInstance
    {
        bool Detached { get; }

        /// <summary>
        ///     Detach this instance's state from its owning instance and the execution tree
        /// </summary>
        void DetachState();

        /// <summary>
        ///     Restore this instance's state as a subordinate to the given activity instance
        ///     (e.g. in the execution tree).
        ///     Restoration should restore the state that was detached
        ///     before.
        /// </summary>
        void AttachState(MigratingScopeInstance targetActivityInstance);

        /// <summary>
        ///     Restore this instance's state as a subordinate to the given transition instance
        ///     (e.g. in the execution tree).
        ///     Restoration should restore the state that was detached
        ///     before.
        /// </summary>
        void AttachState(MigratingTransitionInstance targetTransitionInstance);

        /// <summary>
        ///     Migrate state from the source process definition
        ///     to the target process definition.
        /// </summary>
        void MigrateState();

        /// <summary>
        ///     Migrate instances that are aggregated by this instance
        ///     (e.g. an activity instance aggregates task instances).
        /// </summary>
        void MigrateDependentEntities();
    }
}
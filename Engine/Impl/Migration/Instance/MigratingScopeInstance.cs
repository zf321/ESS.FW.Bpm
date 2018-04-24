using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Impl.migration.instance
{
    /// <summary>
    ///     A process element instance that can have other process element instances as children
    ///     
    /// </summary>
    public abstract class MigratingScopeInstance : MigratingProcessElementInstance
    {
        /// <summary>
        ///     gets all children
        /// </summary>
        public abstract ICollection<MigratingProcessElementInstance> Children { get; }

        /// <summary>
        ///     gets those children that are itself scope instances
        /// </summary>
        public abstract ICollection<MigratingScopeInstance> ChildScopeInstances { get; }

        public abstract void RemoveChild(MigratingScopeInstance migratingActivityInstance);

        public abstract void AddChild(MigratingScopeInstance migratingActivityInstance);

        public abstract void RemoveChild(MigratingCompensationEventSubscriptionInstance migratingEventSubscription);

        public abstract void AddChild(MigratingCompensationEventSubscriptionInstance migratingEventSubscription);

        public abstract bool Migrates();

        public abstract void DetachChildren();

        /// <summary>
        ///     removes this scope; parameters are hints and may be ignored by the implementation
        /// </summary>
        public abstract void Remove(bool skipCustomListeners, bool skipIoMappings);

        public abstract void RemoveUnmappedDependentInstances();
    }
}
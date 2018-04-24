using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Migration;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.migration.instance
{
    /// <summary>
    ///     
    /// </summary>
    public abstract class MigratingProcessElementInstance : IMigratingInstance
    {
        // changes from source to target scope during migration
        protected internal ScopeImpl currentScope;

        protected internal IMigrationInstruction migrationInstruction;

        protected internal MigratingScopeInstance ParentInstance;

        protected internal ScopeImpl sourceScope;
        protected internal ScopeImpl targetScope;

        public virtual ScopeImpl SourceScope
        {
            get { return sourceScope; }
        }

        public virtual ScopeImpl TargetScope
        {
            get { return targetScope; }
        }

        public virtual ScopeImpl CurrentScope
        {
            get { return currentScope; }
        }

        public virtual IMigrationInstruction MigrationInstruction
        {
            get { return migrationInstruction; }
        }

        public virtual MigratingScopeInstance Parent
        {
            get { return ParentInstance; }
            set { ParentInstance = value; }
        }

        public virtual MigratingActivityInstance ClosestAncestorActivityInstance
        {
            get
            {
                var ancestorInstance = ParentInstance;

                while (!(ancestorInstance is MigratingActivityInstance))
                    ancestorInstance = ancestorInstance.Parent;

                return (MigratingActivityInstance) ancestorInstance;
            }
        }

        public abstract void MigrateDependentEntities();
        public abstract void MigrateState();
        public abstract void AttachState(MigratingTransitionInstance targetTransitionInstance);
        public abstract void AttachState(MigratingScopeInstance targetActivityInstance);
        public abstract void DetachState();
        public abstract bool Detached { get; }

        public virtual bool MigratesTo(ScopeImpl other)
        {
            return other == targetScope;
        }

        public abstract void AddMigratingDependentInstance(IMigratingInstance migratingInstance);

        public abstract ExecutionEntity ResolveRepresentativeExecution();
    }
}
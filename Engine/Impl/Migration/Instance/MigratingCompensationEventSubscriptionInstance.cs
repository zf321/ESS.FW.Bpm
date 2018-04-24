using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Migration;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.migration.instance
{
    /// <summary>
    ///     
    /// </summary>
    public class MigratingCompensationEventSubscriptionInstance : MigratingProcessElementInstance, IRemovingInstance
    {
        public static readonly MigrationLogger MigrationLogger = ProcessEngineLogger.MigrationLogger;

        protected internal EventSubscriptionEntity EventSubscription;

        public MigratingCompensationEventSubscriptionInstance(IMigrationInstruction migrationInstruction,
            ScopeImpl sourceScope, ScopeImpl targetScope, EventSubscriptionEntity eventSubscription)
        {
            this.migrationInstruction = migrationInstruction;
            this.EventSubscription = eventSubscription;
            this.sourceScope = sourceScope;
            this.targetScope = targetScope;
            currentScope = sourceScope;
        }

        public override bool Detached
        {
            get { return ReferenceEquals(EventSubscription.ExecutionId, null); }
        }

        public override MigratingScopeInstance Parent
        {
            set
            {
                if (ParentInstance != null)
                    ParentInstance.RemoveChild(this);

                ParentInstance = value;

                if (value != null)
                    value.AddChild(this);
            }
        }

        public virtual void Remove()
        {
            //eventSubscription.delete();
        }

        public override void DetachState()
        {
            //eventSubscription.Execution = null;
        }

        public override void AttachState(MigratingTransitionInstance targetTransitionInstance)
        {
            throw MigrationLogger.CannotAttachToTransitionInstance(this);
        }

        public override void MigrateState()
        {
            //eventSubscription.Activity = (ActivityImpl) targetScope;
            //currentScope = targetScope;
        }

        public override void MigrateDependentEntities()
        {
        }

        public override void AddMigratingDependentInstance(IMigratingInstance migratingInstance)
        {
        }

        public override ExecutionEntity ResolveRepresentativeExecution()
        {
            return null;
        }

        public override void AttachState(MigratingScopeInstance targetActivityInstance)
        {
            Parent = targetActivityInstance;

            var representativeExecution = targetActivityInstance.ResolveRepresentativeExecution();
            //eventSubscription.Execution = representativeExecution;
        }
    }
}
using ESS.FW.Bpm.Engine.Impl.Bpmn.Parser;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.migration.instance
{
    public class MigratingEventSubscriptionInstance : IMigratingInstance, IRemovingInstance, IEmergingInstance
    {
        public static readonly MigrationLogger MigrationLogger = ProcessEngineLogger.MigrationLogger;

        protected internal EventSubscriptionDeclaration EventSubscriptionDeclaration;

        protected internal EventSubscriptionEntity EventSubscriptionEntity;
        protected internal EventSubscriptionDeclaration TargetDeclaration;
        protected internal ScopeImpl TargetScope;
        protected internal bool UpdateEvent;

        public MigratingEventSubscriptionInstance(EventSubscriptionEntity eventSubscriptionEntity, ScopeImpl targetScope,
            bool updateEvent, EventSubscriptionDeclaration targetDeclaration)
        {
            this.EventSubscriptionEntity = eventSubscriptionEntity;
            this.TargetScope = targetScope;
            this.UpdateEvent = updateEvent;
            this.TargetDeclaration = targetDeclaration;
        }

        public MigratingEventSubscriptionInstance(EventSubscriptionEntity eventSubscriptionEntity)
            : this(eventSubscriptionEntity, null, false, null)
        {
        }

        public MigratingEventSubscriptionInstance(EventSubscriptionDeclaration eventSubscriptionDeclaration)
        {
            this.EventSubscriptionDeclaration = eventSubscriptionDeclaration;
        }

        public virtual void Create(ExecutionEntity scopeExecution)
        {
            //eventSubscriptionDeclaration.createSubscriptionForExecution(scopeExecution);
        }

        public virtual bool Detached
        {
            get { return ReferenceEquals(EventSubscriptionEntity.ExecutionId, null); }
        }

        public virtual void DetachState()
        {
            //eventSubscriptionEntity.Execution = null;
        }

        public virtual void AttachState(MigratingScopeInstance newOwningInstance)
        {
            //eventSubscriptionEntity.Execution = newOwningInstance.resolveRepresentativeExecution();
        }

        public virtual void AttachState(MigratingTransitionInstance targetTransitionInstance)
        {
            throw MigrationLogger.CannotAttachToTransitionInstance(this);
        }

        public virtual void MigrateState()
        {
            if (UpdateEvent)
            {
                //targetDeclaration.updateSubscription(eventSubscriptionEntity);
            }
            //eventSubscriptionEntity.Activity = (ActivityImpl) targetScope;
        }

        public virtual void MigrateDependentEntities()
        {
            // do nothing
        }

        public virtual void Remove()
        {
            //eventSubscriptionEntity.Delete();
        }
    }
}
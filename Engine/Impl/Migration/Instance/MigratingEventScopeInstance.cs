using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Migration;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.migration.instance
{
    /// <summary>
    ///     
    /// </summary>
    public class MigratingEventScopeInstance : MigratingScopeInstance
    {
        //public static readonly MigrationLogger MIGRATION_LOGGER = ProcessEngineLogger.MIGRATION_LOGGER;

        protected internal ISet<MigratingCompensationEventSubscriptionInstance> ChildCompensationSubscriptionInstances =
            new HashSet<MigratingCompensationEventSubscriptionInstance>();

        protected internal List<MigratingEventScopeInstance> ChildInstances = new List<MigratingEventScopeInstance>();

        protected internal ExecutionEntity EventScopeExecution;
        protected internal IList<IMigratingInstance> MigratingDependentInstances = new List<IMigratingInstance>();

        protected internal MigratingCompensationEventSubscriptionInstance MigratingEventSubscription;

        public MigratingEventScopeInstance(IMigrationInstruction migrationInstruction,
            ExecutionEntity eventScopeExecution, ScopeImpl sourceScope, ScopeImpl targetScope,
            IMigrationInstruction eventSubscriptionInstruction, EventSubscriptionEntity eventSubscription,
            ScopeImpl eventSubscriptionSourceScope, ScopeImpl eventSubscriptionTargetScope)
        {
            MigratingEventSubscription = new MigratingCompensationEventSubscriptionInstance(
                eventSubscriptionInstruction, eventSubscriptionSourceScope, eventSubscriptionTargetScope,
                eventSubscription);
            this.migrationInstruction = migrationInstruction;
            this.EventScopeExecution = eventScopeExecution;

            // compensation handlers (not boundary events)
            this.sourceScope = sourceScope;
            this.targetScope = targetScope;
        }

        /// <summary>
        ///     Creates an emerged scope
        /// </summary>
        public MigratingEventScopeInstance(EventSubscriptionEntity eventSubscription,
            ExecutionEntity eventScopeExecution, ScopeImpl targetScope)
        {
            MigratingEventSubscription = new MigratingCompensationEventSubscriptionInstance(null, null, targetScope,
                eventSubscription);
            this.EventScopeExecution = eventScopeExecution;

            // compensation handlers (not boundary events)
            // or parent flow scopes
            this.targetScope = targetScope;
            currentScope = targetScope;
        }

        public override bool Detached
        {
            get { return ReferenceEquals(EventScopeExecution.ParentId, null); }
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

        public override ICollection<MigratingProcessElementInstance> Children
        {
            get
            {
                ISet<MigratingProcessElementInstance> children =
                    new HashSet<MigratingProcessElementInstance>(ChildInstances);
                children.UnionWith(ChildCompensationSubscriptionInstances);
                return children;
            }
        }

        public override ICollection<MigratingScopeInstance> ChildScopeInstances
        {
            get { return new HashSet<MigratingScopeInstance>(ChildInstances); }
        }

        public virtual MigratingCompensationEventSubscriptionInstance EventSubscription
        {
            get { return MigratingEventSubscription; }
        }

        public override void DetachState()
        {
            MigratingEventSubscription.DetachState();
            //eventScopeExecution.Parent = null;
        }

        public override void AttachState(MigratingScopeInstance targetActivityInstance)
        {
            Parent = targetActivityInstance;

            MigratingEventSubscription.AttachState(targetActivityInstance);

            var representativeExecution = targetActivityInstance.ResolveRepresentativeExecution();
            //eventScopeExecution.Parent = representativeExecution;
        }

        public override void AttachState(MigratingTransitionInstance targetTransitionInstance)
        {
            //throw MIGRATION_LOGGER.cannotAttachToTransitionInstance(this);
        }

        public override void MigrateState()
        {
            MigratingEventSubscription.MigrateState();

            //eventScopeExecution.setActivity((ActivityImpl) targetScope);
            //eventScopeExecution.setProcessDefinition(targetScope.ProcessDefinition);

            currentScope = targetScope;
        }

        public override void MigrateDependentEntities()
        {
            foreach (var dependentEntity in MigratingDependentInstances)
                dependentEntity.MigrateState();
        }

        public override void AddMigratingDependentInstance(IMigratingInstance migratingInstance)
        {
            MigratingDependentInstances.Add(migratingInstance);
        }

        public override ExecutionEntity ResolveRepresentativeExecution()
        {
            return EventScopeExecution;
        }

        public override void RemoveChild(MigratingScopeInstance migratingScopeInstance)
        {
            ChildInstances.Remove((MigratingEventScopeInstance) migratingScopeInstance);
        }

        public override void AddChild(MigratingScopeInstance migratingScopeInstance)
        {
            if (migratingScopeInstance is MigratingEventScopeInstance)
                ChildInstances.Add((MigratingEventScopeInstance) migratingScopeInstance);
        }

        public override void AddChild(MigratingCompensationEventSubscriptionInstance migratingEventSubscription)
        {
            ChildCompensationSubscriptionInstances.Add(migratingEventSubscription);
        }

        public override void RemoveChild(MigratingCompensationEventSubscriptionInstance migratingEventSubscription)
        {
            ChildCompensationSubscriptionInstances.Remove(migratingEventSubscription);
        }

        public override bool Migrates()
        {
            return targetScope != null;
        }

        public override void DetachChildren()
        {
            ISet<MigratingProcessElementInstance> childrenCopy = new HashSet<MigratingProcessElementInstance>(Children);
            foreach (var child in childrenCopy)
                child.DetachState();
        }

        public override void Remove(bool skipCustomListeners, bool skipIoMappings)
        {
            // never invokes listeners and io mappings because this does not remove an active
            // activity instance
            //eventScopeExecution.remove();
            MigratingEventSubscription.Remove();
            Parent = null;
        }

        public override void RemoveUnmappedDependentInstances()
        {
        }
    }
}
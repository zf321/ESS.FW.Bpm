using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.Bpm.Engine.History.Impl.Producer;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Migration;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Runtime;

namespace ESS.FW.Bpm.Engine.Impl.migration.instance
{
    /// <summary>
    ///     
    /// </summary>
    public class MigratingActivityInstance : MigratingScopeInstance, IMigratingInstance
    {
        //public static readonly MigrationLogger MIGRATION_LOGGER = ProcessEngineLogger.MIGRATION_LOGGER;
        protected internal bool ActiveState;

        protected internal IActivityInstance activityInstance;

        protected internal ISet<MigratingActivityInstance> childActivityInstances =
            new HashSet<MigratingActivityInstance>();

        protected internal ISet<MigratingEventScopeInstance> childCompensationInstances =
            new HashSet<MigratingEventScopeInstance>();

        protected internal ISet<MigratingCompensationEventSubscriptionInstance> ChildCompensationSubscriptionInstances =
            new HashSet<MigratingCompensationEventSubscriptionInstance>();

        protected internal ISet<MigratingTransitionInstance> childTransitionInstances =
            new HashSet<MigratingTransitionInstance>();

        protected internal IList<IEmergingInstance> EmergingDependentInstances = new List<IEmergingInstance>();

        // behaves differently if the current activity is scope or not
        protected internal IMigratingActivityInstanceBehavior InstanceBehavior;
        protected internal IList<IMigratingInstance> migratingDependentInstances = new List<IMigratingInstance>();

        protected internal IList<IRemovingInstance> RemovingDependentInstances = new List<IRemovingInstance>();
        // scope execution for actual scopes,
        // concurrent execution in case of non-scope activity with expanded tree
        protected internal ExecutionEntity RepresentativeExecution;

        /// <summary>
        ///     Creates a migrating activity instances
        /// </summary>
        public MigratingActivityInstance(IActivityInstance activityInstance, IMigrationInstruction migrationInstruction,
            ScopeImpl sourceScope, ScopeImpl targetScope, ExecutionEntity scopeExecution)
        {
            this.activityInstance = activityInstance;
            this.migrationInstruction = migrationInstruction;
            this.sourceScope = sourceScope;
            currentScope = sourceScope;
            this.targetScope = targetScope;
            RepresentativeExecution = scopeExecution;
            InstanceBehavior = DetermineBehavior(sourceScope);

            if ((activityInstance.ChildActivityInstances.Length == 0) &&
                (activityInstance.ChildTransitionInstances.Length == 0))
                ActiveState = RepresentativeExecution.IsActive;
        }

        /// <summary>
        ///     Creates an emerged activity instance
        /// </summary>
        public MigratingActivityInstance(ScopeImpl targetScope, ExecutionEntity scopeExecution)
        {
            this.targetScope = targetScope;
            currentScope = targetScope;
            RepresentativeExecution = scopeExecution;
            InstanceBehavior = DetermineBehavior(targetScope);
        }

        public virtual IList<IMigratingInstance> MigratingDependentInstances
        {
            get { return migratingDependentInstances; }
        }

        public virtual IActivityInstance ActivityInstance
        {
            get { return activityInstance; }
        }

        public virtual string ActivityInstanceId
        {
            get
            {
                if (activityInstance != null)
                    return activityInstance.Id;
                // - this branch is only executed for emerging activity instances
                // - emerging activity instances are never leaf activities
                // - therefore it is fine to always look up the activity instance id on the parent
                var execution = ResolveRepresentativeExecution();
                //return execution.ParentActivityInstanceId;
                return string.Empty;
            }
        }

        /// <summary>
        ///     Returns a copy of all children, modifying the returned set does not have any further effect.
        /// </summary>
        public override ICollection<MigratingProcessElementInstance> Children
        {
            get
            {
                ISet<MigratingProcessElementInstance> childInstances = new HashSet<MigratingProcessElementInstance>();
                //childInstances.addAll(childActivityInstances);
                //childInstances.addAll(childTransitionInstances);
                //childInstances.addAll(childCompensationInstances);
                //childInstances.addAll(childCompensationSubscriptionInstances);
                return childInstances;
            }
        }

        public override ICollection<MigratingScopeInstance> ChildScopeInstances
        {
            get
            {
                ISet<MigratingScopeInstance> childInstances = new HashSet<MigratingScopeInstance>();
                //childInstances.Add(childActivityInstances);
                //childInstances.addAll(childCompensationInstances);
                return childInstances;
            }
        }

        public virtual ISet<MigratingActivityInstance> ChildActivityInstances
        {
            get { return childActivityInstances; }
        }

        public virtual ISet<MigratingTransitionInstance> ChildTransitionInstances
        {
            get { return childTransitionInstances; }
        }

        public virtual ISet<MigratingEventScopeInstance> ChildCompensationInstances
        {
            get { return childCompensationInstances; }
        }

        public override MigratingScopeInstance Parent
        {
            get { return (MigratingActivityInstance) base.Parent; }
            set
            {
                if (ParentInstance != null)
                    ParentInstance.RemoveChild(this);

                ParentInstance = value;

                if (value != null)
                    value.AddChild(this);
            }
        }

        public override bool Detached
        {
            get { return InstanceBehavior.Detached; }
        }

        public override void DetachState()
        {
            DetachDependentInstances();

            InstanceBehavior.DetachState();

            //setParent(null);
        }

        public override void AttachState(MigratingScopeInstance activityInstance)
        {
            //setParent(activityInstance);
            InstanceBehavior.AttachState();

            foreach (var dependentInstance in migratingDependentInstances)
                dependentInstance.AttachState(this);
        }

        public override void AttachState(MigratingTransitionInstance targetTransitionInstance)
        {
            //throw MIGRATION_LOGGER.cannotAttachToTransitionInstance(this);
        }

        public override void MigrateDependentEntities()
        {
            foreach (var migratingInstance in migratingDependentInstances)
            {
                migratingInstance.MigrateState();
                migratingInstance.MigrateDependentEntities();
            }

            var representativeExecution = ResolveRepresentativeExecution();
            foreach (var emergingInstance in EmergingDependentInstances)
                emergingInstance.Create(representativeExecution);
        }

        public override void MigrateState()
        {
            InstanceBehavior.MigrateState();
        }


        protected internal virtual IMigratingActivityInstanceBehavior DetermineBehavior(ScopeImpl scope)
        {
            if (scope.IsScope)
                return new MigratingScopeActivityInstanceBehavior(this);
            return new MigratingNonScopeActivityInstanceBehavior(this);
        }

        public override void DetachChildren()
        {
            ISet<MigratingActivityInstance> childrenCopy = new HashSet<MigratingActivityInstance>(childActivityInstances);
            // First detach all dependent entities, only then detach the activity instances.
            // This is because detaching activity instances may trigger execution tree compaction which in turn
            // may overwrite certain dependent entities (e.g. variables)
            foreach (var child in childrenCopy)
                child.DetachDependentInstances();

            foreach (var child in childrenCopy)
                child.DetachState();

            ISet<MigratingTransitionInstance> transitionChildrenCopy =
                new HashSet<MigratingTransitionInstance>(childTransitionInstances);
            foreach (var child in transitionChildrenCopy)
                child.DetachState();

            ISet<MigratingEventScopeInstance> compensationChildrenCopy =
                new HashSet<MigratingEventScopeInstance>(childCompensationInstances);
            foreach (var child in compensationChildrenCopy)
                child.DetachState();

            ISet<MigratingCompensationEventSubscriptionInstance> compensationSubscriptionsChildrenCopy =
                new HashSet<MigratingCompensationEventSubscriptionInstance>(ChildCompensationSubscriptionInstances);
            foreach (var child in compensationSubscriptionsChildrenCopy)
                child.DetachState();
        }

        public virtual void DetachDependentInstances()
        {
            foreach (var dependentInstance in migratingDependentInstances)
                if (!dependentInstance.Detached)
                    dependentInstance.DetachState();
        }

        public override ExecutionEntity ResolveRepresentativeExecution()
        {
            return InstanceBehavior.ResolveRepresentativeExecution();
        }

        public override void AddMigratingDependentInstance(IMigratingInstance migratingInstance)
        {
            migratingDependentInstances.Add(migratingInstance);
        }

        public virtual void AddRemovingDependentInstance(IRemovingInstance removingInstance)
        {
            RemovingDependentInstances.Add(removingInstance);
        }

        public virtual void AddEmergingDependentInstance(IEmergingInstance emergingInstance)
        {
            EmergingDependentInstances.Add(emergingInstance);
        }

        public virtual void AddChild(MigratingTransitionInstance transitionInstance)
        {
            childTransitionInstances.Add(transitionInstance);
        }

        public virtual void RemoveChild(MigratingTransitionInstance transitionInstance)
        {
            childTransitionInstances.Remove(transitionInstance);
        }

        public virtual void AddChild(MigratingActivityInstance activityInstance)
        {
            childActivityInstances.Add(activityInstance);
        }

        public virtual void RemoveChild(MigratingActivityInstance activityInstance)
        {
            childActivityInstances.Remove(activityInstance);
        }

        public override void AddChild(MigratingScopeInstance migratingActivityInstance)
        {
            if (migratingActivityInstance is MigratingActivityInstance)
                AddChild((MigratingActivityInstance) migratingActivityInstance);
            else if (migratingActivityInstance is MigratingEventScopeInstance)
                AddChild((MigratingEventScopeInstance) migratingActivityInstance);
            else
                throw new System.Exception();
        }

        public override void RemoveChild(MigratingScopeInstance child)
        {
            if (child is MigratingActivityInstance)
                RemoveChild((MigratingActivityInstance) child);
            else if (child is MigratingEventScopeInstance)
                RemoveChild((MigratingEventScopeInstance) child);
        }

        public virtual void AddChild(MigratingEventScopeInstance compensationInstance)
        {
            childCompensationInstances.Add(compensationInstance);
        }

        public virtual void RemoveChild(MigratingEventScopeInstance compensationInstance)
        {
            childCompensationInstances.Remove(compensationInstance);
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

        public override void RemoveUnmappedDependentInstances()
        {
            foreach (var removingInstance in RemovingDependentInstances)
                removingInstance.Remove();
        }

        public override void Remove(bool skipCustomListeners, bool skipIoMappings)
        {
            InstanceBehavior.Remove(skipCustomListeners, skipIoMappings);
        }

        protected internal virtual void MigrateHistory(IDelegateExecution execution)
        {
            if (activityInstance.Id.Equals(activityInstance.ProcessInstanceId))
                MigrateProcessInstanceHistory(execution);
            else
                MigrateActivityInstanceHistory(execution);
        }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void migrateProcessInstanceHistory(final org.camunda.bpm.engine.delegate.DelegateExecution execution)
        protected internal virtual void MigrateProcessInstanceHistory(IDelegateExecution execution)
        {
            var historyLevel = Context.ProcessEngineConfiguration.HistoryLevel;
            if (!historyLevel.IsHistoryEventProduced(HistoryEventTypes.ProcessInstanceMigrate, this))
                return;

            HistoryEventProcessor.ProcessHistoryEvents(new HistoryEventCreatorAnonymousInnerClass(this, execution));
        }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void migrateActivityInstanceHistory(final org.camunda.bpm.engine.delegate.DelegateExecution execution)
        protected internal virtual void MigrateActivityInstanceHistory(IDelegateExecution execution)
        {
            var historyLevel = Context.ProcessEngineConfiguration.HistoryLevel;
            if (!historyLevel.IsHistoryEventProduced(HistoryEventTypes.ActivityInstanceMigrate, this))
                return;

            HistoryEventProcessor.ProcessHistoryEvents(new HistoryEventCreatorAnonymousInnerClass2(this));
        }

        public virtual ExecutionEntity CreateAttachableExecution()
        {
            return InstanceBehavior.CreateAttachableExecution();
        }

        public virtual void DestroyAttachableExecution(ExecutionEntity execution)
        {
            InstanceBehavior.DestroyAttachableExecution(execution);
        }


        private class HistoryEventCreatorAnonymousInnerClass : HistoryEventProcessor.HistoryEventCreator
        {
            private readonly IDelegateExecution _execution;
            private readonly MigratingActivityInstance _outerInstance;

            public HistoryEventCreatorAnonymousInnerClass(MigratingActivityInstance outerInstance,
                IDelegateExecution execution)
            {
                this._outerInstance = outerInstance;
                this._execution = execution;
            }

            public override HistoryEvent CreateHistoryEvent(IHistoryEventProducer producer)
            {
                return producer.CreateProcessInstanceUpdateEvt(_execution);
            }
        }

        private class HistoryEventCreatorAnonymousInnerClass2 : HistoryEventProcessor.HistoryEventCreator
        {
            private readonly MigratingActivityInstance _outerInstance;

            public HistoryEventCreatorAnonymousInnerClass2(MigratingActivityInstance outerInstance)
            {
                this._outerInstance = outerInstance;
            }

            public override HistoryEvent CreateHistoryEvent(IHistoryEventProducer producer)
            {
                return producer.CreateActivityInstanceMigrateEvt(_outerInstance);
            }
        }


        protected internal interface IMigratingActivityInstanceBehavior
        {
            bool Detached { get; }

            void DetachState();

            void AttachState();

            void MigrateState();

            void Remove(bool skipCustomListeners, bool skipIoMappings);

            ExecutionEntity ResolveRepresentativeExecution();

            ExecutionEntity CreateAttachableExecution();

            void DestroyAttachableExecution(ExecutionEntity execution);
        }

        protected internal class MigratingNonScopeActivityInstanceBehavior : IMigratingActivityInstanceBehavior
        {
            private readonly MigratingActivityInstance _outerInstance;

            public MigratingNonScopeActivityInstanceBehavior(MigratingActivityInstance outerInstance)
            {
                this._outerInstance = outerInstance;
            }


            public virtual bool Detached
            {
                get
                {
                    return true;
                    //return resolveRepresentativeExecution().getActivity() == null;
                }
            }

            public virtual void DetachState()
            {
                var currentExecution = ResolveRepresentativeExecution();

                //currentExecution.setActivity(null);
                //currentExecution.leaveActivityInstance();
                //currentExecution.Active = false;

                //outerInstance.Parent.destroyAttachableExecution(currentExecution);
            }

            public virtual void AttachState()
            {
                //outerInstance.representativeExecution = outerInstance.Parent.createAttachableExecution();

                //outerInstance.representativeExecution.setActivity((IPvmActivity) outerInstance.sourceScope);
                //outerInstance.representativeExecution.ActivityInstanceId = outerInstance.activityInstance.Id;
                //outerInstance.representativeExecution.Active = outerInstance.activeState;
            }

            public virtual void MigrateState()
            {
                var currentExecution = ResolveRepresentativeExecution();
                //currentExecution.setProcessDefinition(outerInstance.targetScope.ProcessDefinition);
                //currentExecution.setActivity((IPvmActivity) outerInstance.targetScope);

                _outerInstance.currentScope = _outerInstance.targetScope;

                if (_outerInstance.targetScope.IsScope)
                    BecomeScope();

                //outerInstance.migrateHistory(currentExecution);
            }

            public virtual ExecutionEntity ResolveRepresentativeExecution()
            {
                //if (outerInstance.representativeExecution.ReplacedBy != null)
                //{
                //    return outerInstance.representativeExecution.resolveReplacedBy();
                //}
                return _outerInstance.RepresentativeExecution;
            }

            public virtual void Remove(bool skipCustomListeners, bool skipIoMappings)
            {
                // nothing to do; we don't remove non-scope instances
            }

            public virtual ExecutionEntity CreateAttachableExecution()
            {
                //throw MIGRATION_LOGGER.cannotBecomeSubordinateInNonScope(outerInstance);
                throw new System.Exception();
            }

            public virtual void DestroyAttachableExecution(ExecutionEntity execution)
            {
                throw new System.Exception();
                //throw MIGRATION_LOGGER.cannotDestroySubordinateInNonScope(outerInstance);
            }

            protected internal virtual void BecomeScope()
            {
                foreach (var dependentInstance in _outerInstance.migratingDependentInstances)
                    dependentInstance.DetachState();

                var currentExecution = ResolveRepresentativeExecution();

                currentExecution = currentExecution.CreateExecution();
                //ExecutionEntity parent = currentExecution.Parent;
                //parent.setActivity(null);

                //if (!parent.Concurrent)
                //{
                //    parent.leaveActivityInstance();
                //}

                _outerInstance.RepresentativeExecution = currentExecution;
                foreach (var dependentInstance in _outerInstance.migratingDependentInstances)
                    dependentInstance.AttachState(_outerInstance);

                _outerInstance.InstanceBehavior = new MigratingScopeActivityInstanceBehavior(_outerInstance);
            }
        }

        protected internal class MigratingScopeActivityInstanceBehavior : IMigratingActivityInstanceBehavior
        {
            private readonly MigratingActivityInstance _outerInstance;

            public MigratingScopeActivityInstanceBehavior(MigratingActivityInstance outerInstance)
            {
                this._outerInstance = outerInstance;
            }


            public virtual bool Detached
            {
                get
                {
                    return true;
                    //var representativeExecution = resolveRepresentativeExecution();
                    //return representativeExecution != representativeExecution.getProcessInstance() &&
                    //       representativeExecution.Parent == null;
                }
            }

            public virtual void DetachState()
            {
                var currentScopeExecution = ResolveRepresentativeExecution();

                //ExecutionEntity parentExecution = currentScopeExecution.Parent;
                //currentScopeExecution.Parent = null;

                //if (outerInstance.sourceScope.ActivityBehavior is CompositeActivityBehavior)
                //{
                //    parentExecution.leaveActivityInstance();
                //}

                //outerInstance.Parent.destroyAttachableExecution(parentExecution);
            }

            public virtual void AttachState()
            {
                //var newParentExecution = outerInstance.Parent.createAttachableExecution();

                //var currentScopeExecution = resolveRepresentativeExecution();
                //currentScopeExecution.Parent = newParentExecution;

                //if (outerInstance.sourceScope.ActivityBehavior is CompositeActivityBehavior)
                //{
                //    newParentExecution.ActivityInstanceId = outerInstance.activityInstance.Id;
                //}
            }

            public virtual void MigrateState()
            {
                var currentScopeExecution = ResolveRepresentativeExecution();
                //currentScopeExecution.setProcessDefinition(outerInstance.targetScope.ProcessDefinition);

                //ExecutionEntity parentExecution = currentScopeExecution.Parent;

                //if (parentExecution != null && parentExecution.Concurrent)
                //{
                //    parentExecution.setProcessDefinition(outerInstance.targetScope.ProcessDefinition);
                //}

                //outerInstance.currentScope = outerInstance.targetScope;

                //if (!outerInstance.targetScope.Scope)
                //{
                //    becomeNonScope();
                //    currentScopeExecution = resolveRepresentativeExecution();
                //}

                //if (isLeafActivity(outerInstance.targetScope))
                //{
                //    currentScopeExecution.setActivity((IPvmActivity) outerInstance.targetScope);
                //}

                //if (outerInstance.sourceScope.ActivityBehavior is MigrationObserverBehavior)
                //{
                //    ((MigrationObserverBehavior) outerInstance.sourceScope.ActivityBehavior).migrateScope(
                //        currentScopeExecution);
                //}

                //outerInstance.migrateHistory(currentScopeExecution);
            }

            public virtual ExecutionEntity ResolveRepresentativeExecution()
            {
                return _outerInstance.RepresentativeExecution;
            }

            public virtual void Remove(bool skipCustomListeners, bool skipIoMappings)
            {
                var currentExecution = ResolveRepresentativeExecution();
                //ExecutionEntity parentExecution = currentExecution.Parent;

                //currentExecution.setActivity((IPvmActivity) outerInstance.sourceScope);
                //currentExecution.ActivityInstanceId = outerInstance.activityInstance.Id;

                //currentExecution.deleteCascade("migration", skipCustomListeners, skipIoMappings);

                //outerInstance.Parent.destroyAttachableExecution(parentExecution);

                //outerInstance.setParent(null);
                //foreach (var child in outerInstance.childTransitionInstances)
                //{
                //    child.setParent(null);
                //}
                //foreach (var child in outerInstance.childActivityInstances)
                //{
                //    child.setParent(null);
                //}
                foreach (var child in _outerInstance.childCompensationInstances)
                    child.Parent = null;
            }

            public virtual ExecutionEntity CreateAttachableExecution()
            {
                var scopeExecution = ResolveRepresentativeExecution();
                var attachableExecution = scopeExecution;

                if (_outerInstance.currentScope.ActivityBehavior is IModificationObserverBehavior)
                {
                    var behavior = (IModificationObserverBehavior) _outerInstance.currentScope.ActivityBehavior;
                    //attachableExecution = (ExecutionEntity) behavior.createInnerInstance(scopeExecution);
                }
                return attachableExecution;
            }

            public virtual void DestroyAttachableExecution(ExecutionEntity execution)
            {
                //if (outerInstance.currentScope.ActivityBehavior is ModificationObserverBehavior)
                //{
                //    var behavior = (ModificationObserverBehavior) outerInstance.currentScope.ActivityBehavior;
                //    behavior.destroyInnerInstance(execution);
                //}
                //else
                //{
                //    if (execution.Concurrent)
                //    {
                //        execution.remove();
                //        execution.Parent.tryPruneLastConcurrentChild();
                //        execution.Parent.forceUpdate();
                //    }
                //}
            }

            protected internal virtual void BecomeNonScope()
            {
                foreach (var dependentInstance in _outerInstance.migratingDependentInstances)
                    dependentInstance.DetachState();

                //ExecutionEntity parentExecution = outerInstance.representativeExecution.Parent;

                //parentExecution.setActivity(outerInstance.representativeExecution.getActivity());
                //parentExecution.ActivityInstanceId = outerInstance.representativeExecution.ActivityInstanceId;

                //outerInstance.representativeExecution.remove();
                //outerInstance.representativeExecution = parentExecution;

                //foreach (var dependentInstance in outerInstance.migratingDependentInstances)
                //{
                //    dependentInstance.attachState(outerInstance);
                //}

                _outerInstance.InstanceBehavior = new MigratingNonScopeActivityInstanceBehavior(_outerInstance);
            }

            protected internal virtual bool IsLeafActivity(ScopeImpl scope)
            {
                return scope.Activities.Count == 0;
            }
        }
    }
}
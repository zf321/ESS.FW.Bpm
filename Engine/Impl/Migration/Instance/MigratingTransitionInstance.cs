using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Migration;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Runtime;

namespace ESS.FW.Bpm.Engine.Impl.migration.instance
{
    /// <summary>
    ///     
    /// </summary>
    public class MigratingTransitionInstance : MigratingProcessElementInstance, IMigratingInstance
    {
        //public static readonly MigrationLogger MIGRATION_LOGGER = ProcessEngineLogger.MIGRATION_LOGGER;
        protected internal bool ActiveState;
        protected internal MigratingAsyncJobInstance jobInstance;
        protected internal IList<IMigratingInstance> migratingDependentInstances = new List<IMigratingInstance>();

        protected internal ExecutionEntity RepresentativeExecution;

        protected internal ITransitionInstance transitionInstance;


        public MigratingTransitionInstance(ITransitionInstance transitionInstance,
            IMigrationInstruction migrationInstruction, ScopeImpl sourceScope, ScopeImpl targetScope,
            ExecutionEntity asyncExecution)
        {
            this.transitionInstance = transitionInstance;
            this.migrationInstruction = migrationInstruction;
            this.sourceScope = sourceScope;
            this.targetScope = targetScope;
            currentScope = sourceScope;
            RepresentativeExecution = asyncExecution;
            ActiveState = RepresentativeExecution.IsActive;
        }

        public virtual MigratingAsyncJobInstance DependentJobInstance
        {
            set { jobInstance = value; }
        }

        public virtual IList<IMigratingInstance> MigratingDependentInstances
        {
            get { return migratingDependentInstances; }
        }

        public virtual ITransitionInstance TransitionInstance
        {
            get { return transitionInstance; }
        }

        /// <summary>
        ///     Else asyncBefore
        /// </summary>
        public virtual bool AsyncAfter
        {
            get { return jobInstance.AsyncAfter; }
        }

        public virtual bool AsyncBefore
        {
            get { return jobInstance.AsyncBefore; }
        }

        public virtual MigratingJobInstance JobInstance
        {
            get { return jobInstance; }
        }

        public override MigratingScopeInstance Parent
        {
            get { return (MigratingActivityInstance) ParentInstance; }
            set
            {
                if ((value != null) && !(value is MigratingActivityInstance))
                {
                    //throw MigrationLogger.cannotHandleChild(value, this);
                }

                var parentActivityInstance = (MigratingActivityInstance) value;

                if (value != null)
                    ((MigratingActivityInstance) value).RemoveChild(this);

                value = parentActivityInstance;

                if (value != null)
                    parentActivityInstance.AddChild(this);
            }
        }

        public override bool Detached
        {
            get { return jobInstance.Detached; }
        }

        public override void DetachState()
        {
            jobInstance.DetachState();
            foreach (var dependentInstance in migratingDependentInstances)
                dependentInstance.DetachState();

            var execution = ResolveRepresentativeExecution();
            execution.IsActive = false;
            //Parent.destroyAttachableExecution(execution);

            Parent = null;
        }

        public override void AttachState(MigratingScopeInstance scopeInstance)
        {
            if (!(scopeInstance is MigratingActivityInstance))
            {
                //throw MIGRATION_LOGGER.cannotHandleChild(scopeInstance, this);
            }

            var activityInstance = (MigratingActivityInstance) scopeInstance;

            Parent = activityInstance;

            RepresentativeExecution = activityInstance.CreateAttachableExecution();
            RepresentativeExecution.ActivityInstanceId = null;
            //representativeExecution.Active = activeState;

            jobInstance.AttachState(this);

            foreach (var dependentInstance in migratingDependentInstances)
                dependentInstance.AttachState(this);
        }

        public override void AttachState(MigratingTransitionInstance targetTransitionInstance)
        {
            //throw MIGRATION_LOGGER.cannotAttachToTransitionInstance(this);
        }

        public override void MigrateState()
        {
            var representativeExecution = ResolveRepresentativeExecution();

            //representativeExecution.setProcessDefinition(targetScope.ProcessDefinition);
            //representativeExecution.setActivity((IPvmActivity) targetScope);
        }

        public override void MigrateDependentEntities()
        {
            jobInstance.MigrateState();
            jobInstance.MigrateDependentEntities();

            foreach (var dependentInstance in migratingDependentInstances)
            {
                dependentInstance.MigrateState();
                dependentInstance.MigrateDependentEntities();
            }
        }

        public override ExecutionEntity ResolveRepresentativeExecution()
        {
            //if (representativeExecution.ReplacedBy != null)
            //{
            //    return representativeExecution.resolveReplacedBy();
            //}
            //return representativeExecution;
            return null;
        }

        public override void AddMigratingDependentInstance(IMigratingInstance migratingInstance)
        {
            migratingDependentInstances.Add(migratingInstance);
        }
    }
}
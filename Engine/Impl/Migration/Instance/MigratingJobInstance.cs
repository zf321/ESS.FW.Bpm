using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.migration.instance
{
    public abstract class MigratingJobInstance : IMigratingInstance, IRemovingInstance
    {
        protected internal JobEntity jobEntity;

        protected internal IList<IMigratingInstance> MigratingDependentInstances = new List<IMigratingInstance>();
        protected internal JobDefinitionEntity targetJobDefinitionEntity;
        protected internal ScopeImpl targetScope;

        public MigratingJobInstance(JobEntity jobEntity, JobDefinitionEntity jobDefinitionEntity, ScopeImpl targetScope)
        {
            this.jobEntity = jobEntity;
            targetJobDefinitionEntity = jobDefinitionEntity;
            this.targetScope = targetScope;
        }

        public MigratingJobInstance(JobEntity jobEntity) : this(jobEntity, null, null)
        {
        }

        public virtual JobEntity JobEntity
        {
            get { return jobEntity; }
        }

        public virtual ScopeImpl TargetScope
        {
            get { return targetScope; }
        }

        public virtual JobDefinitionEntity TargetJobDefinitionEntity
        {
            get { return targetJobDefinitionEntity; }
        }

        public virtual bool Detached
        {
            get { return ReferenceEquals(jobEntity.ExecutionId, null); }
        }

        public virtual void DetachState()
        {
            //jobEntity.Execution = null;

            foreach (var dependentInstance in MigratingDependentInstances)
                dependentInstance.DetachState();
        }

        public virtual void AttachState(MigratingScopeInstance newOwningInstance)
        {
            AttachTo(newOwningInstance.ResolveRepresentativeExecution());

            foreach (var dependentInstance in MigratingDependentInstances)
                dependentInstance.AttachState(newOwningInstance);
        }

        public virtual void AttachState(MigratingTransitionInstance targetTransitionInstance)
        {
            AttachTo(targetTransitionInstance.ResolveRepresentativeExecution());

            foreach (var dependentInstance in MigratingDependentInstances)
                dependentInstance.AttachState(targetTransitionInstance);
        }

        public virtual void MigrateState()
        {
            // update activity reference
            var activityId = targetScope.Id;
            //jobEntity.ActivityId = activityId;
            migrateJobHandlerConfiguration();

            if (targetJobDefinitionEntity != null)
            {
                //jobEntity.JobDefinition = targetJobDefinitionEntity;
            }

            // update process definition reference
            //var processDefinition = (ProcessDefinitionEntity) targetScope.ProcessDefinition;
            //jobEntity.ProcessDefinitionId = processDefinition.Id;
            //jobEntity.ProcessDefinitionKey = processDefinition.Key;

            // update deployment reference
            //jobEntity.DeploymentId = processDefinition.DeploymentId;
        }

        public virtual void MigrateDependentEntities()
        {
            foreach (var migratingDependentInstance in MigratingDependentInstances)
                migratingDependentInstance.MigrateState();
        }

        public virtual void Remove()
        {
            //jobEntity.delete();
        }

        public virtual void AddMigratingDependentInstance(IMigratingInstance migratingInstance)
        {
            MigratingDependentInstances.Add(migratingInstance);
        }

        protected internal virtual void AttachTo(ExecutionEntity execution)
        {
            //jobEntity.Execution = execution;
        }

        public virtual bool Migrates()
        {
            return targetScope != null;
        }

        protected internal abstract void migrateJobHandlerConfiguration();
    }
}
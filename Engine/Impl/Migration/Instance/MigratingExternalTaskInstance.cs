using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Repository;

namespace ESS.FW.Bpm.Engine.Impl.migration.instance
{
    /// <summary>
    ///     
    /// </summary>
    public class MigratingExternalTaskInstance : IMigratingInstance
    {
        public static readonly MigrationLogger MigrationLogger = ProcessEngineLogger.MigrationLogger;

        protected internal IList<IMigratingInstance> DependentInstances = new List<IMigratingInstance>();

        protected internal ExternalTaskEntity ExternalTask;
        protected internal MigratingActivityInstance MigratingActivityInstance;

        public MigratingExternalTaskInstance(ExternalTaskEntity externalTask,
            MigratingActivityInstance migratingActivityInstance)
        {
            this.ExternalTask = externalTask;
            this.MigratingActivityInstance = migratingActivityInstance;
        }

        public virtual string Id
        {
            get { return ExternalTask.Id; }
        }

        public virtual ScopeImpl TargetScope
        {
            get { return MigratingActivityInstance.TargetScope; }
        }

        public virtual void MigrateDependentEntities()
        {
            foreach (var migratingDependentInstance in DependentInstances)
                migratingDependentInstance.MigrateState();
        }

        public virtual bool Detached
        {
            get { return ReferenceEquals(ExternalTask.ExecutionId, null); }
        }

        public virtual void DetachState()
        {
            //externalTask.Execution.removeExternalTask(externalTask);
            //externalTask.Execution = null;
        }

        public virtual void AttachState(MigratingScopeInstance owningInstance)
        {
            var representativeExecution = owningInstance.ResolveRepresentativeExecution();
            //representativeExecution.addExternalTask(externalTask);

            //externalTask.Execution = representativeExecution;
        }

        public virtual void AttachState(MigratingTransitionInstance targetTransitionInstance)
        {
            throw MigrationLogger.CannotAttachToTransitionInstance(this);
        }

        public virtual void MigrateState()
        {
            var targetActivity = MigratingActivityInstance.TargetScope;
            var targetProcessDefinition = (IProcessDefinition) targetActivity.ProcessDefinition;

            ExternalTask.ActivityId = targetActivity.Id;
            ExternalTask.ProcessDefinitionId = targetProcessDefinition.Id;
            ExternalTask.ProcessDefinitionKey = targetProcessDefinition.Key;
        }

        public virtual void AddMigratingDependentInstance(IMigratingInstance migratingInstance)
        {
            DependentInstances.Add(migratingInstance);
        }
    }
}
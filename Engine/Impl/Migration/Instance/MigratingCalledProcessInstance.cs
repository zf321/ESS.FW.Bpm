using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.migration.instance
{
    /// <summary>
    ///     
    /// </summary>
    public class MigratingCalledProcessInstance : IMigratingInstance
    {
        public static readonly MigrationLogger MigrationLogger = ProcessEngineLogger.MigrationLogger;

        protected internal ExecutionEntity ProcessInstance;

        public MigratingCalledProcessInstance(ExecutionEntity processInstance)
        {
            this.ProcessInstance = processInstance;
        }

        public virtual bool Detached
        {
            get { return ReferenceEquals(ProcessInstance.SuperExecutionId, null); }
        }

        public virtual void DetachState()
        {
            //processInstance.SuperExecution = (null);
        }

        public virtual void AttachState(MigratingScopeInstance targetActivityInstance)
        {
            //processInstance.setSuperExecution(targetActivityInstance.resolveRepresentativeExecution());
        }

        public virtual void AttachState(MigratingTransitionInstance targetTransitionInstance)
        {
            throw MigrationLogger.CannotAttachToTransitionInstance(this);
        }

        public virtual void MigrateState()
        {
            // nothing to do
        }

        public virtual void MigrateDependentEntities()
        {
            // nothing to do
        }
    }
}
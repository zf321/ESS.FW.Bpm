using System;

namespace ESS.FW.Bpm.Engine.Impl.migration.instance
{
    //using CaseExecutionEntity = org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseExecutionEntity;

    /// <summary>
    ///     
    /// </summary>
    public class MigratingCalledCaseInstance : IMigratingInstance
    {
        public static readonly MigrationLogger MigrationLogger = ProcessEngineLogger.MigrationLogger;

        //protected internal CaseExecutionEntity CaseInstance;

        //public MigratingCalledCaseInstance(CaseExecutionEntity caseInstance)
        //{
        //    this.CaseInstance = caseInstance;
        //}

        public virtual bool Detached
        {
            get { throw new NotImplementedException();/* return ReferenceEquals(CaseInstance.SuperExecutionId, null); */}
        }

        public virtual void DetachState()
        {
            //caseInstance.setSuperExecution(null);
        }

        public virtual void AttachState(MigratingScopeInstance targetActivityInstance)
        {
            //caseInstance.setSuperExecution(targetActivityInstance.resolveRepresentativeExecution());
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
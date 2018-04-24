using ESS.FW.Bpm.Engine.Impl.migration.instance;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.migration.validation.instance
{
    /// <summary>
    ///     
    /// </summary>
    public class AsyncProcessStartMigrationValidator : IMigratingTransitionInstanceValidator
    {
        public virtual void Validate(MigratingTransitionInstance migratingInstance,
            MigratingProcessInstance migratingProcessInstance,
            MigratingTransitionInstanceValidationReportImpl instanceReport)
        {
            var targetActivity = (ActivityImpl) migratingInstance.TargetScope;

            if (targetActivity != null)
                if (IsProcessStartJob(migratingInstance.JobInstance.JobEntity) && !IsTopLevelActivity(targetActivity))
                    instanceReport.AddFailure(
                        "A transition instance that instantiates the process can only be migrated to a process-level flow node");
        }

        protected internal virtual bool IsProcessStartJob(JobEntity job)
        {
            //var configuration = (AsyncContinuationJobHandler.AsyncContinuationConfiguration) job.JobHandlerConfiguration;
            //return PvmAtomicOperation_Fields.PROCESS_START.CanonicalName.Equals(configuration.AtomicOperation);
            return false;
        }

        protected internal virtual bool IsTopLevelActivity(ActivityImpl activity)
        {
            return activity.FlowScope == activity.ProcessDefinition;
        }
    }
}
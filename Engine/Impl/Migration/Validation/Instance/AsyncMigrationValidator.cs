using ESS.FW.Bpm.Engine.Impl.migration.instance;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;

namespace ESS.FW.Bpm.Engine.Impl.migration.validation.instance
{
    public class AsyncMigrationValidator : IMigratingTransitionInstanceValidator
    {
        public virtual void Validate(MigratingTransitionInstance migratingInstance,
            MigratingProcessInstance migratingProcessInstance,
            MigratingTransitionInstanceValidationReportImpl instanceReport)
        {
            var targetActivity = (ActivityImpl) migratingInstance.TargetScope;

            if (targetActivity != null)
                if (migratingInstance.AsyncAfter)
                {
                    if (!targetActivity.AsyncAfter)
                        instanceReport.AddFailure("Target activity is not asyncAfter");
                }
                else
                {
                    if (!targetActivity.AsyncBefore)
                        instanceReport.AddFailure("Target activity is not asyncBefore");
                }
        }
    }
}
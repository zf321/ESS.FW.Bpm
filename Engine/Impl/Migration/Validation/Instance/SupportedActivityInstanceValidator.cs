using ESS.FW.Bpm.Engine.Impl.migration.instance;
using ESS.FW.Bpm.Engine.Impl.migration.validation.activity;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;

namespace ESS.FW.Bpm.Engine.Impl.migration.validation.instance
{
    /// <summary>
    ///     
    /// </summary>
    public class SupportedActivityInstanceValidator : IMigratingActivityInstanceValidator
    {
        public virtual void Validate(MigratingActivityInstance migratingInstance,
            MigratingProcessInstance migratingProcessInstance,
            MigratingActivityInstanceValidationReportImpl instanceReport)
        {
            var sourceScope = migratingInstance.SourceScope;

            if (sourceScope != sourceScope.ProcessDefinition)
            {
                var sourceActivity = (ActivityImpl) migratingInstance.SourceScope;

                if (!SupportedActivityValidator.Instance.IsSupportedActivity(sourceActivity))
                    instanceReport.AddFailure(
                        "The type of the source activity is not supported for activity instance migration");
            }
        }
    }
}
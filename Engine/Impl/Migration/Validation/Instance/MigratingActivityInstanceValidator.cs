using ESS.FW.Bpm.Engine.Impl.migration.instance;

namespace ESS.FW.Bpm.Engine.Impl.migration.validation.instance
{
    public interface IMigratingActivityInstanceValidator
    {
        void Validate(MigratingActivityInstance migratingInstance, MigratingProcessInstance migratingProcessInstance,
            MigratingActivityInstanceValidationReportImpl instanceReport);
    }
}
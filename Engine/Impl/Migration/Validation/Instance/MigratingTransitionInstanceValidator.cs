using ESS.FW.Bpm.Engine.Impl.migration.instance;

namespace ESS.FW.Bpm.Engine.Impl.migration.validation.instance
{
    /// <summary>
    ///     
    /// </summary>
    public interface IMigratingTransitionInstanceValidator
    {
        void Validate(MigratingTransitionInstance migratingInstance, MigratingProcessInstance migratingProcessInstance,
            MigratingTransitionInstanceValidationReportImpl instanceReport);
    }
}
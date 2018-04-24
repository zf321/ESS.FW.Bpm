using ESS.FW.Bpm.Engine.Impl.migration.instance;

namespace ESS.FW.Bpm.Engine.Impl.migration.validation.instance
{
    /// <summary>
    ///     
    /// </summary>
    public interface IMigratingCompensationInstanceValidator
    {
        /// <param name="migratingInstance"> </param>
        /// <param name="migratingProcessInstance"> </param>
        /// <param name="ancestorInstanceReport">
        ///     the report of the closest ancestor activity instance;
        ///     errors should be added to this report
        /// </param>
        void Validate(MigratingEventScopeInstance migratingInstance, MigratingProcessInstance migratingProcessInstance,
            MigratingActivityInstanceValidationReportImpl ancestorInstanceReport);

        /// <param name="migratingInstance"> </param>
        /// <param name="migratingProcessInstance"> </param>
        /// <param name="ancestorInstanceReport">
        ///     the report of the closest ancestor activity instance;
        ///     errors should be added to this report
        /// </param>
        void Validate(MigratingCompensationEventSubscriptionInstance migratingInstance,
            MigratingProcessInstance migratingProcessInstance,
            MigratingActivityInstanceValidationReportImpl ancestorInstanceReport);
    }
}
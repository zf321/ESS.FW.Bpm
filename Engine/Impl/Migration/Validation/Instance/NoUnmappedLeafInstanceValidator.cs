using ESS.FW.Bpm.Engine.Impl.migration.instance;

namespace ESS.FW.Bpm.Engine.Impl.migration.validation.instance
{
    public class NoUnmappedLeafInstanceValidator : IMigratingActivityInstanceValidator,
        IMigratingTransitionInstanceValidator, IMigratingCompensationInstanceValidator
    {
        public virtual void Validate(MigratingActivityInstance migratingInstance,
            MigratingProcessInstance migratingProcessInstance,
            MigratingActivityInstanceValidationReportImpl instanceReport)
        {
            if (IsInvalid(migratingInstance))
                instanceReport.AddFailure("There is no migration instruction for this instance's activity");
        }

        public virtual void Validate(MigratingCompensationEventSubscriptionInstance migratingInstance,
            MigratingProcessInstance migratingProcessInstance,
            MigratingActivityInstanceValidationReportImpl ancestorInstanceReport)
        {
            if (IsInvalid(migratingInstance))
                ancestorInstanceReport.AddFailure("Cannot migrate subscription for compensation handler '" +
                                                  migratingInstance.SourceScope.Id + "'. " +
                                                  "There is no migration instruction for the compensation boundary event");
        }

        public virtual void Validate(MigratingEventScopeInstance migratingInstance,
            MigratingProcessInstance migratingProcessInstance,
            MigratingActivityInstanceValidationReportImpl ancestorInstanceReport)
        {
            if (IsInvalid(migratingInstance))
                ancestorInstanceReport.AddFailure("Cannot migrate subscription for compensation handler '" +
                                                  migratingInstance.EventSubscription.SourceScope.Id + "'. " +
                                                  "There is no migration instruction for the compensation start event");
        }

        public virtual void Validate(MigratingTransitionInstance migratingInstance,
            MigratingProcessInstance migratingProcessInstance,
            MigratingTransitionInstanceValidationReportImpl instanceReport)
        {
            if (IsInvalid(migratingInstance))
                instanceReport.AddFailure("There is no migration instruction for this instance's activity");
        }

        protected internal virtual bool IsInvalid(MigratingActivityInstance migratingInstance)
        {
            return HasNoInstruction(migratingInstance) && (migratingInstance.Children.Count == 0);
        }

        protected internal virtual bool IsInvalid(MigratingEventScopeInstance migratingInstance)
        {
            return HasNoInstruction(migratingInstance.EventSubscription) && (migratingInstance.Children.Count == 0);
        }

        protected internal virtual bool IsInvalid(MigratingTransitionInstance migratingInstance)
        {
            return HasNoInstruction(migratingInstance);
        }

        protected internal virtual bool IsInvalid(MigratingCompensationEventSubscriptionInstance migratingInstance)
        {
            return HasNoInstruction(migratingInstance);
        }

        protected internal virtual bool HasNoInstruction(MigratingProcessElementInstance migratingInstance)
        {
            return migratingInstance.MigrationInstruction == null;
        }
    }
}
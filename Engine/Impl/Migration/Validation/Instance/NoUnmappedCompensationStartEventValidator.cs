using ESS.FW.Bpm.Engine.Impl.migration.instance;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;

namespace ESS.FW.Bpm.Engine.Impl.migration.validation.instance
{
    /// <summary>
    ///     Subscriptions for compensation start events must be migrated, similar to compensation boundary events.
    ///     However, this is not validated by <seealso cref="NoUnmappedLeafInstanceValidator" /> because
    ///     the corresponding event scope instance need not be a leaf in the instance tree (the scope itself may contain
    ///     event subscriptions).
    ///     
    /// </summary>
    public class NoUnmappedCompensationStartEventValidator : IMigratingCompensationInstanceValidator
    {
        public virtual void Validate(MigratingEventScopeInstance migratingInstance,
            MigratingProcessInstance migratingProcessInstance,
            MigratingActivityInstanceValidationReportImpl ancestorInstanceReport)
        {
            var eventSubscription = migratingInstance.EventSubscription;

            var eventHandlerActivity = (ActivityImpl) eventSubscription.SourceScope;

            // note: compensation event scopes without children are already handled by NoUnmappedLeafInstanceValidator
            if (eventHandlerActivity.TriggeredByEvent && (eventSubscription.TargetScope == null) &&
                (migratingInstance.Children.Count > 0))
                ancestorInstanceReport.AddFailure("Cannot migrate subscription for compensation handler '" +
                                                  eventSubscription.SourceScope.Id + "'. " +
                                                  "There is no migration instruction for the compensation start event");
        }

        public virtual void Validate(MigratingCompensationEventSubscriptionInstance migratingInstance,
            MigratingProcessInstance migratingProcessInstance,
            MigratingActivityInstanceValidationReportImpl ancestorInstanceReport)
        {
            // Compensation start event subscriptions are MigratingEventScopeInstances
            // because they reference an event scope execution
        }
    }
}
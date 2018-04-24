using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Parser;
using ESS.FW.Bpm.Engine.Impl.Event;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.migration.instance.parser
{
    /// <summary>
    ///     
    /// </summary>
    public class EventSubscriptionInstanceHandler :
        IMigratingDependentInstanceParseHandler<MigratingActivityInstance, IList<EventSubscriptionEntity>>
    {
        public static readonly ISet<string> SUPPORTED_EVENT_TYPES = new HashSet<string>
        {
            EventType.Message.Name,
            EventType.Signal.Name,
            EventType.Conditonal.Name
        };

        protected internal virtual ISet<string> SupportedEventTypes
        {
            get { return SUPPORTED_EVENT_TYPES; }
        }

        public virtual void Handle(MigratingInstanceParseContext parseContext, MigratingActivityInstance owningInstance,
            IList<EventSubscriptionEntity> elements)
        {
            var targetDeclarations = GetDeclarationsByTriggeringActivity(owningInstance.TargetScope);

            foreach (var eventSubscription in elements)
            {
                if (!SupportedEventTypes.Contains(eventSubscription.EventType))
                    continue;

                var migrationInstruction = parseContext.FindSingleMigrationInstruction(eventSubscription.ActivityId);
                var targetActivity = parseContext.GetTargetActivity(migrationInstruction);

                if ((targetActivity != null) && owningInstance.MigratesTo((ScopeImpl) targetActivity.EventScope))
                {
                    // the event subscription is migrated
                    //EventSubscriptionDeclaration targetDeclaration = targetDeclarations.Remove(targetActivity.Id);

                    //owningInstance.addMigratingDependentInstance(new MigratingEventSubscriptionInstance(eventSubscription, targetActivity, migrationInstruction.UpdateEventTrigger, targetDeclaration));
                }
                else
                {
                    // the event subscription will be removed
                    owningInstance.AddRemovingDependentInstance(new MigratingEventSubscriptionInstance(eventSubscription));
                }

                //parseContext.consume(eventSubscription);
            }

            if (owningInstance.Migrates())
                AddEmergingEventSubscriptions(owningInstance, targetDeclarations.Values);
        }

        protected internal virtual IDictionary<string, EventSubscriptionDeclaration> GetDeclarationsByTriggeringActivity
            (ScopeImpl eventScope)
        {
            var declarations = EventSubscriptionDeclaration.GetDeclarationsForScope(eventScope);

            return new Dictionary<string, EventSubscriptionDeclaration>(declarations);
        }

        protected internal virtual void AddEmergingEventSubscriptions(MigratingActivityInstance owningInstance,
            ICollection<EventSubscriptionDeclaration> emergingDeclarations)
        {
            foreach (var eventSubscriptionDeclaration in emergingDeclarations)
                owningInstance.AddEmergingDependentInstance(
                    new MigratingEventSubscriptionInstance(eventSubscriptionDeclaration));
        }
    }
}
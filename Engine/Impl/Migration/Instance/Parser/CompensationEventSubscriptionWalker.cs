using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Helper;
using ESS.FW.Bpm.Engine.Impl.tree;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.migration.instance.parser
{
    /// <summary>
    ///     Ensures that event subscriptions are visited in a top-down fashion, i.e.
    ///     for a compensation handler in a scope that has an event scope execution, it is guaranteed
    ///     that first the scope subscription is visited, and then the compensation handler
    ///     
    /// </summary>
    public class CompensationEventSubscriptionWalker : ReferenceWalker<EventSubscriptionEntity>
    {
        public CompensationEventSubscriptionWalker(ICollection<MigratingActivityInstance> collection)
            : base(CollectCompensationEventSubscriptions(collection))
        {
        }

        protected internal static IList<EventSubscriptionEntity> CollectCompensationEventSubscriptions(
            ICollection<MigratingActivityInstance> activityInstances)
        {
            IList<EventSubscriptionEntity> eventSubscriptions = new List<EventSubscriptionEntity>();
            foreach (var activityInstance in activityInstances)
                if (activityInstance.SourceScope.IsScope)
                {
                    var scopeExecution = activityInstance.ResolveRepresentativeExecution();
                    //((List<EventSubscriptionEntity>) eventSubscriptions).AddRange(
                    //    scopeExecution.CompensateEventSubscriptions);
                }
            return eventSubscriptions;
        }

        protected internal override ICollection<EventSubscriptionEntity> NextElements()
        {
            var eventSubscriptionEntity = CurrentElement;
            var compensatingExecution = CompensationUtil.GetCompensatingExecution(eventSubscriptionEntity);
            if (compensatingExecution != null)
            {
                //return compensatingExecution.CompensateEventSubscriptions;
            }
            return null;
        }
    }
}
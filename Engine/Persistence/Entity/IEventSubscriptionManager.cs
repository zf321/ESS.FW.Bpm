using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{
    public interface IEventSubscriptionManager
    {
        void DeleteAndFlushEventSubscription(EventSubscriptionEntity persistentObject);
        void DeleteEventSubscription(EventSubscriptionEntity persistentObject);
        EventSubscriptionEntity FindEventSubscriptionById(string id);
        IList<EventSubscriptionEntity> FindEventSubscriptions(string executionId, string type, string activityId);
        IList<EventSubscriptionEntity> FindEventSubscriptionsByConfiguration(string type, string configuration);
        IList<EventSubscriptionEntity> FindEventSubscriptionsByExecution(string executionId);
        IList<EventSubscriptionEntity> FindEventSubscriptionsByExecutionAndType(string executionId, string type, bool lockResult);
        IList<EventSubscriptionEntity> FindEventSubscriptionsByNameAndExecution(string type, string eventName, string executionId, bool lockResult);
        IList<EventSubscriptionEntity> FindEventSubscriptionsByNameAndTenantId(string type, string eventName, string tenantId);
        IList<EventSubscriptionEntity> FindEventSubscriptionsByProcessInstanceId(string processInstanceId);
        IList<EventSubscriptionEntity> FindMessageStartEventSubscriptionByName(string messageName);
        EventSubscriptionEntity FindMessageStartEventSubscriptionByNameAndTenantId(string messageName, string tenantId);
        IList<EventSubscriptionEntity> FindSignalEventSubscriptionsByEventName(string eventName);
        IList<EventSubscriptionEntity> FindSignalEventSubscriptionsByEventNameAndTenantId(string eventName, string tenantId);
        IList<EventSubscriptionEntity> FindSignalEventSubscriptionsByEventNameAndTenantIdIncludeWithoutTenantId(string eventName, string tenantId);
        IList<EventSubscriptionEntity> FindSignalEventSubscriptionsByExecution(string executionId);
        IList<EventSubscriptionEntity> FindSignalEventSubscriptionsByNameAndExecution(string name, string executionId);
        void Insert(EventSubscriptionEntity persistentObject);
    }
}
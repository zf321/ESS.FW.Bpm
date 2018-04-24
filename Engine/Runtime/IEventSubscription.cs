using System;

namespace ESS.FW.Bpm.Engine.Runtime
{
    /// <summary>
    ///     A message event subscription exists, if an <seealso cref="IExecution" /> waits for an event like a message.
    ///     
    /// </summary>
    public interface IEventSubscription
    {
        /// <summary>
        ///     The unique identifier of the event subscription.
        /// </summary>
        string Id { get; }

        /// <summary>
        ///     The event subscriptions type. "message" identifies message event subscriptions,
        ///     "signal" identifies signal event subscription, "compensation" identifies event subscriptions
        ///     used for compensation events.
        /// </summary>
        string EventType { get; }

        /// <summary>
        ///     The name of the event this subscription belongs to as defined in the process model.
        /// </summary>
        string EventName { get; }

        /// <summary>
        ///     The execution that is subscribed on the referenced event.
        /// </summary>
        string ExecutionId { get; }

        /// <summary>
        ///     The process instance this subscription belongs to.
        /// </summary>
        string ProcessInstanceId { get; }

        /// <summary>
        ///     The identifier of the activity that this event subscription belongs to.
        ///     This could for example be the id of a receive task.
        /// </summary>
        string ActivityId { get; }

        /// <summary>
        ///     The id of the tenant this event subscription belongs to. Can be <code>null</code>
        ///     if the subscription belongs to no single tenant.
        /// </summary>
        string TenantId { get; }

        /// <summary>
        ///     The time this event subscription was created.
        /// </summary>
        DateTime Created { get; }
    }
}
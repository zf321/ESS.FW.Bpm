using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Event
{

    /// <summary>
    ///     
    /// </summary>
    public interface IEventHandler
    {
        string EventHandlerType { get; }

        void HandleEvent(EventSubscriptionEntity eventSubscription, object payload, CommandContext commandContext);
    }
}
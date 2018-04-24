namespace ESS.FW.Bpm.Engine.History.Impl.Event
{
    public interface IHistoryEventType
    {
        /// The type of the entity.
        string EntityType { get; }

        /// The name of the event fired on the entity
        string EventName { get; }
    }
}
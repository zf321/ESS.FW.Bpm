

namespace ESS.FW.Bpm.Engine.Impl.Event
{
    /// <summary>
    ///     Defines the existing event types, on which the subscription can be done.
    ///     Since the the event type for message and signal are historically lower case
    ///     the enum variant can't be used, so we have to reimplement an enum like class.
    ///     That is done so we can restrict the event types to only the defined ones.
    ///      
    /// </summary>
    public sealed class EventType
    {
        public static readonly EventType Message = new EventType("message");
        public static readonly EventType Signal = new EventType("signal");
        public static readonly EventType Compensate = new EventType("compensate");
        public static readonly EventType Conditonal = new EventType("conditional");
        
        public string Name { get; }

        private EventType(string name)
        {
            Name = name;
        }
        
    }
}


using ESS.FW.Bpm.Engine.Impl.Pvm.Process;

namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior
{
    /// <summary>
    ///     <para>Specialization of the Start Event for Event Sub-Processes.</para>
    ///     <para>
    ///         The start event's behavior is realized by the start behavior of the event subprocess it is embedded in.
    ///         The start behavior of the event subprocess must be either either
    ///         <seealso cref="ActivityStartBehavior#INTERRUPT_EVENT_SCOPE" /> or
    ///         <seealso cref="ActivityStartBehavior#CONCURRENT_IN_FLOW_SCOPE" />
    ///     </para>
    ///     
    ///     
    /// </summary>
    public class EventSubProcessStartEventActivityBehavior : NoneStartEventActivityBehavior
    {
    }
}
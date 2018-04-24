using ESS.FW.Bpm.Engine.Impl.Pvm.Process;

namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior
{
    /// <summary>
    ///     <para>The BPMN Boundary Event.</para>
    ///     <para>
    ///         The behavior of the boundary event is defined via it's <seealso cref="ActivityStartBehavior" />. It must be
    ///         either
    ///         {@value ActivityStartBehavior#CANCEL_EVENT_SCOPE} or {@value ActivityStartBehavior#CONCURRENT_IN_FLOW_SCOPE}
    ///         meaning
    ///         that it will either cancel the scope execution for the activity it is attached to (it's event scope) or will be
    ///         executed concurrently
    ///         in it's flow scope.
    ///     </para>
    ///     <para>The boundary event does noting "special" in its inner behavior.</para>
    ///     
    ///     
    /// </summary>
    public class BoundaryEventActivityBehavior : FlowNodeActivityBehavior
    {
    }
}
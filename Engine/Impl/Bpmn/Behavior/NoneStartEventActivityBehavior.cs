namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior
{
    /// <summary>
    ///     implementation of the 'none start event': a start event that has no
    ///     specific trigger but the programmatic one (processService.startProcessInstanceXXX()).
    ///     
    /// </summary>
    public class NoneStartEventActivityBehavior : FlowNodeActivityBehavior
    {
        // Nothing to see here.
        // The default behaviour of the BpmnActivity is exactly what
        // a none start event should be doing.
    }
}
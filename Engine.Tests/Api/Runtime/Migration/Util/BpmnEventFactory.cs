using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Model.Bpmn;

namespace Engine.Tests.Api.Runtime.Migration.Util
{
    /// <summary>
    /// </summary>
    public interface BpmnEventFactory
    {
        IMigratingBpmnEventTrigger AddBoundaryEvent(IProcessEngine engine, IBpmnModelInstance modelInstance,
            string activityId, string boundaryEventId);

        IMigratingBpmnEventTrigger AddEventSubProcess(IProcessEngine engine, IBpmnModelInstance modelInstance,
            string parentId, string subProcessId, string startEventId);
    }
}
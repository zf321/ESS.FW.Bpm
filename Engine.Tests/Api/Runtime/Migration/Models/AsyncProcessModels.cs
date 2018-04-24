using ESS.FW.Bpm.Model.Bpmn;

namespace Engine.Tests.Api.Runtime.Migration.Models
{
    /// <summary>
    /// </summary>
    public class AsyncProcessModels
    {
        public static readonly IBpmnModelInstance ASYNC_BEFORE_USER_TASK_PROCESS =
                ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess)
            //.ActivityBuilder("userTask")
            //.CamundaAsyncBefore()
            //.Done()
            ;

        public static readonly IBpmnModelInstance ASYNC_BEFORE_SUBPROCESS_USER_TASK_PROCESS =
                ModifiableBpmnModelInstance.Modify(ProcessModels.SubprocessProcess)
            //.ActivityBuilder("userTask")
            //.CamundaAsyncBefore()
            //.Done()
            ;

        public static readonly IBpmnModelInstance ASYNC_BEFORE_START_EVENT_PROCESS =
                ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess)
            //.FlowNodeBuilder("startEvent")
            //.CamundaAsyncBefore()
            //.Done()
            ;

        public static readonly IBpmnModelInstance ASYNC_BEFORE_SUBPROCESS_START_EVENT_PROCESS =
                ModifiableBpmnModelInstance.Modify(ProcessModels.SubprocessProcess)
            //.FlowNodeBuilder("subProcessStart")
            //.CamundaAsyncBefore()
            //.Done()
            ;

        public static readonly IBpmnModelInstance ASYNC_AFTER_USER_TASK_PROCESS =
                ModifiableBpmnModelInstance.Modify(ProcessModels.TwoTasksProcess)
            //.ActivityBuilder("userTask1")
            //.CamundaAsyncAfter()
            //.Done()
            ;

        public static readonly IBpmnModelInstance ASYNC_AFTER_SUBPROCESS_USER_TASK_PROCESS = ProcessModels.NewModel()
            .StartEvent()
            .SubProcess("subProcess")
            //.EmbeddedSubProcess()
            //.StartEvent()
            .UserTask("userTask1")
            .CamundaAsyncAfter()
            .SubProcessDone()
            .UserTask("userTask2")
            .EndEvent()
            .Done();
    }
}
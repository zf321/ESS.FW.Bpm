using ESS.FW.Bpm.Model.Bpmn;

namespace Engine.Tests.Api.Runtime.Migration.Models
{
    /// <summary>
    /// </summary>
    public class MessageReceiveModels
    {
        public const string MESSAGE_NAME = "Message";

        public static readonly IBpmnModelInstance ONE_RECEIVE_TASK_PROCESS = ProcessModels.NewModel()
            .StartEvent()
            .ReceiveTask("receiveTask")
            //.Message(MESSAGE_NAME)
            .UserTask("userTask")
            .EndEvent()
            .Done();

        public static readonly IBpmnModelInstance SUBPROCESS_RECEIVE_TASK_PROCESS = ProcessModels.NewModel()
            .StartEvent()
            .SubProcess()
            //.EmbeddedSubProcess()
            //.StartEvent()
            .ReceiveTask("receiveTask")
            //.Message(MESSAGE_NAME)
            .UserTask("userTask")
            .EndEvent()
            .SubProcessDone()
            .EndEvent()
            .Done();

        public static readonly IBpmnModelInstance ONE_MESSAGE_CATCH_PROCESS = ProcessModels.NewModel()
            .StartEvent()
            .IntermediateCatchEvent("messageCatch")
            //.Message(MESSAGE_NAME)
            .UserTask("userTask")
            .EndEvent()
            .Done();
    }
}
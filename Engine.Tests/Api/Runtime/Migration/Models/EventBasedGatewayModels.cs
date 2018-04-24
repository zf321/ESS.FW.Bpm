using ESS.FW.Bpm.Model.Bpmn;

namespace Engine.Tests.Api.Runtime.Migration.Models
{
    /// <summary>
    /// </summary>
    public class EventBasedGatewayModels
    {
        public const string MESSAGE_NAME = "Message";
        public const string SIGNAL_NAME = "Signal";

        public static readonly IBpmnModelInstance TIMER_EVENT_BASED_GW_PROCESS = ProcessModels.NewModel()
            .StartEvent()
            .EventBasedGateway()
            //.Id("eventBasedGateway")
            .IntermediateCatchEvent("timerCatch")
            .TimerWithDuration("PT10M")
            .UserTask("afterTimerCatch")
            .EndEvent()
            .Done();

        public static readonly IBpmnModelInstance MESSAGE_EVENT_BASED_GW_PROCESS = ProcessModels.NewModel()
            .StartEvent()
            .EventBasedGateway()
            //.Id("eventBasedGateway")
            .IntermediateCatchEvent("messageCatch")
            //.Message(MESSAGE_NAME)
            .UserTask("afterMessageCatch")
            .EndEvent()
            .Done();

        public static readonly IBpmnModelInstance SIGNAL_EVENT_BASED_GW_PROCESS = ProcessModels.NewModel()
            .StartEvent()
            .EventBasedGateway()
            //.Id("eventBasedGateway")
            .IntermediateCatchEvent("signalCatch")
            .Signal(SIGNAL_NAME)
            .UserTask("afterSignalCatch")
            .EndEvent()
            .Done();
    }
}
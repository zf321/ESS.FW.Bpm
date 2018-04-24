using ESS.FW.Bpm.Model.Bpmn;

namespace Engine.Tests.Api.Runtime.Migration.Models
{
    /// <summary>
    /// </summary>
    public class SignalCatchModels
    {
        public const string SIGNAL_NAME = "Signal";

        public static readonly IBpmnModelInstance ONE_SIGNAL_CATCH_PROCESS = ProcessModels.NewModel()
            .StartEvent()
            .IntermediateCatchEvent("signalCatch")
            .Signal(SIGNAL_NAME)
            .UserTask("userTask")
            .EndEvent()
            .Done();

        public static readonly IBpmnModelInstance SUBPROCESS_SIGNAL_CATCH_PROCESS = ProcessModels.NewModel()
            .StartEvent()
            .SubProcess("subProcess")
            //.EmbeddedSubProcess()
            //.StartEvent()
            .IntermediateCatchEvent("signalCatch")
            .Signal(SIGNAL_NAME)
            .UserTask("userTask")
            .EndEvent()
            .SubProcessDone()
            .EndEvent()
            .Done();
    }
}
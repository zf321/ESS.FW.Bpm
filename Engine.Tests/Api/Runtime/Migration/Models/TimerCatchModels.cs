using ESS.FW.Bpm.Model.Bpmn;

namespace Engine.Tests.Api.Runtime.Migration.Models
{
    /// <summary>
    /// </summary>
    public class TimerCatchModels
    {
        public static readonly IBpmnModelInstance ONE_TIMER_CATCH_PROCESS = ProcessModels.NewModel()
            .StartEvent()
            .IntermediateCatchEvent("timerCatch")
            .TimerWithDuration("PT10M")
            .UserTask("userTask")
            .EndEvent()
            .Done();

        public static readonly IBpmnModelInstance SUBPROCESS_TIMER_CATCH_PROCESS = ProcessModels.NewModel()
            .StartEvent()
            .SubProcess("subProcess")
            //.EmbeddedSubProcess()
            //.StartEvent()
            .IntermediateCatchEvent("timerCatch")
            .TimerWithDuration("PT10M")
            .UserTask("userTask")
            .EndEvent()
            .SubProcessDone()
            .EndEvent()
            .Done();
    }
}
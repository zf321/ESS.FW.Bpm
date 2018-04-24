using ESS.FW.Bpm.Model.Bpmn;

namespace Engine.Tests.Api.Runtime.Migration.Models
{
    /// <summary>
    /// </summary>
    public class ExternalTaskModels
    {
        public const string EXTERNAL_TASK_TYPE = "external";
        public const string TOPIC = "foo";
        public const int PRIORITY = 100;

        public static readonly IBpmnModelInstance ONE_EXTERNAL_TASK_PROCESS = ProcessModels.NewModel()
            .StartEvent()
            .ServiceTask("externalTask")
            .CamundaType(EXTERNAL_TASK_TYPE)
            .CamundaTopic(TOPIC)
            .CamundaTaskPriority(PRIORITY.ToString())
            .EndEvent()
            .Done();

        public static readonly IBpmnModelInstance SUBPROCESS_PROCESS = ProcessModels.NewModel()
            .StartEvent()
            .SubProcess()
            //.EmbeddedSubProcess()
            //.StartEvent()
            .ServiceTask("externalTask")
            .CamundaType(EXTERNAL_TASK_TYPE)
            .CamundaTopic(TOPIC)
            .CamundaTaskPriority(PRIORITY.ToString())
            .EndEvent()
            .SubProcessDone()
            .EndEvent()
            .Done();
    }
}
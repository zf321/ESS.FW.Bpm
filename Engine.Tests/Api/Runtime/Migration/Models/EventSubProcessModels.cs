using ESS.FW.Bpm.Model.Bpmn;

namespace Engine.Tests.Api.Runtime.Migration.Models
{
    /// <summary>
    /// </summary>
    public class EventSubProcessModels
    {
        public const string MESSAGE_NAME = "Message";
        public const string SIGNAL_NAME = "Signal";
        public const string VAR_CONDITION = "${any=='any'}";
        public const string FALSE_CONDITION = "${false}";

        public const string EVENT_SUB_PROCESS_START_ID = "eventSubProcessStart";
        public const string EVENT_SUB_PROCESS_TASK_ID = "eventSubProcessTask";
        public const string EVENT_SUB_PROCESS_ID = "eventSubProcess";
        public const string SUB_PROCESS_ID = "subProcess";
        public const string USER_TASK_ID = "userTask";

        public static readonly IBpmnModelInstance CONDITIONAL_EVENT_SUBPROCESS_PROCESS =
            ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess)
                .AddSubProcessTo(ProcessModels.ProcessKey)
                //.Id(EVENT_SUB_PROCESS_ID)
                .TriggerByEvent()
                //.EmbeddedSubProcess()
                //.StartEvent(MigrationEventSubProcessTest.EVENT_SUB_PROCESS_START_ID)
                //.Condition(VAR_CONDITION)
                .UserTask(EVENT_SUB_PROCESS_TASK_ID)
                .EndEvent()
                .SubProcessDone()
                .Done();


        public static readonly IBpmnModelInstance FALSE_CONDITIONAL_EVENT_SUBPROCESS_PROCESS =
            ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess)
                .AddSubProcessTo(ProcessModels.ProcessKey)
                //.Id(EVENT_SUB_PROCESS_ID)
                .TriggerByEvent()
                //.EmbeddedSubProcess()
                //.StartEvent(MigrationEventSubProcessTest.EVENT_SUB_PROCESS_START_ID)
                //.Condition(FALSE_CONDITION)
                .UserTask(EVENT_SUB_PROCESS_TASK_ID)
                .EndEvent()
                .SubProcessDone()
                .Done();


        public static readonly IBpmnModelInstance MESSAGE_EVENT_SUBPROCESS_PROCESS =
            ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess)
                .AddSubProcessTo(ProcessModels.ProcessKey)
                //.Id(EVENT_SUB_PROCESS_ID)
                .TriggerByEvent()
                //.EmbeddedSubProcess()
                //.StartEvent(MigrationEventSubProcessTest.EVENT_SUB_PROCESS_START_ID)
                //.Message(MESSAGE_NAME)
                .UserTask(EVENT_SUB_PROCESS_TASK_ID)
                .EndEvent()
                .SubProcessDone()
                .Done();

        public static readonly IBpmnModelInstance MESSAGE_INTERMEDIATE_EVENT_SUBPROCESS_PROCESS =
            ProcessModels.NewModel()
                .StartEvent()
                .SubProcess(EVENT_SUB_PROCESS_ID)
                //.EmbeddedSubProcess()
                //.StartEvent()
                .IntermediateCatchEvent("catchMessage")
                //.Message(MESSAGE_NAME)
                .UserTask("userTask")
                .EndEvent()
                .SubProcessDone()
                .EndEvent()
                .Done();

        public static readonly IBpmnModelInstance TIMER_EVENT_SUBPROCESS_PROCESS =
            ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess)
                .AddSubProcessTo(ProcessModels.ProcessKey)
                //.Id(EVENT_SUB_PROCESS_ID)
                .TriggerByEvent()
                //.EmbeddedSubProcess()
                //.StartEvent(MigrationEventSubProcessTest.EVENT_SUB_PROCESS_START_ID)
                //.TimerWithDuration("PT10M")
                .UserTask(EVENT_SUB_PROCESS_TASK_ID)
                .EndEvent()
                .SubProcessDone()
                .Done();

        public static readonly IBpmnModelInstance SIGNAL_EVENT_SUBPROCESS_PROCESS =
            ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess)
                .AddSubProcessTo(ProcessModels.ProcessKey)
                //.Id(EVENT_SUB_PROCESS_ID)
                .TriggerByEvent()
                //.EmbeddedSubProcess()
                //.StartEvent(MigrationEventSubProcessTest.EVENT_SUB_PROCESS_START_ID)
                //.Signal(SIGNAL_NAME)
                .UserTask(EVENT_SUB_PROCESS_TASK_ID)
                .EndEvent()
                .SubProcessDone()
                .Done();

        public static readonly IBpmnModelInstance ESCALATION_EVENT_SUBPROCESS_PROCESS =
            ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess)
                .AddSubProcessTo(ProcessModels.ProcessKey)
               // .Id(EVENT_SUB_PROCESS_ID)
                .TriggerByEvent()
                //.EmbeddedSubProcess()
                //.StartEvent(MigrationEventSubProcessTest.EVENT_SUB_PROCESS_START_ID)
                //.Escalation()
                .UserTask(EVENT_SUB_PROCESS_TASK_ID)
                .EndEvent()
                .SubProcessDone()
                .Done();

        public static readonly IBpmnModelInstance ERROR_EVENT_SUBPROCESS_PROCESS =
            ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess)
                .AddSubProcessTo(ProcessModels.ProcessKey)
               // .Id(EVENT_SUB_PROCESS_ID)
                .TriggerByEvent()
                //.EmbeddedSubProcess()
                //.StartEvent(MigrationEventSubProcessTest.EVENT_SUB_PROCESS_START_ID)
                //.Error()
                .UserTask(EVENT_SUB_PROCESS_TASK_ID)
                .EndEvent()
                .SubProcessDone()
                .Done();

        public static readonly IBpmnModelInstance COMPENSATE_EVENT_SUBPROCESS_PROCESS =
            ModifiableBpmnModelInstance.Modify(ProcessModels.SubprocessProcess)
                .AddSubProcessTo(SUB_PROCESS_ID)
                //.Id(EVENT_SUB_PROCESS_ID)
                .TriggerByEvent()
                //.EmbeddedSubProcess()
                //.StartEvent(MigrationEventSubProcessTest.EVENT_SUB_PROCESS_START_ID)
                //.Compensation()
                .UserTask(EVENT_SUB_PROCESS_TASK_ID)
                .EndEvent()
                .SubProcessDone()
                .Done();

        public static readonly IBpmnModelInstance NESTED_EVENT_SUB_PROCESS_PROCESS =
            ModifiableBpmnModelInstance.Modify(ProcessModels.SubprocessProcess)
                .AddSubProcessTo(SUB_PROCESS_ID)
                //.Id(EVENT_SUB_PROCESS_ID)
                .TriggerByEvent()
                //.EmbeddedSubProcess()
                //.StartEvent(MigrationEventSubProcessTest.EVENT_SUB_PROCESS_START_ID)
                //.Message(MESSAGE_NAME)
                .UserTask(EVENT_SUB_PROCESS_TASK_ID)
                .EndEvent()
                .SubProcessDone()
                .Done();
    }
}
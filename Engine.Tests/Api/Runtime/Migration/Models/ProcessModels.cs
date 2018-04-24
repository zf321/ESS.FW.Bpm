using ESS.FW.Bpm.Model.Bpmn;
using ESS.FW.Bpm.Model.Bpmn.builder;

namespace Engine.Tests.Api.Runtime.Migration.Models
{
    /// <summary>
    /// </summary>
    public class ProcessModels
    {
        public const string ProcessKey = "Process";

        public const string UserTaskId = "userTask";

        public static readonly IBpmnModelInstance OneTaskProcess = GetOneTaskProcess();


        private static IBpmnModelInstance GetOneTaskProcess()
        {
          return NewModel().StartEvent("startEvent").UserTask(UserTaskId).Name<UserTaskBuilder>("User Task").EndEvent("endEvent").Done();
        }

        public static readonly IBpmnModelInstance OneTaskProcessWithDocumentation =
            ModifiableBpmnModelInstance.Modify(OneTaskProcess)
                .AddDocumentation("This is a documentation!");

        public static readonly IBpmnModelInstance TwoTasksProcess =
            NewModel()
                .StartEvent("startEvent")
                .UserTask("userTask1")
                .SequenceFlowId("flow1")
                .UserTask("userTask2")
                .EndEvent("endEvent")
                .Done();

        public static readonly IBpmnModelInstance SubprocessProcess =
            NewModel()
                .StartEvent()
                .SubProcess("subProcess")
                .EmbeddedSubProcess().StartEvent("subProcessStart").UserTask(UserTaskId).Name<UserTaskBuilder>("User Task").EndEvent("subProcessEnd").SubProcessDone().EndEvent()
                .Done();

        public static readonly IBpmnModelInstance DoubleSubprocessProcess =
            NewModel()
                .StartEvent()
                .SubProcess("outerSubProcess") .EmbeddedSubProcess().StartEvent()
                .SubProcess("innerSubProcess") .EmbeddedSubProcess().StartEvent()
                .UserTask(UserTaskId)
                .Name<UserTaskBuilder>("User Task")
                .EndEvent()
                .SubProcessDone()
                .EndEvent()
                .SubProcessDone()
                .EndEvent()
                .Done();

        public static readonly IBpmnModelInstance DoubleParallelSubprocessProcess =
            NewModel()
                .StartEvent()
                .SubProcess("outerSubProcess")
                .EmbeddedSubProcess()
                .StartEvent()
                .ParallelGateway("fork")
                .SubProcess("innerSubProcess1")
                .EmbeddedSubProcess()
                .StartEvent()
                .UserTask("userTask1")
                .Name<UserTaskBuilder>("User ITask 1")
                .EndEvent()
                .SubProcessDone()
                .EndEvent()
                .MoveToLastGateway()
                .SubProcess("innerSubProcess2")
                .EmbeddedSubProcess()
                .StartEvent()
                .UserTask("userTask2")
                .Name<UserTaskBuilder>("User Task 2")
                .EndEvent()
                .SubProcessDone()
                .EndEvent()
                .SubProcessDone()
                .EndEvent()
                .Done();

        public static readonly IBpmnModelInstance TripleSubprocessProcess =
            NewModel()
                .StartEvent()
                .SubProcess("subProcess1")
                .EmbeddedSubProcess()
                .StartEvent()
                .SubProcess("subProcess2")
                .EmbeddedSubProcess()
                .StartEvent()
                .SubProcess("subProcess3")
                .EmbeddedSubProcess()
                .StartEvent()
                .UserTask(UserTaskId)
                .Name<UserTaskBuilder>("User Task")
                .EndEvent()
                .SubProcessDone()
                .EndEvent()
                .SubProcessDone()
                .EndEvent()
                .SubProcessDone()
                .EndEvent()
                .Done();

        public static readonly IBpmnModelInstance OneReceiveTaskProcess =
            NewModel()
                .StartEvent()
                .ReceiveTask("receiveTask")
                .Message("Message")
                .EndEvent()
                .Done();

        public static readonly IBpmnModelInstance ParallelGatewayProcess =
            NewModel()
                .StartEvent()
                .ParallelGateway("fork")
                .UserTask("userTask1")
                .Name<UserTaskBuilder>("User ITask 1")
                .EndEvent()
                .MoveToLastGateway()
                .UserTask("userTask2")
                .Name<UserTaskBuilder>("IUser ITask 2")
                .EndEvent()
                .Done();

        public static readonly IBpmnModelInstance ParallelSubprocessProcess =
            NewModel()
                .StartEvent()
                .ParallelGateway("fork")
                .SubProcess("subProcess1")
                .EmbeddedSubProcess()
                .StartEvent()
                .UserTask("userTask1")
                .EndEvent()
                .SubProcessDone()
                .EndEvent()
                .MoveToLastGateway()
                .SubProcess("subProcess2")
                .EmbeddedSubProcess()
                .StartEvent()
                .UserTask("userTask2")
                .EndEvent()
                .SubProcessDone()
                .EndEvent()
                .Done();

        public static readonly IBpmnModelInstance ParallelDoubleSubprocessProcess =
            NewModel()
                .StartEvent()
                .ParallelGateway("fork")
                .SubProcess("subProcess1")
                .EmbeddedSubProcess()
                .StartEvent()
                .SubProcess("nestedSubProcess1")
                .EmbeddedSubProcess()
                .StartEvent()
                .UserTask("userTask1")
                .EndEvent()
                .SubProcessDone()
                .EndEvent()
                .SubProcessDone()
                .EndEvent()
                .MoveToLastGateway()
                .SubProcess("subProcess2")
                .EmbeddedSubProcess()
                .StartEvent()
                .SubProcess("nestedSubProcess2")
                .EmbeddedSubProcess()
                .StartEvent()
                .UserTask("userTask2")
                .EndEvent()
                .SubProcessDone()
                .EndEvent()
                .SubProcessDone()
                .EndEvent()
                .Done();

        public static readonly IBpmnModelInstance ParallelTaskAndSubprocessProcess =
            NewModel()
                .StartEvent()
                .ParallelGateway("fork")
                .SubProcess("subProcess")
                .EmbeddedSubProcess()
                .StartEvent()
                .UserTask("userTask1")
                .EndEvent()
                .SubProcessDone()
                .EndEvent()
                .MoveToLastGateway()
                .UserTask("userTask2")
                .EndEvent()
                .Done();

        public static readonly IBpmnModelInstance ParallelGatewaySubprocessProcess =
            NewModel()
                .StartEvent()
                .SubProcess("subProcess")
                .EmbeddedSubProcess()
                .StartEvent()
                .ParallelGateway("fork")
                .UserTask("userTask1")
                .Name<UserTaskBuilder>("IUser ITask 1")
                .EndEvent()
                .MoveToLastGateway()
                .UserTask("userTask2")
                .Name<UserTaskBuilder>("User ITask 2")
                .SubProcessDone()
                .EndEvent()
                .Done();

        public static readonly IBpmnModelInstance ScopeTaskProcess =
                ModifiableBpmnModelInstance.Modify(OneTaskProcess)
            //.ActivityBuilder(UserTaskId)
            //.CamundaInputParameter("foo", "bar")
            //.Done()
            ;

        public static readonly IBpmnModelInstance ScopeTaskSubprocessProcess =
                ModifiableBpmnModelInstance.Modify(SubprocessProcess)
            //.ActivityBuilder(UserTaskId)
            //.CamundaInputParameter("foo", "bar")
            //.Done()
            ;

        public static readonly IBpmnModelInstance ParallelScopeTasks =
                ModifiableBpmnModelInstance.Modify(ParallelGatewayProcess)
            //.ActivityBuilder("userTask1")
            //.CamundaInputParameter("foo", "bar")
            //.MoveToActivity("userTask2")
            //.CamundaInputParameter("foo", "bar")
            //.Done()
            ;

        public static readonly IBpmnModelInstance ParallelScopeTasksSubProcess =
                ModifiableBpmnModelInstance.Modify(ParallelGatewaySubprocessProcess)
            //.ActivityBuilder("userTask1")
            //.CamundaInputParameter("foo", "bar")
            //.MoveToActivity("userTask2")
            //.CamundaInputParameter("foo", "bar")
            //.Done()
            ;

        public static readonly IBpmnModelInstance UnsupportedActivities =
            ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(ProcessKey)
                .StartEvent("startEvent")
                .BusinessRuleTask("decisionTask")
                .CamundaDecisionRef("testDecision")
                .IntermediateThrowEvent("throwEvent")
                .Message("Message")
                .ServiceTask("serviceTask")
                .CamundaExpression("${true}")
                .SendTask("sendTask")
                .CamundaExpression("${true}")
                .ScriptTask("scriptTask")
                .ScriptText("foo")
                .EndEvent("endEvent")
                .Done();

        public static IProcessBuilder NewModel()
        {
            return NewModel(ProcessKey);
        }

        public static IProcessBuilder NewModel(string processKey)
        {
            return ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(processKey);
        }

        public static IProcessBuilder NewModel(int processNumber)
        {
            return NewModel(ProcessKey + processNumber);
        }

        public static IBpmnModelInstance SetOneTaskProcess(int processNumber)
        {
            return NewModel(processNumber)
                .StartEvent()
                .UserTask(UserTaskId)
                .EndEvent()
                .Done();
        }
    }
}
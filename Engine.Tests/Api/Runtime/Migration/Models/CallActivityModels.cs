using ESS.FW.Bpm.Model.Bpmn;

namespace Engine.Tests.Api.Runtime.Migration.Models
{
    /// <summary>
    /// </summary>
    public class CallActivityModels
    {
        public static IBpmnModelInstance oneBpmnCallActivityProcess(string calledProcessKey)
        {
            return ProcessModels.NewModel()
                .StartEvent()
                .CallActivity("callActivity")
                .CalledElement(calledProcessKey)
                .UserTask("userTask")
                .EndEvent()
                .Done();
        }

        public static IBpmnModelInstance subProcessBpmnCallActivityProcess(string calledProcessKey)
        {
            return ProcessModels.NewModel()
                .StartEvent()
                .SubProcess("subProcess")
                //.EmbeddedSubProcess()
                //.StartEvent()
                .CallActivity("callActivity")
                .CalledElement(calledProcessKey)
                .UserTask("userTask")
                .EndEvent()
                .SubProcessDone()
                .EndEvent()
                .Done();
        }

        public static IBpmnModelInstance oneCmmnCallActivityProcess(string caseCaseKey)
        {
            return ProcessModels.NewModel()
                .StartEvent()
                .CallActivity("callActivity")
                .CamundaCaseRef(caseCaseKey)
                .UserTask("userTask")
                .EndEvent()
                .Done();
        }

        public static IBpmnModelInstance oneBpmnCallActivityProcessAsExpression(int processNumber)
        {
            return ProcessModels.NewModel(processNumber)
                .StartEvent()
                .CallActivity()
                .CalledElement("${NextProcess}")
                .CamundaIn("NextProcess", "NextProcess")
                .EndEvent()
                .Done();
        }

        public static IBpmnModelInstance oneBpmnCallActivityProcessAsExpressionAsync(int processNumber)
        {
            return ProcessModels.NewModel(processNumber)
                .StartEvent()
                .CamundaAsyncBefore(true)
                .CallActivity()
                .CalledElement("${NextProcess}")
                .CamundaIn("NextProcess", "NextProcess")
                .EndEvent()
                .Done();
        }

        public static IBpmnModelInstance oneBpmnCallActivityProcessPassingVariables(int processNumber,
            int calledProcessNumber)
        {
            return ProcessModels.NewModel(processNumber)
                .StartEvent()
                .CallActivity()
                .CalledElement("Process" + calledProcessNumber)
                //.CamundaInputParameter("NextProcess", "Process" + (processNumber + 1))
                .CamundaIn("NextProcess", "NextProcess")
                .EndEvent()
                .Done();
        }
    }
}
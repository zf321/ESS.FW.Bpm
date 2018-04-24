using ESS.FW.Bpm.Model.Bpmn;

namespace Engine.Tests.Api.Runtime.Migration.Models
{
    /// <summary>
    /// </summary>
    public class ServiceTaskModels
    {
        public static IBpmnModelInstance oneClassDelegateServiceTask(string className)
        {
            return ProcessModels.NewModel()
                .StartEvent()
                .ServiceTask("serviceTask")
                .CamundaClass(className)
                .EndEvent()
                .Done();
        }
    }
}
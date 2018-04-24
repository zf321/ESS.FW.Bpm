using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Model.Bpmn;
using ESS.FW.Bpm.Model.Bpmn.instance;

namespace Engine.Tests.Bpmn.ExecutionListener
{
    
    public class ModelExecutionContextExecutionListener : IDelegateListener<IBaseDelegateExecution>
    {

        public static IBpmnModelInstance ModelInstance;
        public static IFlowElement FlowElement;

        public static void Clear()
        {
            ModelInstance = null;
            FlowElement = null;
        }

        public void Notify(IBaseDelegateExecution execution)
        {
            ModelInstance = ((IDelegateExecution)execution).BpmnModelInstance;
            FlowElement = ((IDelegateExecution)execution).BpmnModelElementInstance;
        }
    }

}
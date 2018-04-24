using ESS.FW.Bpm.Model.Bpmn.instance;

namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    public interface ISendTaskBuilder:IActivityBuilder<ISendTask>
    {
        ISendTaskBuilder CamundaClass(string camundaClass);
        ISendTaskBuilder CamundaDelegateExpression(string camundaExpression);
        ISendTaskBuilder CamundaExpression(string camundaExpression);
        ISendTaskBuilder CamundaResultVariable(string camundaResultVariable);
        ISendTaskBuilder CamundaTaskPriority(string taskPriority);
        ISendTaskBuilder CamundaTopic(string camundaTopic);
        ISendTaskBuilder CamundaType(string camundaType);
        ISendTaskBuilder Implementation(string implementation);
        ISendTaskBuilder Message(IMessage message);
        ISendTaskBuilder Message(string messageName);
        ISendTaskBuilder Operation(IOperation operation);
    }
}
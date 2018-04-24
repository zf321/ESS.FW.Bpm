using ESS.FW.Bpm.Model.Bpmn.instance;

namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    public interface IMessageEventDefinitionBuilder : IBaseElementBuilder<IMessageEventDefinition>
    {
        IMessageEventDefinitionBuilder CamundaTaskPriority(string taskPriority);
        IMessageEventDefinitionBuilder CamundaTopic(string camundaTopic);
        IMessageEventDefinitionBuilder CamundaType(string camundaType);
        //IBaseElementBuilder<IMessageEventDefinition> Id(string identifier);
        IMessageEventDefinitionBuilder Message(string message);
        IEventBuilder<IEvent> MessageEventDefinitionDone();
    }
}
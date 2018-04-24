using ESS.FW.Bpm.Model.Bpmn.instance;

namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    public interface IThrowEventBuilder<TE>:IFlowNodeBuilder<TE> where TE: IThrowEvent
    {
        CompensateEventDefinitionBuilder CompensateEventDefinition();
        CompensateEventDefinitionBuilder CompensateEventDefinition(string id);
        IThrowEventBuilder<TE> Escalation(string escalationCode);
        IThrowEventBuilder<TE> Message(string messageName);
        MessageEventDefinitionBuilder MessageEventDefinition();
        MessageEventDefinitionBuilder MessageEventDefinition(string id);
        IThrowEventBuilder<TE> Signal(string signalName);
    }
}
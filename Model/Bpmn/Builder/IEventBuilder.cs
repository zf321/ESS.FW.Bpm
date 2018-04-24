using ESS.FW.Bpm.Model.Bpmn.instance;

namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    public interface IEventBuilder<TE> : IFlowNodeBuilder<TE>, IBpmnModelElementBuilder<TE> where TE: IEvent
    {
        IEventBuilder<TE> CamundaInputParameter(string name, string value);
        IEventBuilder<TE> CamundaOutputParameter(string name, string value);
    }
}
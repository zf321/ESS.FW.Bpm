using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Bpmn.instance.bpmndi;

namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    public interface IBaseElementBuilder<TE>:IBpmnModelElementBuilder<TE>
        where TE : IBaseElement
    {
        IBaseElementBuilder<TE> AddExtensionElement(IBpmnModelElementInstance extensionElement);
        IBpmnEdge CreateBpmnEdge(ISequenceFlow sequenceFlow);
        IBpmnShape CreateBpmnShape(IFlowNode node);
        TOut Id<TOut>(string identifier) where TOut : IBaseElementBuilder<TE>;
    }
}
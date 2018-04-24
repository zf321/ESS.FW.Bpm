using ESS.FW.Bpm.Model.Bpmn.instance;

namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    public interface IFlowElementBuilder<TE>: IBaseElementBuilder<TE> where TE: IFlowElement
    {
        TOut Name<TOut>(string name) where TOut:IFlowElementBuilder<TE>;
    }
}
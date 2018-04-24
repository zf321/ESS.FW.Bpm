using ESS.FW.Bpm.Model.Bpmn.instance;

namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    public interface ICallableElementBuilder<TE>  where TE: ICallableElement
    {
        ICallableElementBuilder<TE> Name(string name);
    }
}
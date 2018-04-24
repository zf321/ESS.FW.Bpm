using ESS.FW.Bpm.Model.Bpmn.instance;

namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    public interface IProcessBuilder: IBaseElementBuilder<IProcess>, ICallableElementBuilder<IProcess>
    {
        IProcessBuilder CamundaJobPriority(string jobPriority);
        IProcessBuilder CamundaTaskPriority(string taskPriority);
        IProcessBuilder Closed();
        IProcessBuilder Executable();
        IProcessBuilder ProcessType(ProcessType processType);
        StartEventBuilder StartEvent();
        StartEventBuilder StartEvent(string id);
    }
}
using ESS.FW.Bpm.Model.Bpmn.instance;

namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    public interface ITaskBuilder<TE>: IActivityBuilder<TE> where TE:ITask
    {
        ITaskBuilder<TE> CamundaAsync();
        ITaskBuilder<TE> CamundaAsync(bool isCamundaAsync);
    }
}
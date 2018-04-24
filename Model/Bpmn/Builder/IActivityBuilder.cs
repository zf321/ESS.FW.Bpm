using ESS.FW.Bpm.Model.Bpmn.instance;
using System;

namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    public interface IActivityBuilder<TE>: IFlowNodeBuilder<TE> where TE:IActivity
    {
        BoundaryEventBuilder BoundaryEvent();
        BoundaryEventBuilder BoundaryEvent(string id);
        IActivityBuilder<TE> CamundaInputParameter(string name, string value);
        IActivityBuilder<TE> CamundaOutputParameter(string name, string value);
        MultiInstanceLoopCharacteristicsBuilder MultiInstance();
        //IActivityBuilder<TE> Func(Action<TE> action);
    }
}
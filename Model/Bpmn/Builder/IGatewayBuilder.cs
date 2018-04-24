using ESS.FW.Bpm.Model.Bpmn.instance;

namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    public interface IGatewayBuilder<TE>: IFlowNodeBuilder<TE> where TE: IFlowNode
    //where TB : IGatewayBuilder<TB, TE>
    //where TE : /*IGateway*/IFlowNode
    {
        IGatewayBuilder<TE> GatewayDirection(GatewayDirection gatewayDirection);
    }
}
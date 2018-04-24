

using ESS.FW.Bpm.Model.Bpmn.instance.bpmndi;

namespace ESS.FW.Bpm.Model.Bpmn.instance
{
    /// <summary>
    /// The BPMN gateway element
    /// 
    /// 
    /// </summary>
    public interface IGateway : IFlowNode
    {

        GatewayDirection GatewayDirection { get; set; }


        new IBpmnShape DiagramElement { get; }
    }
}
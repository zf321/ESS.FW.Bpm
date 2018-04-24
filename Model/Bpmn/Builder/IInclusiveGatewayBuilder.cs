using ESS.FW.Bpm.Model.Bpmn.instance;

namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    public interface IInclusiveGatewayBuilder
    {
        IInclusiveGatewayBuilder DefaultFlow(ISequenceFlow sequenceFlow);
    }
}
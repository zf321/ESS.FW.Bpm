using ESS.FW.Bpm.Model.Bpmn.instance;

namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    public interface IComplexGatewayBuilder
    {
        IComplexGatewayBuilder ActivationCondition(string conditionExpression);
        IComplexGatewayBuilder DefaultFlow(ISequenceFlow sequenceFlow);
    }
}
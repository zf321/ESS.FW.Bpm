namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    public interface IEventBasedGatewayBuilder
    {
        IEventBasedGatewayBuilder CamundaAsyncAfter();
        IEventBasedGatewayBuilder CamundaAsyncAfter(bool isCamundaAsyncAfter);
        IEventBasedGatewayBuilder EventGatewayType(EventBasedGatewayType eventGatewayType);
        IEventBasedGatewayBuilder Instantiate();
    }
}
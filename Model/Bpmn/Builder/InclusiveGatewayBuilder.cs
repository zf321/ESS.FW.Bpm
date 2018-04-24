

using ESS.FW.Bpm.Model.Bpmn.instance;

namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    /// <summary>
    /// 
    /// </summary>
    public class InclusiveGatewayBuilder : AbstractInclusiveGatewayBuilder/*<InclusiveGatewayBuilder>*/
    {

        public InclusiveGatewayBuilder(IBpmnModelInstance modelInstance, INclusiveGateway element) : base(modelInstance, element)
        {
        }
    }
}
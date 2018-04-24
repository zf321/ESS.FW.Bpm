using System;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml.instance;

namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    /// <summary>
	/// 
	/// </summary>
	public abstract class AbstractGatewayBuilder<TE> : AbstractFlowNodeBuilder<TE>, IGatewayBuilder<TE> where TE : IGateway
    {
        protected internal AbstractGatewayBuilder(IBpmnModelInstance modelInstance, TE element) : base(modelInstance, element)
        {
        }

        /// <summary>
        /// Sets the direction of the gateway build.
        /// </summary>
        /// <param name="gatewayDirection">  the direction to set </param>
        /// <returns> the builder object </returns>
        public virtual IGatewayBuilder<TE> GatewayDirection(GatewayDirection gatewayDirection)
        {
            element.GatewayDirection = gatewayDirection;
            return this;
        }
    }

}
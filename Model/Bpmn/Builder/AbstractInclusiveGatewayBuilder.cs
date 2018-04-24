using System;
using ESS.FW.Bpm.Model.Bpmn.instance;



namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class AbstractInclusiveGatewayBuilder : AbstractGatewayBuilder<INclusiveGateway>, IInclusiveGatewayBuilder
    {

        protected internal AbstractInclusiveGatewayBuilder(IBpmnModelInstance modelInstance, INclusiveGateway element) : base(modelInstance, element)
        {
        }

        /// <summary>
        /// Sets the default sequence flow for the build inclusive gateway.
        /// </summary>
        /// <param name="sequenceFlow">  the default sequence flow to set </param>
        /// <returns> the builder object </returns>
        public virtual IInclusiveGatewayBuilder DefaultFlow(ISequenceFlow sequenceFlow)
        {
            element.Default = sequenceFlow;
            return this;
        }
    }
}
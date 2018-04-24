using System;
using ESS.FW.Bpm.Model.Bpmn.instance;



namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    /// <summary>
	/// 
	/// </summary>
	public class AbstractParallelGatewayBuilder : AbstractGatewayBuilder<IParallelGateway>
	{

	  protected internal AbstractParallelGatewayBuilder(IBpmnModelInstance modelInstance, IParallelGateway element) : base(modelInstance, element)
	  {
	  }

	}

}
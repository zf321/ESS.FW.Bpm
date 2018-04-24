using System;
using ESS.FW.Bpm.Model.Bpmn.instance;



namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    /// <summary>
	/// 
	/// </summary>
	public abstract class AbstractExclusiveGatewayBuilder : AbstractGatewayBuilder<IExclusiveGateway>
	{

	  protected internal AbstractExclusiveGatewayBuilder(IBpmnModelInstance modelInstance, IExclusiveGateway element) : base(modelInstance, element)
	  {
	  }

	  /// <summary>
	  /// Sets the default sequence flow for the build exclusive gateway.
	  /// </summary>
	  /// <param name="sequenceFlow">  the default sequence flow to set </param>
	  /// <returns> the builder object </returns>
	  public virtual AbstractExclusiveGatewayBuilder DefaultFlow(ISequenceFlow sequenceFlow)
	  {
		element.Default = sequenceFlow;
		return this;
	  }

	}

}
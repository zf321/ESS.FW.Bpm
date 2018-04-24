using System;
using ESS.FW.Bpm.Model.Bpmn.instance;



namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    /// <summary>
	/// 
	/// </summary>
	public class AbstractComplexGatewayBuilder : AbstractGatewayBuilder<IComplexGateway>, IComplexGatewayBuilder
    {

	  protected internal AbstractComplexGatewayBuilder(IBpmnModelInstance modelInstance, IComplexGateway element) : base(modelInstance, element)
	  {
	  }

	  /// <summary>
	  /// Sets the default sequence flow for the build complex gateway.
	  /// </summary>
	  /// <param name="sequenceFlow">  the default sequence flow to set </param>
	  /// <returns> the builder object </returns>
	  public virtual IComplexGatewayBuilder DefaultFlow(ISequenceFlow sequenceFlow)
	  {
		element.Default = sequenceFlow;
		return this;
	  }

	  /// <summary>
	  /// Sets the activation condition expression for the build complex gateway
	  /// </summary>
	  /// <param name="conditionExpression">  the activation condition expression to set </param>
	  /// <returns> the builder object </returns>
	  public virtual IComplexGatewayBuilder ActivationCondition(string conditionExpression)
	  {
		IActivationCondition activationCondition = CreateInstance<IActivationCondition>(typeof(IActivationCondition));
		activationCondition.TextContent = conditionExpression;
		element.ActivationCondition = activationCondition;
		return this;
	  }

	}

}
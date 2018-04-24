using System;
using ESS.FW.Bpm.Model.Bpmn.instance;



namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    /// <summary>
	/// 
	/// </summary>
	public class AbstractEventBasedGatewayBuilder : AbstractGatewayBuilder<IEventBasedGateway>, IEventBasedGatewayBuilder
    {

	  protected internal AbstractEventBasedGatewayBuilder(IBpmnModelInstance modelInstance, IEventBasedGateway element) : base(modelInstance, element)
	  {
	  }

	  /// <summary>
	  /// Sets the build event based gateway to be instantiate.
	  /// </summary>
	  /// <returns> the builder object </returns>
	  public virtual IEventBasedGatewayBuilder Instantiate()
	  {
		element.Instantiate = true;
		return this;
	  }

	  /// <summary>
	  /// Sets the event gateway type of the build event based gateway.
	  /// </summary>
	  /// <param name="eventGatewayType">  the event gateway type to set </param>
	  /// <returns> the builder object </returns>
	  public virtual IEventBasedGatewayBuilder EventGatewayType(EventBasedGatewayType eventGatewayType)
	  {
		element.EventGatewayType = eventGatewayType;
		return this;
	  }


	  public override /*IEventBasedGatewayBuilder*/IFlowNodeBuilder<IEventBasedGateway> CamundaAsyncAfter(bool isCamundaAsyncAfter)
	  {
		throw new System.NotSupportedException("'asyncAfter' is not supported for 'Event Based Gateway'");
	  }

        IEventBasedGatewayBuilder IEventBasedGatewayBuilder.CamundaAsyncAfter()
        {
            throw new NotImplementedException();
        }

        IEventBasedGatewayBuilder IEventBasedGatewayBuilder.CamundaAsyncAfter(bool isCamundaAsyncAfter)
        {
            throw new NotImplementedException();
        }
    }

}
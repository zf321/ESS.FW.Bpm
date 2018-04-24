

using ESS.FW.Bpm.Model.Bpmn.instance;

namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    /// <summary>
	/// 
	/// </summary>
	public class EventBasedGatewayBuilder : AbstractEventBasedGatewayBuilder/*<EventBasedGatewayBuilder>*/
	{

	  public EventBasedGatewayBuilder(IBpmnModelInstance modelInstance, IEventBasedGateway element) : base(modelInstance, element)
	  {
	  }

	}

}
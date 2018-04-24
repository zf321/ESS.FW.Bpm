

using ESS.FW.Bpm.Model.Bpmn.builder;

namespace ESS.FW.Bpm.Model.Bpmn.instance
{

	using EventBasedGatewayBuilder = EventBasedGatewayBuilder;

	/// <summary>
	/// The BPMN eventBasedGateway element
	/// 
	/// 
	/// </summary>
	public interface IEventBasedGateway : IGateway
	{

	  EventBasedGatewayBuilder Builder();

	  bool Instantiate {get;set;}


	  EventBasedGatewayType EventGatewayType {get;set;}


	}

}
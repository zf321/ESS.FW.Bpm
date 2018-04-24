

using ESS.FW.Bpm.Model.Bpmn.builder;

namespace ESS.FW.Bpm.Model.Bpmn.instance
{
    
	/// <summary>
	/// The BPMN inclusiveGateway element
	/// 
	/// 
	/// </summary>
	public interface INclusiveGateway : IGateway
	{

	  InclusiveGatewayBuilder Builder();

	  ISequenceFlow Default {get;set;}


	}

}
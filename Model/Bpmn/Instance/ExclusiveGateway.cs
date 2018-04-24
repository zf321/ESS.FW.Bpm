

using ESS.FW.Bpm.Model.Bpmn.builder;

namespace ESS.FW.Bpm.Model.Bpmn.instance
{

	using ExclusiveGatewayBuilder = ExclusiveGatewayBuilder;

	/// <summary>
	/// The BPMN exclusiveGateway element
	/// 
	/// 
	/// </summary>
	public interface IExclusiveGateway : IGateway
	{

	  ExclusiveGatewayBuilder Builder();

	  ISequenceFlow Default {get;set;}

	}

}
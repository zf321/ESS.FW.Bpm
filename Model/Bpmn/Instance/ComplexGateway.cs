

using ESS.FW.Bpm.Model.Bpmn.builder;

namespace ESS.FW.Bpm.Model.Bpmn.instance
{

	using ComplexGatewayBuilder = ComplexGatewayBuilder;

	/// <summary>
	/// The BPMN complexGateway element
	/// 
	/// 
	/// </summary>
	public interface IComplexGateway : IGateway
	{

	  ComplexGatewayBuilder Builder();

	  ISequenceFlow Default {get;set;}


	  IActivationCondition ActivationCondition {get;set;}


	}

}
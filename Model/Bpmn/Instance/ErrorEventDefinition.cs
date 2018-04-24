

namespace ESS.FW.Bpm.Model.Bpmn.instance
{

	/// <summary>
	/// The BPMN errorEventDefinition element
	/// 
	/// 
	/// </summary>
	public interface IErrorEventDefinition : IEventDefinition
	{

	  IError Error {get;set;}


	  string CamundaErrorCodeVariable {set;get;}


	  string CamundaErrorMessageVariable {set;get;}

	}

}
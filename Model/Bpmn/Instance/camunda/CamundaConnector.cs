

namespace ESS.FW.Bpm.Model.Bpmn.instance.camunda
{

	/// <summary>
	/// The BPMN connector camunda extension element
	/// 
	/// 
	/// </summary>
	public interface ICamundaConnector : IBpmnModelElementInstance
	{

	  ICamundaConnectorId CamundaConnectorId {get;set;}


	  ICamundaInputOutput CamundaInputOutput {get;set;}


	}

}
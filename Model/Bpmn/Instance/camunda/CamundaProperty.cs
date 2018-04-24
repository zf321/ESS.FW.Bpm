

namespace ESS.FW.Bpm.Model.Bpmn.instance.camunda
{

	/// <summary>
	/// The BPMN property camunda extension element
	/// 
	/// 
	/// </summary>
	public interface ICamundaProperty : IBpmnModelElementInstance
	{

	  string CamundaId {get;set;}


	  string CamundaName {get;set;}


	  string CamundaValue {get;set;}


	}

}
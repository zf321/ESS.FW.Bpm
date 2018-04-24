

namespace ESS.FW.Bpm.Model.Bpmn.instance.camunda
{

	/// <summary>
	/// The BPMN value camunda extension element
	/// 
	/// 
	/// </summary>
	public interface ICamundaValue : IBpmnModelElementInstance
	{

	  string CamundaId {get;set;}


	  string CamundaName {get;set;}

	}

}
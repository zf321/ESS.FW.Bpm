

namespace ESS.FW.Bpm.Model.Bpmn.instance.camunda
{

	/// <summary>
	/// The BPMN inputParameter camunda extension element
	/// 
	/// 
	/// </summary>
	public interface ICamundaInputParameter : IBpmnModelElementInstance, ICamundaGenericValueElement
	{

	  string CamundaName {get;set;}


	}

}


namespace ESS.FW.Bpm.Model.Bpmn.instance.camunda
{

	/// <summary>
	/// The BPMN outputParameter camunda extension element
	/// 
	/// 
	/// </summary>
	public interface ICamundaOutputParameter : IBpmnModelElementInstance, ICamundaGenericValueElement
	{

	  string CamundaName {get;set;}


	}

}
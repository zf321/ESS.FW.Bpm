

namespace ESS.FW.Bpm.Model.Bpmn.instance.camunda
{

	/// <summary>
	/// The BPMN camundaEntry camunda extension element
	/// 
	/// 
	/// </summary>
	public interface ICamundaEntry : IBpmnModelElementInstance, ICamundaGenericValueElement
	{

	  string CamundaKey {get;set;}


	}

}
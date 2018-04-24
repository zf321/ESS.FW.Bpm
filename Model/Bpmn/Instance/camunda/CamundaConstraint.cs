

namespace ESS.FW.Bpm.Model.Bpmn.instance.camunda
{

	/// <summary>
	/// The BPMN constraint camunda extension element
	/// 
	/// 
	/// </summary>
	public interface ICamundaConstraint : IBpmnModelElementInstance
	{

	  string CamundaName {get;set;}


	  string CamundaConfig {get;set;}


	}

}
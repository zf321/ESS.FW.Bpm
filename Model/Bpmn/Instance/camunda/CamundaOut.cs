

namespace ESS.FW.Bpm.Model.Bpmn.instance.camunda
{

	/// <summary>
	/// The BPMN out camunda extension element
	/// 
	/// 
	/// </summary>
	public interface ICamundaOut : IBpmnModelElementInstance
	{

	  string CamundaSource {get;set;}


	  string CamundaSourceExpression {get;set;}


	  string CamundaVariables {get;set;}


	  string CamundaTarget {get;set;}


	  bool CamundaLocal {get;set;}


	}

}
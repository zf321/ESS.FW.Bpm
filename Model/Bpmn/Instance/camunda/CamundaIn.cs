

namespace ESS.FW.Bpm.Model.Bpmn.instance.camunda
{

	/// <summary>
	/// The BPMN in camunda extension element
	/// 
	/// 
	/// </summary>
	public interface ICamundaIn : IBpmnModelElementInstance
	{

	  string CamundaSource {get;set;}


	  string CamundaSourceExpression {get;set;}


	  string CamundaVariables {get;set;}


	  string CamundaTarget {get;set;}


	  string CamundaBusinessKey {get;set;}


	  bool CamundaLocal {get;set;}


	}

}
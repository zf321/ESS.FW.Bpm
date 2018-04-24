

namespace ESS.FW.Bpm.Model.Bpmn.instance.camunda
{

	/// <summary>
	/// The BPMN field camunda extension element
	/// 
	/// 
	/// </summary>
	public interface ICamundaField : IBpmnModelElementInstance
	{

	  string CamundaName {get;set;}


	  string CamundaExpression {get;set;}


	  string CamundaStringValue {get;set;}


	  ICamundaString CamundaString {get;set;}


	  ICamundaExpression CamundaExpressionChild {get;set;}


	}

}
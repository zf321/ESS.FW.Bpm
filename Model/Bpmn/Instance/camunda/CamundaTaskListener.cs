using System.Collections.Generic;



namespace ESS.FW.Bpm.Model.Bpmn.instance.camunda
{


	/// <summary>
	/// The BPMN taskListener camunda extension element
	/// 
	/// 
	/// </summary>
	public interface ICamundaTaskListener : IBpmnModelElementInstance
	{

	  string CamundaEvent {get;set;}


	  string CamundaClass {get;set;}


	  string CamundaExpression {get;set;}


	  string CamundaDelegateExpression {get;set;}


	  ICollection<ICamundaField> CamundaFields {get;}

	  ICamundaScript CamundaScript {get;set;}


	}

}
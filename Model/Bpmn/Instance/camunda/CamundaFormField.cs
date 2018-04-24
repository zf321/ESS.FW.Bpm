using System.Collections.Generic;



namespace ESS.FW.Bpm.Model.Bpmn.instance.camunda
{


	/// <summary>
	/// The BPMN formField camunda extension element
	/// 
	/// 
	/// </summary>
	public interface ICamundaFormField : IBpmnModelElementInstance
	{

	  string CamundaId {get;set;}


	  string CamundaLabel {get;set;}


	  string CamundaType {get;set;}


	  string CamundaDatePattern {get;set;}


	  string CamundaDefaultValue {get;set;}


	  ICamundaProperties CamundaProperties {get;set;}


	  ICamundaValidation CamundaValidation {get;set;}


	  ICollection<ICamundaValue> CamundaValues {get;}

	}

}
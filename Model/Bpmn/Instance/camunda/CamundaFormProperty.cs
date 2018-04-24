using System.Collections.Generic;



namespace ESS.FW.Bpm.Model.Bpmn.instance.camunda
{


	/// <summary>
	/// The BPMN formProperty camunda extension element
	/// 
	/// 
	/// </summary>
	public interface ICamundaFormProperty : IBpmnModelElementInstance
	{

	  string CamundaId {get;set;}


	  string CamundaName {get;set;}


	  string CamundaType {get;set;}


	  bool CamundaRequired {get;set;}


	  bool CamundaReadable {get;set;}


	  bool CamundaWriteable {get;set;}


	  string CamundaVariable {get;set;}


	  string CamundaExpression {get;set;}


	  string CamundaDatePattern {get;set;}


	  string CamundaDefault {get;set;}


	  ICollection<ICamundaValue> CamundaValues {get;}

	}

}
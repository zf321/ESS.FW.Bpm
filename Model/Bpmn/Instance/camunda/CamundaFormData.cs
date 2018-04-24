using System.Collections.Generic;



namespace ESS.FW.Bpm.Model.Bpmn.instance.camunda
{


	/// <summary>
	/// The BPMN formData camunda extension element
	/// 
	/// 
	/// </summary>
	public interface ICamundaFormData : IBpmnModelElementInstance
	{

	  ICollection<ICamundaFormField> CamundaFormFields {get;}

	}

}
using System.Collections.Generic;



namespace ESS.FW.Bpm.Model.Bpmn.instance.camunda
{


	/// <summary>
	/// The BPMN validation camunda extension element
	/// 
	/// 
	/// </summary>
	public interface ICamundaValidation : IBpmnModelElementInstance
	{

	  ICollection<ICamundaConstraint> CamundaConstraints {get;}

	}

}
using System.Collections.Generic;



namespace ESS.FW.Bpm.Model.Bpmn.instance.camunda
{


	/// <summary>
	/// The BPMN inputOutput camunda extension element
	/// 
	/// 
	/// </summary>
	public interface ICamundaInputOutput : IBpmnModelElementInstance
	{

	  ICollection<ICamundaInputParameter> CamundaInputParameters {get;}

	  ICollection<ICamundaOutputParameter> CamundaOutputParameters {get;}

	}

}
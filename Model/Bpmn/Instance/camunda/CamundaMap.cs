using System.Collections.Generic;



namespace ESS.FW.Bpm.Model.Bpmn.instance.camunda
{


	/// <summary>
	/// The BPMN camundaMap extension element
	/// 
	/// 
	/// </summary>
	public interface ICamundaMap : IBpmnModelElementInstance
	{

	  ICollection<ICamundaEntry> CamundaEntries {get;}

	}

}
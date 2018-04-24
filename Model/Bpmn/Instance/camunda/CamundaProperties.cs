using System.Collections.Generic;



namespace ESS.FW.Bpm.Model.Bpmn.instance.camunda
{


	/// <summary>
	/// The BPMN properties camunda extension element
	/// 
	/// 
	/// </summary>
	public interface ICamundaProperties : IBpmnModelElementInstance
	{

	    ICollection<ICamundaProperty> CamundaProperties { get; }

	}

}